using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JsonFormats;

namespace TitleScene
{

    public class WorldSelect : MonoBehaviour
    {
        GameSystem gameSystem;
        [SerializeField] private GameObject btnPref;  //ボタンプレハブ
        public World[] WorldsData;
        CommunicationManager CommunicationManager;
        private string fetchStatus = "start";


        private void Awake()
        {
            CommunicationManager = new CommunicationManager();
        }

        private void Update()
        {
            checkFetchStatus();
        }

        private async void checkFetchStatus()
        {
            switch (this.fetchStatus)
            {
                case "start":
                    this.fetchStatus = "fetching";
                    await CommunicationManager.fetchMapsAsync().ContinueWith(task =>
                    {
                        this.WorldsData = task.Result;
                        this.fetchStatus = "fetched";
                    });
                    return;
                case "fetched":
                    SetWorldSelectButton();
                    this.fetchStatus = "done";
                    return;
            default:
                    return;
            }
        }

        // ButtonをScrollViewに追加する関数
        public void SetWorldSelectButton()
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
}