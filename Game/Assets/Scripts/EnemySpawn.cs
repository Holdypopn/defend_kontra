using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    internal static Dictionary<int, Queue<GameObject>> RowEnemyMap = new Dictionary<int, Queue<GameObject>>()
    {
        {0, new Queue<GameObject>() },
        {1, new Queue<GameObject>() },
        {2, new Queue<GameObject>() },
        {3, new Queue<GameObject>() }
    };
    //Defines the resource tick
    private ActionTick spawnActionTick;
    public int SpawnTick = 10000;

    public GameObject Enemy;

    private GameObject[] rows;

    // Use this for initialization
    void Start()
    {
        spawnActionTick = new ActionTick(SpawnTick);
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnActionTick.IsAction())
        {
            System.Random r = new System.Random();
            SpawnEnemy(r.Next(0, 4));
        }
    }

    private void SpawnEnemy(int row)
    {
        int index = 0;
        foreach (Transform child in transform)
        {
            if (index == row)
            {
                GameObject go = Instantiate(Enemy, child.GetChild(9).position + new Vector3(0, 1.1f), Quaternion.identity); //TODO: child.GetChild(9) ///TODO 1.1f Player size
                RowEnemyMap[index].Enqueue(go);
            }

            index++;
        }
    }

    public static void RemoveGameObject(GameObject go)
    {
        foreach(var item in RowEnemyMap)
        {
            if (item.Value.Count != 0 && item.Value.Peek() == go)
                item.Value.Dequeue();
        }
    }
}
