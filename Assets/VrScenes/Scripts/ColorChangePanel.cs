using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorChangePanel : MonoBehaviour
{
    List<Material> contentMaterials = new List<Material>();
    [SerializeField] public GameObject panelPref;

    public void SetupColorChangePanel(GameObject targetObject)
    {
        //Material material = targetObject.GetComponent<Material>();
        FetchMaterial();
        SetColorPanel(targetObject);
    }

    public void FetchMaterial()
    {
        for (int i = 0; i < 10; i++)
        {
            string materialName = "Color" + i.ToString();
            Material material = Resources.Load(materialName, typeof(Material)) as Material;
            contentMaterials.Add(material);
        }
    }

    public void SetColorPanel(GameObject targetObject)
    { 
        int materialCount = contentMaterials.Count;
        Material currentMaterial = targetObject.GetComponent<Renderer>().material;

        //Content取得(ボタンを並べる場所)
        GameObject colorChangePanel = GameObject.Find("Canvas/ColorChangePanel");
        Transform currentColorPanel = colorChangePanel.transform.Find("CurrentColorPanel");
        currentColorPanel.Find("CurrentNameText").gameObject.GetComponent<Text>().text = currentMaterial.name;
        Material currentMaterial2D = new Material(Shader.Find("UI/Default"));
        currentMaterial2D.color = currentMaterial.color;
        currentColorPanel.Find("CurrentRawImage").gameObject.GetComponent<RawImage>().material = currentMaterial2D;
        RectTransform content = colorChangePanel.transform.Find("Scroll View/Viewport/Content").gameObject.GetComponent<RectTransform>();

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

            Text textMaterialNameLabel = toggleObject.transform.Find("MaterialNameLabel").gameObject.GetComponent<Text>();
            textMaterialNameLabel.text = contentMaterials[i].name;

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
