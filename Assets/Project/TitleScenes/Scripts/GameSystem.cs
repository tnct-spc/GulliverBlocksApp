using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using VrScene;
using MergeScene;
using JsonFormats;

namespace TitleScene
{
    public class GameSystem : MonoBehaviour
    {
        public GameObject MapSelectPanel;
        public GameObject MergeMapSelectPanel;
        public GameObject ModeSelectPanel;
        public GameObject PlaybackModeButton;
        public GameObject ViewModeButton;
        public GameObject EditMapNamePanel;
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

        public void OnClickWorldSelectButton(string ID, bool isMerge)
        {
            BlockManager.WorldID = ID;
            BlockManager.IsMerge = isMerge;
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
            List<World> mapsData = new List<World>() { };
            MapSelectPanel.GetComponent<WorldSelect>().WorldsData.ForEach(data =>
            {
                if (!data.isMerge)
                    mapsData.Add(data.world);
            });
            MergeMapSelectPanel.transform.GetComponent<MergeMapSelect>().WorldsData.Clear();
            MergeMapSelectPanel.transform.GetComponent<MergeMapSelect>().WorldsData.AddRange(mapsData);
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

        public void OnClickBackButton(GameObject currentUI)
        {
            currentUI.SetActive(false);
        }

        public void OnClickEndGameButton()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
            UnityEngine.Application.Quit();
        }


        private string selectMapName;

        public void OnClickEditMapNameButton(string mapName)
        {
            EditMapNamePanel.SetActive(true);
            EditMapNamePanel.transform.Find("InputField").GetComponent<InputField>().text = mapName;
            selectMapName = mapName;
        }

        public void OnClickResterMapNameButton()
        {
            string changedMapName = EditMapNamePanel.transform.Find("InputField").GetComponent<InputField>().text;
            if (changedMapName != selectMapName)
            {
                var wordData = MapSelectPanel.GetComponent<WorldSelect>().WorldsData.Find(w => w.world.name == selectMapName);
                MapSelectPanel.GetComponent<WorldSelect>().WorldsData.RemoveAll(w => w.world.name == selectMapName);
                wordData.world.name = changedMapName;
                MapSelectPanel.GetComponent<WorldSelect>().WorldsData.Insert(0, wordData);
                MapSelectPanel.GetComponent<WorldSelect>().setWorldSelectButton();
            }
            EditMapNamePanel.SetActive(false);
        }
    }
}