﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using System.Text.RegularExpressions;

public class BlockManager : MonoBehaviour
{
    private struct Block
    {
        public float x;
        public float y;
        public float z;
        public string ID;
        public float time;
        public bool put;
        public string colorID;

        public Block(float x, float y, float z, string ID, float time, bool put, string colorID)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.ID = ID;
            this.time = time;
            this.put = put;
            this.colorID = colorID;
        }

        public Vector3 getPosition()
        {
            Vector3 position = new Vector3(x, y, z);
            return position;
        }
    }

    private List<(Block block_struct, GameObject block_instance)> blocks_data = new List<(Block block_struct, GameObject block_instance)>();
    public static string WorldID;

    private void Start()
    {
        var _ = fetchAndPlaceBlocks();  // 警告メッセージ回避のために変数に代入する
    }

    async System.Threading.Tasks.Task fetchAndPlaceBlocks()
    {
        string server_url = "http://gulliverblocks.herokuapp.com/get_blocks/" + WorldID + "/";

        string response_json;

        using (var http_client = new HttpClient())
        {
            // getリクエストを投げてレスポンスのbodyを読み込む
            response_json = await http_client.GetStringAsync(server_url);
        }

        placeBlock(jsonToBlock(response_json));
        applyColorRules();
    }

    private List<Block> jsonToBlock(string json)
    {

        // jsonの不要な文字列を削除
        json = json.Replace("{\"blocks\":[", "");
        json = json.Replace("]}", "");

        string[] json_array = Regex.Split(json, @"(?<=}),");  // 要素に分ける

        // jsonからBlockを生成
        List<Block> blocks = new List<Block>();
        for (int i = 0; i < json_array.Length; i++)
        {
            Block block = JsonUtility.FromJson<Block>(json_array[i]);
            blocks.Add(block);
        }

        return blocks;
    }

    void placeBlock(List<Block> blocks)
    {
        Object cube = (GameObject)Resources.Load("Cube");
        for (int i = 0; i < blocks.Count; i++)
        {
            GameObject instance = Instantiate(cube, blocks[i].getPosition(), Quaternion.identity) as GameObject;
            string colorName = "Color" + blocks[i].colorID.ToString();
            Material colorMaterial = Resources.Load(colorName) as Material;
            instance.GetComponent<Renderer>().sharedMaterial = colorMaterial;
            blocks_data.Add((blocks[i], instance));
        }
    }

    public void applyColorRules()
    {
        string rulesJson = "{ \"rules\": [{ \"type\": \"color\", \"target\": 1, \"to\": 3},{ \"type\": \"ID\", \"target\": \"17411e0b-f945-47b0-9a87-974434eb5993\", \"to\": 1 }] }";
        Rule[] ruleData = JsonHelper.FromJson<Rule>(rulesJson);
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
                    List<GameObject> targetObjectList = searchBlockByColor(targetColorName);
                    for (int j = 0; j < targetObjectList.Count; j++)
                    {
                        targetObjectList[j].GetComponent<Renderer>().sharedMaterial = toColorMaterial;
                    }

                }
                else if (type == "ID")
                {
                    string targetID = target;
                    GameObject targetObject = searchBlockByID(targetID);
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

    public List<GameObject> searchBlockByColor(string targetColorName)
    {
        List<GameObject> blockObjectList = new List<GameObject>();
        Material targetColorMaterial = Resources.Load(targetColorName) as Material;
        if(targetColorMaterial != null)
        {
            for (int i = 0; i < blocks_data.Count; i++)
            {
                if("Color" + blocks_data[i].block_struct.colorID == targetColorName)
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

    public GameObject searchBlockByID(string targetID)
    {
        GameObject blockObject = null;
        for(int i = 0; i < blocks_data.Count; i++)
        {
            if (blocks_data[i].block_struct.ID == targetID)
            {
                blockObject = blocks_data[i].block_instance;
                break;
            }
        }
        if (blockObject == null) Debug.Log("Target(ID) is Invalid.");

        return blockObject;
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.rules;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] rules;
        }
    }

    [System.Serializable]
    public class Rule
    {
        public string type;
        public string target;
        public string to;
    }
}
