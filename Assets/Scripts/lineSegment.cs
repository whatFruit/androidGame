using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

//NAMING CONVENTION:
// LIKE_THIS = Constants ; should be made into constant in release
// like_this = variable based on another variable
// likeThis = variable
// Like_This = variable that Should Not be assinged to more than once in release


public class lineSegment : MonoBehaviour
{

    //==========================feilds==============================================

    delegate Vector3 lineFunction(float time);

    //TO BE MADE CONSTANT
    [Range(2,300)]
    public int NEW_NUM_NODES = 100;
    private int NUM_NODES;
    [Range(0.005f,0.15f)]
    public float NODE_INTERVAL;
    [Range(0.1f,10f)]
    public float LINE_WIDTH = 1;
    private float TOUCH_RADIUS;
    [Range(-0.5f, 0.5f)]
    public float LINE_SPEED;

    //TEMP to assign constant math
    private void UPDATECONST()
    {
        TOUCH_RADIUS = LINE_WIDTH/1.5f;
        NUM_NODES = NEW_NUM_NODES;
    }

    //the sanitized user touch input data
    static List<touchProfile> currentTouches;

    //the line along which linesegments can be spawned
    public Transform spawnerStartT;
    public Transform spawnerEndT;
    //UPDATE
    private Vector3 spawner_Start_Cord;
    private Vector3 spawner_End_Cord;

    //properties of stage
    public SpriteRenderer backGround;//stage
    private float stageHeight;
    private float stageWidth;

    //line function
    public enum LINE_FUNCTION {STRAIGHT, SIN};
    public LINE_FUNCTION linFun = LINE_FUNCTION.STRAIGHT;
    lineFunction currentLineFunction;

    //private float time = 0;

    //gradient properties
    private Vector3[] linePositions;
    private Gradient lineGradient;
    private GradientColorKey[] lineColorKeys;
    private GradientAlphaKey[] lineAlphaKeys;

    //Line Renderer Component
    private LineRenderer lineRenderer;

    //required to display color
    public Material lineRenderMaterial;

    //state of lineSegment
    private int currentIndex = 0;
    private float lineTime = 0; //when 1, line is fully past stage


    //==========================Inialization==============================================

    // Use this for initialization
    void Start()
    {
        UPDATECONST();
        spawner_Start_Cord = spawnerStartT.TransformPoint(Vector3.zero);
        spawner_End_Cord = spawnerEndT.TransformPoint(Vector3.zero);
        stageHeight = backGround.bounds.size.y;
        stageWidth = backGround.bounds.size.x;
        updateLineFunction();
        makeLine(currentLineFunction);
    }

    //takes delegate function to initate all the nodes of give line
    void makeLine(lineFunction drawLineFunc)
    {
        

        lineRenderer = (LineRenderer)gameObject.AddComponent(typeof(LineRenderer));
        lineRenderer.material = lineRenderMaterial;
        lineRenderer.positionCount = NUM_NODES;
        lineRenderer.sortingOrder = 1;
        lineRenderer.numCornerVertices = 90;
        lineRenderer.numCapVertices = 90;
        lineRenderer.endWidth = lineRenderer.startWidth = LINE_WIDTH;
        linePositions = new Vector3[NUM_NODES];

        for (int i = 0; i < NUM_NODES; i++)
        {
            linePositions[i] = drawLineFunc(NODE_INTERVAL*i);
        }
        lineRenderer.SetPositions(linePositions);

        lineGradient = new Gradient();
        lineGradient.mode = GradientMode.Fixed;
        lineColorKeys = new GradientColorKey[1];
        lineAlphaKeys = new GradientAlphaKey[1];

        lineColorKeys[0].color = Color.red;
        lineColorKeys[0].time = 0.0f;

        lineAlphaKeys[0].alpha = 1.0f;
        lineAlphaKeys[0].time = 1.0f;

        lineGradient.SetKeys(lineColorKeys, lineAlphaKeys);
        lineRenderer.colorGradient = lineGradient;
    }

    //called my GameManager to set reference to sanitized touch input
    public static void setTouchProfileList(List<touchProfile> _currentTouchProfiles)
    {
        currentTouches = _currentTouchProfiles;
    }

    //==========================runtime==============================================

