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
        public static string WorldID = "114e3ba9-a403-4c5c-a018-c7219c5bcc90";
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
            var _task = CommunicationManager.fetchColorsAsync(WorldID);
            yield return new WaitUntil(() => task.IsCompleted); // 通信中の場合次のフレームに処理を引き継ぐ
            task.Result.ForEach(this.AddBlock);// 全てのブロックを配置
            yield return new WaitUntil(() => _task.IsCompleted);
            print(_task.Result);
            _task.Result.ForEach(this.ApplyColorRules);
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
            NeutralPositions.Add(Blocks[BlocksCount].transform.position.y);
            this.BlocksCount += 1;
        }

        public async void RepeatPlaceBlocks()
        {
            isRepeating = true;
            while (BlockNumber < this.BlocksCount)
            {
                while (PlayButton.GetComponent<Toggle>().isOn == false) await Task.Delay(1);
                FallingBlock((int)SeekBar.value);
                SeekBar.value++;
                await Task.Delay(1000);
            }
            PlayButton.GetComponent<Toggle>().isOn = false;
            isRepeating = false;
        }

        async void FallingBlock(int i)
        {
            float Accel = 0f;
            for (float j = 20; j > NeutralPositions[i]; j -= Accel)
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
        private void ApplyColorRules(Rule ruleData)
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
        }

    }
}