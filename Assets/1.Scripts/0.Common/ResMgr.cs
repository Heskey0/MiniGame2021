using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResMgr : Singleton<ResMgr>
{
    public void Init()
    {
        ObjectPoolMgr.Instance.CreatePool("TypeItem", "UI/MainScene/SpawnPanel/TypeItem",10);
        ObjectPoolMgr.Instance.CreatePool("Item", "UI/MainScene/SpawnPanel/Item", 10);
    }

    public GameObject GetInstance(string resPath)
    {
        //先从对象池中找
        if (ObjectPoolMgr.Instance.GetPool(resPath) != null)
        {
            Debug.Log("poolLoaded");
            return ObjectPoolMgr.Instance[resPath].Instantiate();
        }

        //没有对应的对象池
        return GetResources<GameObject>(resPath).Instantiate();
    }

    public T GetResources<T>(string resPath) where T : Object
    {
        return Resources.Load<T>(resPath);
    }

    public void Release(GameObject go)
    {
        GameObject.Destroy(go);
    }
}