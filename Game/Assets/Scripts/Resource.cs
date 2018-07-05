using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resource
{
    public int Stone;
    public int Ammo;
    private Transform transform;

    public Resource(int stone, int ammo, Transform transform)
    {
        Stone = stone;
        Ammo = ammo;
        this.transform = transform;

        UpdateUi();
    }

    internal void AddRandomResource()
    {
        System.Random rnd = new System.Random();
        int number = rnd.Next(1, 4);

        switch (number)
        {
            case 1:
                Ammo++;
                Debug.Log("Add Ammo");
                break;
            case 2:
                Stone++;
                Debug.Log("Add Stone");
                break;
            case 3://Fail nothing
                break;
        }

        UpdateUi();
    }

    private void UpdateUi()
    {
        transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>().text = Stone.ToString(); //UI Text Stone
        transform.GetChild(0).GetChild(1).GetChild(3).GetComponent<Text>().text = Ammo.ToString(); //Ui Text Ammo
    }

    public void UseStone()
    {
        Stone--;
        UpdateUi();
    }

    public void UseAmmo()
    {
        Ammo--;
        UpdateUi();
    }
}
