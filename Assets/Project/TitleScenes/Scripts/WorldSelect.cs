using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JsonFormats;
using System.Linq;

namespace TitleScene
{

    public class WorldSelect : MonoBehaviour
    {
        [SerializeField] GameSystem gameSystem;
        [SerializeField] private GameObject btnPref;  //ボタンプレハブ
        public List<(World world, bool isMerge)> WorldsData = new List<(World world, bool isMerge)>();
        CommunicationManager CommunicationManager;


        private void Awake()
        {
            CommunicationManager = new CommunicationManager();
            StartCoroutine("FetchData");
        }

        private void Update()
        {
        }

        IEnumerator FetchData()
        {
            var fetchMapsTask = CommunicationManager.fetchMapsAsync();
            var fetchMergesTask = CommunicationManager.fetchMergesAsync();
            yield return new WaitUntil(() => fetchMapsTask.IsCompleted);
            yield return new WaitUntil(() => fetchMergesTask.IsCompleted);
            fetchMapsTask.Result.ForEach(d => this.WorldsData.Add((d, false)));
            fetchMergesTask.Result.ForEach(d => this.WorldsData.Add((d, true)));
            setWorldSelectButton();
        }

        // ButtonをScrollViewに追加する関数
        public void setWorldSelectButton()
        {
            int btnCount = WorldsData.Count;

            //Content取得(ボタンを並べる場所)
            GameObject canvas = GameObject.Find("Canvas");
            RectTransform content = canvas.transform.Find("SelectPanel/Scroll View/Viewport/Content").gameObject.GetComponent<RectTransform>();

            //Contentの高さ決定
            float btnSpace = content.GetComponent<VerticalLayoutGroup>().spacing;      // WorldSelectButton間の高さを取得
            float btnHeight = btnPref.GetComponent<LayoutElement>().preferredHeight;   // WorldSelectButton自体の高さを取得
            content.sizeDelta = new Vector2(0, (btnHeight + btnSpace) * btnCount); // 上２つの要素からcontentの高さを作成

            for (int i = 0; i < btnCount; i++)
            {
                int btnNum = i;

                //ボタン生成
                GameObject btn = (GameObject)Instantiate(btnPref);

                //ボタンをContentの子に設定
                btn.transform.SetParent(content, false);

                //ボタンのテキスト変更
                btn.transform.GetComponentInChildren<Text>().text = WorldsData[btnNum].world.name;

                //ボタンのクリックイベント登録
                btn.transform.GetComponent<Button>().onClick.AddListener(() => gameSystem.OnClickWorldSelectButton(WorldsData[btnNum].world.ID, WorldsData[btnNum].isMerge));

            }
        }
    }
}