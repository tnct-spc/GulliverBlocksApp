﻿using UnityEngine;
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

public class GameSystem : MonoBehaviour
{
// 表示中は1眼になるようにする
private void splashscreen()
{
    if (!UnityEngine.Rendering.SplashScreen.isFinished) {
        XRSettings.enabled = false;
    }
}
}

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
        public GameObject DeleteMapPanel;
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

        public void OnClickChangeMapButton(string mapName, string command)
        {
            if(command == "EditMapName")
            {
                EditMapNamePanel.SetActive(true);
                EditMapNamePanel.transform.Find("InputField").GetComponent<InputField>().text = mapName;
            }
            else if(command == "Delete")
            {
                DeleteMapPanel.SetActive(true);
                DeleteMapPanel.transform.Find("Text(DeletePanel)").GetComponent<Text>().text = mapName;
            }
            selectMapName = mapName;
        }

        public void OnClickAcceptChangeMapButton(bool isEdit)
        {
            WorldSelect worldSelect = MapSelectPanel.GetComponent<WorldSelect>();
            if (isEdit)
            {
                string changedMapName = EditMapNamePanel.transform.Find("InputField").GetComponent<InputField>().text;
                if (changedMapName != selectMapName)
                {
                    var changingWordData = worldSelect.WorldsData.Find(w => w.world.name == selectMapName);
                    changingWordData.world.name = changedMapName;
                    worldSelect.WorldsData.Insert(0, changingWordData);
                }
                EditMapNamePanel.SetActive(false);
            }
            else
            {
                DeleteMapPanel.SetActive(false);
            }
            worldSelect.WorldsData.RemoveAll(w => w.world.name == selectMapName);
            worldSelect.setWorldSelectButton();
        }
    }
}