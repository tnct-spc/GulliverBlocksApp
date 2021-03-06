﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using JsonFormats;

namespace VrScene
{
    public class BlockManager : MonoBehaviour
    {
        public int BlocksCount;
        private List<Block> Blocks = new List<Block> { };
        private List<BlockInfo> UpdateBlocks = new List<BlockInfo> { }; // websocketで送られてきたものを一時的に保存
        private List<Rule> ColorRules = new List<Rule> { };
        public static string WorldID = "114e3ba9-a403-4c5c-a018-c7219c5bcc90";
        public static bool IsMerge = false;
        GameObject GameSystem;
        public float BlockNumber = 0;
        public bool isRepeating = false;
        InputManager InputManager;
        GameObject SeekBar;
        Slider seekbarSlider;
        Toggle PlayBackButton;
        GameManager GameManager;
        public GameObject LoadingWindow;
        public CommunicationManager CommunicationManager;
        CommunicationManager.WsClient WsClient;
        // patternBlocksの構造: {"pattern_name": {"pattern_group_id": [(BlockInfo),]}, }
        private Dictionary<string, Dictionary<string, List<BlockInfo>>> patternBlocks = new Dictionary<string, Dictionary<string, List<BlockInfo>>>();
        private List<GameObject> patternObjects = new List<GameObject>();
        private List<GameObject> usedPatternBlocks = new List<GameObject>();
        private float X_RATIO = 0.32f;
        private float Y_RATIO = 0.384f;
        private float Z_RATIO = 0.32f;
        public GameObject Floor;
        public GameObject Walls;
        public List<float> HighestPositions = new List<float>() {0, 0, 0, 0};//{一番大きいx, 一番小さいx, 一番大きいz, 一番小さいz}
        private void Awake()
        {
            CommunicationManager = new CommunicationManager();
            this.WsClient = new CommunicationManager.WsClient(WorldID);
            this.WsClient.OnBlockReceived += (sender, e) => this.UpdateBlocks = e.Blocks;// WSが来た時のイベント, parse済みのものがe.Blocksに入る
            this.WsClient.StartConenction();
            StartCoroutine("PingLoop");
        }

        private IEnumerator PingLoop()
        {
            while(true)
            {
                this.WsClient.ping();
                yield return new WaitForSeconds(30f);
            }
        }
        private void OnBlockUpdate(List<BlockInfo> blocks)
        {
            this.UpdateBlocks = blocks;
        }

        private void CheckBlockupdate()
        {
            /*
             * 毎フレームwebsocketの受信がないかチェックする,
             * 一フレームに2回以上送られてくることは想定していない
             */
            if (this.UpdateBlocks.Count > 0)
            {
                UpdateBlocks.FindAll(b => b.status == "add").ForEach(b => AddBlock(b));
                UpdateBlocks.FindAll(b => b.status == "delete").ForEach(b => DeleteBlock(b));
                UpdateBlocks.FindAll(b => b.status == "update").ForEach(b => UpdateBlock(b));
                this.ColorRules.ForEach(this.ApplyColorRule);

                if (UpdateBlocks.FindAll(b => b.status == "update").Count> 0)
                {
                    UpdatePatternBlocks(UpdateBlocks.FindAll(b => b.status == "update"));
                }
            }
            this.UpdateBlocks = new List<BlockInfo> { };
        }

        private void Update()
        {
            CheckBlockupdate();
        }
        private void Start()
        {
            LoadingWindow.SetActive(true);
            GameSystem = GameObject.Find("GameSystem");
            InputManager = GameSystem.GetComponent<InputManager>();
            SeekBar = InputManager.Seekbar;
            seekbarSlider = InputManager.seekbarSlider;
            PlayBackButton = InputManager.PlayBackButton;
            GameManager = GameSystem.GetComponent<GameManager>();
            StartCoroutine("FetchData");
        }

