using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    List<Tile> selectableTiles = new List<Tile>();
    List<GameObject> tiles = new List<GameObject>();

    Stack<Tile> path = new Stack<Tile>();
    protected Tile currentTile;

    protected bool moving = false;
    public float jumpHeight = 2;
    public float moveSpeed = 2;
    public float jumpVelocity = 4.5f;

    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3();

    float halfHeight = 0;

    //TODO Simplify with ENUM
    JumpState jumpState = JumpState.None;
    Vector3 jumpTarget;

    protected void Init()
    {
        tiles.AddRange(GameObject.FindGameObjectsWithTag("EnemyTile"));
        tiles.AddRange(GameObject.FindGameObjectsWithTag("WallTile"));
        tiles.AddRange(GameObject.FindGameObjectsWithTag("ResourceTile"));
        tiles.AddRange(GameObject.FindGameObjectsWithTag("RepairTile"));
        tiles.AddRange(GameObject.FindGameObjectsWithTag("BaseTile"));
        tiles.AddRange(GameObject.FindGameObjectsWithTag("Wall"));

        halfHeight = GetComponent<Collider>().bounds.extents.y;
    }

    public void GetCurrentTile()
    {
        currentTile = GetTargetTile(gameObject);
        currentTile.current = true;
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

        selectableTiles.Add(t);
        
        foreach (Tile tile in t.adjacencyList)
        {
            tile.parent = t;
        }
    }

    public void MoveToTile(Tile tile)
    {
        path.Clear();
        moving = true;

        Tile next = tile;

        while(next != null)
        {
            path.Push(next);
            next = next.parent;
        }
    }

    public void Move()
    {
        if (path.Count > 0)
        {
            Tile t = path.Peek();
            Vector3 target = t.transform.position;

            //Calculate the units position on top on the target tile
            target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;

            if (Vector3.Distance(transform.position, target) >= 0.05f)
            {
                bool jump = transform.position.y != target.y;

                if (jump)
                {
                    Jump(target);
                }
                else
                {
                    CalculateHeading(target);
                    SetHorizontalVelocity();
                }
                transform.forward = heading;
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                //Tile center reached
                transform.position = target;
                path.Pop();
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
        if(currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }

        foreach(Tile t in selectableTiles)
        {
            t.Reset();
        }
        selectableTiles.Clear();
    }

    void Jump(Vector3 target)
    {
        switch(jumpState)
        {
            case JumpState.FallDown:
                FallDownward(target);
                break;
            case JumpState.JumpUp:
                JumpUpward(target);
                break;
            case JumpState.MoveToEdge:
                MoveToEdge();
                break;
            default:
                PrepareJump(target);
                break;
        }
    }

    void MoveToEdge()
    {
        if(Vector3.Distance(transform.position, jumpTarget) >= 0.05f)
        {
            SetHorizontalVelocity();
        }
        else
        {
            jumpState = JumpState.FallDown;

            velocity /= 5.0f;
            velocity.y = 1.5f;
        }
    }

    void JumpUpward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;

        if(transform.position.y > target.y)
        {
            jumpState = JumpState.FallDown;
        }
    }

    void FallDownward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;

        if(transform.position.y <= target.y)
        {
            jumpState = JumpState.None;

            Vector3 p = transform.position;
            p.y = target.y;
            transform.position = p;

            velocity = new Vector3();
        }
    }

    void PrepareJump(Vector3 target)
    {
        float targetY = target.y;

        target.y = transform.position.y;

        CalculateHeading(target);

        //Lower than character unit -> falling
        if (transform.position.y > targetY)
        {
            jumpState = JumpState.MoveToEdge;

            jumpTarget = transform.position + (target - transform.position) / 2.0f;
        }
        else
        {
            jumpState = JumpState.JumpUp;

            velocity = heading * moveSpeed / 3.0f;

            float difference = transform.position.y;
                        
            velocity.y = jumpVelocity * (0.5f + difference / 2.0f);
        }
    }

    public enum JumpState
    {
        FallDown,
        MoveToEdge,
        JumpUp,
        None
    }
}
