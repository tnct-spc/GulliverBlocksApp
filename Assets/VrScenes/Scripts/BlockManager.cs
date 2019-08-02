using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class BlockManager : MonoBehaviour
{
    static string response_json;
    public bool isPlacingBlock = true;
    public bool hasEndedPlacingBlock = true;
    private int blockNumber = 0;
    List<Block> blocks;
    private struct Block
    {
        public float x;
        public float y;
        public float z;
        public string ID;
        public float time;
        public bool put;
        public int colorID;

        public Block(float x, float y, float z, string ID, float time, bool put, int colorID)
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

    async Task fetchAndPlaceBlocks()
    {
        string server_url = "http://gulliverblocks.herokuapp.com/get_blocks/" + WorldID + "/";

        using (var http_client = new HttpClient())
        {
            // getリクエストを投げてレスポンスのbodyを読み込む
            response_json = await http_client.GetStringAsync(server_url);
        }
        if (GameManager.Mode != "Vr")
        {
            isPlacingBlock = true;
            hasEndedPlacingBlock = false;
            PlaceBlock();
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

        return blocks;
    }

    public async void PlaceBlock()
    {
        hasEndedPlacingBlock = false;

        blocks = jsonToBlock(response_json);
        Object cube = (GameObject)Resources.Load("Cube");
        for (int i = blockNumber; blockNumber < blocks.Count; blockNumber++)
        {
            while (isPlacingBlock == false) await Task.Delay(1);
            if (hasEndedPlacingBlock) break;
            GameObject instance = Instantiate(cube, blocks[blockNumber].getPosition(), Quaternion.identity) as GameObject;
            string colorName = "Color" + blocks[blockNumber].colorID.ToString();
            Material colorMaterial = Resources.Load(colorName) as Material;
            instance.GetComponent<Renderer>().sharedMaterial = colorMaterial;
            instance.name = "Cube" + blockNumber;
            blocks_data.Add((blocks[blockNumber], instance));
            if (GameManager.Mode == "Vr") await Task.Delay(1000);
        }
        hasEndedPlacingBlock = true;
    }

    public void DestroyBlocks()
    {
        isPlacingBlock = true;
        if (blocks == null) return;
        for (int i = 0; i < blocks.Count; i++)
        {
            GameObject cube = GameObject.Find("Cube" + i);
            Destroy(cube);
        }
        blockNumber = 0;
        hasEndedPlacingBlock = true;
    }
}
