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
        public GameObject SelectCreatingPanel;
        public GameObject CreateNewMapPanel;
        public GameObject PlaybackModeButton;
        public GameObject ViewModeButton;
        public GameObject EditMapNamePanel;
        public GameObject DeleteMapPanel;
        public GameManager GameManager;
        public ToggleGroup toggleGroup;
        public InputField MapNameInputField;

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

        public void OnClickCreateButton()
        {
            SelectCreatingPanel.SetActive(true);
        }

        public void OnClickCreateMerge()
        {
            SelectCreatingPanel.SetActive(false);
            StartCoroutine("fetchMerge");
        }
        IEnumerator fetchMerge()
        {
            var communicationManager = new CommunicationManager();
            var fetchMapsTask = communicationManager.fetchMapsAsync();
            yield return new WaitUntil(() => fetchMapsTask.IsCompleted);
            MergeMapSelectPanel.transform.GetComponent<MergeMapSelect>().WorldsData.Clear();
            MergeMapSelectPanel.transform.GetComponent<MergeMapSelect>().WorldsData.AddRange(fetchMapsTask.Result);
            MergeMapSelectPanel.SetActive(true);
        }

        public void OnClickOpenCreateNewWorldPanel()
        {
            SelectCreatingPanel.SetActive(false);
            CreateNewMapPanel.SetActive(true);
        }
        public void OnClickCreateNewWorld()
        {
            // TODO
            return;
        }

        public void OnClickMergeButton()
        {
            SceneManager.LoadScene("Merge");
            MapManager.WorldList = MergeMapSelectPanel.transform.GetComponent<MergeMapSelect>().CheckToggles();
            MapManager.MergeName = this.MapNameInputField.text;
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