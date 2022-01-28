using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using UnityEngine;
using UnityEngine.UI;




public class DialogueMgr : Singleton<DialogueMgr>
{

    #region Basic Properties
    
    private GameObject _root;
    private Text _text;
    //private TimerModel _timerTask;
    private SequenceNode s;
    private GameObject _canvas;

    #endregion

    #region Other Properties

    private float displayTime = 4f;    //text displays time 1f

    #endregion
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="txt"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="continueTime">文字浮现的时间</param>
    public void Log(string txt,float x, float y=20, float continueTime = 5)
    {
        _text.text = txt;
        _root.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
        _root.SetActive(true);
        
        
        s = new SequenceNode();
        s.Append(DelayAction.Allocate(0f, delegate { _root.SetActive(true); }));
        //TODO: animations during the text display time
        s.Append(DelayAction.Allocate(0.1f, ()=>_root.transform.DOPunchScale(Vector3.one, 0.5f).SetEase(Ease.InCubic)));
        //s.Append(DelayAction.Allocate(displayTime, delegate { Debug.Log("display text"); }));
        s.Append(DelayAction.Allocate(displayTime,delegate { _root.SetActive(false); }));

        Setup.Instance.ExecuteNode(s);
    }

    public DialogueMgr()
    {
        _canvas = GameObject.Find("Canvas");
        _root = Resources.Load<GameObject>("Dialogue").Instantiate();
        _root.transform.SetParent(_canvas.transform);
        _text = _root.GetComponent<Text>();
        
        _root.SetActive(false);
        

    }
}
