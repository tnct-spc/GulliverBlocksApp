﻿using System;
using System.Linq;
using UnityEngine;

#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace Assets.UrlSchemeReceiver
{
    public static class UrlSchemeReceiver
    {
        public static bool OpenFromUrlScheme { get; private set; }

        public static string OpenUrl { get; private set; }
        public static  string SpecificPart { get; private set; }
        

        static UrlSchemeReceiver()
        {
#if UNITY_EDITOR

#elif UNITY_ANDROID
            var clazz = new AndroidJavaClass("urlreceiver.IntentReceiveActivity");
            var specific = clazz.CallStatic<string>("GetSpecific");
            var scheme = clazz.CallStatic<string>("GetScheme");
            OpenFromUrlScheme = !string.IsNullOrEmpty(specific);
            OpenUrl = scheme + ":" + specific;
            SpecificPart = specific?.Split(new []{"//"},StringSplitOptions.None).LastOrDefault();
            Debug.Log("url is" + OpenUrl);
#elif UNITY_IOS
            var scheme = OnOpenURLListener_GetOpenURLString();
            OpenFromUrlScheme = scheme != null;
            OpenUrl = scheme;
            SpecificPart = scheme?.Split(new []{"//"},StringSplitOptions.None).LastOrDefault();
#else
            // nothing to do
#endif
        }

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    public static extern string OnOpenURLListener_GetOpenURLString();
#endif

    }
}