        IEnumerator FetchData()
        {
            Task<List<BlockInfo>> fetchBlocksTask;
            Task<List<Rule>> fetchColorRulesTask; 
            if (IsMerge)
            {
                fetchBlocksTask = CommunicationManager.fetchMergedBlocksAsync(WorldID);
                fetchColorRulesTask = CommunicationManager.fetchMergedColorRulesAsync(WorldID);
            } else
            {
                fetchBlocksTask = CommunicationManager.fetchMapBlocksAsync(WorldID);
                fetchColorRulesTask = CommunicationManager.fetchColorsAsync(WorldID);
            }
            yield return new WaitUntil(() => fetchBlocksTask.IsCompleted); // 通信中の場合次のフレームに処理を引き継ぐ
            fetchBlocksTask.Result.ForEach(this.AddBlock); // 全てのブロックを配置
            this.ReplacePatternWithObject(); // パターン認識されたブロックをオブジェクトに置き換える

            yield return new WaitUntil(() => fetchColorRulesTask.IsCompleted);
            this.ColorRules = fetchColorRulesTask.Result;
            this.ColorRules.ForEach(this.ApplyColorRule);

            if (GameManager.Mode == "PlayBack") InputManager.PlayBackModeUI.SetActive(true);
            SetFloor();
            SetWall();
            LoadingWindow.SetActive(false);
        }
        
        public void StopPlayback()
        {
            GameManager.Mode = "Vr";
            seekbarSlider.value = seekbarSlider.maxValue;
            InputManager.PlayBackModeUI.SetActive(false);
            isRepeating = false;
            for (int i = 0; i < BlocksCount; i++)
            {
                Blocks[i].SetActive(true);
                Vector3 pos = Blocks[i].transform.position;
                pos.y = Blocks[i].GetPosition().y;
                Blocks[i].transform.position = pos;
            }
        }
        public void StartPlayback()
        {
            GameManager.Mode = "PlayBack";
            InputManager.PlayBackModeUI.SetActive(true);
            InputManager.ViewModeUI.SetActive(false);
            for (int i =0;i<BlocksCount;i++)
            {
                Blocks[i].SetActive(false);
                Vector3 pos = Blocks[i].transform.position;
                pos.y = Blocks[i].GetPosition().y + 20;
                Blocks[i].transform.position = pos;
            }
        }

        void InitialPlacement(List<BlockInfo> blocksInfo)
        {
            blocksInfo.ForEach(b => AddBlock(b));
        }

        private void SetFloor()
        {
            GameObject FloorA;

            Vector3 CornerPosition = new Vector3(HighestPositions[0], 0, HighestPositions[2]);
            Vector3 AnotherCornerPosition = new Vector3(HighestPositions[1], 0, HighestPositions[3]);
            GameObject FloorObj = Resources.Load("Floor_10") as GameObject;

            if (IsMerge)
            {
                for (float i = AnotherCornerPosition.x - 6.4f; i < CornerPosition.x + 6.4f; i += 3.2f)
                {
                    for (float j = AnotherCornerPosition.z - 6.4f; j < CornerPosition.z + 6.4f; j += 3.2f)
                    {
                        FloorA = (GameObject)Instantiate(FloorObj, new Vector3(i, 0.2f, j), Quaternion.identity);
                        FloorA.transform.parent = Floor.transform;
                    }
                }
            }
            else
            {
                for (float i = -4; i < 4; i++)
                {
                    for (float j = -4; j < 4; j++)
                    {
                        FloorA = (GameObject)Instantiate(FloorObj, new Vector3(10 * 0.32f * i, 0.2f, 10 * 0.32f * j), Quaternion.identity);
                        FloorA.transform.parent = Floor.transform;
                    }
                }
            }
            Floor.transform.position = new Vector3(3.2f, 0, 3.2f);
        }

