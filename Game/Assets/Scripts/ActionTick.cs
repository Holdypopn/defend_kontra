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

public ActionTick(int tick, bool callOnFirstCheck = true)
{
    this.callOnFirstCheck = callOnFirstCheck;
    this.tick = tick;
}

private void wait()
{
    Thread.Sleep(tick);
}
}

