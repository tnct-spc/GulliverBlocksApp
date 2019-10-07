using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace TitleScene
{
    public class UserSetting : MonoBehaviour
    {
        public InputField UserNameInputField;
        public InputField ServerAddressInputField;
        public InputField PasswordInputField;
        public Text UserNameText;
        public Text ServerAddressText;
        public Text PasswordText;

        private const string UserNameKey = "USER_NAME";
        private const string PasswordKey = "PASSWORD";
        private const string ServerAddressKey = "SERVER_ADDRESS";

        private CommunicationManager CommunicationManager;

        void Start()
        {
            CommunicationManager = new CommunicationManager();

            UserNameText.text = "ユーザー名：" + LoadData(UserNameKey);
            PasswordText.text = "パスワード";
            ServerAddressText.text = "サーバーアドレス：" + CommunicationManager.ServerAddress;
            UserNameInputField.text = LoadData(UserNameKey);
            PasswordInputField.text = LoadData(PasswordKey);
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

        public void SavePassword()
        {
            PlayerPrefs.SetString(PasswordKey, PasswordInputField.text);
            PlayerPrefs.Save();
        }

        public string LoadData(string key)
        {
            return PlayerPrefs.GetString(key, "");
        }

        public void Login()
        {
            StartCoroutine("LoginAPI");
        }

        public void Logout()
        {
            StartCoroutine("LogoutAPI");
        }

        IEnumerator LoginAPI()
        {
            string userName = PlayerPrefs.GetString(UserNameKey, "");
            string password = PlayerPrefs.GetString(PasswordKey, "");
            Task<string> loginTask = CommunicationManager.loginAsync(userName, password);
            yield return new WaitUntil(() => loginTask.IsCompleted);
            if (loginTask.Result == "")
            {
                Debug.Log("faild to login");
            }
            else
            {
                Debug.Log("success to login");
            }
        }

        IEnumerator LogoutAPI()
        {
            Task<string> logoutTask = CommunicationManager.logoutAsync();
            yield return new WaitUntil(() => logoutTask.IsCompleted);
            if (logoutTask.Result == "")
            {
                Debug.Log("faild to logout");
            }
            else
            {
                Debug.Log("success to logout");
            }
        }
    }
}
