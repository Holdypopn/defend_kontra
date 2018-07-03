using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour
{
    private const int deathzone = 50; //deathzone in pixel
    private bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
    private bool isDraging = false;
    private Vector2 startTouch, swipeDelta;
    private PlayerMove selectedPlayer = null;

    public Vector2 SwipeDelta
    {
        get
        {
            var temp = swipeDelta;
            swipeDelta = Vector2.zero;
            return temp;
        }
    }
    public bool SwipeLeft
    {
        get
        {
            var temp = swipeLeft;
            swipeLeft = false;
            return temp;
        }
    }
    public bool SwipeRight
    {
        get
        {
            var temp = swipeRight;
            swipeRight = false;
            return temp;
        }
    }
    public bool SwipeUp
    {
        get
        {
            var temp = swipeUp;
            swipeUp = false;
            return temp;
        }
    }
    public bool SwipeDown
    {
        get
        {
            var temp = swipeDown;
            swipeDown = false;
            return temp;
        }
    }

    public PlayerMove SelectedPlayer
    {
        get
        {
            var temp = selectedPlayer;
            selectedPlayer = null;
            return temp;
        }
    }

    private void Update()
    {
        tap = swipeLeft = swipeRight = swipeUp = swipeDown = false;

        #region Standalone Inputs
        if (Input.GetMouseButtonDown(0))
        {
            tap = true;
            isDraging = true;
            startTouch = Input.mousePosition;

        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDraging = false;
            Reset();
        }
        #endregion

        #region Mobile Inputs

        if (Input.touches.Length > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                tap = true;
                startTouch = Input.touches[0].position;

            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                Reset();
            }
        }

        #endregion

        //Calculate the distance
        swipeDelta = Vector2.zero;
        if (isDraging)
        {
            if (Input.touches.Length > 0)
                swipeDelta = Input.touches[0].position - startTouch;
            else if (Input.GetMouseButton(0))
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
        }

        //Did we cross the deadzone
        if (swipeDelta.magnitude > 50)
        {
            //Get selected Player
            Ray ray = Camera.main.ScreenPointToRay(startTouch);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Player")
                {
                    selectedPlayer = hit.collider.GetComponent<PlayerMove>();
                }
            }

            //Find direction
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                //Left or right
                if (x < 0)
                    swipeLeft = true;
                else
                    swipeRight = true;
            }
            else
            {
                //Up or down
                if (y < 0)
                    swipeDown = true;
                else
                    swipeUp = true;
            }

            Reset();
        }
    }

    private void Reset()
    {
        startTouch = swipeDelta = Vector2.zero;
        isDraging = false;
    }
}