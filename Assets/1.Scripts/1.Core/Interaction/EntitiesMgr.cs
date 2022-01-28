using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using QFramework;
using UnityEngine;

public class EntitiesMgr : MonoBehaviour
{
    private static EntitiesMgr _instance;
    public static EntitiesMgr Instance => _instance;

    private Camera main_camera;
    public static List<Entity> EntitiesList = new List<Entity>();

    [Header("进大远小的缩放比例")] [SerializeField] private float ScaleRate = 1;


    #region Init

    void InitCamera()
    {
        CameraController.RotateLeftViewCallback += ScaleWithinRotation;
        CameraController.RotateRightViewCallback += ScaleWithinRotation;

        CameraController.ChangeViewCallback += () =>
        {
            if (CameraController.viewing_state == ViewingState.V3d)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).GetComponent<Entity>() == null)
                    {
                        continue;
                    }
                    transform.GetChild(i).transform.localScale = Vector3.one;
                    transform.GetChild(i).GetComponent<BoxCollider>().size =
                        new Vector3(0.7f, 0.4f, 0.7f) * SpawnMgr.gap * 1.25f;
                }

                return;
            }
        };
    }

    void InitEntities()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            EntitiesList.Add(transform.GetChild(i).gameObject.AddComponentGracefully<Entity>());
        }
        
        for (int i = 0; i < transform.childCount; i++)
        {
            var tmpChild = transform.GetChild(i).gameObject;
            if (transform.GetChild(i).GetComponent<Entity>() == null)
            {
                tmpChild.transform.localScale = new Vector3(1,0.3f,1) * (18 - 3) / 18 * ScaleRate;
                continue;
            }
            tmpChild.GetComponent<BoxCollider>().size = new Vector3(1, 0.1f, 1) * 1.05f * SpawnMgr.gap;
        }
    }

    #endregion

    void Start()
    {
        _instance = this;
        main_camera = Camera.main;


        InitEntities();
        InitCamera();
    }

    public GameObject GetGameObjectByImageId(int imageId)
    {
        var childCount = transform.childCount;
        //find matched image id
        for (int i = 0; i < childCount; i++)
        {
            var tmpEntityComponent = transform.GetChild(i).GetComponent<Entity>();
            if ( tmpEntityComponent != null && tmpEntityComponent.imageId == imageId)
            {
                return tmpEntityComponent.gameObject;
            }
        }
        //invalid imageId
        throw new UnityException("invalid imageId");
    }
    
    public bool CheckCoordByImageId(int imageId, float position_X, float position_Y)
    {
        float x, y;
        GetCoordByImageId(imageId, out x, out y);
        return x == position_X && y == position_Y;
    }

    public void GetCoordByImageId(int imageId, out float position_X, out float position_Y)
    {
        
        var childCount = transform.childCount;
        //find matched image id
        for (int i = 0; i < childCount; i++)
        {
            var tmpEntityComponent = transform.GetChild(i).GetComponent<Entity>();
            if (tmpEntityComponent != null&& tmpEntityComponent.imageId == imageId)
            {
                position_X = tmpEntityComponent.positionX;
                position_Y = tmpEntityComponent.positionY;   
                return;
            }
        }
        //invalid imageId
        throw new UnityException("invalid imageId");

    }

    
    public void SetImageID(GameObject dst, Texture2D t)
    {
        for (int i = 0; i < dst.transform.childCount; i++)
        {
            var tmpEntityComponent = transform.GetChild(i).GetComponent<Entity>();
            if (tmpEntityComponent == null)
            {
                continue;
            }
            transform.GetChild(i).GetComponent<MeshRenderer>().material.mainTexture = t;
        }
    }

    public void ScaleWithinRotation()
    {
        var cameraPosition = main_camera.transform.position;
        for (int i = 0; i < transform.childCount; i++)
        {
            var tmpEntityComponent = transform.GetChild(i).GetComponent<Entity>();
            if (tmpEntityComponent == null)
            {
                continue;
            }
            var tmpChild = transform.GetChild(i);
            //var colliderSize = tmpChild.GetComponent<BoxCollider>().size;
            // DOTween.To((DOGetter<Vector3>) (() => colliderSize), (DOSetter<Vector3>) (x => colliderSize = x), new Vector3(1,13,1),0.5f).SetTarget<TweenerCore<Vector3, Vector3, VectorOptions>>((object) colliderSize);
            tmpChild.GetComponent<BoxCollider>().size = new Vector3(1, 13, 1);

            //dis 大 => scale 小
            float dis;
            Debug.Log(CameraController.RotateCount);
            if (CameraController.RotateCount == 0)
            {
                dis = Vector3.Distance(new Vector3(0, 0, cameraPosition.z),
                    new Vector3(0, 0, tmpEntityComponent.positionY));
                //((int)( SpawnItem.Go.transform.position.x / SpawnMgr.gap), (int)(SpawnItem.Go.transform.position.z / SpawnMgr.gap))
                //position y
                dis = (int)(tmpChild.position.z / SpawnMgr.gap);
                //Debug.Log("dis:"+dis);
            }
            else if (CameraController.RotateCount == 1)
            {
                dis = (int)(tmpChild.position.x / SpawnMgr.gap);
            }
            else if (CameraController.RotateCount == 2)
            {
                dis = -(int)(tmpChild.position.z / SpawnMgr.gap);
            }
            else
            {
                dis = -(int)(tmpChild.position.x / SpawnMgr.gap);
            }

            tmpChild.transform.localScale = Vector3.one * (6-dis) * ScaleRate;
            //tmpChild.transform.localScale = Vector3.one * (18 - dis) / 18 * ScaleRate;
        }
    }

    /// <summary>
    /// clear all entities
    /// </summary>
    public void ClearAll()
    {
        transform.DestroyChildren();
    }

    void Update()
    {

        var cameraPosition = main_camera.transform.position;
        for (int i = 0; i < transform.childCount; i++)
        {
            var tmpChild = transform.GetChild(i);
            var tmpEntityComponent = transform.GetChild(i).GetComponent<Entity>();
            if (tmpEntityComponent == null)
            {
                if (CameraController.viewing_state == ViewingState.V3d)
                {
                    tmpChild.transform.localScale = new Vector3(1,0.3f,1) * (18 - 3) / 18 * 3;
                    //tmpChild.GetComponent<BoxCollider>().size = new Vector3(1, 0.1f, 1) * 1.05f * SpawnMgr.gap;
                }

                continue;
            }

            //var colliderSize = tmpChild.GetComponent<BoxCollider>().size;
            // DOTween.To((DOGetter<Vector3>) (() => colliderSize), (DOSetter<Vector3>) (x => colliderSize = x), new Vector3(1,13,1),0.5f).SetTarget<TweenerCore<Vector3, Vector3, VectorOptions>>((object) colliderSize);
            if (CameraController.viewing_state == ViewingState.V2d)
            {
                // float dis;
                // if (CameraController.RotateCount % 2 != 0)
                // {
                //     dis = Vector3.Distance(new Vector3(0, 0, cameraPosition.z),
                //         new Vector3(0, 0, tmpChild.transform.position.z));
                // }
                // else
                // {
                //     dis = Vector3.Distance(new Vector3(cameraPosition.x, 0, 0),
                //         new Vector3(tmpChild.transform.position.z, 0, 0));
                // }
                //
                // tmpChild.transform.localScale = Vector3.one * (18 - dis) / 18 * ScaleRate;
                // tmpChild.GetComponent<BoxCollider>().size = new Vector3(1, 13, 1) * 1.05f * SpawnMgr.gap;
                
                //ScaleWithinRotation();
            }
            else
            {
                tmpChild.transform.localScale = Vector3.one * (18 - 5) / 18 * 3;
                tmpChild.GetComponent<BoxCollider>().size =new Vector3(0.5f, 0.3f, 0.5f) * SpawnMgr.gap * 1.25f;              
            }
        }
    }
}