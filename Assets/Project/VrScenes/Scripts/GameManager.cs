﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VrScene
{
    public class GameManager : MonoBehaviour
    {
        public static string Mode = "PlayBack";
        private void Awake()
        {
            if (Mode == "プレイバックモード") Mode = "PlayBack";
        }
        public void Back_To_Title_If_Android()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                SceneManager.LoadScene("Title");
            }
        }
    }
}