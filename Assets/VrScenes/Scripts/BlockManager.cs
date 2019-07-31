using System.Collections;
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
        public int ID;
        public float time;
        public bool put;
        public int colorID;

        public Block(float x, float y, float z, int ID, float time, bool put, int colorID)
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
        var _ =  fetchAndPlaceBlocks();  // 警告メッセージ回避のために変数に代入する
    }

    async System.Threading.Tasks.Task fetchAndPlaceBlocks()
    {
        string server_url = "http://gulliverblocks.herokuapp.com/get_blocks/" + WorldID;

        string response_json;

        using (var http_client = new HttpClient())
        {
            // getリクエストを投げてレスポンスのbodyを読み込む
            response_json = await http_client.GetStringAsync(server_url);
        }

        placeBlock(jsonToBlock(response_json));
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
            Material colorMaterial2 = Resources.Load(colorName) as Material;
            instance.GetComponent<Renderer>().sharedMaterial = colorMaterial2;
            blocks_data.Add((blocks[i], instance));
        }
    }
}
