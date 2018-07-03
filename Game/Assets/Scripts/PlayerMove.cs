using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : Movement
{
    public Swipe SwipeManager;

	// Use this for initialization
	void Start ()
    {
        Init();
	}
	
	// Update is called once per frame
	void Update ()
    {
        Debug.DrawRay(transform.position, transform.forward);
        if (!moving)
        {
            Debug.Log("End of Moving");
            FindSelectableTiles();
            CheckSwipe();
        }
        else
        {
            Move();
        }
	}

    void CheckSwipe()
    {
        //if(Input.GetMouseButtonUp(0))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //    RaycastHit hit;
        //    if(Physics.Raycast(ray, out hit))
        //    {
        //        if(hit.collider.tag.Contains("Tile") || hit.collider.tag.Contains("Wall"))
        //        {
        //            Tile t = hit.collider.GetComponent<Tile>();

        //            if(t.selectable)
        //            {
        //                //Move targets
        //                MoveToTile(t);
        //            }
        //        }
        //    }
        //}

        Tile t = null;

        if (SwipeManager.SwipeDown)
            t = currentTile.GetDownNeighbour(jumpHeight);
        if (SwipeManager.SwipeLeft)
            t = currentTile.GetLeftNeighbour(jumpHeight);
        if (SwipeManager.SwipeRight)
            t = currentTile.GetRightNeighbour(jumpHeight);
        if (SwipeManager.SwipeUp)
            t = currentTile.GetUpNeighbour(jumpHeight);
        
        if (t!= null)
        {
            if (t.selectable)
            {
                //Move targets
                MoveToTile(t);
            }
        }
    }
}
