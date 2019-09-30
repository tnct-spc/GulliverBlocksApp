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
        public GameObject ModeSelectPanel;
        public Transform Content;
        public List<(World world, bool isMerge)> WorldsData = new List<(World world, bool isMerge)>();
        CommunicationManager CommunicationManager;


        private void Awake()
        {
            CommunicationManager = new CommunicationManager();
            StartCoroutine("FetchData");
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
            foreach(Transform childTransform in Content)
            {
                if (childTransform.name != "CreateMapButton")
                    Destroy(childTransform.gameObject);
            }

            int btnCount = WorldsData.Count;

            //Contentの高さ決定
            float btnSpace = Content.GetComponent<VerticalLayoutGroup>().spacing;      // WorldSelectButton間の高さを取得
            float btnHeight = btnPref.GetComponent<LayoutElement>().preferredHeight;   // WorldSelectButton自体の高さを取得
            Content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (btnHeight + btnSpace) * btnCount); // 上２つの要素からcontentの高さを作成

            for (int i = 0; i < btnCount; i++)
            {
                int btnNum = i;

                //ボタン生成
                GameObject panel = (GameObject)Instantiate(btnPref);

                //ボタンをContentの子に設定
                panel.transform.SetParent(Content, false);

                Transform selectBtn = panel.transform.Find("selectButton");
                Transform editBtn = panel.transform.Find("EditButton");
                Transform deleteBtn = panel.transform.Find("DeleteButton");

                //ボタンのテキスト変更
                selectBtn.GetComponentInChildren<Text>().text = WorldsData[btnNum].world.name;

                //ボタンのクリックイベント登録
                selectBtn.GetComponent<Button>().onClick.AddListener(() => gameSystem.OnClickWorldSelectButton(WorldsData[btnNum].world.ID, WorldsData[btnNum].isMerge));
                editBtn.GetComponent<Button>().onClick.AddListener(() => gameSystem.OnClickChangeMapButton(WorldsData[btnNum].world.name, "EditMapName"));
                deleteBtn.GetComponent<Button>().onClick.AddListener(() => gameSystem.OnClickChangeMapButton(WorldsData[btnNum].world.name, "Delete"));

            }
        }
    }
}