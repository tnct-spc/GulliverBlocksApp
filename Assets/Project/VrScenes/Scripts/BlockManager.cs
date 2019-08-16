﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine.Networking;
using JsonFormats;

namespace VrScene
{
    public class BlockManager : MonoBehaviour
    {
        private List<(IncludingBlockInfo block_Info, GameObject block_instance)> blocks_data = new List<(IncludingBlockInfo block_info, GameObject block_instance)>();
        public static string WorldID;
        public string ServerAddress = "gulliverblocks.herokuapp.com";
        GameObject GameSystem;
        static string response_json;
        public float BlockNumber = 0;
        public bool isRepeating = false;
        InputManager InputManager;
        Slider SeekBar;
        Toggle PlayButton;
        GameManager GameManager;
        Block[] blockJson;
        GameObject[] Cube;
        public GameObject LoadingWindow;

        private void Start()
        {
            LoadingWindow.SetActive(true);
            GameSystem = GameObject.Find("GameSystem");
            InputManager = GameSystem.GetComponent<InputManager>();
            SeekBar = InputManager.SeekBar;
            PlayButton = InputManager.PlayButton;
            GameManager = GameSystem.GetComponent<GameManager>();
            StartCoroutine("FetchAndPlaceBlocks");
        }

        IEnumerator FetchAndPlaceBlocks()
        {
            string server_url = "https://" + ServerAddress + "/get_blocks/" + WorldID + "/";
            //URLをGETで用意
            UnityWebRequest webRequest = UnityWebRequest.Get(server_url);
            //URLに接続して結果が戻ってくるまで待機
            yield return webRequest.SendWebRequest();

            //エラーが出ていないかチェック
            if (webRequest.isNetworkError)
            {
                //通信失敗
                Debug.Log(webRequest.error);
            }
            else
            {
                blockJson = CommunicationManager.JsonHelper.FromJson<Block>(webRequest.downloadHandler.text, "Blocks");
                Cube = new GameObject[GetBlockJsonLength()];
                InitialPlacement();
                ApplyColorRules();
                if (GameManager.Mode == "Vr") InputManager.PlayModeUI.SetActive(true);
                LoadingWindow.SetActive(false);
            }
        }

        void InitialPlacement()
        {
            Object block = (GameObject)Resources.Load("pblock1x1");
            for (int i = 0; i < GetBlockJsonLength(); i++)
            {
                Cube[i] = Instantiate(block, blockJson[i].GetPosition(), Quaternion.identity) as GameObject;
                string colorName = "Color" + blockJson[i].colorID.ToString();
                Material colorMaterial = Resources.Load(colorName) as Material;
                Cube[i].GetComponent<Renderer>().sharedMaterial = colorMaterial;
                IncludingBlockInfo blockInfo = Cube[i].GetComponent<IncludingBlockInfo>();
                blockInfo.SetBlockData(blockJson[i]);
                Cube[i].name = "Cube" + i;
                blocks_data.Add((blockInfo, Cube[i]));
                if (GameManager.Mode == "Vr") Cube[i].SetActive(false);
            }
        }

        public int GetBlockJsonLength()
        {
            return blockJson.Length;
        }

        public async void RepeatPlaceBlocks()
        {
            isRepeating = true;
            while (BlockNumber < blockJson.Length)
            {
                while (PlayButton.GetComponent<Toggle>().isOn == false) await Task.Delay(1);
                SeekBar.value++;
                await Task.Delay(1000);
            }
            PlayButton.GetComponent<Toggle>().isOn = false;
            isRepeating = false;
        }

        public void ClearBlocks()
        {
            for (int i = 0; i < blockJson.Length; i++)
            {
                Cube[i].SetActive(false);
            }
        }

        public void PlaceBlocks(float value)
        {
            ClearBlocks();
            for (int i = 0; i < value; i++)
            {
                Cube[i].SetActive(true);
            }
            BlockNumber = value;
        }

        private void ApplyColorRules()
        {
            string rulesJson = "{ \"rules\": [{ \"type\": \"color\", \"target\": 1, \"to\": 3},{ \"type\": \"ID\", \"target\": \"17411e0b-f945-47b0-9a87-974434eb5993\", \"to\": 1 }] }";
            Rule[] ruleData = CommunicationManager.JsonHelper.FromJson<Rule>(rulesJson, "Rules");
            for (int i = 0; i < ruleData.Length; i++)
            {
                string type = ruleData[i].type;
                string target = ruleData[i].target;
                Material toColorMaterial = Resources.Load("Color" + ruleData[i].to) as Material;
                if (toColorMaterial != null)
                {
                    if (type == "color")
                    {
                        string targetColorName = "Color" + target;
                        List<GameObject> targetObjectList = SearchBlockByColor(targetColorName);
                        for (int j = 0; j < targetObjectList.Count; j++)
                        {
                            targetObjectList[j].GetComponent<Renderer>().sharedMaterial = toColorMaterial;
                        }

                    }
                    else if (type == "ID")
                    {
                        string targetID = target;
                        GameObject targetObject = SearchBlockByID(targetID);
                        targetObject.GetComponent<Renderer>().sharedMaterial = toColorMaterial;
                    }
                    else
                    {
                        Debug.Log("Type is Invalid.");
                    }
                }
                else
                {
                    Debug.Log("To is Invalid.");
                }
            }
        }

        private List<GameObject> SearchBlockByColor(string targetColorName)
        {
            List<GameObject> blockObjectList = new List<GameObject>();
            Material targetColorMaterial = Resources.Load(targetColorName) as Material;
            if (targetColorMaterial != null)
            {
                for (int i = 0; i < blocks_data.Count; i++)
                {
                    if ("Color" + blocks_data[i].block_Info.colorID == targetColorName)
                    {
                        blockObjectList.Add(blocks_data[i].block_instance);
                    }
                }
            }
            else
            {
                Debug.Log("Target(Color) is Invalid.");
            }

            return blockObjectList;
        }

        private GameObject SearchBlockByID(string targetID)
        {
            GameObject blockObject = null;
            for (int i = 0; i < blocks_data.Count; i++)
            {
                if (blocks_data[i].block_Info.ID == targetID)
                {
                    blockObject = blocks_data[i].block_instance;
                    break;
                }
            }
            if (blockObject == null) Debug.Log("Target(ID) is Invalid.");

            return blockObject;
        }
    }

}