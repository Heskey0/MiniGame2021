using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public static class CSExtern
{
    /******************************  boolean  ******************************/
    #region boolean
    

    
    public static bool IsNullOrEmpty(this string selfStr)
    {
        return string.IsNullOrEmpty(selfStr);
    }

    public static bool Do(this bool selfCondition, Action action)
    {
        if (selfCondition)
        {
            action();
        }

        return selfCondition;
    }

    #endregion

    /******************************  IO  ******************************/
    #region IO
    
    public static string CreateDirIfNotExists(this string dirFullPath)
    {
        if (!Directory.Exists(dirFullPath))
        {
            Directory.CreateDirectory(dirFullPath);
        }

        return dirFullPath;
    }
    
    public static void DeleteDirIfExists(this string dirFullPath)
    {
        if (Directory.Exists(dirFullPath))
        {
            Directory.Delete(dirFullPath, true);
        }
    }
    
    public static void EmptyDirIfExists(this string dirFullPath)
    {
        if (Directory.Exists(dirFullPath))
        {
            Directory.Delete(dirFullPath, true);
        }

        Directory.CreateDirectory(dirFullPath);
    }
    
    public static bool DeleteFileIfExists(this string fileFullPath)
    {
        if (File.Exists(fileFullPath))
        {
            File.Delete(fileFullPath);
            return true;
        }

        return false;
    }
    
    public static string CombinePath(this string selfPath, string toCombinePath)
    {
        return Path.Combine(selfPath, toCombinePath);
    }
    
    #endregion

    /******************************  Gameobject  ******************************/
    #region Gameobject
    
    public static T Find<T>(this GameObject selfGameObj, string path) where T:Component
    {
        var go = selfGameObj.transform.Find(path);
        if (go == null)
        {
            return null;
        }
        return go.GetComponent<T>();
    }
    
    public static GameObject DestroyAllChildren(this GameObject selfGameObj)
    {
        var childCount = selfGameObj.transform.childCount;

        for (var i = 0; i < childCount; i++)
        {
            GameObject.Destroy(selfGameObj.transform.GetChild(i).gameObject);
        }

        return selfGameObj;
    }
    
    public static GameObject DontDestoryOnLoad(this GameObject selfGameObj)
    {
        GameObject.DontDestroyOnLoad(selfGameObj);
        return selfGameObj;
    }

    public static GameObject Instantiate(this GameObject selfObj)
    {
        return GameObject.Instantiate(selfObj);
    }

    
    public static GameObject Layer(this GameObject selfObj, int layer)
    {
        try
        {
            selfObj.layer = layer;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        return selfObj;
    }

    public static GameObject Name(this GameObject selfObj, string newName)
    {
        selfObj.name = newName;
        return selfObj;
    }
    
    
    public static GameObject AddComponent4go<T>(this GameObject selfGameObj) where T:Component
    {
        selfGameObj.AddComponentGracefully<T>();
        return selfGameObj;
    }
    
    #endregion

    /******************************  Transform  ******************************/
    #region Transform
    
    public static T Identity<T>(this T selfComponent) where T : Component
    {
        selfComponent.transform.position = Vector3.zero;
        selfComponent.transform.rotation = Quaternion.identity;
        selfComponent.transform.localScale = Vector3.one;
        return selfComponent;
    }

    public static T LocalIdentity<T>(this T selfComponent) where T : Component
    {
        selfComponent.transform.localPosition = Vector3.zero;
        selfComponent.transform.localRotation = Quaternion.identity;
        selfComponent.transform.localScale = Vector3.one;
        return selfComponent;
    }
    
    public static T LocalScaleIdentity<T>(this T selfComponent) where T : Component
    {
        selfComponent.transform.localScale = Vector3.one;
        return selfComponent;
    }

    #endregion
    
    /******************************  float  ******************************/
    #region float

    public static bool FloatEqualTo(this float self, float other)
    {
        return Mathf.Abs(self - other) <= 0.01f;
    }

    #endregion
    
    public static T AddComponentGracefully<T>(this GameObject selfObj) where T : Component
    {
        if (selfObj.GetComponent<T>() != null)
        {
            return selfObj.GetComponent<T>();
        }
        return selfObj.AddComponent<T>();
    }
}