        private void SetWall()
        {
            GameObject Wall;
            GameObject WallObj = Resources.Load("Wall") as GameObject;

            Vector3 CornerPosition = new Vector3(HighestPositions[0], 0, HighestPositions[2]);
            Vector3 AnotherCornerPosition = new Vector3(HighestPositions[1], 0, HighestPositions[3]);

            if (IsMerge)
            {
                Wall = (GameObject)Instantiate(WallObj, new Vector3(CornerPosition.x + 6.4f, 0, 0), Quaternion.identity);
                Wall.transform.localScale = new Vector3(0.1f, 1000, 1000);
                Wall.transform.parent = Walls.transform;

                Wall = (GameObject)Instantiate(WallObj, new Vector3(AnotherCornerPosition.x + -6.4f, 0, 0), Quaternion.identity);
                Wall.transform.localScale = new Vector3(0.1f, 1000, 1000);
                Wall.transform.parent = Walls.transform;

                Wall = (GameObject)Instantiate(WallObj, new Vector3(0, 0, CornerPosition.z + 6.4f), Quaternion.identity);
                Wall.transform.localScale = new Vector3(1000, 1000, 0.1f);
                Wall.transform.parent = Walls.transform;

                Wall = (GameObject)Instantiate(WallObj, new Vector3(0, 0, AnotherCornerPosition.z - 6.4f), Quaternion.identity);
                Wall.transform.localScale = new Vector3(1000, 1000, 0.1f);
                Wall.transform.parent = Walls.transform;
            }
            else
            {
                Wall = (GameObject)Instantiate(WallObj, new Vector3(4 * 3.2f + 3.2f, 0, 0), Quaternion.identity);
                Wall.transform.localScale = new Vector3(0.1f, 1000, 1000);
                Wall.transform.parent = Walls.transform;

                Wall = (GameObject)Instantiate(WallObj, new Vector3(-4 * 3.2f, 0, 0), Quaternion.identity);
                Wall.transform.localScale = new Vector3(0.1f, 1000, 1000);
                Wall.transform.parent = Walls.transform;

                Wall = (GameObject)Instantiate(WallObj, new Vector3(0, 0, 4 * 3.2f + 3.2f), Quaternion.identity);
                Wall.transform.localScale = new Vector3(1000, 1000, 0.1f);
                Wall.transform.parent = Walls.transform;

                Wall = (GameObject)Instantiate(WallObj, new Vector3(0, 0, -4 * 3.2f), Quaternion.identity);
                Wall.transform.localScale = new Vector3(1000, 1000, 0.1f);
                Wall.transform.parent = Walls.transform;
            }
            Walls.transform.position = new Vector3(-3.2f, 0 ,-3.2f);
        }

        private void AddBlock(BlockInfo blockInfo)
        {
            Object blockPrefab = (GameObject)Resources.Load("Block");
            GameObject blockObject = Instantiate(blockPrefab, blockInfo.GetPosition(), Quaternion.identity) as GameObject;
            blockObject.name = blockInfo.ID;
            Block block = blockObject.GetComponent<Block>();
            block.SetColor(blockInfo.colorID, false);
            block.SetBlockData(blockInfo);
            if (GameManager.Mode == "PlayBack") block.SetActive(false);
            this.Blocks.Add(block);
            // NeutralPositions.Add(Blocks[BlocksCount].transform.position.y);
            this.BlocksCount += 1;

            if (blockInfo.pattern_name != null && blockInfo.pattern_name != "" && blockInfo.pattern_name != "null")
            {
                // pattern_nameがKeysに存在しないなら新しく追加する
                List<string> patternNameKeys = new List<string>(patternBlocks.Keys);
                if (!(patternNameKeys.IndexOf(blockInfo.pattern_name) >= 0))
                {
                    patternBlocks[blockInfo.pattern_name] = new Dictionary<string, List<BlockInfo>>();
                }
                // pattern_group_idがKeysに存在しないなら新しく追加する
                List<string> pattern_group_id_keys = new List<string>(patternBlocks[blockInfo.pattern_name].Keys);
                if (!(pattern_group_id_keys.IndexOf(blockInfo.pattern_group_id) >= 0))
                {
                    patternBlocks[blockInfo.pattern_name][blockInfo.pattern_group_id] = new List<BlockInfo>();
                }

                patternBlocks[blockInfo.pattern_name][blockInfo.pattern_group_id].Add(blockInfo);
            }
        }

        private void DeleteBlock(BlockInfo blockInfo)
        {
            var block =  this.Blocks.Find(b => b.ID == blockInfo.ID);
            GameObject blockObject = usedPatternBlocks.Find(b => b.name == blockInfo.ID);
            usedPatternBlocks.Remove(blockObject);
            block.Delete();
            this.Blocks.Remove(block);
            this.BlocksCount -= 1;
        }

        private void UpdateBlock(BlockInfo blockInfo)
        {
            var block = this.Blocks.Find(b => b.ID == blockInfo.ID);
            block.SetBlockData(blockInfo);
        }

