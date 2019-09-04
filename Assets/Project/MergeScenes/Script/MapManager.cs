using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using JsonFormats;
using VrScene;

namespace MergeScene
{
    public class MapManager : MonoBehaviour
    {
        public int BlocksCount;
        public static string[] WorldList;
        List<float> NeutralPositions = new List<float>();
        private List<Block> Blocks = new List<Block> { };
        private List<Rule> ColorRules = new List<Rule> { };
        public float BlockNumber = 0;
        CommunicationManager CommunicationManager;
        public static string WorldID = "c24f8dd5-238d-45d8-8648-a797334bb8be";

        private void Awake()
        {
            CommunicationManager = new CommunicationManager();
        }
        private void Start()
        {
            StartCoroutine("FetchMap");
        }

        IEnumerator FetchMap()
        {
            for(int i = 0; i < WorldList.Length; i++)
            {
                var fetchBlocksTask = CommunicationManager.fetchMapBlocksAsync(WorldList[i]);
                var fetchColorRulesTask = CommunicationManager.fetchColorsAsync(WorldList[i]);
                yield return new WaitUntil(() => fetchBlocksTask.IsCompleted); // 通信中の場合次のフレームに処理を引き継ぐ
                fetchBlocksTask.Result.ForEach(this.AddBlock);// 全てのブロックを配置
                //yield return new WaitUntil(() => fetchColorRulesTask.IsCompleted);
                //this.ColorRules = fetchColorRulesTask.Result;
                //this.ColorRules.ForEach(this.ApplyColorRules);
            }
        }

        private void AddBlock(BlockInfo blockInfo)
        {
            Object blockPrefab = (GameObject)Resources.Load("pblock1x1");
            Block block = (Instantiate(blockPrefab, blockInfo.GetPosition(), Quaternion.identity) as GameObject).GetComponent<Block>();
            block.SetColor(blockInfo.colorID);
            block.SetBlockData(blockInfo);
            this.Blocks.Add(block);
            NeutralPositions.Add(Blocks[BlocksCount].transform.position.y);
            this.BlocksCount += 1;
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
        /*private void ApplyColorRules(Rule ruleData)
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
                List<Block> targetBlocks = this.Blocks.FindAll(block => block.colorID == ruleData.origin);
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
        }*/
    }
}
