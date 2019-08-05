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

public static class JsonHelper
{
    public static T[] FromJson<T>(string json, string command)
    {
        if (command == "Maps")
        {
            MapsWrapper<T> wrapper = JsonUtility.FromJson<MapsWrapper<T>>(json);
            return wrapper.maps;
        }
        else if (command == "Rules")
        {
            RulesWrapper<T> wrapper = JsonUtility.FromJson<RulesWrapper<T>>(json);
            return wrapper.rules;
        }
        else if (command == "Blocks")
        {
            BlocksWrapper<T> wrapper = JsonUtility.FromJson<BlocksWrapper<T>>(json);
            return wrapper.blocks;
        }
        else
        {
            return null;
        }
    }

    [System.Serializable]
    private class MapsWrapper<T>
    {
        public T[] maps;
    }

    [System.Serializable]
    private class RulesWrapper<T>
    {
        public T[] rules;
    }

    [System.Serializable]
    private class BlocksWrapper<T>
    {
        public T[] blocks;
    }
}

public class CommunicationManager
{
    

}
