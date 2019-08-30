using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using VrScene;

namespace TitleScene
{
    public class GameSystem : MonoBehaviour
    {
        public GameObject ModeSelectPanel;
        public GameManager GameManager;
        public ToggleGroup toggleGroup;

        private void Awake()
        {
            XRSettings.enabled = false;
            ModeSelectPanel.SetActive(false);
        }
        public void SelectGameMode()
        {
            ModeSelectPanel.SetActive(true);
        }

        private void Update()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ModeSelectPanel.SetActive(false);
                }
            }
        }

        public void OnClickWorldSelectButton(string ID)
        {
            // BlockManagerにIDを渡す
            BlockManager.WorldID = ID;

            // GameManagerにModeを渡す
            string selectedLabel = toggleGroup.ActiveToggles()
                .First().GetComponentsInChildren<Text>()
                .First(t => t.name == "Label").text;
            if (selectedLabel == "再生モード") selectedLabel = "PlayBack";

            GameManager.Mode = selectedLabel;


            // VrSceneを読み込む
            SceneManager.LoadScene("Vr");
            Screen.orientation = ScreenOrientation.LandscapeLeft;

        }

        public void MoveSetting()
        {
            SceneManager.LoadScene("SettingScene");

            Screen.orientation = ScreenOrientation.LandscapeLeft;

        }
    }
}