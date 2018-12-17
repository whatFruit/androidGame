using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager: MonoBehaviour {

    //Singleton pattern for gameManager
    public static GameManager GM;

    //gloabal neccisities
    public List<touchProfile> touchProf;

    private void Awake()
    {
        if (GM == null)
        {
            GM = this;
        }
        else if (GM != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(GM);
        setupGame();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    /// <summary>
    /// initiates global components/systems
    /// </summary>
    public void setupGame()
    {
        //hook up touchController's sanitized touch location data to GM
        touchProf = touchController.createProfileList();
    }
}
