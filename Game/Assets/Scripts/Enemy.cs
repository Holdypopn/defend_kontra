using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Movement, IDestructible
{
    //Defines the damage tick
    private ActionTick actionTick;
    public float DamageTick = 0.9f;

    public Swipe SwipeManager;

    public float Damage = 1.5f;

    public float MaxHealth = 10;
    private float currentHealth;

    /// <summary>
    /// Is set when a destructible is in front of enemy
    /// </summary>
    private IDestructible destructibleInFront = null;

    public Image healthBar;

    // Use this for initialization
    void Start()
    {
        Init();

        currentHealth = MaxHealth;

        actionTick = new ActionTick(DamageTick);
    }

    // Update is called once per frame
    void Update()
    {
        if (destructibleInFront != null)
        {
            try //TODO: remove try catch and return bool if player is death
            {
                if (actionTick.IsAction())
                {
                    destructibleInFront.TakeDamage(Damage);
                }
            }
            catch
            {
                destructibleInFront = null;
            }
        }
        else if (!moving)
        {
            FindSelectableTiles(true);

            GetMoveOrAttackTile();//Moves the enemy or sets the destructibleInFront

            if (currentTile.tag == "BaseTile")
            {
                Gameover();
            }
        }
        else
        {
            Move();
        }
    }

    private void Gameover()
    {
        Debug.Log("TODO: GAME OVER!");
    }

    void GetMoveOrAttackTile()
    {
        Tile t = currentTile.GetDownNeighbour(jumpHeight, true);

        if (t != null)
        {
            MoveToTile(t);
        }
        else
        {
            var go = currentTile.GetGameObject(-Vector3.forward); //Get Gameobject in front

            if (go == null)//Nothing in Front, reach end of map
                return;

            var isEnemy = go.GetComponent<Enemy>() != null; //Avoid enemy attack enemy
            var destructible = go.GetComponent<IDestructible>();

            if (destructible != null && !isEnemy)
            {
                destructibleInFront = destructible;
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
