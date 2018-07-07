using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Statistics : MonoBehaviour
{
    private int killCount = 0;
    private float Credits = 0;
	// Use this for initialization
	void Start ()
    {
        Enemy.EnemyKilled += OnEnemyKilled;
	}

    private void OnEnemyKilled(EnemyKillEventArgs args)
    {
        killCount++;
        Credits += args.Enemy.MaxHealth;
        transform.Find("KilledEnemies").Find("Count").GetComponent<Text>().text = killCount.ToString();
        transform.Find("Credits").Find("Count").GetComponent<Text>().text = Credits.ToString();
    }
}
