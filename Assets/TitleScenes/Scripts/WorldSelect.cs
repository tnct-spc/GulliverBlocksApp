using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class WorldSelect : MonoBehaviour
{
    GameSystem gameSystem;
    [SerializeField] private GameObject btnPref;  //ボタンプレハブ
    const string SERVER_URL = "http://gulliverblocks.herokuapp.com/get_maps";
    public World[] WorldsData;


    private void Start()
    {
        StartCoroutine("GetWorlds", SERVER_URL);
    }

    public IEnumerator GetWorlds(string url)
    {
        //URLをGETで用意
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        //URLに接続して結果が戻ってくるまで待機
        yield return webRequest.SendWebRequest();

        //エラーが出ていないかチェック
        if (webRequest.isNetworkError)
        {
            //通信失敗
            Debug.Log(webRequest.error);
        }
        else
        {
            //通信成功
            WorldsData = JsonHelper.FromJson<World>(webRequest.downloadHandler.text);

            yield return null;

            setWorldSelectButton();
        }
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.maps;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] maps;
        }
    }


    // ButtonをScrollViewに追加する関数
    public void setWorldSelectButton()
    {
        int btnCount = WorldsData.Length;

        //Content取得(ボタンを並べる場所)
        GameObject canvas = GameObject.Find("Canvas");
        RectTransform content = canvas.transform.Find("SelectPanel/Scroll View/Viewport/Content").gameObject.GetComponent<RectTransform>();

        //Contentの高さ決定
        float btnSpace = content.GetComponent<VerticalLayoutGroup>().spacing;      // WorldSelectButton間の高さを取得
        float btnHeight = btnPref.GetComponent<LayoutElement>().preferredHeight;   // WorldSelectButton自体の高さを取得
        content.sizeDelta = new Vector2(0, (btnHeight + btnSpace) * btnCount); // 上２つの要素からcontentの高さを作成

        gameSystem = gameObject.GetComponent<GameSystem>();

        for (int i = 0; i < btnCount; i++)
        {
            int btnNum = i;

            //ボタン生成
            GameObject btn = (GameObject)Instantiate(btnPref);

            //ボタンをContentの子に設定
            btn.transform.SetParent(content, false);

            //ボタンのテキスト変更
            btn.transform.GetComponentInChildren<Text>().text = WorldsData[btnNum].name;

            //ボタンのクリックイベント登録
            btn.transform.GetComponent<Button>().onClick.AddListener(() => gameSystem.OnClickWorldSelectButton(WorldsData[btnNum].ID));

        }
    }
}