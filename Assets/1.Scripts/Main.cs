using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Main : MonoBehaviour
{
    public Text txt_cur_view;
    //can receive input action ?
    //public static bool InputActionAlive = true;

    public static bool InputActionEditable = true;
    public static bool InputActionAlive
    {
        get
        {
            return InputActionAliveAD && InputActionAliveSpace;
        }
        set
        {
            if (InputActionEditable)
            {
                InputActionAliveAD = value; 
                InputActionAliveSpace = value;
            }
        }
    }

    public static bool InputActionAliveAD = true;
    public static bool InputActionAliveSpace = true;
    void Start()
    {
        CameraController.ChangeViewCallback += delegate { txt_cur_view.text = CameraController.viewing_state.ToString(); };
        CameraController.Init();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            InputActionAlive = !InputActionAlive;
        }
        CameraController.Control();
    }
}
