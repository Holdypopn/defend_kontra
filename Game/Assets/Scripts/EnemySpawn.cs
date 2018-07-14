using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawn : MonoBehaviour
{
    public int Wave = 1;
    internal static Dictionary<int, Queue<GameObject>> RowEnemyMap = new Dictionary<int, Queue<GameObject>>()
    {
        {0, new Queue<GameObject>() },
        {1, new Queue<GameObject>() },
        {2, new Queue<GameObject>() },
        {3, new Queue<GameObject>() },
        {4, new Queue<GameObject>() },
    };
    //Defines the resource tick
    private ActionTick spawnEnemyActionTick;
    private ActionTick waveActionTick;
    private ActionTick waveDurationActionTick;
    public Transform WaveDisplay;

    public int WaveTick = 60000;
    public int WaveDuration = 15000;

    public GameObject Enemy;

    private GameObject[] rows;

    // Use this for initialization
    void Start()
    {
        waveActionTick = new ActionTick(WaveTick, false);
    }

    // Update is called once per frame
    void Update()
    {
        //New Wave starts
        if (waveActionTick != null && waveActionTick.IsAction())
        {
            waveDurationActionTick = new ActionTick(WaveDuration, false);
            spawnEnemyActionTick = new ActionTick(1000, true);
            waveDurationActionTick.IsAction(); //First call starts thread
            waveActionTick = null;

            WaveDisplay.Find("Text").GetComponent<Text>().text = "Wave " + Wave + " starts ...";
            WaveDisplay.gameObject.SetActive(true);
        }

        //Disable wave display after 3 seconds
        if (waveDurationActionTick != null && waveDurationActionTick.TimeToNextTick < WaveDuration - 3000
            && WaveDisplay.gameObject.activeInHierarchy)
        {
            WaveDisplay.gameObject.SetActive(false);
        }


        //Execute enemy spawning during wave
        if (waveDurationActionTick != null && spawnEnemyActionTick != null &&
            waveDurationActionTick.IsRunning() && spawnEnemyActionTick.IsAction())
        {
            System.Random r = new System.Random();
            spawnEnemyActionTick = new ActionTick(r.Next(4000, 8000), false);

            SpawnEnemy(r.Next(0, 5));
        }

        //Wave ends
        if (waveDurationActionTick != null && !waveDurationActionTick.IsRunning())
        {
            spawnEnemyActionTick = null;
            waveDurationActionTick = null;
            waveActionTick = new ActionTick(WaveTick, false);
        }

        //Displaying wave status
        if(waveActionTick != null && waveActionTick.TimeToNextTick < 30000 &&
            waveActionTick.TimeToNextTick > 27000 && !WaveDisplay.gameObject.activeInHierarchy)
        {
            WaveDisplay.Find("Text").GetComponent<Text>().text = "Next Wave in 30 seconds";
            WaveDisplay.gameObject.SetActive(true);
        }
        else if (waveActionTick != null && waveActionTick.TimeToNextTick < 27000)
        {
            WaveDisplay.gameObject.SetActive(false);
        }
    }

    private void SpawnEnemy(int row)
    {
        int index = 0;
        foreach (Transform child in transform)
        {
            if (index == row)
            {
                GameObject go = Instantiate(Enemy, child.GetChild(16).position + new Vector3(0, 1.1f), Quaternion.identity); //TODO: child.GetChild(9) ///TODO 1.1f Player size
                RowEnemyMap[index].Enqueue(go);
            }

            index++;
        }
    }

    public static void RemoveGameObject(GameObject go)
    {
        foreach (var item in RowEnemyMap)
        {
            if (item.Value.Count != 0 && item.Value.Peek() == go)
                item.Value.Dequeue();
        }
    }
}
