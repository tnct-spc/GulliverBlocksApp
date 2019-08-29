using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using JsonFormats;
using WebSocketSharp;
using System;

public class CommunicationManager
{
    public static string ServerAddress = "gulliverblocks.herokuapp.com";

    public async Task<List<BlockInfo>> fetchMapBlocksAsync(string mapId)
    {
        var apiUrl = "https://" + ServerAddress + "/get_blocks/" + mapId + "/";
        var jsonStr = await GetRequest(apiUrl);
        return JsonHelper.FromJson<BlockInfo>(jsonStr, "Blocks");
    }

    public async Task<List<World>> fetchMapsAsync()
    {
        var apiUrl = "https://" + ServerAddress + "/get_maps/";
        var jsonStr = await GetRequest(apiUrl);
        return JsonHelper.FromJson<World>(jsonStr, "Maps");
    }

    public async Task<List<Rule>> fetchColorsAsync(string mapid)
    {
        var apiUrl = "https://" + ServerAddress + "/get_color_rules/" + mapid + "/";
        var jsonStr = await GetRequest(apiUrl);
        return JsonHelper.FromJson<Rule>(jsonStr, "Rules");

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

    public class WsClient
    {
        private WebSocket ws;
        public event EventHandler<AddBlockEventArgs> OnBlockReceived;

        public WsClient(string mapId)
        {
            this.ws = new WebSocket("wss://" + ServerAddress + "/receive/" + mapId + "/");
            this.ws.OnOpen += (sender, e) => Debug.Log("Websocket Open");
            this.ws.OnError += (sender, e) => Debug.Log("Websocket error" + e.Message);
            this.ws.OnClose += (sender, e) => Debug.Log("Websocket Close");
            this.ws.OnMessage += (sender, e) => this.onMessage(e);
        }

        public void StartConenction()
        {
            this.ws.Connect();
        }
        public void onMessage(MessageEventArgs e)
        {
            EventHandler<AddBlockEventArgs> handler = OnBlockReceived;
            AddBlockEventArgs dataE = new AddBlockEventArgs();
            dataE.Blocks = JsonHelper.FromJson<BlockInfo>(e.Data.Replace('\'', '"'), "Blocks"); //TO-DO server側で"を用いるようにする
            handler(this, dataE);
        }

        public class AddBlockEventArgs : EventArgs
        {
            public List<BlockInfo> Blocks { get; set; }
        }
    }

    public static class JsonHelper
    {
        public static List<T> FromJson<T>(string json, string command)
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
            else if (command == "ColorRules")
            {
                ColorWrapper<T> wrapper = JsonUtility.FromJson<ColorWrapper<T>>(json);
                return wrapper.colors;
            }
            else
            {
                return null;
            }
        }

        [System.Serializable]
        private class MapsWrapper<T>
        {
            public List<T> maps;
        }

        [System.Serializable]
        private class RulesWrapper<T>
        {
            public List<T> rules;
        }

        [System.Serializable]
        private class BlocksWrapper<T>
        {
            public List<T> blocks;
        }

        private class ColorWrapper<T>
        {
            public List<T> colors;
        }
    }
}

namespace JsonFormats
{
    [System.Serializable]
    public struct BlockInfo
    {
        public float x;
        public float y;
        public float z;
        public string ID;
        public float time;
        public string status;
        public string colorID;
        public string pattern_name;
        public string pattern_group_id;

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
        public string block_id;
        public string origin;
        public string to;
        public string map_id;
    }

    [System.Serializable]
    public struct World
    {
        public string ID;
        public string name;
    }

}
