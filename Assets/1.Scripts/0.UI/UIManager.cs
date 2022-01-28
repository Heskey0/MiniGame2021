using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    //UI管理者
    public GameObject _uiRoot;

    //存储所有层的根物体
    //层次 根节点
    Dictionary<UILayer, GameObject> _uiLayerRootDic = new Dictionary<UILayer, GameObject>();
    public void Init()
    {
        if (_uiRoot == null)
        {
            _uiRoot = ResMgr.Instance.GetInstance("UI/UIRoot");
            _uiRoot.Name("UIRoot").DontDestoryOnLoad();
        }
        
        _uiLayerRootDic.Add(UILayer.Bg, _uiRoot.transform.Find("Canvas/Bg").gameObject);
        _uiLayerRootDic.Add(UILayer.Normal, _uiRoot.transform.Find("Canvas/Normal").gameObject);
        _uiLayerRootDic.Add(UILayer.Top, _uiRoot.transform.Find("Canvas/Top").gameObject);
        
    }



    public GameObject Add(string uiPath,UILayer layer)
    {
        var root = ResMgr.Instance.GetInstance(uiPath);
        root.transform.SetParent(_uiLayerRootDic[layer].transform, false);
        return root;
    }

    public void RemoveLayer(UILayer layer = UILayer.Normal)
    {
        _uiLayerRootDic[layer].DestroyAllChildren();
    }
    public void Remove(GameObject ui)
    {
        ResMgr.Instance.Release(ui);
    }

    /// <summary>
    /// 替换ui
    /// </summary>
    /// <param name="uiPath"></param>
    /// <param name="layer">默认值为Normal</param>
    public GameObject Replace(string uiPath, UILayer layer = UILayer.Normal)
    {
        RemoveLayer(layer);
        return Add(uiPath,layer);
    }
}

public enum UILayer
{
    Bg,
    Normal,
    Top
}