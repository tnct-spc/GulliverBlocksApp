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
        GameObject canvas;
        RectTransform content;

        private void Awake()
        {
            //Content取得(ボタンを並べる場所)
            canvas = GameObject.Find("Canvas");
            content = canvas.transform.Find("MergeMapSelectPanel/Scroll View/Viewport/Content").gameObject.GetComponent<RectTransform>();
        }

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

            //Contentの高さ決定
            float mapSpace = content.GetComponent<VerticalLayoutGroup>().spacing;
            float mapHeight = mergingMapPref.GetComponent<LayoutElement>().preferredHeight;
            content.sizeDelta = new Vector2(0, (mapHeight + mapSpace) * mergeMapCount);

            for (int i = 0; i < mergeMapCount; i++)
            {
                int mapNum = i;

                //ボタン生成
                GameObject mapToggle = (GameObject)Instantiate(mergingMapPref);
                mapToggle.transform.SetParent(content, false);

                mapToggle.transform.Find("Label").gameObject.GetComponent<Text>().text = WorldsData[i].name;

                mapToggle.GetComponent<Toggle>().isOn = false;
            }
        }

        public string[] CheckToggles()
        {
            List<string> toggleList = new List<string>();
            Transform children = content.GetComponentInChildren<Transform>();
            foreach(Transform child in children)
            {
                if (child.GetComponent<Toggle>().isOn == true)
                {
                    toggleList.Add(child.Find("Label").GetComponent<Text>().text);
                }
            }
            
            return WorldNameToID(toggleList).ToArray();
        }

        private List<string> WorldNameToID(List<string> nameList)
        {
            List<string> idList = new List<string>();
            for(int i = 0; i < WorldsData.Count; i++)
            {
                for(int j = 0; j < nameList.Count; j++)
                {
                    if (WorldsData[i].name == nameList[j]) idList.Add(WorldsData[i].ID);
                }
            }

            return idList;
        }
    }
}
