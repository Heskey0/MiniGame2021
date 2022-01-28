using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasCheck : MonoBehaviour
{
    public static bool hitUI;
    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            hitUI = true;
        }

        hitUI = false;
    }
}