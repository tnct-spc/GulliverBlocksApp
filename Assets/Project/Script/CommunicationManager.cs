using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using JsonFormats;

public class CommunicationManager
{
    public static string ServerAddress = "gulliverblocks.herokuapp.com";

    public async Task<Block[]> fetchMapBlocksAsync(string mapId)
    {
        var apiUrl = "https://" + ServerAddress + "/get_merged_blocks/" + mapId + "/";
        var jsonStr = await GetRequest(apiUrl);
        return JsonHelper.FromJson<Block>(jsonStr, "Blocks");
    }

    public async Task<World[]> fetchMapsAsync()
    {
        var apiUrl = "https://" + ServerAddress + "/get_maps";
        var jsonStr = await GetRequest(apiUrl);
        return JsonHelper.FromJson<World>(jsonStr, "Maps");
    }

    public async Task<World[]> fetchMergesAsync()
    {
        var apiUrl = "https://" + ServerAddress + "/get_merges";
        var jsonStr = await GetRequest(apiUrl);
        return JsonHelper.FromJson<World>(jsonStr, "Merges");
    }

    private static async Task<string> GetRequest(string url)
    {

        UnityWebRequest req = UnityWebRequest.Get(url);
        await req.SendWebRequest();
        if (req.isNetworkError || req.isHttpError)
        {
            Debug.Log(req.error);
            return "";
        }
        else
        {
            return req.downloadHandler.text;
        }
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
            else if (command == "Merges")
            {
                MergesWrapper<T> wrapper = JsonUtility.FromJson<MergesWrapper<T>>(json);
                return wrapper.merges;
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
        private class MergesWrapper<T>
        {
            public T[] merges;
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
}

namespace JsonFormats
{
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
            Vector3 position = new Vector3(0.32f*x, 0.384f*y, 0.32f*z);
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

    [System.Serializable]
    public struct World
    {
        public string ID;
        public string name;
    }
}
