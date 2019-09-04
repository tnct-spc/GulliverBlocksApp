using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using VrScene;
using MergeScene;

namespace TitleScene
{
    public class GameSystem : MonoBehaviour
    {
        public GameObject MapSelectPanel;
        public GameObject MergeMapSelectPanel;
        public GameObject ModeSelectPanel;
        public GameObject PlaybackModeButton;
        public GameObject ViewModeButton;
        public GameManager GameManager;
        public ToggleGroup toggleGroup;

        private void Awake()
        {
            XRSettings.enabled = false;
        }
        public void SelectGameMode()
        {
            MapSelectPanel.SetActive(true);
        }

        private void Update()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    MapSelectPanel.SetActive(false);
                }
            }
        }

        public void OnClickWorldSelectButton(string ID)
        {
            BlockManager.WorldID = ID;
            ModeSelectPanel.SetActive(true);
            PlaybackModeButton.GetComponent<Button>().onClick.AddListener(() => OnClickModeSelectButton("PlayBack"));
            ViewModeButton.GetComponent<Button>().onClick.AddListener(() => OnClickModeSelectButton("Vr"));
        }

        public void OnClickModeSelectButton(string Mode)
        {
            GameManager.Mode = Mode;
            SceneManager.LoadScene("Vr");
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }

        public void OnClickCreateMapButton()
        {
            MergeMapSelectPanel.transform.GetComponent<MergeMapSelect>().WorldsData.Clear();
            MergeMapSelectPanel.transform.GetComponent<MergeMapSelect>().WorldsData.AddRange(MapSelectPanel.GetComponent<WorldSelect>().WorldsData);
            MergeMapSelectPanel.SetActive(true);
        }

        public void OnClickMergeButton()
        {
            SceneManager.LoadScene("Merge");
            MapManager.WorldList = MergeMapSelectPanel.transform.GetComponent<MergeMapSelect>().CheckToggles();
        }

        public void MoveSetting()
        {
            SceneManager.LoadScene("SettingScene");

            Screen.orientation = ScreenOrientation.LandscapeLeft;

        }
    }
}