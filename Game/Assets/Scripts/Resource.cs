using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resource
{
    public int Stone;
    public int Ammo;

    public Resource(int stone, int ammo, Transform transform)
    {
        Stone = stone;
        Ammo = ammo;

        //UpdateUi();
    }

    internal void AddStone()
    {
        Stone++;
    }

    //private void UpdateUi()
    //{
    //    transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>().text = Stone.ToString(); //UI Text Stone
    //    transform.GetChild(0).GetChild(1).GetChild(3).GetComponent<Text>().text = Ammo.ToString(); //Ui Text Ammo
    //}

    public void UseStone()
    {
        Stone--;
        //UpdateUi();
    }

    public void UseAmmo()
    {
        Ammo--;
        //UpdateUi();
    }

    internal void AddAmmo()
    {
        Ammo++;
    }
}
