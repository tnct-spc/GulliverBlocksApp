using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public struct Block
{
    public float x;
    public float y;
    public float z;
    public string ID;
    public float time;
    public bool put;
    public string colorID;

    public Vector3 GetPosition()
    {
        Vector3 position = new Vector3(x, y, z);
        return position;
    }
}


public class BlockManager : MonoBehaviour
{
    private List<(IncludingBlockInfo block_Info, GameObject block_instance)> blocks_data = new List<(IncludingBlockInfo block_info, GameObject block_instance)>();
    public static string WorldID;
    public string ServerAddress = "gulliverblocks.herokuapp.com";

    private void Start()
    {
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
            Block[] blockJson = CommunicationManager.JsonHelper.FromJson<Block>(webRequest.downloadHandler.text, "Blocks");
            PlaceBlock(blockJson);
            ApplyColorRules();
        }
    }

    public void PlaceBlock(Block[] blocks)
    {
        Object cube = (GameObject)Resources.Load("Cube");
        for (int i = 0; i < blocks.Length; i++)
        {
            GameObject instance = Instantiate(cube, blocks[i].GetPosition(), Quaternion.identity) as GameObject;
            string colorName = "Color" + blocks[i].colorID.ToString();
            Material colorMaterial = Resources.Load(colorName) as Material;
            instance.GetComponent<Renderer>().sharedMaterial = colorMaterial;
            IncludingBlockInfo blockInfo = instance.GetComponent<IncludingBlockInfo>();
            blockInfo.SetBlockData(blocks[i]);
            blocks_data.Add((blockInfo, instance));
        }
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
