using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace SettingScene
{
    public class GoToColorSetting : MonoBehaviour
    {

        //　Titleを押したら実行する
        public void MoveTitle()
        {
            SceneManager.LoadScene("ColorSetting");

            Screen.orientation = ScreenOrientation.LandscapeLeft;

        }
    }
}

