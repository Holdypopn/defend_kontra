﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Movement, IDestructible
{
    //Defines the damage tick
    private ActionTick actionTick;
    public int DamageTick = 800;

    public float Damage = 1.5f;

    public float MaxHealth = 10;
    internal float CurrentHealth;

    internal delegate void EnemyKill(EnemyKillEventArgs args);
    internal static event EnemyKill EnemyKilled;
    /// <summary>
    /// Is set when a destructible is in front of enemy
    /// </summary>
    private IDestructible destructibleInFront = null;

    public Image healthBar;

    // Use this for initialization
    void Start()
    {
        Init();

        CurrentHealth = MaxHealth;

        actionTick = new ActionTick(DamageTick);
    }

    // Update is called once per frame
    void Update()
    {
        if (destructibleInFront != null)
        {
            try //TODO: remove try catch and return bool if player is death
            {
                if (currentTile.GetGameObject(-Vector3.forward) == null) //Gameobject is not there anymore ?? NOT WORKING TODO
                    destructibleInFront = null;
                else if (actionTick.IsAction())
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

        if (CurrentHealth <= 0)
        {
            Die();
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
        CurrentHealth -= amount;
        healthBar.fillAmount = CurrentHealth / MaxHealth;

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Die!");
        if (moving) //Reset tile which is in use
        {
            Path.Peek().MovementTo = false;
        }
        EnemySpawn.RemoveGameObject(gameObject);
        OnEnemyKilled();

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        var bullet = collision.gameObject.GetComponent<Bullet>();

        if(bullet != null)
        {
            TakeDamage(bullet.Damage);
            GameObject.Destroy(collision.gameObject);
        }
    }

    private void OnEnemyKilled()
    {
        if(EnemyKilled != null)
        {
            EnemyKilled(new EnemyKillEventArgs(this));
        }
    }
}

public class EnemyKillEventArgs : EventArgs
{
    internal Enemy Enemy;

    public EnemyKillEventArgs (Enemy enemy)
    {
        this.Enemy = enemy;
    }
}