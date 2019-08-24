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
        private List<Block> Blocks = new List<Block> { };
        public static string WorldID;
        GameObject GameSystem;
        public float BlockNumber = 0;
        public bool isRepeating = false;
        InputManager InputManager;
        Slider SeekBar;
        Toggle PlayButton;
        GameManager GameManager;
        public GameObject LoadingWindow;
        CommunicationManager CommunicationManager;

        private void Awake()
        {
            CommunicationManager = new CommunicationManager();
        }

        private void Start()
        {
            LoadingWindow.SetActive(true);
            GameSystem = GameObject.Find("GameSystem");
            InputManager = GameSystem.GetComponent<InputManager>();
            SeekBar = InputManager.SeekBar;
            PlayButton = InputManager.PlayButton;
            GameManager = GameSystem.GetComponent<GameManager>();
            StartCoroutine("FetchData");
        }

        IEnumerator FetchData()
        {
            var task = CommunicationManager.fetchMapBlocksAsync(WorldID);
            yield return new WaitUntil(() => task.IsCompleted); // 通信中の場合次のフレームに処理を引き継ぐ
            task.Result.ForEach(this.AddBlock);// 全てのブロックを配置
            ApplyColorRules();
            if (GameManager.Mode == "Vr") InputManager.PlayModeUI.SetActive(true);
            LoadingWindow.SetActive(false);
        }

        void InitialPlacement(List<BlockInfo> blocksInfo)
        {
            blocksInfo.ForEach(b => AddBlock(b));
        }

        private void AddBlock(BlockInfo blockInfo)
        {
            Object blockPrefab = (GameObject)Resources.Load("pblock1x1");
            Block block = (Instantiate(blockPrefab, blockInfo.GetPosition(), Quaternion.identity) as GameObject).GetComponent<Block>();
            block.SetColor(blockInfo.colorID);
            block.SetBlockData(blockInfo);
            if (GameManager.Mode == "Vr") block.SetActive(false);
            this.Blocks.Add(block);
            this.BlocksCount += 1;
        }

        public async void RepeatPlaceBlocks()
        {
            isRepeating = true;
            while (BlockNumber < this.BlocksCount)
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

        private void ApplyColorRules()
        {
            string rulesJson = "{ \"rules\": [{ \"type\": \"color\", \"target\": 1, \"to\": 3},{ \"type\": \"ID\", \"target\": \"8831ab9d-31b6-449b-8077-d523020de32c\", \"to\": 1 }] }";
            List<Rule> ruleData = CommunicationManager.JsonHelper.FromJson<Rule>(rulesJson, "Rules");
            for (int i = 0; i < ruleData.Count; i++)
            {
                string type = ruleData[i].type;
                string target = ruleData[i].target;
                string to = ruleData[i].to;
                Material toColorMaterial = Resources.Load("Color" + to) as Material;
                if (toColorMaterial == null)
                {
                    Debug.Log("To is Invalid.");
                    break;
                }
                if (type == "color")
                {
                    string targetColorName = "Color" + target;
                    List<Block> targetBlocks = this.Blocks.FindAll(block => block.colorID == target);
                    targetBlocks.ForEach(block => {
                        block.SetColor(to);
                    });

                }
                else if (type == "ID")
                {
                    string targetID = target;
                    Block targetBlock = this.Blocks.Find(block => block.ID == targetID);
                    if (targetBlock == null) break;
                    targetBlock.SetColor(to);
                }
                else
                {
                    Debug.Log("Type is Invalid.");
                }
            }
        }
    }
}