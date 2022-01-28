using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManagent : MonoBehaviour
{


    public GameObject infoMenu;
    public bool isopen = false;
    // Start is called before the first frame update
    void Start()
    {
        infoMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void appear()
    {
        //isopen = !isopen;
        if (Input.GetMouseButton(0))
        {
            infoMenu.SetActive(true);
        }
        //infoMenu.SetActive(true);
;    }

}
