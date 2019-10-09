using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VrScene
{
    public class GameManager : MonoBehaviour
    {
        public static string Mode;
        public GameObject ViewModeUI;

        public void Back_To_Title_If_Android()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                SceneManager.LoadScene("Title");
            }
        }
    }
}