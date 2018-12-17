using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

//NAMING CONVENTION:
// LIKE_THIS = Constants ; should be made into constant in release
// like_this = variable based on another variable
// likeThis = variable
// Like_This = variable that Should Not be assinged to more than once in release


/// <summary>
/// component of lineForm's, encapsulates and abstracts linerenders
/// </summary>
public class lineSegment : MonoBehaviour
{
    //CONSTANTS
    public int NUM_NODES;
    public int TOUCH_INTERVAL;
    public float TOUCH_RADIUS;
    [Range(0.1f, 10f)]
    public float LINE_WIDTH = 1;
    public float LINE_XSPEED;

    //vars
    public int currentNode = 0;
    public float lineTime = 0;
    public Color segmentColor;
    public Color touchedColor;

    //flags
    public bool isCleared = false; //touched over
    public bool isActive = false; //ready to be touched over

    //components
    public LineRenderer lineRenderer;
    public lineSegment nextSegment;

    //line renderer properties
    private Vector3[] linePositions;
    private Gradient lineGradient;
    private GradientColorKey[] lineColorKeys;
    private GradientAlphaKey[] lineAlphaKeys;

    //delegate
    public delegate void simpleCallback();
    public simpleCallback completedCallback;

    /// <summary>
    /// make segment touchable
    /// </summary>
    public void activate(simpleCallback _completedCallback)
    {
        completedCallback = _completedCallback;
        isActive = true;
    }

    /// <summary>
    /// tracks the segment nodes near an 'active' segment's end
    /// </summary>
    private void checkTouch()
    {
        float sqrDist;
        for (int i = currentNode; i < Mathf.Min(currentNode + TOUCH_INTERVAL,NUM_NODES-1); i++)
        {
            foreach (touchProfile tProf in GameManager.GM.touchProf)
            {
                sqrDist = Mathf.Pow(tProf.transformPosition.x - linePositions[i].x, 2) + Mathf.Pow(tProf.transformPosition.y - linePositions[i].y, 2);
                if (sqrDist < TOUCH_RADIUS)
                {
                    nodeHit(i);
                }
            }
        }
    }

    /// <summary>
    /// updates segment to register touch to an availble node
    /// </summary>
    /// <param name="nodeIndex"></param>
    void nodeHit(int nodeIndex)
    {
        currentNode = nodeIndex + 1;

        float lineFrac = (float)nodeIndex / NUM_NODES;

        //Debug.Log("nodeIndex: " + nodeIndex.ToString() + " " + "lineFrac: " + lineFrac.ToString());

        lineGradient = new Gradient();
        lineGradient.mode = GradientMode.Fixed;
        lineColorKeys = new GradientColorKey[2];
        lineAlphaKeys = new GradientAlphaKey[1];

        lineColorKeys[0].color = touchedColor;
        lineColorKeys[0].time = lineFrac;
        lineColorKeys[1].color = segmentColor;
        lineColorKeys[1].time = 1.0f;

        lineAlphaKeys[0].alpha = 1.0f;
        lineAlphaKeys[0].time = 1.0f;

        lineGradient.SetKeys(lineColorKeys, lineAlphaKeys);
        lineRenderer.colorGradient = lineGradient;
    }

    /// <summary>
    /// Updates the x position according to line type, this must be 
    /// defined for each extension of lineSegment
    /// </summary>
    public virtual void updateLinePos()
    {
        //UPDATE CONSTS
        lineRenderer.positionCount = NUM_NODES;
        lineTime += LINE_XSPEED * Time.deltaTime;
        for (int i = 0; i < NUM_NODES; i++)
        {
            linePositions[i].x = 1;
        }
        lineRenderer.SetPositions(linePositions);
    }
}