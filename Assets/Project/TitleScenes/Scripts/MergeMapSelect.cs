using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JsonFormats;

namespace TitleScene
{
    public class MergeMapSelect : MonoBehaviour
    {
        CommunicationManager CommunicationManager;
        public List<World> WorldsData;
        private string fetchStatus = "start";
        public GameObject mergingMapPref;
        public string mergedMapID;

        private void OnEnable()
        {
            CommunicationManager = new CommunicationManager();
        }

        private void OnDisable()
        {
            mergedMapID = null;
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
                        this.WorldsData.AddRange(task.Result);
                        this.fetchStatus = "fetched";
                    });
                    return;
                case "fetched":
                    SetMergingMaps();
                    this.fetchStatus = "done";
                    return;
                default:
                    return;
            }
        }

        // ButtonをScrollViewに追加する関数
        public void SetMergingMaps()
        {
            RemoveMergedMap();

            int mergingMapCount = WorldsData.Count;

            //Content取得(ボタンを並べる場所)
            GameObject canvas = GameObject.Find("Canvas");
            RectTransform content = canvas.transform.Find("SelectMergingMapPanel/Scroll View/Viewport/Content").gameObject.GetComponent<RectTransform>();

            //Contentの高さ決定
            float mapSpace = content.GetComponent<VerticalLayoutGroup>().spacing;
            float mapHeight = mergingMapPref.GetComponent<LayoutElement>().preferredHeight;
            content.sizeDelta = new Vector2(0, (mapHeight + mapSpace) * mergingMapCount);

            for (int i = 0; i < mergingMapCount; i++)
            {
                int mapNum = i;

                //ボタン生成
                GameObject btn = (GameObject)Instantiate(mergingMapPref);

                //ボタンをContentの子に設定
                btn.transform.SetParent(content, false);

                btn.transform.Find("Label").gameObject.GetComponent<Text>().text = WorldsData[i].name;
            }
        }

        private void RemoveMergedMap()
        {
            for(int i = 0; i < WorldsData.Count; i++)
            {
                if(WorldsData[i].ID == mergedMapID)
                {
                    WorldsData.RemoveAt(i);
                    break;
                }
            }
        }

    }
}
