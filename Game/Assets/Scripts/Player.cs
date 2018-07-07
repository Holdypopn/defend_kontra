using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Movement, IDestructible
{
    //Defines the resource tick
    private ActionTick resourceActionTick;
    public int ResourceTick = 1000;

    internal delegate void PlayerSelected(Player player);
    internal static event PlayerSelected PlayerSelect;

    internal delegate void InformationUpdate(Player player);
    internal event InformationUpdate InformationUpdated;

    internal delegate void PlayerDie(Player player);
    internal event PlayerDie PlayerDies;

    //Defines the Repair Tick
    private ActionTick repairActionTick;
    public int RepairTick = 1000;

    //Defines the Repair Tick
    private ActionTick shootActionTick;
    public int ShootTick = 500;
    public float Damage = 1.5f;

    public float RepairEfficiencyPerStone = 0.5f;

    /// <summary>
    /// Resource manager
    /// </summary>
    public Resource Resources;
    public int StartStones = 5;
    public int StartAmmo = 20;

    public Swipe SwipeManager;

    public float MaxHealth = 4;
    private float currentHealth;

    public Image healthBar;

    private Tile wallGround = null;

    // Use this for initialization
    void Start()
    {
        Init();

        currentHealth = MaxHealth;

        resourceActionTick = new ActionTick(ResourceTick);
        repairActionTick = new ActionTick(RepairTick);
        shootActionTick = new ActionTick(ShootTick);

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
            CheckTab();

            if (currentTile == null)//Object is deleted (Wall)
            {
                MoveToTile(wallGround);
            }
            else
            {

                switch (currentTile.tag)
                {
                    case "WallTile":
                        Debug.Log("WallTile");
                        break;
                    case "ResourceTile":
                        if (resourceActionTick.IsAction())
                        {
                            Resources.AddRandomResource();
                            OnInformationUpdated();
                        }
                        break;
                    case "RepairTile":
                        if (repairActionTick.IsAction())
                        {
                            var go = currentTile.GetGameObject(Vector3.forward); //Get Gameobject in front (Wall)

                            if (go != null)//Wall already down when wall is null
                            {
                                var wall = go.GetComponent<Tile>();

                                if (wall != null && Resources.Stone > 0)//Avoid enemy attack and stones available
                                {
                                    if (wall.Repair(RepairEfficiencyPerStone))
                                    {
                                        OnInformationUpdated();
                                        Resources.UseStone();
                                    }
                                }
                            }
                        }
                        break;
                    case "BaseTile":
                        Debug.Log("BaseTile");
                        break;
                    case "Wall":
                        if (shootActionTick.IsAction())
                        {
                            if (Resources.Ammo > 0)
                            {
                                int row = Int32.Parse(currentTile.transform.parent.name.Split('(')[1].Split(')')[0]);//TODO refactor read of row

                                if (transform.GetComponent<Shoot>().Shooting(row, Damage))
                                {
                                    OnInformationUpdated();
                                    Resources.UseAmmo();
                                }
                            }
                        }

                        wallGround = currentTile.GetWallGroundOfRow();
                        break;
                }
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
            if (t != null)
            {
                PlayerSelect(this);

                //Move targets
                MoveToTile(t);
            }
        }
    }

    void CheckTab()
    {
        if (SwipeManager.Tap && SwipeManager.SelectedPlayer == this)
        {
            if (PlayerSelect != null)
            {
                PlayerSelect(this);
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
        OnPlayerDies();
        Destroy(gameObject);
    }

    private void OnInformationUpdated()
    {
        if (InformationUpdated != null)
            InformationUpdated(this);
    }

    private void OnPlayerDies()
    {
        if (PlayerDies != null)
            PlayerDies(this);
    }
}
