using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 放置未Instantiate的GameObject
/// </summary>
public class SpawnMgr : Singleton<SpawnMgr>
{
    private int layer = (1 << Entity.EntityLayer);
    private static TimerModel timerTask;

    public bool isSpawning = false;
    public event Action OnSpawnedCallback = spawned;
    public event Action OnCanceledCallback = Canceled;

    public static float gap =  2.5f;

    public void Spawn(GameObject go, int typeid, int itemid)
    {
        var tmpPosition = go.transform.position;
        var father = go.transform.parent;
        if (isSpawning)
        {
            return;
        }
        //UIManager.Instance.RemoveLayer(UILayer.Top);
        isSpawning = true;

        go.Instantiate()
            .AddComponent4go<SpawnItem>()
            .transform.SetParent(father);
        //go.transform.position = new Vector3(go.transform.position.x, 0, go.transform.position.z);

        //Debug.Log(tmpPosition.y);
        Ray ray;
        RaycastHit hit = new RaycastHit();

        timerTask = TimerMgr.Instance.CreateTimer(Time.deltaTime, -1, () =>
        {
            //return;
            if ( ! SelectMgr.Instance.CatchEntityAlive && ! Main.InputActionAlive )
            {
                return;
            }
            
            //Debug.Log("::::>");

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (CameraController.viewing_state == ViewingState.V3d)
            {
                //last point
                var lastPoint = SpawnItem.Go.transform.position;
                if (Physics.Raycast(ray, out hit, 5000, (1 << Entity.EntityLayer)))
                {
                    SpawnItem.Go.transform.position = new Vector3(lastPoint.x,tmpPosition.y,lastPoint.z);
                    //Debug.Log("hit entity");
                }
                
                else if (Physics.Raycast(ray, out hit, 5000, ~(1<<SpawnItem.Layer) & ~(1<< 10) & ~(1<<Entity.EntityLayer)))
                {
                    if (hit.collider.gameObject.layer == 10)
                    {
                        SpawnItem.Go.transform.position = new Vector3(lastPoint.x,tmpPosition.y,lastPoint.z);
                    }
                    Debug.Log(Time.deltaTime);
                    //next point
                    var nextPoint = new Vector3(UnityEngine.Mathf.Round(hit.point.x / gap) * gap, hit.point.y, UnityEngine.Mathf.Round(hit.point.z / gap) * gap);
                    
                    
                    //判断是否相差小于两个格子
                    if (Vector3.Distance(lastPoint, nextPoint) <= 1f * gap)
                    {
                        SpawnItem.Go.transform.position = new Vector3(nextPoint.x,tmpPosition.y,nextPoint.z);
                        //Debug.Log(SpawnItem.Go.transform.position);
                    }
                    else
                    {
                        SpawnItem.Go.transform.position = new Vector3(lastPoint.x,tmpPosition.y,lastPoint.z);
                    }
                    // Debug.Log("lastPoint:" + lastPoint);
                    // Debug.Log("nextPoint" + nextPoint);
                }
                else
                {
                    SpawnItem.Go.transform.position = new Vector3(lastPoint.x,tmpPosition.y,lastPoint.z);
                }
            }
            else
            {
                var lastPoint = SpawnItem.Go.transform.position;
                if (Physics.Raycast(ray, out hit, 5000, layer))
                {
                    SpawnItem.Go.transform.position = tmpPosition;
                    //Debug.Log("hit entity");
                }
                else if (Physics.Raycast(ray, out hit, 5000, 1 << 10 | ~(1 << SpawnItem.Layer) ))
                {
                    //next point
                    Vector3 nextPoint;
                    if (CameraController.RotateCount % 2 == 0)
                    {
                        nextPoint = new Vector3(UnityEngine.Mathf.Round(hit.point.x/gap)*gap, tmpPosition.y, tmpPosition.z);
                    }
                    else
                    {
                        nextPoint = new Vector3(tmpPosition.x, tmpPosition.y, UnityEngine.Mathf.Round(hit.point.z/gap)*gap);
                    }

                    if (Vector3.Distance(lastPoint, nextPoint) <= 1f * gap)
                    {
                        SpawnItem.Go.transform.position = new Vector3(nextPoint.x,tmpPosition.y,nextPoint.z); 
                    }
                    else
                    {
                        SpawnItem.Go.transform.position = new Vector3(lastPoint.x,tmpPosition.y,lastPoint.z); 
                    }
                }
                
                SpawnItem.Go.transform.position = new Vector3(SpawnItem.Go.transform.position.x, tmpPosition.y, SpawnItem.Go.transform.position.z);
            }

            //left click to spawn the item
            if (Input.GetMouseButtonDown(0))
            {
                OnSpawnedCallback?.Invoke();
                OnSpawnedCallback = spawned;

                //convert spawnItem to Entity , then Set entity id
                SpawnItem.Go //.Layer(0)
                    .AddComponentGracefully<Entity>()
                    .Newid((int)( SpawnItem.Go.transform.position.x / SpawnMgr.gap), (int)(SpawnItem.Go.transform.position.z / SpawnMgr.gap));
                SpawnItem.Go.transform.SetParent(EntitiesMgr.Instance.transform);
                
                
                GameObject.Destroy(SpawnItem.Go.GetComponent<SpawnItem>());
                SpawnItem.Go = null;
                isSpawning = false;
                timerTask.Stop();
                
                GlobalStateMachine.Instance.CheckState();
            }

            //right click to cancle
            if (Input.GetMouseButtonDown(1))
            {
                OnCanceledCallback?.Invoke();
                OnCanceledCallback = Canceled;

                // SpawnItem.Go
                //     .Layer(0);
                //GameObject.Destroy(SpawnItem.Go);
                SpawnItem.Go.transform.position = tmpPosition;

                SpawnItem.Go.transform.SetParent(EntitiesMgr.Instance.transform);
                SpawnItem.Go = null;
                isSpawning = false;
                timerTask.Stop();
            }
        });

        //callback invoke when item has been spawned
        OnSpawnedCallback?.Invoke();

        timerTask.Start();
    }


    public static void spawned()
    {
        //SpawnItem.Go.AddComponent<Entity>();
    }

    public static void Canceled()
    {
    }
}