using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToEditer : MonoBehaviour
{
    public GameObject Button1;
    public GameObject RuntimeHierarchy;
    public GameObject RuntimeInspector;
    bool push = true;
  

    public void PushDown()
    {
        push = false;
    }

    public void PushUp()
    {
        push = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (push)
        {
            this.RuntimeInspector.SetActive(true);
            this.RuntimeHierarchy.SetActive(true);
        }
        else
        {
            this.Button1.SetActive(false);
            this.RuntimeInspector.SetActive(false);
            this.RuntimeHierarchy.SetActive(false);
        }
    }
}
