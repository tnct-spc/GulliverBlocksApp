using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    public GameObject ModeSelectPanel;

    private void Awake()
    {
        ModeSelectPanel.SetActive(false);
    }
    public void SelectGameMode()
    {
        ModeSelectPanel.SetActive(true);
    }

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