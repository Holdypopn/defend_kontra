﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject Projectile;
    public float velocity = 40;
    public Vector3 Offset = new Vector3(0, 0, 0.5f);

    private bool bulletIsOnWay;
    private float oldDamage;

    // Update is called once per frame
    public bool Shooting(int row, float Damage)
    {
        if (EnemySpawn.RowEnemyMap[row].Count != 0)
        {
            if (bulletIsOnWay && EnemySpawn.RowEnemyMap[row].Peek().GetComponent<Enemy>().CurrentHealth <= oldDamage) //Bullet is on the way which kills the player
            {
                return false;
            }
            else
            { 
                bulletIsOnWay = true;
                oldDamage = Damage;

                GameObject go = (GameObject)Instantiate(Projectile, transform.position + Offset, Quaternion.identity);
                var dir = EnemySpawn.RowEnemyMap[row].Peek().transform.position - transform.position;
                go.GetComponent<Rigidbody>().AddForce(dir * velocity);

                go.GetComponent<Bullet>().Damage = Damage;

                Destroy(go, 3);
                return true;
            }
        }

        bulletIsOnWay = false;
        oldDamage = 0;
        return false;
    }
}
