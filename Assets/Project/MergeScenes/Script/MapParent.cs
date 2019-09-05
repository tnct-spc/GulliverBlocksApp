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
        public Vector2 currentDiff = Vector2.zero;

        public void AddBlock(List<BlockInfo> blocksInfo)
        {
            blocksInfo.ForEach(blockInfo =>
            {
                Object blockPrefab = (GameObject)Resources.Load("pblock1x1");
                Block block = (Instantiate(blockPrefab, blockInfo.GetPosition(), Quaternion.identity) as GameObject).GetComponent<Block>();
                block.SetColor(blockInfo.colorID);
                block.SetBlockData(blockInfo);
                this.Blocks.Add(block);
                block.gameObject.transform.parent = this.gameObject.transform;
            });
        }

        public void Move(int x, int y)
        {

            this.currentDiff.x += x;
            this.currentDiff.y += y;
            this.gameObject.transform.Translate(0.32f*x, 0.0f, 0.32f*y);
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
            });
        }

    }
}

