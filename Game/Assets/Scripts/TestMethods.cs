using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMethods : MonoBehaviour {

    public Player p;

	public void TakeDamage()
    {
        p.TakeDamage(1);
    }
}
