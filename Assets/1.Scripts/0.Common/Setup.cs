using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Setup : MonoBehaviour
{
    public static Setup Instance;
    private void Awake()
    {
        Instance = this;
        
        /*
         * 初始化引擎
         * 初始化UI,Res
         * 打开SpawnPanel
         */
        GameObject root = new GameObject("GameRoot");
        root.AddComponent4go<GameEngine>()
            .DontDestoryOnLoad();

        
        ResMgr.Instance.Init();
        SelectMgr.Instance.Init();

        GlobalStateMachine.Instance.CheckState();
        //DialogueMgr.Instance.Log("Hello, TT", 0 ,120);


        //var s = new SequenceNode();
        //s.Append(DelayAction.Allocate(1f, () => { Debug.Log("TestDelay"); }));
        //this.Delay(1f,()=>Debug.Log("delay 1f"));
        //this.ExecuteNode(s);
        //s.Execute(Time.deltaTime);
    }
    
}