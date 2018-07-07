using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Statistics : MonoBehaviour
{
    public static int killCount = 0;
    public static float Credits = 100;
    private static Transform t;
	// Use this for initialization
	void Start ()
    {
        Enemy.EnemyKilled += OnEnemyKilled;
        t = transform;
	}

    private void OnEnemyKilled(EnemyKillEventArgs args)
    {
        killCount++;
        Credits += args.Enemy.MaxHealth;
        UpdateUi(transform);
        PlayerInformation.UpdateStats();
    }

    private static void UpdateUi(Transform t)
    {
        t.Find("KilledEnemies").Find("Count").GetComponent<Text>().text = killCount.ToString();
        t.Find("Credits").Find("Count").GetComponent<Text>().text = Credits.ToString();
    }


    public static bool Pay(float credits)
    {
        if (credits <= Credits)
        {
            Credits -= credits;
            UpdateUi(t);
            return true;
        }

        return false;
    }
}
