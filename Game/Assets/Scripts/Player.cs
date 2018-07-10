using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Movement, IDestructible
{
    public PlayerPropertys PlayerPropertys = new PlayerPropertys();

    //Defines the resource tick
    private ActionTick resourceActionTick;
    public int resourceTick;
    public int BaseResourceTick = 1000;

    internal delegate void PlayerSelected(Player player);
    internal static event PlayerSelected PlayerSelect;

    internal delegate void InformationUpdate(Player player);
    internal event InformationUpdate InformationUpdated;

    internal delegate void PlayerDie(Player player);
    internal event PlayerDie PlayerDies;

    //Defines the Repair Tick
    private ActionTick repairActionTick;
    internal int repairTick;
    public int BaseRepairTick = 1000;

    //Defines the Repair Tick
    private ActionTick shootActionTick;
    public int BaseShootTick = 500;
    internal int shootTick;
    internal float damage;
    public float BaseDamage = 1.5f;

    public float RepairEfficiencyPerStone = 0.5f;

    /// <summary>
    /// Resource manager
    /// </summary>
    public Resource Resources;
    public int StartStones = 5;
    public int StartAmmo = 20;

    public Swipe SwipeManager;
    private Animator animator;

    public float BaseMaxHealth = 4;
    internal float maxHealth;
    public float currentHealth;

    public Image healthBar;

    private Tile wallGround = null;

    // Use this for initialization
    void Start()
    {
        maxHealth = BaseMaxHealth;
        shootTick = BaseShootTick;
        repairTick = BaseRepairTick;
        resourceTick = BaseResourceTick;
        damage = BaseDamage;
        currentHealth = maxHealth;
        
        Init();
        
        resourceActionTick = new ActionTick(resourceTick);
        repairActionTick = new ActionTick(repairTick);
        shootActionTick = new ActionTick(shootTick);

        Resources = new Resource(StartStones, StartAmmo, transform);

        animator = transform.GetComponent<Animator>();

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
                        break;
                    case "StoneResourceTile":
                        if (resourceActionTick.IsAction())
                        {
                            Resources.AddStone();
                            OnInformationUpdated();
                            animator.SetTrigger("PickUp");
                        }
                        break;
                    case "AmmoResourceTile":
                        if (resourceActionTick.IsAction())
                        {
                            Resources.AddAmmo();
                            OnInformationUpdated();
                            animator.SetTrigger("PickUp");
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
                                        animator.SetTrigger("Repair");
                                    }
                                }
                            }
                        }
                        break;
                    case "BaseTile":
                        break;
                    case "Wall":
                        if (shootActionTick.IsAction())
                        {
                            if (Resources.Ammo > 0)
                            {
                                int row = Int32.Parse(currentTile.transform.parent.name.Split('(')[1].Split(')')[0]);//TODO refactor read of row

                                if (transform.GetComponent<Shoot>().Shooting(row, damage))
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
            animator.Play("Run");
        }
    }

    void CheckSwipe()
    {
        Tile t = null;

        if (SwipeManager.SwipeDown)
            t = currentTile.GetRightNeighbour(jumpHeight);

        if (SwipeManager.SwipeLeft)
            t = currentTile.GetDownNeighbour(jumpHeight);

        if (SwipeManager.SwipeRight)
            t = currentTile.GetUpNeighbour(jumpHeight);

        if (SwipeManager.SwipeUp)
            t = currentTile.GetLeftNeighbour(jumpHeight);

        if (SwipeManager.SelectedPlayer == this)
        {
            if (t != null)
            {
                //Move targets
                MoveToTile(t);
            }
        }
    }

    void CheckTab()
    {
        if (SwipeManager.Tap && SwipeManager.SelectedPlayer == this)
        {
            SwipeManager.SelectedPlayer = null;
            if (PlayerSelect != null)
            {
                PlayerSelect(this);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        healthBar.fillAmount = currentHealth / maxHealth;

        PlayerInformation.UpdateStats();

        if (currentHealth <= 0)
        {
            Debug.Log("Player die!");
            Die();
        }
    }

    public void Heal()
    {
        currentHealth += PlayerPropertys.HealPercentage * maxHealth;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthBar.fillAmount = currentHealth / maxHealth;
        PlayerInformation.UpdateStats();
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

    public void UpdateMoveSpeed()
    {
        moveSpeed = BaseMoveSpeed + (BaseMoveSpeed * PlayerPropertys.MoveSpeedLevel * PlayerPropertys.MoveSpeedPercentage);
    }

    public void UpdateHealth()
    {
        maxHealth = BaseMaxHealth + (BaseMaxHealth * PlayerPropertys.HealthLevel * PlayerPropertys.HealthPercentage);
        healthBar.fillAmount = currentHealth / maxHealth;
    }

    public void UpdateFireRate()
    {
        shootTick = BaseShootTick - (int)(BaseShootTick * PlayerPropertys.FireRateLevel * PlayerPropertys.FireRatePercentage);
        shootActionTick = new ActionTick(shootTick);
    }

    public void UpdateDamage()
    {
        damage = BaseDamage + (BaseDamage * PlayerPropertys.DamageLevel * PlayerPropertys.DamagePercentage);
    }

    public void UpdateRepairSpeed()
    {
        repairTick = BaseRepairTick - (int)(BaseRepairTick * PlayerPropertys.RepairSpeedLevel * PlayerPropertys.RepairSpeedPercentage);
        repairActionTick = new ActionTick(shootTick);
    }

    public void UpdateResourceSpeed()
    {
        resourceTick = BaseResourceTick - (int)(BaseResourceTick * PlayerPropertys.ResourceSpeedLevel * PlayerPropertys.ResourceSpeedPercentage);
        resourceActionTick = new ActionTick(resourceTick);
    }
}
