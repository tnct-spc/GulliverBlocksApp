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

    private void Start()
    {
        getDataFromServerAndCreateBlock();
    }

    void placeBlock(Block[] block_list)
    {
        Object cube = (GameObject)Resources.Load("Cube");
        for (int i = 0; i < block_list.Length; i++)
        {
            Instantiate(cube, block_list[i].getPosition(), Quaternion.identity);
        }
    }

    private Block[] jsonToBlock(string json)
    {
        // jsonの不要な文字列を削除
        json.Trim('{', '}');
        json = json.Replace("[", "");
        json = json.Replace("]", "");
        json = json.Replace("blocks:", "");

        string[] json_array = Regex.Split(json, @"},");  // 要素に分ける

        // jsonからBlockを生成
        List<Block> block_list = new List<Block>();
        for (int i = 0; i < json_array.Length; i++)
        {
            json_array[i] = json_array[i].Insert(json_array[i].Length, "}");
            Block block = JsonUtility.FromJson<Block>(json_array[i]);
            block_list.Add(block);
        }
        Block[] blocks = block_list.ToArray();

        return blocks;
    }

    async System.Threading.Tasks.Task getDataFromServerAndCreateBlock()
    {
        string server_url = "https://gulliverblocksserver.herokuapp.com";

        string response_json;

        using (var http_client = new HttpClient())
        {
            // getリクエストを投げる
            HttpResponseMessage response = await http_client.GetAsync(server_url);

            // 通信に失敗したら例外を投げる
            response.EnsureSuccessStatusCode();

            // レスポンスのbodyを読み込む
            response_json = await response.Content.ReadAsStringAsync();
        }

        Block[] block_list = jsonToBlock(response_json);

        placeBlock(block_list);
    }
}