        private void UpdatePatternBlocks(List<BlockInfo> blockInfos)
        {
            patternBlocks.Clear();
            foreach (BlockInfo blockInfo in blockInfos)
            {
                if (blockInfo.pattern_name != null && blockInfo.pattern_name != "" && blockInfo.pattern_name != "null")
                {
                    // pattern_nameがKeysに存在しないなら新しく追加する
                    List<string> patternNameKeys = new List<string>(patternBlocks.Keys);
                    if (!(patternNameKeys.IndexOf(blockInfo.pattern_name) >= 0))
                    {
                        patternBlocks[blockInfo.pattern_name] = new Dictionary<string, List<BlockInfo>>();
                    }
                    // pattern_group_idがKeysに存在しないなら新しく追加する
                    List<string> pattern_group_id_keys = new List<string>(patternBlocks[blockInfo.pattern_name].Keys);
                    if (!(pattern_group_id_keys.IndexOf(blockInfo.pattern_group_id) >= 0))
                    {
                        patternBlocks[blockInfo.pattern_name][blockInfo.pattern_group_id] = new List<BlockInfo>();
                    }
                    patternBlocks[blockInfo.pattern_name][blockInfo.pattern_group_id].Add(blockInfo);
                }
                else
                {
                    GameObject blockObject = usedPatternBlocks.Find(b => b.name == blockInfo.ID);
                    if (blockObject != null)
                    {
                        blockObject.SetActive(true);
                    }
                    usedPatternBlocks.Remove(blockObject);
                }
            }
            ReplacePatternWithObject();
        }

        public void BlockSkip(int slider,bool SkipOrBack)
        {
            Vector3 pos = Blocks[slider].transform.position;
            if (SkipOrBack)
            {
                pos.y = Blocks[slider].GetPosition().y;
            }
            else
            {
                pos.y = Blocks[slider].GetPosition().y + 20;
            }
            Blocks[slider].transform.position = pos;
        }

        public async void RepeatPlaceBlocks()
        {
            isRepeating = true;
            SeekBar.SetActive(true);
            while (true)
            {
                if (isRepeating == false)
                {
                    await Task.Delay(1);
                }
                else
                {
                    if (seekbarSlider.value == seekbarSlider.maxValue)
                    {
                        while (Blocks[(int)seekbarSlider.maxValue - 1].transform.position.y != Blocks[(int)seekbarSlider.maxValue -1].GetPosition().y)
                        {
                            await Task.Delay(1);
                        }
                        await Task.Delay(3000);
                        StartPlayback();
                        break;
                    }
                    double FirstBlockTime = Blocks[(int)seekbarSlider.value].time;
                    FallingBlock((int)seekbarSlider.value);
                    seekbarSlider.value++;

                    if (seekbarSlider.value != seekbarSlider.maxValue)
                    {
                        if (Blocks[(int)seekbarSlider.value].time - FirstBlockTime > 1)
                            await Task.Delay(1000);
                    }
                }
            }
            PlayBackButton.GetComponent<Toggle>().isOn = false;
            if(GameManager.Mode == "PlayBack")
            {
                ClearBlocks();
                seekbarSlider.value = 0;
                isRepeating = false;
            }
        }

        async void FallingBlock(int i)
        {
            float Accel = 0f;
            for (float j = Blocks[i].transform.position.y; j > Blocks[i].GetPosition().y; j -= Accel)
            {
                Vector3 pos = Blocks[i].transform.position;
                pos.y = j;
                Blocks[i].transform.position = pos;
                Accel += 0.00981f;
                await Task.Delay(1);
            }
            Vector3 pos2 = Blocks[i].transform.position;
            pos2.y = Blocks[i].GetPosition().y;
            Blocks[i].transform.position = pos2;
        }
        public void ClearBlocks()
        {
            for (int i = 0; i < this.BlocksCount; i++)
            {
                Blocks[i].SetActive(false);
            }
        }

        public void PlaceBlocks(float value)
        {
            ClearBlocks();
            for (int i = 0; i < value; i++)
            {
                Blocks[i].SetActive(true);
            }
            BlockNumber = value;
        }
        public void ApplyColorRule(Rule ruleData)
        {
            string type = ruleData.type;
            string to = ruleData.to;
            Material toColorMaterial = Resources.Load("Materials/Color" + to) as Material;
            if (toColorMaterial == null)
            {
                Debug.Log("To is Invalid.");
                return;
            }
            if (type == "color")
            {
                string origin = ruleData.origin.Replace(" (Instance)", "");
                List<Block> targetBlocks = this.Blocks.FindAll(block => (block.map_id == ruleData.map_id) && block.applied_colorID == origin);
                //Debug.Log(targetBlocks.Count);
                targetBlocks.ForEach(block =>
                {
                    block.SetColor(to, false);
                });

            }
            else if (type == "ID")
            {
                Block targetBlock = this.Blocks.Find(block => block.ID == ruleData.block_id);
                if (targetBlock == null) return;
                targetBlock.SetColor(to, false);
            }
            else
            {
                Debug.Log("Type is Invalid.");
            }
        }

