using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool walkable = true;
    public bool current = false;

    /// <summary>
    /// Contains the neighbours of the tile
    /// </summary>
    public List<Tile> adjacencyList = new List<Tile>();
    
    public Tile parent = null;

    public void Reset()
    {
        adjacencyList.Clear();

        current = false;
        
        parent = null;
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
