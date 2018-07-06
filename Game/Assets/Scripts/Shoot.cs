using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject Projectile;
    public float velocity = 40;
    public Vector3 Offset = new Vector3(0, 0, 0.5f);
    
    // Update is called once per frame
    public bool Shooting(int row, float Damage)
    {
        if(EnemySpawn.RowEnemyMap[row].Count != 0)
        {
            GameObject go = (GameObject)Instantiate(Projectile, transform.position + Offset, Quaternion.identity);
            var dir = EnemySpawn.RowEnemyMap[row].Peek().transform.position - transform.position;
            go.GetComponent<Rigidbody>().AddForce(dir * velocity);

            go.GetComponent<Bullet>().Damage = Damage;

            Destroy(go, 3);
            return true;
        }

        return false;
    }
}
