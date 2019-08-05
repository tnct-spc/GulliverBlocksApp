using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncludingBlockInfo : MonoBehaviour
{
    public float x;
    public float y;
    public float z;
    public string ID;
    public float time;
    public bool put;
    public string colorID;

    public void SetBlockData(Block block)
    {
        this.x = block.x;
        this.y = block.y;
        this.z = block.z;
        this.ID = block.ID;
        this.time = block.time;
        this.put = block.put;
        this.colorID = block.colorID;
    }

    public Vector3 GetPosition()
    {
        Vector3 position = new Vector3(x, y, z);
        return position;
    }
}
