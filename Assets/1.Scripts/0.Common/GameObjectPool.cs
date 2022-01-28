using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameObjectPool
{
    private string _poolName;
    private string _path;
    private int _initSize;


    private Stack<GameObject> _objStack;

    public GameObjectPool(string poolName, string path, int initSize)
    {
        this._poolName = poolName;
        this._path = path;
        this._initSize = initSize;
        _objStack = new Stack<GameObject>(initSize);
    }

    public GameObject Get()
    {
        if (_objStack.Count == 0)
        {
            _objStack.Push(ResMgr.Instance.GetResources<GameObject>(_path));
        }
        GameObject go = null;
        go = _objStack.Pop();
        return go;
    }

    public void Release(GameObject go) => _objStack.Push(go);
}