    // Update is called once per frame
    void Update()
    {
        UPDATECONST();
        updateLineFunction();
        checkTouch();
        updateLinePos();

        //POST UPDATE BASED ON CONST
        lineRenderer.endWidth = lineRenderer.startWidth = LINE_WIDTH;

        //POST UPDATE BASED ON OTHER VARIABLE
        spawner_Start_Cord = spawnerStartT.TransformPoint(Vector3.zero);
        spawner_End_Cord = spawnerEndT.TransformPoint(Vector3.zero);
        stageHeight = backGround.bounds.size.y;
        stageWidth = backGround.bounds.size.x;
    }

    void updateLineFunction()
    {
        switch (linFun)
        {
            case LINE_FUNCTION.STRAIGHT:
                currentLineFunction = straightLine;
                break;
            case LINE_FUNCTION.SIN:
                currentLineFunction = sinLine;
                break;
            default:
                break;
        }
    }

    //on a perframe based, move line down toward end of line path and update based on line function
    void updateLinePos()
    {
        //UPDATE CONSTS
        linePositions = new Vector3[NUM_NODES];
        lineRenderer.positionCount = NUM_NODES;


        lineTime += LINE_SPEED * Time.deltaTime;
        for (int i = 0; i < NUM_NODES; i++)
        {
            linePositions[i] = currentLineFunction(NODE_INTERVAL * i) + (Vector3.down * lineTime * stageHeight * 2);
        }
        lineRenderer.SetPositions(linePositions);
    }

    //checks to see if user is touching any nodes by
    //measuring distance between currentTouchProfiles and every node's position
    void checkTouch()
    {
        float sqrDist;
        for (int i = 0; i < NUM_NODES; i++)
        {
            foreach (touchProfile tProf in currentTouches)
            {
                sqrDist = Mathf.Pow(tProf.transformPosition.x - linePositions[i].x, 2) + Mathf.Pow(tProf.transformPosition.y - linePositions[i].y, 2);
                if (sqrDist < TOUCH_RADIUS)
                {
                    nodeHit(i);
                }
            }
        }
    }

    //handles event when touch close to a line node
    void nodeHit(int nodeIndex)
    {
        nodeIndex++;
        if (nodeIndex > currentIndex)
        {

            currentIndex = nodeIndex;

            float lineFrac = (float)nodeIndex/NUM_NODES;

            Debug.Log("nodeIndex: " + nodeIndex.ToString() + " " + "lineFrac: " + lineFrac.ToString());

            lineGradient = new Gradient();
            lineGradient.mode = GradientMode.Fixed;
            lineColorKeys = new GradientColorKey[2];
            lineAlphaKeys = new GradientAlphaKey[1];

            lineColorKeys[0].color = Color.grey;
            lineColorKeys[0].time = lineFrac;
            lineColorKeys[1].color = Color.red;
            lineColorKeys[1].time = 1.0f;

            lineAlphaKeys[0].alpha = 1.0f;
            lineAlphaKeys[0].time = 1.0f;

            lineGradient.SetKeys(lineColorKeys, lineAlphaKeys);
            lineRenderer.colorGradient = lineGradient;

        }
    }

    //============================A.1 Delgate functions===============================

    [Header("General Function Properties")]
    [Range(0.01f, 30)]
    public float lineLength;

    //draws a single straight line
    Vector3 straightLine(float time)
    {
        Vector3 midPoint = Vector3.Lerp(spawner_Start_Cord, spawner_End_Cord, 0.5f);
        midPoint.y += lineLength * time;
        return midPoint;
    }


    [Header("Sin Function Properties")]
    [Range(0.01f,100)]
    public float sinAmplitude;
    [Range(0.01f, 100)]
    public float sinOffset;
    [Range(0.01f, 100)]
    public float sinWaveCons;
    //draws a sin line
    Vector3 sinLine(float time)
    {
        Vector3 midPoint = Vector3.Lerp(spawner_Start_Cord, spawner_End_Cord, 0.5f);
        midPoint.y += lineLength * time;
        midPoint.x += (float)sinAmplitude * Mathf.Sin(time * sinWaveCons + sinOffset * Time.time);
        return midPoint;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < NUM_NODES; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(linePositions[i], TOUCH_RADIUS);
        }

        foreach (touchProfile t in currentTouches)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(t.transformPosition, 0.1f);
        }
    }
}
