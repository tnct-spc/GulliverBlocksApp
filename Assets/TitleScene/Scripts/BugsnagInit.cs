using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BugsnagUnity;

public class BugsnagInit : MonoBehaviour
{
    void Start()
    {
        Bugsnag.Configuration.ReleaseStage = "development";
        Bugsnag.Init("9e80dcefe88a14ddb6a942635bf3df08"); 
    }
}
