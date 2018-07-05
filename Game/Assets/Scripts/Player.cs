using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Movement, IDestructible
{
    //Defines the damage tick
    private ActionTick resourceActionTick;
    public float ResourceTick = 0.9f;

    /// <summary>
    /// Resource manager
    /// </summary>
    private Resource Resources;
    public int StartStones = 5;
    public int StartAmmo = 20;

    public Swipe SwipeManager;

    public float MaxHealth = 4;
    private float currentHealth;

    public Image healthBar;

    // Use this for initialization
    void Start()
    {
        Init();

        currentHealth = MaxHealth;

        resourceActionTick = new ActionTick(ResourceTick);

        Resources = new Resource(StartStones, StartAmmo, transform);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward);
        if (!moving)
        {
            FindSelectableTiles();
            CheckSwipe();
            
            switch (currentTile.tag)
            {
                case "WallTile":
                    Debug.Log("WallTile");
                    break;
                case "ResourceTile":
                    if(resourceActionTick.IsAction())
                    {
                        Resources.AddRandomResource();
                    }
                    break;
                case "RepairTile":
                    break;
                case "BaseTile":
                    Debug.Log("BaseTile");
                    break;
                case "Wall":
                    Debug.Log("Wall");
                    break;
            }
        }
        else
        {
            Move();
        }
    }

    void CheckSwipe()
    {
        Tile t = null;

        if (SwipeManager.SwipeDown)
            t = currentTile.GetDownNeighbour(jumpHeight);
        if (SwipeManager.SwipeLeft)
            t = currentTile.GetLeftNeighbour(jumpHeight);
        if (SwipeManager.SwipeRight)
            t = currentTile.GetRightNeighbour(jumpHeight);
        if (SwipeManager.SwipeUp)
            t = currentTile.GetUpNeighbour(jumpHeight);

        if (SwipeManager.SelectedPlayer == this)
        {
            SwipeManager.SelectedPlayer = null;
            if (t != null)
            {
                //Move targets
                MoveToTile(t);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        healthBar.fillAmount = currentHealth / MaxHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
