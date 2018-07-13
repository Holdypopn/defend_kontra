using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    List<Tile> selectableTiles = new List<Tile>();
    List<GameObject> tiles = new List<GameObject>();

    internal Stack<Tile> Path = new Stack<Tile>();
    protected Tile currentTile;

    protected bool moving = false;
    public float jumpHeight = 2;
    public float BaseMoveSpeed = 2;
    internal float moveSpeed;
    public float jumpVelocity = 4.5f;

    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3();

    float halfHeight = 0;

    //TODO Simplify with ENUM
    ClimpState climpState = ClimpState.None;
    Vector3 jumpTarget;

    protected void Init()
    {
        tiles.AddRange(GameObject.FindGameObjectsWithTag("EnemyTile"));
        tiles.AddRange(GameObject.FindGameObjectsWithTag("WallTile"));
        tiles.AddRange(GameObject.FindGameObjectsWithTag("StoneResourceTile"));
        tiles.AddRange(GameObject.FindGameObjectsWithTag("AmmoResourceTile"));
        tiles.AddRange(GameObject.FindGameObjectsWithTag("RepairTile"));
        tiles.AddRange(GameObject.FindGameObjectsWithTag("BaseTile"));
        tiles.AddRange(GameObject.FindGameObjectsWithTag("Wall"));

        halfHeight = GetComponent<Collider>().bounds.extents.y;

        moveSpeed = BaseMoveSpeed;
    }

    public void GetCurrentTile()
    {
        currentTile = GetTargetTile(gameObject);
    }

    public Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile tile = null;

        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
        {
            tile = hit.collider.GetComponent<Tile>();
        }

        return tile;
    }

    public void ComputeAdjacencyList(bool isEnemy = false)
    {
        foreach (GameObject tile in tiles)
        {
            if (tile == null)//tile is destoyed
                continue;
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbors(jumpHeight, isEnemy);
        }
    }

    public void FindSelectableTiles(bool isEnemy = false)
    {
        ComputeAdjacencyList(isEnemy);
        GetCurrentTile();

        Tile t = currentTile;

        if (t == null) //currentile is null (Wall is broken)
            return;

        selectableTiles.Add(t);

        foreach (Tile tile in t.adjacencyList)
        {
            tile.parent = t;
        }
    }

    public void MoveToTile(Tile tile)
    {
        if (tile.MovementTo)
        {
            Debug.Log("Tile already in use!");
            return;
        }

        Path.Clear();
        moving = true;

        Tile next = tile;

        while (next != null)
        {
            Path.Push(next);
            next = next.parent;
        }
    }
    private float oldDistance = float.MaxValue;
    public void Move()
    {
        if (Path.Count > 0)
        {
            Tile t = Path.Peek();
            Vector3 target = t.transform.position;

            t.MovementTo = true;

            //Calculate the units position on top on the target tile
            target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;

            float d = Vector3.Distance(transform.position, target);
            if (d < oldDistance && !(transform.position.x == target.x && transform.position.z == target.z))
            {
                bool climp = transform.position.y != target.y;

                if (climp)
                {
                    if (ClimpState.MoveToCenter == climpState)
                        oldDistance = d;

                    Climp(target);
                }
                else
                {
                    oldDistance = d;
                    CalculateHeading(target);
                    SetHorizontalVelocity();
                }
                transform.forward = heading;
                transform.position += velocity * Time.deltaTime;

                transform.GetChild(0).GetComponent<HealthBarRotation>().Refresh();
            }
            else
            {
                //Rotate
                transform.eulerAngles = new Vector3(0, 0, 0);
                transform.GetChild(0).GetComponent<HealthBarRotation>().Refresh();

                //Tile center reached
                transform.position = target;
                Path.Pop();
                t.MovementTo = false;
                climpState = ClimpState.None;
                oldDistance = float.MaxValue;
            }
        }
        else
        {
            RemoveSelectableTiles();
            moving = false;
        }
    }

    private void SetHorizontalVelocity()
    {
        velocity = heading * moveSpeed;
    }

    void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        heading.Normalize();
    }

    protected void RemoveSelectableTiles()
    {
        foreach (Tile t in selectableTiles)
        {
            t.Reset();
        }
        selectableTiles.Clear();
    }

    void Climp(Vector3 target)
    {
        switch (climpState)
        {
            case ClimpState.ClimpDown:
                ClimpDown(target);
                break;
            case ClimpState.ClimpUp:
                ClimpUp(target);
                break;
            case ClimpState.MoveToEdge:
                MoveToEdge(target);
                break;
            case ClimpState.MoveToCenter:
                MoveToCenter(target);
                break;
            default:
                PrepareClimp(target);
                break;
        }
    }

    void MoveToEdge(Vector3 target)
    {
        //Wenn runter und z ist größer als zielblock
        bool a = (transform.position.y > target.y) && transform.position.z > target.z + 0.5 - transform.localScale.x / 2;

        //Wenn rauf und z ist kleiner als zielblock
        bool b = (transform.position.y < target.y) && transform.position.z < target.z - 0.5 - transform.localScale.x / 2;
        
        if (a || b)
        {
            SetHorizontalVelocity();
        }
        else
        {
            if (transform.position.y > target.y)
                climpState = ClimpState.ClimpDown;
            else
                climpState = ClimpState.ClimpUp;


            velocity /= 5.0f;
            velocity.y = 1.5f;
        }
    }

    void ClimpUp(Vector3 target)
    {

        var h = new Vector3(heading.x, 2, 0);

        velocity = h * moveSpeed / 3.0f;

        //velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.y > target.y)
        {
            climpState = ClimpState.MoveToCenter;
        }
    }

    void ClimpDown(Vector3 target)
    {
        var h = new Vector3(heading.x, -2, 0);

        velocity = h * moveSpeed / 3.0f;
        
        if (transform.position.y <= target.y)
        {
            climpState = ClimpState.MoveToCenter;

            Vector3 p = transform.position;
            p.y = target.y;
            transform.position = p;

            velocity = new Vector3();
        }
    }

    void MoveToCenter(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;

        var h = new Vector3(heading.x, 0, heading.z);

        velocity = h * moveSpeed / 3.0f;
    }

    void PrepareClimp(Vector3 target)
    { 
        target.y = transform.position.y;

        CalculateHeading(target);

        climpState = ClimpState.MoveToEdge;
    }
    
    public enum ClimpState
    {
        ClimpDown,
        MoveToEdge,
        ClimpUp,
        MoveToCenter,
        None
    }
}
