using System.Collections;
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
        List<float> NeutralPositions = new List<float>();
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
        CommunicationManager CommunicationManager;
        CommunicationManager.WsClient WsClient;
        public GameObject Floor;

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
                this.ColorRules.ForEach(this.ApplyColorRules);
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
            SetFloor();
            StartCoroutine("FetchData");
        }

        IEnumerator FetchData()
        {
            var fetchBlocksTask = IsMerge ? CommunicationManager.fetchMergedBlocksAsync(WorldID) :
                CommunicationManager.fetchMapBlocksAsync(WorldID);
            var fetchColorRulesTask = CommunicationManager.fetchColorsAsync(WorldID);
            yield return new WaitUntil(() => fetchBlocksTask.IsCompleted); // 通信中の場合次のフレームに処理を引き継ぐ
            fetchBlocksTask.Result.ForEach(this.AddBlock);// 全てのブロックを配置
            yield return new WaitUntil(() => fetchColorRulesTask.IsCompleted);
            this.ColorRules = fetchColorRulesTask.Result;
            this.ColorRules.ForEach(this.ApplyColorRules);
            if (GameManager.Mode == "PlayBack") InputManager.PlayBackModeUI.SetActive(true);
            LoadingWindow.SetActive(false);
        }
        
        public void StopPlayback()
        {
            GameManager.Mode = "Vr";
            seekbarSlider.value = seekbarSlider.maxValue;
            InputManager.PlayBackModeUI.SetActive(false);
            isRepeating = false;
        }
        public void StartPlayback()
        {
            GameManager.Mode = "PlayBack";
            InputManager.PlayBackModeUI.SetActive(true);
        }

        void InitialPlacement(List<BlockInfo> blocksInfo)
        {
            blocksInfo.ForEach(b => AddBlock(b));
        }

        private void SetFloor()
        {
            GameObject FloorA;
            GameObject FloorB;
            GameObject Floor1 = (GameObject)Resources.Load("Floor1");
            GameObject Floor2 = (GameObject)Resources.Load("Floor2");
            for (float i = -24; i < 24; i++)
            {
                for (float j = -24; j < 24; j++)
                {
                    FloorA = (GameObject)Instantiate(Floor1, new Vector3(0.32f * i, -0.2379662f, 0.32f * j), Quaternion.identity);
                    FloorA.transform.parent = Floor.transform;
                    FloorB = (GameObject)Instantiate(Floor2, new Vector3(0.32f * i, -0.05f, 0.32f * j), Quaternion.identity);
                    FloorB.transform.parent = Floor.transform;
                }
            }
        }

        private void AddBlock(BlockInfo blockInfo)
        {
            Object blockPrefab = (GameObject)Resources.Load("pblock1x1");
            Block block = (Instantiate(blockPrefab, blockInfo.GetPosition(), Quaternion.identity) as GameObject).GetComponent<Block>();
            block.SetColor(blockInfo.colorID);
            block.SetBlockData(blockInfo);
            if (GameManager.Mode == "PlayBack") block.SetActive(false);
            this.Blocks.Add(block);
            NeutralPositions.Add(Blocks[BlocksCount].transform.position.y);
            this.BlocksCount += 1;
        }

        private void DeleteBlock(BlockInfo blockInfo)
        {
            var block =  this.Blocks.Find(b => b.ID == blockInfo.ID);
            block.Delete();
            this.Blocks.Remove(block);
            this.BlocksCount -= 1;
        }

        private void UpdateBlock(BlockInfo blockInfo)
        {
            var block = this.Blocks.Find(b => b.ID == blockInfo.ID);
            block.SetBlockData(blockInfo);
        }

        public async void RepeatPlaceBlocks()
        {
            isRepeating = true;
            SeekBar.SetActive(true);
            while (true)
            {
                if (seekbarSlider.value == seekbarSlider.maxValue)
                {
                    await Task.Delay(3000);
                    break;
                }
                int FirstBlockTime = (int)Blocks[(int)seekbarSlider.value].time;
                FallingBlock((int)seekbarSlider.value);
                seekbarSlider.value++;
                if(seekbarSlider.value != seekbarSlider.maxValue)
                {
                    if ((FirstBlockTime - (int)Blocks[(int)seekbarSlider.value].time) * (FirstBlockTime - (int)Blocks[(int)seekbarSlider.value].time) >= 10)
                        await Task.Delay(1000);
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
            for (float j = NeutralPositions[i]+20; j > NeutralPositions[i]; j -= Accel)
            {
                Vector3 pos = Blocks[i].transform.position;
                pos.y = j;
                Blocks[i].transform.position = pos;
                Accel += 0.00981f;
                await Task.Delay(1);
            }
            Vector3 pos2 = Blocks[i].transform.position;
            pos2.y = NeutralPositions[i];
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
        public void ApplyColorRules(Rule ruleData)
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
                List<Block> targetBlocks = this.Blocks.FindAll(block => block.colorID == origin);
                targetBlocks.ForEach(block =>
                {
                    block.SetColor(to);
                });

            }
            else if (type == "ID")
            {
                Block targetBlock = this.Blocks.Find(block => block.ID == ruleData.block_id);
                if (targetBlock == null) return;
                targetBlock.SetColor(to);
            }
            else
            {
                Debug.Log("Type is Invalid.");
            }
        }

        public Rule MakeColorRules(string type, string origin, string to)
        {
            Rule rule = new Rule();
            rule.type = type;
            rule.origin = origin;
            rule.to = to;
            return rule;
        }
    }
}