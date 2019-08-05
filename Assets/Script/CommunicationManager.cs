using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicationManager
{
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
}
