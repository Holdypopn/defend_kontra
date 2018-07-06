using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

//TODO works for now but needs refactoring
public class ActionTick
{
    //Defines the damage tick
    public int tick = 1000;

    Thread t;

    public bool IsAction()
    {
        if (t == null || !t.IsAlive)
        {
            t = new Thread(new ThreadStart(wait));
            t.Start();
            return true;
        }
        else
            return false;
    }

    public ActionTick(int tick)
    {
        this.tick = tick;
    }

    private void wait()
    {
        Thread.Sleep(tick);
    }
}

