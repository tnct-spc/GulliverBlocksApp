using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct World
{
    public string ID;
    public string name;
}

[System.Serializable]
public struct Block
{
    public float x;
    public float y;
    public float z;
    public string ID;
    public float time;
    public bool put;
    public string colorID;

    public Vector3 GetPosition()
    {
        Vector3 position = new Vector3(x, y, z);
        return position;
    }
}

[System.Serializable]
public struct Rule
{
    public string type;
    public string target;
    public string to;
}

public class CommunicationManager
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
