using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JsonFormats;
using VrScene;

namespace MergeScene
{
    public class MapParent : MonoBehaviour
    {
        private List<Block> Blocks = new List<Block> { };
        private List<Rule> ColorRules = new List<Rule> { };
        public string MapId;
        public Vector2 currentDiff = Vector2.zero;
        public int currentRotate = 0;

        public void AddBlock(List<BlockInfo> blocksInfo)
        {
            blocksInfo.ForEach(blockInfo =>
            {
                Object blockPrefab = (GameObject)Resources.Load("Block");
                Block block = (Instantiate(blockPrefab, blockInfo.GetPosition(), Quaternion.identity) as GameObject).GetComponent<Block>();
                block.SetColor(blockInfo.colorID, false);
                block.SetBlockData(blockInfo);
                this.Blocks.Add(block);
                block.gameObject.transform.parent = this.gameObject.transform;
            });
        }

        public void Move(int x, int y)
        {
            this.currentDiff.x += x;
            this.currentDiff.y += y;

            var vec = new Vector3(x*0.32f, 0, y*0.32f);
            var rotatedVec = Quaternion.Euler(0, this.currentRotate * 90, 0) * vec;
            this.gameObject.transform.Translate(rotatedVec);
        }

        public void Rotate(int r)
        {
            this.currentRotate -= r;
            this.currentRotate %= 4;
            this.transform.rotation = Quaternion.Euler(0.0f, -90f*this.currentRotate, 0.0f);
            Debug.Log(this.currentRotate);
        }

        public void ApplyColorRules(List<Rule> rulesData)
        {
            rulesData.ForEach(ruleData =>
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
            });
        }

    }
}

