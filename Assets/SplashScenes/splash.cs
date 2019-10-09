using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class sprite : MonoBehaviour
{
    // 表示中は1眼になるようにする
    private void splashscreen()
    {
        if (!Application.isShowingSplashScreen) {
            XRSettings.enabled = false;
        }
    }
}
