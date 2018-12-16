using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class lineForm{

    //variables needed for all line segments
    public float yLength;
    public float arcLength;
    public float xOffset;
    public int NUM_NODES;

    //factory method for line form
    public abstract lineForm CreateLine();

    
    
}
