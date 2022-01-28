using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    public static GameObject Go = null;
    public static int Layer = 15;

    void Start()
    {
        Go = gameObject;
        gameObject.Layer(Layer);
    }
    
}