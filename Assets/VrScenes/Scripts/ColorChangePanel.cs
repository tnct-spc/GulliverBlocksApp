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
        for (int i = 0; i < 10; i++)
        {
            string materialName = "Color" + i.ToString();
            Material material = Resources.Load(materialName, typeof(Material)) as Material;
            contentMaterials.Add(material);
        }
        SetColorPanel();
    }

    public void SetColorPanel()
    { 
        int materialCount = contentMaterials.Count;

        //Content取得(ボタンを並べる場所)
        GameObject canvas = GameObject.Find("Canvas");
        RectTransform content = canvas.transform.Find("ColorChangePanel/Scroll View/Viewport/Content").gameObject.GetComponent<RectTransform>();

        //Contentの高さ決定
        float panelSpace = content.GetComponent<VerticalLayoutGroup>().spacing;      // WorldSelectButton間の高さを取得
        float panelHeight = panelPref.GetComponent<LayoutElement>().preferredHeight;   // WorldSelectButton自体の高さを取得
        content.sizeDelta = new Vector2(0, (panelHeight + panelSpace) * materialCount); // 上２つの要素からcontentの高さを作成

        bool isFirst = true;
        ToggleGroup toggleGroup = null;
        for (int i = 0; i < materialCount; i++)
        {
            int panelNum = i;
            Material material = new Material(Shader.Find("UI/Default"));
            material.color = contentMaterials[i].color;
            GameObject panel = (GameObject)Instantiate(panelPref);

            panel.transform.SetParent(content, false);

            GameObject toggleObject = panel.transform.Find("ColorChangeToggle").gameObject;

            Toggle toggle =  toggleObject.GetComponent<Toggle>();
            toggle.isOn = false;

            Text textNameLabel = toggleObject.transform.Find("NameLabel").gameObject.GetComponent<Text>();
            textNameLabel.text = "Name : " + contentMaterials[i].name;

            RawImage rawImage = toggleObject.transform.Find("MaterialRawImage").gameObject.GetComponent<RawImage>();
            rawImage.material = material;

            if (isFirst)
            {
                toggle.isOn = true;
                toggleGroup = toggleObject.AddComponent<ToggleGroup>();
                isFirst = false;
            }

            toggle.group = toggleGroup;
        }
    }
}
