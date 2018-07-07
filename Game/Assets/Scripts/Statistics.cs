using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Statistics : MonoBehaviour {

    private int killCount = 0;
	// Use this for initialization
	void Start ()
    {
        Enemy.EnemyKilled += OnEnemyKilled;
	}

    private void OnEnemyKilled()
    {
        killCount++;
        transform.Find("KilledEnemies").Find("Count").GetComponent<Text>().text = killCount.ToString();
    }
}
