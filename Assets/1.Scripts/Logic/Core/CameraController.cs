using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public static class CameraController
{
    public static ViewingState viewing_state;
    public static GameObject camera_root;
    public static Camera main_camera;

    public static Action ChangeViewCallback;
    public static Action RotateLeftViewCallback;
    public static Action RotateRightViewCallback;

    public static Action PressADcallBack = null;
    public static Action PressSpacecallBack = null;


    private static Vector3 camera_v3_position = new Vector3(-8.817436f, 3.085193f, 6.41746f);
    private static Vector3 camera_v3_rotation = new Vector3(38.778f,122.111f,0.003f);
    public static void Init()
    {
        main_camera = Camera.main;
        camera_root = main_camera.transform.parent.gameObject;

        viewing_state = ViewingState.V3d;
        main_camera.orthographic = false;
        main_camera.transform.position = camera_v3_position;
        main_camera.transform.eulerAngles = camera_v3_rotation;
        ChangeViewCallback?.Invoke();
    }

    public static void Control()
    {

        switch (viewing_state)
        {
            case ViewingState.V2d:
                RotateV2d();
                break;
            case ViewingState.V3d:
                break;
            default:
                break;
        }

        ChangeView();
    }

    private static int rotate_count = 0;
    public static int RotateCount
    {
        get
        {
            return rotate_count;
        }
        set
        {
            //
            RotateRightViewCallback?.Invoke();
            rotating = true;
            camera_root.transform.DORotate(new Vector3(0, 90 * rotate_count, 0), 0.5f).OnComplete(()=>rotating=false);
            // camera_root.transform.Rotate(0, 90, 0);
            
            PressADcallBack?.Invoke();
            rotate_count = value;
            GlobalStateMachine.Instance.CheckState();
        }
    }

    private static bool rotating = false;
    private static void RotateV2d()
    {
        if ( ! Main.InputActionAliveAD )
        {
            return;
        }
        GlobalStateMachine.Instance.CheckState();
        
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            rotate_count++;
            if (rotate_count>=4)
            {
                rotate_count = 0;
            }
            DOTween.CompleteAll();
            RotateRightViewCallback?.Invoke();
            rotating = true;
            camera_root.transform.DORotate(new Vector3(0, 90 * rotate_count, 0), 0.5f).OnComplete(()=>rotating=false);
            // camera_root.transform.Rotate(0, 90, 0);
            
            PressADcallBack?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            rotate_count--;
            if (rotate_count<0)
            {
                rotate_count = 3;
            }
            DOTween.CompleteAll();
            RotateLeftViewCallback?.Invoke();
            rotating = true;
            camera_root.transform.DORotate(new Vector3(0, 90 * rotate_count, 0), 0.5f).OnComplete(()=>rotating=false);
            // camera_root.transform.Rotate(0, -90, 0);
            
            PressADcallBack?.Invoke();
        }
    }

    private static void ChangeView()
    {
        if ( ! Main.InputActionAliveSpace)
        {
            return;
        }
        
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (SpawnMgr.Instance.isSpawning || rotating)
            {
                return;
            }
            
            PressSpacecallBack?.Invoke();

            switch (viewing_state)
            {
                case ViewingState.V2d:
                    ChangeToView3d();
                    break;
                
                case ViewingState.V3d:
                    ChangeToView2d();
                    break;
                
                default:
                    break;
            }
            


            ChangeViewCallback?.Invoke();
        }
    }

    public static void ChangeToView2d()
    {
        GlobalStateMachine.Instance.CheckState();
        if (viewing_state == ViewingState.V2d)
        {
            return;
        }
        viewing_state = ViewingState.V2d;
        main_camera.orthographic = true;
        //main_camera.transform.localPosition = new Vector3(0, 1, -10);
        DOTween.CompleteAll();
        rotating = true;
        main_camera.transform.DOLocalMove(new Vector3(0, -1, -10), 0.5f);//*********0,1,-10
        main_camera.transform.eulerAngles = Vector3.zero;
        camera_root.transform.DORotate(new Vector3(0, 90 * rotate_count, 0), 0.5f).OnComplete(()=>rotating=false);
    }
    public static void ChangeToView3d()
    {
        GlobalStateMachine.Instance.CheckState();
        if (viewing_state == ViewingState.V3d)
        {
            return;
        }
        viewing_state = ViewingState.V3d;
        main_camera.orthographic = false;
        camera_root.transform.eulerAngles = Vector3.zero;
        //main_camera.transform.localPosition = new Vector3(5.979244f, 6.829037f, -6.04038f);
        //main_camera.transform.eulerAngles = new Vector3(49.711f, -41.459f, 0);
        DOTween.CompleteAll();
        rotating = true;
        main_camera.transform.DOLocalMove(camera_v3_position, 0.5f);
        main_camera.transform.DORotate(camera_v3_rotation, 0.5f).OnComplete(()=>rotating=false);
    }
}

public enum ViewingState
{
    V2d,
    V3d,
}