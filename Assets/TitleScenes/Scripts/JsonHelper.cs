using System;
using UnityEngine;
using System.Collections;

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.maps;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] maps;
    }
}
