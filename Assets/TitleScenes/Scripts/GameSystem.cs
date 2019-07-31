using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    public GameObject ModeSelectPanel;
    public GameManager GameManager;

    private void Awake()
    {
        ModeSelectPanel.SetActive(false);
    }
    public void SelectGameMode()
    {
        ModeSelectPanel.SetActive(true);
    }

    public void StartViewMode()
    {
        GameManager.Mode = "View";

        SceneManager.LoadScene("Vr");

        Screen.orientation = ScreenOrientation.LandscapeLeft;

    }

    public void StartPlayMode()
    {
        GameManager.Mode = "Vr";

        SceneManager.LoadScene("Vr");

        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    public void MoveSetting()
    {
        SceneManager.LoadScene("SettingScene");

        Screen.orientation = ScreenOrientation.LandscapeLeft;

    }
}