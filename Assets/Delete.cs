using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delete : MonoBehaviour
{
    public GameObject Button1;
    public GameObject RuntimeHierarchy;
    public GameObject RuntimeInspector;
    bool push = true;
   


    public void OnClick()
    {
        this.Button1.SetActive(false);
        this.RuntimeHierarchy.SetActive(false);
        this.RuntimeInspector.SetActive(false);
        Debug.Log("Back to Game");
    }

}



