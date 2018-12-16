using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//profile each touch to associate the constant touch finger id with a position
//needed since unity's touches are just structs, and this allows us to keep track
//of a gesture's current touch position via the stable finger id.
public class touchProfile
{
    public int touchFingerId;
    public Vector3 touchPosition;
    public Vector3 transformPosition;

    public touchProfile(int _touchFingerId, Vector3 _touchPosition)
    {
        touchFingerId = _touchFingerId;
        touchPosition = _touchPosition;
    }
}

/// <summary>
/// Central Reciever for all touch input, and passes recieved data out to:
///     -line collision detection (sqrMagnitude)
///     -Menu buttons
/// </summary>
public class touchController : MonoBehaviour {

    public static int NUM_TOUCHES = 2;
    public static LayerMask touchLayerMask; //set in Awake() the layer which touches are ray cast to.
    private static List<touchProfile> currentTouches; //x,y (z=0) screen coords of touch

    private void Awake()
    {
        touchLayerMask = LayerMask.GetMask("touchLayer");
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        updateTouches();

	}

    //ensures that currentTouches tracks the <NUM_TOUCHES> most recent touches
    //are being observed
    private void updateTouches()
    {
        foreach (Touch touch in Input.touches)
        {
            //if a touch has ended, remove it from current Touches
            if (touch.phase == TouchPhase.Ended)
            {
                removeTouch(touch);
                continue;
            }
            if (touch.phase == TouchPhase.Began)
            {
                addTouch(touch);
                continue;
            }
            if (touch.phase == TouchPhase.Moved)
            {
                moveTouch(touch);
                continue;
            }
        }
        transformTouch();
    }

    //search currentTouches for matching finger id, and remove that touch
    private void removeTouch(Touch touch)
    {
        for (int i = 0; i < currentTouches.Count; i++)
        {
            if (currentTouches[i].touchFingerId == touch.fingerId)
            {
                currentTouches.RemoveAt(i);
                break;
            }
        }
    }

    //add new touch to end of currentTouches, and remove extra touches from from front of currentTouches
    //ensures most recent touches are being considered
    private void addTouch(Touch touch)
    {
        currentTouches.Add(new touchProfile(touch.fingerId, touch.position));

        while (currentTouches.Count > NUM_TOUCHES)
        {
            currentTouches.RemoveAt(0);
        }
    }

    //search currentTouches for matching finger id, and update the current position
    private void moveTouch(Touch touch)
    {
        for (int i = 0; i < currentTouches.Count; i++)
        {
            if (currentTouches[i].touchFingerId == touch.fingerId)
            {
                currentTouches[i].touchPosition = touch.position;
                break;
            }
        }
    }

    //generate profileList, and hand reference back to caller
    public static List<touchProfile> createProfileList()
    {
        currentTouches = new List<touchProfile>();
        return currentTouches;
    }

    private void transformTouch()
    {
        foreach (touchProfile t in currentTouches)
        {
            Ray newRay = Camera.main.ScreenPointToRay(t.touchPosition);
            RaycastHit newHit;
            if (Physics.Raycast(newRay, out newHit, touchLayerMask))
            {
                t.transformPosition = newHit.point;
            }
        }
    }
}
