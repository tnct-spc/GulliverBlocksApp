using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace VrScene
{
    public class ColorChangePanel : MonoBehaviour
    {
        [SerializeField] public GameObject panelPref;
        private List<Material> contentMaterials = new List<Material>();
        private GameObject contentObject;
        private GameObject colorChangePanel;
        private ToggleGroup toggleGroup;
        private GameObject targetBlock;
        public GameObject lightUpObject = null;


        private void OnDisable()
        {
            foreach (Transform childTransform in contentObject.transform)
            {
                Destroy(childTransform.gameObject);
            }
        }

        public void SetupColorChangePanel(GameObject targetObject)
        {
            targetBlock = targetObject;
            colorChangePanel = GameObject.Find("Canvas/ColorChangePanel");
            contentObject = GameObject.Find("Canvas/ColorChangePanel/Scroll View/Viewport/Content");
            SetColorPanel(targetBlock);
        }

        public Material CopyTo2DMaterial(Material original)
        {
            Material material2D = new Material(Shader.Find("UI/Default"));
            material2D.color = original.color;
            return material2D;
        }

        public void SetColorPanel(GameObject targetObject)
        {
            int materialCount = contentMaterials.Count;
            Material currentMaterial = targetObject.GetComponent<Renderer>().material;

            //currentColorPanelの設定
            Transform currentColorPanel = colorChangePanel.transform.Find("CurrentColorPanel");
            currentColorPanel.Find("CurrentNameText").gameObject.GetComponent<Text>().text = currentMaterial.name;
            currentColorPanel.Find("CurrentRawImage").gameObject.GetComponent<RawImage>().material = CopyTo2DMaterial(currentMaterial);

            //Contentの高さ決定
            RectTransform content = contentObject.GetComponent<RectTransform>();
            float panelSpace = content.GetComponent<VerticalLayoutGroup>().spacing;
            float panelHeight = panelPref.GetComponent<LayoutElement>().preferredHeight;
            content.sizeDelta = new Vector2(0, (panelHeight + panelSpace) * materialCount);

            bool isFirst = true;
            for (int i = 0; i < materialCount; i++)
            {
                int panelNum = i;

                //ContentObjectを生成し、contentの子要素にする
                GameObject panel = (GameObject)Instantiate(panelPref);
                panel.transform.SetParent(content, false);

                //Toggleの設定
                GameObject toggleObject = panel.transform.Find("ColorChangeToggle").gameObject;
                Toggle toggle = toggleObject.GetComponent<Toggle>();
                toggle.isOn = false;

                //Materialの名前を設定
                Text textMaterialNameLabel = toggleObject.transform.Find("MaterialNameLabel").gameObject.GetComponent<Text>();
                textMaterialNameLabel.text = contentMaterials[i].name;

                //RawImageの設定
                RawImage rawImage = toggleObject.transform.Find("MaterialRawImage").gameObject.GetComponent<RawImage>();
                rawImage.material = CopyTo2DMaterial(contentMaterials[i]);

                //ToggleGroupの設定
                if (isFirst)
                {
                    toggle.isOn = true;
                    toggleGroup = toggleObject.AddComponent<ToggleGroup>();
                    isFirst = false;
                }

                toggle.group = toggleGroup;
            }
        }

        public void OnClickChangeButton()
        {
            Toggle checkToggle = toggleGroup.ActiveToggles().FirstOrDefault();
            string toMaterialName = checkToggle.transform.Find("MaterialNameLabel").gameObject.GetComponent<Text>().text;
            targetBlock.GetComponent<Renderer>().material = Resources.Load(toMaterialName) as Material;
        }

        public void OnClickCancelButton()
        {
            colorChangePanel.SetActive(false);
        }
    }
}
