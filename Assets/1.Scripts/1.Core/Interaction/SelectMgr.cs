using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;


public class SelectMgr : Singleton<SelectMgr>
{
    //为true时，能catch entity
    //为false时，根据判断 Main.InputActionAlive 判断是否能catch entity
    public bool CatchEntityAlive = false;
    public Entity SelectedEntity;

    private TimerModel _timer;
    RaycastHit hit = new RaycastHit();

    public void Init()
    {
        _timer = TimerMgr.Instance.CreateTimer(0, -1, () =>
        {
            if (SpawnMgr.Instance.isSpawning)
            {
                return;
            }

            catchEntity();
        });

        _timer.Start();
    }

    public void Release()
    {
        _timer.Stop();
        _timer = null;
    }

    private void catchEntity()
    {
        if ( ! CatchEntityAlive && ! Main.InputActionAlive )
        {
            return;
        }


        /*
         * catch an entity hit by ray
         */
        if (Input.GetMouseButtonDown(0))
        {

            //Debug.Log("getMouseButtonDown");

            //if (CanvasCheck.hitUI)
            //{
            //    return;
            //}

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                var tmpEntity = hit.transform.gameObject.GetComponent<Entity>();
                if (! tmpEntity)
                {
                    return;
                }

                SelectedEntity = tmpEntity;

                SpawnMgr.Instance.Spawn(SelectedEntity.gameObject, 1, 1);
                GameObject.Destroy(SelectedEntity.gameObject);
                Debug.Log("select Entity:" + SelectedEntity.positionX + ":" + SelectedEntity.positionY);
            }
        }
    }
}