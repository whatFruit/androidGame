using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private void Awake()
    {
        //hook up touchController's sanitized touch location data to lineSegment
        lineSegment.setTouchProfileList(touchController.createProfileList());
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
