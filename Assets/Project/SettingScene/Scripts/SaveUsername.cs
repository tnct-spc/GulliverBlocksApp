using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TitleScene
{
    public class SaveUsername : MonoBehaviour
    {
        public InputField UserNameInputField;
        public InputField ServerAddressInputField;
        public Text UserNameText;
        public Text ServerAddressText;

        private const string UserNameKey = "USER_NAME";
        private const string ServerAddressKey = "SERVER_ADDRESS";

        void Start()
        {
            UserNameText.text = "ユーザー名：" + LoadData(UserNameKey);
            ServerAddressText.text = "サーバーアドレス：" + CommunicationManager.ServerAddress;
            UserNameInputField.text = LoadData(UserNameKey);
            ServerAddressInputField.text = CommunicationManager.ServerAddress;
            PlayerPrefs.Save();
        }

        public void SaveUseName()
        {
            PlayerPrefs.SetString(UserNameKey, UserNameInputField.text);
            PlayerPrefs.Save();
            UserNameText.text = "ユーザー名：" + LoadData(UserNameKey);
        }

        public void SaveServerAddress()
        {
            PlayerPrefs.SetString(ServerAddressKey, ServerAddressInputField.text);
            PlayerPrefs.Save();
            ServerAddressText.text = "サーバーアドレス：" + LoadData(ServerAddressKey);
        }

        public string LoadData(string key)
        {
            return PlayerPrefs.GetString(key, "");
        }
    }
}
