using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool walkable = true;
    public bool current = false;
    public bool target = false;
    public bool selectable = false;

    /// <summary>
    /// Contains the neighbours of the tile
    /// </summary>
    public List<Tile> adjacencyList = new List<Tile>();

    //Needed BFS (breadth first search)
    public bool visited = false;
    public Tile parent = null;
    public int distance = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (current)
        {
            GetComponent<Renderer>().material.color = Color.magenta;
        }
        else if (target)
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
        else if (selectable)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white;
        }
    }

    public void Reset()
    {
        adjacencyList.Clear();

        current = false;
        target = false;
        selectable = false;

        visited = false;
        parent = null;
        distance = 0;
    }

    internal Tile GetDownNeighbour(float jumpHeight)
    {
        return CheckTile(-Vector3.forward, jumpHeight);
    }

    internal Tile GetRightNeighbour(float jumpHeight)
    {
        return CheckTile(Vector3.right, jumpHeight);
    }

    internal Tile GetLeftNeighbour(float jumpHeight)
    {
        return CheckTile(-Vector3.right, jumpHeight);
    }

    internal Tile GetUpNeighbour(float jumpHeight)
    {
        return CheckTile(Vector3.forward, jumpHeight);
    }

    /// <summary>
    /// Finds the neighbours of the tile
    /// </summary>
    /// <param name="jumpHeight">How far can player walk in blocks (mostly one)</param>
    public void FindNeighbors(float jumpHeight)
    {
        Reset();

        GetDownNeighbour(jumpHeight);
        GetUpNeighbour(jumpHeight);
        GetLeftNeighbour(jumpHeight);
        GetRightNeighbour(jumpHeight);
    }

    public Tile CheckTile(Vector3 direction, float jumpHeight)
    {
        Vector3 halfExtents = new Vector3(0.25f, (1 + jumpHeight) / 2.0f, 0.25f); //Range of jumping
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        Tile firstCollider = null;

        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();

            if (tile != null && tile.walkable)
            {
                RaycastHit hit;
                //Check if block is blocked (item is already on the block)
                if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1))
                {
                    adjacencyList.Add(tile);
                    if (firstCollider == null)
                        firstCollider = tile;
                }
            }
        }

        return firstCollider;
    }
}
