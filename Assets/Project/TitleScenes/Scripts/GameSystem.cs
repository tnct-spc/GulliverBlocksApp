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
        public GameObject MapSelectPanel;
        public GameObject SelectMergingMapPanel;
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
            // GameManagerにModeを渡す
            string selectedLabel = toggleGroup.ActiveToggles()
                .First().GetComponentsInChildren<Text>()
                .First(t => t.name == "Label").text;

            if(selectedLabel == "MergeMode")
            {
                SelectMergingMapPanel.SetActive(true);
                SelectMergingMapPanel.GetComponent<MergeMapSelect>().mergedMapID = ID;
            }
            else
            {
                // BlockManagerにIDを渡す
                BlockManager.WorldID = ID;

                GameManager.Mode = selectedLabel.Replace("Mode", "");

                // VrSceneを読み込む
                SceneManager.LoadScene("Vr");
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }
        }

        public void MoveSetting()
        {
            SceneManager.LoadScene("SettingScene");

            Screen.orientation = ScreenOrientation.LandscapeLeft;

        }
    }
}