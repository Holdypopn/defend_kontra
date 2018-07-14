using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

//TODO works for now but needs refactoring - Eventbased mechanism?
public class ActionTick
{
    //Defines the damage tick
    private bool callOnFirstCheck;
    private int tick;
    Thread t;
    DateTime threadStart;

    public int TimeToNextTick
    {
        get
        {
            if (t == null || !t.IsAlive)
                return -1;

            return tick - (int)(DateTime.Now - threadStart).TotalMilliseconds;
        }
    }

    public bool IsAction()
    {
        if (t == null || !t.IsAlive)
        {
            t = new Thread(new ThreadStart(wait));
            t.Start();

            //Decides if true is returned on first call or not
            if (callOnFirstCheck)
                return true;
            
            callOnFirstCheck = true; 
            return false;
        }
        else
            return false;
    }

    public bool IsRunning()
    {
        if (t == null || !t.IsAlive)
            return false;

        return true;
    }

public ActionTick(int tick, bool callOnFirstCheck = true)
{
    this.callOnFirstCheck = callOnFirstCheck;
    this.tick = tick;
}

private void wait()
{
    threadStart =  DateTime.Now;
    Thread.Sleep(tick);
}
}

