using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;

[System.Serializable]
public class World
{
    public string ID;
    public string name;
}

public class WorldSelect : MonoBehaviour
{

    [SerializeField] private GameObject btnPref;  //ボタンプレハブ
    const int BUTTON_COUNT = 10;
    const string SERVER_URL = "http://gulliverblocks.herokuapp.com/get_maps";
    public World[] WorldsData;


    private void Start()
    {
        setWorldSelectButton();
        StartCoroutine("GetWorlds", SERVER_URL);
    }

    IEnumerator GetWorlds(string url)
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
            Debug.Log(webRequest.downloadHandler.text);
            WorldsData = JsonHelper.FromJson<World>(webRequest.downloadHandler.text);

            for(int i = 0; i < WorldsData.Length; i++)
            {
                Debug.Log(WorldsData[i].ID);
                Debug.Log(WorldsData[i].name);
            }
        }
    }

    private void setWorldSelectButton()
    {
        //Content取得(ボタンを並べる場所)
        GameObject canvas = GameObject.Find("Canvas");
        RectTransform content = canvas.transform.Find("SelectPanel/Scroll View/Viewport/Content").gameObject.GetComponent<RectTransform>();

        //Contentの高さ決定
        float btnSpace = content.GetComponent<VerticalLayoutGroup>().spacing;      // WorldSelectButton間の高さを取得
        float btnHeight = btnPref.GetComponent<LayoutElement>().preferredHeight;   // WorldSelectButton自体の高さを取得
        content.sizeDelta = new Vector2(0, (btnHeight + btnSpace) * BUTTON_COUNT); // 上２つの要素からcontentの高さを作成


        for (int i = 0; i < BUTTON_COUNT; i++)
        {
            int btnNum = i;

            //ボタン生成
            GameObject btn = (GameObject)Instantiate(btnPref);

            //ボタンをContentの子に設定
            btn.transform.SetParent(content, false);

            //ボタンのテキスト変更
            btn.transform.GetComponentInChildren<Text>().text = "World_" + btnNum.ToString();

            //ボタンのクリックイベント登録
            btn.transform.GetComponent<Button>().onClick.AddListener(() => OnClickWorldSelectButton(btnNum));

        }
    }
 
    public void OnClickWorldSelectButton(int btnNum)
    {
        Debug.Log(btnNum);
    }
}