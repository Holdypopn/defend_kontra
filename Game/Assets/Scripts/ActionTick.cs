using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ActionTick
{
    //Defines the damage tick
    private float nextActionTime = 0.0f;
    public float tick = 0.9f;

    public bool IsAction()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime += tick;

            return true;
        }

        return false;
    }

    public ActionTick(float tick)
    {
        this.tick = tick;
    }
}