        private void ReplacePatternWithObject()
        {
            /*
             パターン認識されたブロックをオブジェクトに置き換える
             */

            // オブジェクトをいったん全部消す
            foreach (GameObject _patternObject in patternObjects)
            {
                GameObject patternObject = _patternObject;
                Destroy(patternObject);
            }
            patternObjects.Clear();

            List<string> patternNameKeys = new List<string>(patternBlocks.Keys);
            foreach (string patternName in patternNameKeys)
            {
                string patternMaterialName = patternName + "Material";
                Material patternMaterial = Resources.Load(patternMaterialName) as Material;

                List<string> patternGroupIdKeys = new List<string>(patternBlocks[patternName].Keys);
                foreach (string patternGroupId in patternGroupIdKeys)
                {   
                    // 使ったblockの非表示
                    foreach(BlockInfo blockInfo in patternBlocks[patternName][patternGroupId])
                    {
                        GameObject blockObject = GameObject.Find(blockInfo.ID);
                        if (blockObject != null)
                        {
                            blockObject.SetActive(false);
                            usedPatternBlocks.Add(blockObject);
                        }
                    }

                    // 原点に近い順にsort
                    patternBlocks[patternName][patternGroupId].Sort(
                        (a, b) => (a.x * a.x + a.y * a.y + a.z * a.z) - (b.x * b.x + b.y * b.y + b.z * b.z)
                    );

                    // サイズなどは先に出しておく
                    BlockInfo nearestBlock = patternBlocks[patternName][patternGroupId][0];
                    BlockInfo farthestBlock = patternBlocks[patternName][patternGroupId][patternBlocks[patternName][patternGroupId].Count - 1];
                    float width = (Mathf.Abs(farthestBlock.x - nearestBlock.x) + 1) * X_RATIO;
                    float height = (Mathf.Abs(farthestBlock.y - nearestBlock.y) + 1) * Y_RATIO;
                    float depth = (Mathf.Abs(farthestBlock.z - nearestBlock.z) + 1) * Z_RATIO;

                    // 各パターンに応じた処理をする
                    switch (patternName)
                    {
                        case "verticalRoad":
                            {
                                // 道路を生成
                                GameObject patternObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                patternObject.transform.position = new Vector3(
                                    (nearestBlock.x + farthestBlock.x) * X_RATIO / 2.0f,
                                    (nearestBlock.y + farthestBlock.y) * Y_RATIO / 2.0f,
                                    (nearestBlock.z + farthestBlock.z) * Z_RATIO / 2.0f
                                );
                                patternObject.name = patternGroupId;
                                patternObject.transform.localScale = new Vector3(width, height, depth);
                                patternObject.GetComponent<Renderer>().sharedMaterial = patternMaterial;
                                patternObject.AddComponent<VerticalRoadManager>();
                                patternObjects.Add(patternObject);
                                break;
                            }

                        case "horizontalRoad":
                            {
                                // 道路を生成
                                GameObject patternObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                patternObject.transform.position = new Vector3(
                                    (nearestBlock.x + farthestBlock.x) * X_RATIO / 2.0f,
                                    (nearestBlock.y + farthestBlock.y) * Y_RATIO / 2.0f,
                                    (nearestBlock.z + farthestBlock.z) * Z_RATIO / 2.0f
                                );
                                patternObject.name = patternGroupId;
                                patternObject.transform.localScale = new Vector3(width, height, depth);
                                patternObject.GetComponent<Renderer>().sharedMaterial = patternMaterial;
                                patternObject.AddComponent<HorizontalRoadManager>();
                                patternObjects.Add(patternObject);
                                break;
                            }
                    }
                }
            }
        }

        public Rule MakeColorRules(string type, string map_id, string origin, string to)
        {
            Rule rule = new Rule();
            rule.type = type;
            rule.origin = origin;
            rule.to = to;
            rule.map_id = map_id;
            return rule;
        }
    }
}
