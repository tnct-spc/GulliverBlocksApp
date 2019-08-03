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

    public void SetBlockData(float x, float y, float z, string ID, float time, bool put, string colorID)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.ID = ID;
        this.time = time;
        this.put = put;
        this.colorID = colorID;
    }

    public Vector3 GetPosition()
    {
        Vector3 position = new Vector3(x, y, z);
        return position;
    }
}
