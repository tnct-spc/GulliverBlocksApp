using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorChangePanel : MonoBehaviour
{
    List<Material> contentMaterials = new List<Material>();
    [SerializeField] public GameObject panelPref;

    void OnEnable()
    {
        Debug.Log("1");
        for (int i = 0; i < 10; i++)
        {
            string materialName = "Color" + i.ToString();
            Material material = Resources.Load(materialName, typeof(Material)) as Material;
            contentMaterials.Add(material);
            Debug.Log(contentMaterials[i].name);
        }
        Debug.Log("2");
        Debug.Log("count: "+ contentMaterials.Count);
        SetColorPanel();
    }

    public void SetColorPanel()
    {
        Debug.Log("3");
        int materialCount = contentMaterials.Count;

        //Content取得(ボタンを並べる場所)
        GameObject canvas = GameObject.Find("Canvas");
        RectTransform content = canvas.transform.Find("ColorChangePanel/Scroll View/Viewport/Content").gameObject.GetComponent<RectTransform>();

        //Contentの高さ決定
        float panelSpace = content.GetComponent<VerticalLayoutGroup>().spacing;      // WorldSelectButton間の高さを取得
        float panelHeight = panelPref.GetComponent<LayoutElement>().preferredHeight;   // WorldSelectButton自体の高さを取得
        content.sizeDelta = new Vector2(0, (panelHeight + panelSpace) * materialCount); // 上２つの要素からcontentの高さを作成

        Debug.Log("4");
        bool isFirst = true;
        for (int i = 0; i < materialCount; i++)
        {
            int panelNum = i;

            //ボタン生成
            GameObject panel = (GameObject)Instantiate(panelPref);

            //ボタンをContentの子に設定
            panel.transform.SetParent(content, false);

            Toggle toggle = panel.transform.Find("ColorChangeToggle").GetComponent<Toggle>();
            toggle.isOn = false;

            if (isFirst)
            {
                toggle.isOn = true;
                isFirst = false;
            }
        }
        Debug.Log("5");
    }
}
