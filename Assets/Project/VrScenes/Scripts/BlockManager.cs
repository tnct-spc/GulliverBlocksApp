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
        // patternBlocksの構造: {"pattern_name": {"pattern_group_id": [(BlockInfo),]}, }
        private Dictionary<string, Dictionary<string, List<BlockInfo>>> patternBlocks = new Dictionary<string, Dictionary<string, List<BlockInfo>>>();
        private float X_RATIO = 0.32f;
        private float Y_RATIO = 0.384f;
        private float Z_RATIO = 0.32f;

        private void Awake()
        {
            CommunicationManager = new CommunicationManager();
            this.WsClient = new CommunicationManager.WsClient(WorldID);
            this.WsClient.OnBlockReceived += (sender, e) => this.UpdateBlocks = e.Blocks;// WSが来た時のイベント, parse済みのものがe.Blocksに入る
            this.WsClient.StartConenction();
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
            StartCoroutine("FetchData");
        }

        IEnumerator FetchData()
        {
            var fetchBlocksTask = IsMerge ? CommunicationManager.fetchMergedBlocksAsync(WorldID) :
                CommunicationManager.fetchMapBlocksAsync(WorldID);
            var fetchColorRulesTask = CommunicationManager.fetchColorsAsync(WorldID);
            yield return new WaitUntil(() => fetchBlocksTask.IsCompleted); // 通信中の場合次のフレームに処理を引き継ぐ
            fetchBlocksTask.Result.ForEach(this.AddBlock); // 全てのブロックを配置
            this.ReplacePatternWithObject(); // パターン認識されたブロックをオブジェクトに置き換える

            yield return new WaitUntil(() => fetchColorRulesTask.IsCompleted);
            this.ColorRules = fetchColorRulesTask.Result;
            this.ColorRules.ForEach(this.ApplyColorRules);
            if (GameManager.Mode == "PlayBack") InputManager.PlayBackModeUI.SetActive(true);
            LoadingWindow.SetActive(false);
        }

        void InitialPlacement(List<BlockInfo> blocksInfo)
        {
            blocksInfo.ForEach(b => AddBlock(b));
        }

        private void AddBlock(BlockInfo blockInfo)
        {
            if (blockInfo.pattern_name == "")
            {
                Object blockPrefab = (GameObject)Resources.Load("pblock1x1");
                Block block = (Instantiate(blockPrefab, blockInfo.GetPosition(), Quaternion.identity) as GameObject).GetComponent<Block>();
                block.SetColor(blockInfo.colorID);
                block.SetBlockData(blockInfo);
                if (GameManager.Mode == "Vr") block.SetActive(false);
                this.Blocks.Add(block);
                this.BlocksCount += 1;
            }
            else
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

                patternBlocks[blockInfo.pattern_name][blockInfo.pattern_group_id].Add(blockInfo); ;
            }
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
            while (BlockNumber < this.BlocksCount)
            {
                while (PlayBackButton.GetComponent<Toggle>().isOn == false)
                {
                    SeekBar.SetActive(false);
                    await Task.Delay(1);
                }
                SeekBar.SetActive(true);
                if (seekbarSlider.value == seekbarSlider.maxValue) break;
                FallingBlock((int)seekbarSlider.value);
                seekbarSlider.value++;
                await Task.Delay(1000);
            }
            PlayBackButton.GetComponent<Toggle>().isOn = false;
            ClearBlocks();
            seekbarSlider.value = 0;
            isRepeating = false;
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
            Material toColorMaterial = Resources.Load("Color" + to) as Material;
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

        private void ReplacePatternWithObject()
        {
            /*
             パターン認識されたブロックをオブジェクトに置き換える
             */
            List<string> patternNameKeys = new List<string>(patternBlocks.Keys);
            foreach (string patternName in patternNameKeys)
            {
                // string patternMaterialName = patternName + "Material";
                // Material patternMaterial = Resources.Load(patternMaterialName) as Material;

                List<string> patternGroupIdKeys = new List<string>(patternBlocks[patternName].Keys);
                foreach (string patternGroupId in patternGroupIdKeys)
                {
                    // 原点に近い順にsort
                    patternBlocks[patternName][patternGroupId].Sort(
                        (a, b) => (a.x * a.x + a.y * a.y + a.z * a.z) - (b.x * b.x + b.y * b.y + b.z * b.z)
                    );

                    BlockInfo nearestBlock = patternBlocks[patternName][patternGroupId][0];
                    BlockInfo farthestBlock = patternBlocks[patternName][patternGroupId][patternBlocks[patternName][patternGroupId].Count - 1];
                    float width = (Mathf.Abs(farthestBlock.x - nearestBlock.x) + 1) * X_RATIO;
                    float height = (Mathf.Abs(farthestBlock.y - nearestBlock.y) + 1) * Y_RATIO;
                    float depth = (Mathf.Abs(farthestBlock.z - nearestBlock.z) + 1) * Z_RATIO;

                    // パターンオブジェクトを生成
                    GameObject patternObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    patternObject.transform.position = new Vector3(
                        (nearestBlock.x + farthestBlock.x) * X_RATIO / 2,
                        (nearestBlock.y + farthestBlock.y) * Y_RATIO / 2,
                        (nearestBlock.z + farthestBlock.z) * Z_RATIO / 2
                    );
                    patternObject.transform.localScale = new Vector3(width, height, depth);
                    // patternObject.GetComponent<Renderer>().sharedMaterial = patternMaterial;

                    // 各パターンに応じた処理をする
                    switch (patternName)
                    {
                        case "road":
                            patternObject.AddComponent<RoadManager>();
                            break;
                    }
                }
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