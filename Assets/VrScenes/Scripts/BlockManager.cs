using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class BlockManager : MonoBehaviour
{
    GameObject GameSystem;
    static string response_json;
    public bool isPlacingBlock = false;
    public float BlockNumber = 0;
    List<Block> blocks;
    InputManager InputManager;
    Slider SeekBar;

    public int BlockCount = 0;

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
        GameSystem = GameObject.Find("GameSystem");
        InputManager = GameSystem.GetComponent<InputManager>();
        SeekBar = InputManager.SeekBar;
        var _ = FetchAndPlaceBlocks();
    } // 警告メッセージ回避のために変数に代入する
    

    async System.Threading.Tasks.Task FetchAndPlaceBlocks()
    {
        string server_url = "http://gulliverblocks.herokuapp.com/get_blocks/" + WorldID + "/";

        using (var http_client = new HttpClient())
        {
            // getリクエストを投げてレスポンスのbodyを読み込む
            response_json = await http_client.GetStringAsync(server_url);
        }
        getBlockCount(jsonToBlock(response_json));
        if (GameManager.Mode != "Vr")
        {
            PlaceBlocks(BlockCount);
            ApplyColorRules();
        }
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
        blocks.Sort((x, y) => x.time.CompareTo(y.time));

        return blocks;
    }
    void getBlockCount(List<Block> blocks)
    {
        blocks = jsonToBlock(response_json);
        BlockCount = blocks.Count;
    }

    public void StartPlaceBlocks()
    {
        RepeatPlaceBlocks(jsonToBlock(response_json));
    }
    async void RepeatPlaceBlocks(List<Block> blocks)
    {
        isPlacingBlock = true;
        for (float i = BlockNumber; BlockNumber < BlockCount + 1; BlockNumber++)
        {
            while (isPlacingBlock == false) await Task.Delay(1);
            SeekBar.value++;
            if (GameManager.Mode == "Vr")
            {
                await Task.Delay(1000);
            }
        }
        isPlacingBlock = false;
        InputManager.PlayButton.GetComponent<Toggle>().isOn = false;
    }

    public void DestroyBlocks()
    {
        for (int i = 0; i < BlockCount; i++)
        {
            GameObject cube = GameObject.Find("Cube" + i);
            Destroy(cube);
        }
        BlockNumber = 0;
    }

    public void PlaceBlocks(float value)
    {
        DestroyBlocks();
        blocks = jsonToBlock(response_json);
        Object cube = (GameObject)Resources.Load("Cube");
        for (int i = 0; i < value; i++)
        {
            GameObject instance = Instantiate(cube, blocks[i].getPosition(), Quaternion.identity) as GameObject;
            string colorName = "Color" + blocks[i].colorID.ToString();
            Material colorMaterial = Resources.Load(colorName) as Material;
            instance.GetComponent<Renderer>().sharedMaterial = colorMaterial;
            instance.name = "Cube" + i;
            blocks_data.Add((blocks[i], instance));
        }
        BlockNumber = value;
    }

    private void ApplyColorRules()
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

    private GameObject SearchBlockByID(string targetID)
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

    private static class JsonHelper
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