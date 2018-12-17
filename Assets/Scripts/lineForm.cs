using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//should never be instantiated
public class lineForm : MonoBehaviour{

    //Controls speed for ALL line Fomrs
    public static float ySpeed;
    public List<lineSegment> gameLineSegments;


    /// <summary>
    /// Contstructor For lineform
    /// begins first segment of form
    /// </summary>
    /// <returns>new line form</returns>
    public lineForm()
    {
        gameLineSegments[0].activate(segmenetCompleted);//start first line segment
    }

    //factory method for line form
    public static lineForm CreateLine() {
        return new lineForm();
    }

    //for all lineForms, move by a fixed amount by phsics delta time
    public void updateLineFormPos()
    {
        gameObject.transform.Translate(Vector3.down*Time.fixedDeltaTime*ySpeed);
    }

    /// <summary>
    /// Callback for complete lineSegment,
    /// Removes first segment of the form, and 
    /// then either start the next one, or complete the form
    /// </summary>
    public void segmenetCompleted()
    {
        gameLineSegments.RemoveAt(0);

        if(gameLineSegments.Count == 0)
        {
            finishedForm();
        }
        else
        {
            gameLineSegments[0].activate(segmenetCompleted);
        }
    }

    public void finishedForm()
    {

    }

    
    
}
