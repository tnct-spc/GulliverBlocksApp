﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace SettingScene
{
    public class MoveScene : MonoBehaviour
    {

        //　Titleを押したら実行する
        public void MoveTitle()
        {
            SceneManager.LoadScene("Title");

            Screen.orientation = ScreenOrientation.LandscapeLeft;

        }
    }
}
