using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{

    //　スタートボタンを押したら実行する
    public void StartGame()
    {
        SceneManager.LoadScene("Vr");

        Screen.orientation = ScreenOrientation.LandscapeLeft;

    }

    public void MoveSetting()
    {
        SceneManager.LoadScene("SettingScene");

        Screen.orientation = ScreenOrientation.LandscapeLeft;

    }
}