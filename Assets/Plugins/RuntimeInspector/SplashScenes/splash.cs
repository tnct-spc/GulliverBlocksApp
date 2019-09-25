using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class sprite : MonoBehaviour
{
    // 表示中は1眼になるようにする
    private void splashscreen()
    {
        if (!UnityEngine.Rendering.SplashScreen.isFinished) {
            XRSettings.enabled = false;
        }
    }
}
