﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject Projectile;
    private float velocity = 40;
    private Vector3 Offset = new Vector3(0, 0.6f, 0.2f);

    private bool bulletIsOnWay;
    private float oldDamage;
    Enemy enemy;

    private bool init = true;
    private float temporaryLive;

    // Update is called once per frame
    public bool Shooting(int row, float Damage)
    {
        //New enemy
        if(EnemySpawn.RowEnemyMap[row].Count != 0 && enemy != EnemySpawn.RowEnemyMap[row].Peek().GetComponent<Enemy>())
        {
            init = true;
        }

        if (EnemySpawn.RowEnemyMap[row].Count != 0)
        {
            if (init)
            {
                temporaryLive = EnemySpawn.RowEnemyMap[row].Peek().GetComponent<Enemy>().CurrentHealth;
                enemy = EnemySpawn.RowEnemyMap[row].Peek().GetComponent<Enemy>();
                init = false;
            }
            if (temporaryLive <= 0) //Bullet is on the way which kills the player
            {
                return false;
            }
            else
            {
                temporaryLive -= Damage;

                GameObject go = (GameObject)Instantiate(Projectile, transform.position + Offset, Quaternion.identity);
                var dir = EnemySpawn.RowEnemyMap[row].Peek().transform.position - Offset - transform.position;
                go.GetComponent<Rigidbody>().AddForce(dir * velocity);

                go.GetComponent<Bullet>().Damage = Damage;

                Destroy(go, 10);
                return true;
            }
        }
        
        init = true;
        return false;
    }
}
