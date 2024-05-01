using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public GameObject MonsCanvas;

    // Update is called once per frame
    void Update()
    {
        if (transform.GetChild(0).gameObject.activeInHierarchy)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                MonsCanvas.SetActive(true);
            }
        }
    }
}
