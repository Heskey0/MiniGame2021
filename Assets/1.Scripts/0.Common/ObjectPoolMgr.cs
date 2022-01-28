using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPoolMgr : Singleton<ObjectPoolMgr>
{
    private Dictionary<string, GameObjectPool> _pools = new Dictionary<string, GameObjectPool>();

    public GameObjectPool CreatePool(string poolName, string path, int initSize)
    {
        GameObjectPool pool = new GameObjectPool(poolName, path, initSize);
        _pools.Add(poolName, pool);
        return pool;
    }

    public GameObjectPool GetPool(string poolName)
    {
        if (_pools.ContainsKey(poolName))
        {
            return _pools[poolName];
        }

        return null;
    }

    /// <summary>
    /// 索引到对应Pool的Obj
    /// </summary>
    /// <param name="poolName"></param>
    public GameObject this[string poolName]
    {
        get{
            GameObject result = null;
            if (_pools.ContainsKey(poolName))
            {
                GameObjectPool pool = _pools[poolName];
                result = pool.Get();
            }
            else
            {
                Debug.LogError("没有这个对象池" + poolName);
            }

            return result;
        }
        
    }

    public void Release()
    {
    }
}