using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, IDestructible
{
    public bool walkableEnemy = false;
    public bool walkablePlayer = true;
    public bool current = false;
    public bool destructible = false;
    public float MaxHealth = 20;

    public float currentHealth;

    /// <summary>
    /// Contains the neighbours of the tile
    /// </summary>
    public List<Tile> adjacencyList = new List<Tile>();

    public Tile parent = null;

    void Start()
    {
        currentHealth = MaxHealth;
    }

    public void Reset()
    {
        adjacencyList.Clear();

        current = false;

        parent = null;
    }

    internal Tile GetDownNeighbour(float jumpHeight, bool isEnemy = false)
    {
        return CheckTile(-Vector3.forward, jumpHeight, isEnemy);
    }

    internal Tile GetRightNeighbour(float jumpHeight, bool isEnemy = false)
    {
        return CheckTile(Vector3.right, jumpHeight, isEnemy);
    }

    internal Tile GetLeftNeighbour(float jumpHeight, bool isEnemy = false)
    {
        return CheckTile(-Vector3.right, jumpHeight, isEnemy);
    }

    internal Tile GetUpNeighbour(float jumpHeight, bool isEnemy = false)
    {
        return CheckTile(Vector3.forward, jumpHeight, isEnemy);
    }

    /// <summary>
    /// Finds the neighbours of the tile
    /// </summary>
    /// <param name="jumpHeight">How far can player walk in blocks (mostly one)</param>
    public void FindNeighbors(float jumpHeight, bool isEnemy = false)
    {
        Reset();

        GetDownNeighbour(jumpHeight, isEnemy);
        GetUpNeighbour(jumpHeight, isEnemy);
        GetLeftNeighbour(jumpHeight, isEnemy);
        GetRightNeighbour(jumpHeight, isEnemy);
    }

    public Tile CheckTile(Vector3 direction, float jumpHeight, bool isEnemy = false)
    {
        Vector3 halfExtents = new Vector3(0.25f, (1 + jumpHeight) / 2.0f, 0.25f); //Range of jumping
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        Tile firstCollider = null;

        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();

            if (tile != null && ((tile.walkablePlayer && !isEnemy) || (tile.walkableEnemy && isEnemy)))
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

    /// <summary>
    /// For enemy, gets the down tile/player gameobject which blocks the enemy
    /// </summary>
    /// <returns></returns>
    public GameObject GetDownGameObject()
    {
        Vector3 direction = -Vector3.forward;

        Vector3 halfExtents = new Vector3(0.25f, 0, 0.25f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();

            if (tile != null)
            {
                RaycastHit hit;
                //Check if block is on front 
                if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1))
                {
                    return tile.gameObject;
                }

                return hit.transform.gameObject;
            }
        }
        return null;
    }

    public void TakeDamage(float amount)
    {
        if (destructible)
        {
            currentHealth -= amount;

            if (currentHealth <= 0)
                Destroy();
        }

        AnimateTileStatus();
    }

    public void Repair(float amount)
    {
        if (destructible)
        {
            currentHealth += amount;

            if (currentHealth > MaxHealth)
                currentHealth = MaxHealth;
        }

        AnimateTileStatus();
    }

    public void AnimateTileStatus()
    {
        int value = 100;

        if (currentHealth / MaxHealth <= 0.25f)
            value = 25;
        else if (currentHealth / MaxHealth <= 0.5f)
            value = 50;
        else if (currentHealth / MaxHealth <= 0.75f)
            value = 75;
        else
            return;

        Material material = Resources.Load<Material>("Tile/" + name + value);

        GetComponent<Renderer>().material = material;
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
