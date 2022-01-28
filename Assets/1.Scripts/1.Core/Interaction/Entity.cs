using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Entity : MonoBehaviour
{
    [Header("图片ID")]public int imageId;
    
    [FormerlySerializedAs("typeid")] public float positionX = 1;
    [FormerlySerializedAs("itemid")] public float positionY = 1;

    [Header("是否是障碍物")]public bool isNull = false;
    public const int EntityLayer = 12;

    public void Newid(int newTypeid,int newItemid)
    {
        positionX = newTypeid;
        positionY = newItemid;
    }

    public static implicit operator bool(Entity entity) => entity!=null&&!entity.isNull;
 
    void Start()
    {
        //gameObject.Layer(EntityLayer);
    }

    private void Update()
    {
        if (GetComponent<SpawnItem>() != null)
        {
            return;
        }
        gameObject.Layer(EntityLayer);
    }
}