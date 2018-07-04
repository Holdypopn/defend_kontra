using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Movement, IDestructible
{
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
    }

    // Update is called once per frame
    void Update()
    {
        if(destructibleInFront != null)
        {
            try //TODO: remove try catch and return bool if player is death
            {
                destructibleInFront.TakeDamage(Damage);
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
        Debug.Log("Curr: " + currentTile.name);
        Tile t = currentTile.GetDownNeighbour(jumpHeight, true);

        Debug.Log("t: " + t);

        if (t != null)
        {
            MoveToTile(t);
        }
        else
        {
            var go = currentTile.GetDownGameObject();

            if(go == null)//Nothing in Front, reach end of map
                return;

            var isEnemy = go.GetComponent<Enemy>() != null; //Avoid enemy attack enemy
            var destructible = go.GetComponent<IDestructible>();

            if(destructible != null && !isEnemy)
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
