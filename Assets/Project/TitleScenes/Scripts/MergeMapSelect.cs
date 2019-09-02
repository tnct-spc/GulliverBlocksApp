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
        public GameObject mergingMapPref;
        private ToggleGroup toggleGroup;

        private void OnEnable()
        { 
            CommunicationManager = new CommunicationManager();
            SetMergeMaps();
        }

        private void OnDisable()
        {
            WorldsData.Clear();
        }

        // ButtonをScrollViewに追加する関数
        public void SetMergeMaps()
        {
            int mergeMapCount = WorldsData.Count;

            //Content取得(ボタンを並べる場所)
            GameObject canvas = GameObject.Find("Canvas");
            RectTransform content = canvas.transform.Find("MergeMapSelectPanel/Scroll View/Viewport/Content").gameObject.GetComponent<RectTransform>();

            //Contentの高さ決定
            float mapSpace = content.GetComponent<VerticalLayoutGroup>().spacing;
            float mapHeight = mergingMapPref.GetComponent<LayoutElement>().preferredHeight;
            content.sizeDelta = new Vector2(0, (mapHeight + mapSpace) * mergeMapCount);

            bool isFirst = true;
            for (int i = 0; i < mergeMapCount; i++)
            {
                int mapNum = i;

                //ボタン生成
                GameObject mapToggle = (GameObject)Instantiate(mergingMapPref);
                mapToggle.transform.SetParent(content, false);

                mapToggle.transform.Find("Label").gameObject.GetComponent<Text>().text = WorldsData[i].name;

                Toggle toggle = mapToggle.GetComponent<Toggle>();
                toggle.isOn = false;

                //ToggleGroupの設定
                if (isFirst)
                {
                    toggle.isOn = true;
                    toggleGroup = mapToggle.AddComponent<ToggleGroup>();
                    isFirst = false;
                }

                toggle.group = toggleGroup;
            }
        }
    }
}
