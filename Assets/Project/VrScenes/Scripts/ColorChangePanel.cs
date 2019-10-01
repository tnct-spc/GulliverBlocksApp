﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace VrScene
{
    public class ColorChangePanel : MonoBehaviour
    {
        BlockManager blockManager;
        [SerializeField] public GameObject panelPref;
        private List<Material> contentMaterials = new List<Material>();
        public GameObject contentObject;
        public GameObject colorChangePanel;
        private ToggleGroup toggleGroup;
        private GameObject targetBlock;
        public GameObject lightUpObject = null;

        private void Awake()
        {
            blockManager = GameObject.Find("GameSystem").GetComponent<BlockManager>();
        }

        public void OnEnable()
        {
            contentMaterials.Clear();
            Object[] loadedMaterials = Resources.LoadAll("Materials", typeof(Material));
            foreach(Material material in loadedMaterials)
            {
                contentMaterials.Add(material);
            }
        }

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
            SetColorPanel(targetBlock);
        }

        public Material CopyTo2DMaterial(Material original)
        {
            Material material2D = new Material(Shader.Find("UI/Default"));
            if(original.shader == Shader.Find("Standard"))
            {
                // 普通の色のマテリアル
                material2D.color = original.color;
            }
            else
            {
                // 画像テクスチャのマテリアル
                material2D.mainTexture = original.mainTexture;
            }
            return material2D;
        }

        public void SetColorPanel(GameObject targetObject)
        {
            int materialCount = contentMaterials.Count;
            Material currentMaterial = targetObject.GetComponent<Renderer>().material;

            //currentColorPanelの設定
            Transform currentColorPanel = colorChangePanel.transform.Find("CurrentColorPanel");
            string currentMaterialName = ChengeMaterialNameToColorName(currentMaterial.name);
            currentColorPanel.Find("CurrentNameText").gameObject.GetComponent<Text>().text = currentMaterialName;
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
                switch (i)
                {
                    case 0:
                        textMaterialNameLabel.text = "黒";
                        break;
                    case 1:
                        textMaterialNameLabel.text = "赤";
                        break;
                    case 2:
                        textMaterialNameLabel.text = "金属";
                        break;
                    case 3:
                        textMaterialNameLabel.text = "レンガ";
                        break;
                    case 4:
                        textMaterialNameLabel.text = "白";
                        break;
                    case 5:
                        textMaterialNameLabel.text = "黄色";
                        break;
                    case 6:
                        textMaterialNameLabel.text = "オレンジ";
                        break;
                    case 7:
                        textMaterialNameLabel.text = "黄緑";
                        break;
                    case 8:
                        textMaterialNameLabel.text = "水色";
                        break;
                    case 9:
                        textMaterialNameLabel.text = "青";
                        break;
                    case 10:
                        textMaterialNameLabel.text = "緑";
                        break;
                    case 11:
                        textMaterialNameLabel.text = "紫";
                        break;
                    case 12:
                        textMaterialNameLabel.text = "木材";
                        break;
                }

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

        string ChengeMaterialNameToColorName (string materialName)
        {
            string colorName = "";
            string str1 = materialName.Replace("Color", "");
            string str2 = str1.Replace(" (Instance)", "");
            switch (str2)
            {
                case "0":
                    colorName = "黒";
                    break;
                case "1":
                    colorName = "赤";
                    break;
                case "2":
                    colorName = "黄色";
                    break;
                case "3":
                    colorName = "オレンジ";
                    break;
                case "4":
                    colorName = "黄緑";
                    break;
                case "5":
                    colorName = "水色";
                    break;
                case "6":
                    colorName = "青";
                    break;
                case "7":
                    colorName = "緑";
                    break;
                case "8":
                    colorName = "紫";
                    break;
                case "9":
                    colorName = "木材";
                    break;
                case "10":
                    colorName = "金属";
                    break;
                case "11":
                    colorName = "レンガ";
                    break;
                case "12":
                    colorName = "白";
                    break;
            }
            return colorName;
        }
        string ChengeColorNameToNumber(string colorName)
        {
            string colorNumber = "";
            switch (colorName)
            {
                case "黒":
                    colorNumber = "0";
                    break;
                case "赤":
                    colorNumber = "1";
                    break;
                case "黄色":
                    colorNumber = "2";
                    break;
                case "オレンジ":
                    colorNumber = "3";
                    break;
                case "黄緑":
                    colorNumber = "4";
                    break;
                case "水色":
                    colorNumber = "5";
                    break;
                case "青":
                    colorNumber = "6";
                    break;
                case "緑":
                    colorNumber = "7";
                    break;
                case "紫":
                    colorNumber = "8";
                    break;
                case "木材":
                    colorNumber = "9";
                    break;
                case "金属":
                    colorNumber = "10";
                    break;
                case "レンガ":
                    colorNumber = "11";
                    break;
                case "白":
                    colorNumber = "12";
                    break;
            }
            return colorNumber;
        }
        public void OnClickChangeButton()
        {
            Toggle checkToggle = toggleGroup.ActiveToggles().FirstOrDefault();
            string MaterialNumber = ChengeColorNameToNumber(checkToggle.transform.Find("MaterialNameLabel").gameObject.GetComponent<Text>().text);
            targetBlock.GetComponent<Block>().SetColor(MaterialNumber);
        }

        public void OnClickAllColorChangeButton()
        {
            Toggle checkToggle = toggleGroup.ActiveToggles().FirstOrDefault();
            string MaterialNunber = ChengeColorNameToNumber(checkToggle.transform.Find("MaterialNameLabel").gameObject.GetComponent<Text>().text);
            string MaterialName = "Color" + MaterialNunber;
            Material toMaterial = contentMaterials.Find(material => material.name == MaterialName);
            blockManager.ApplyColorRules(blockManager.MakeColorRules("color", targetBlock.GetComponent<Renderer>().material.name.Replace("Color", ""), toMaterial.name.Replace("Color", "")));
        }

        public void OnClickCancelButton()
        {
            colorChangePanel.SetActive(false);
            //Emissionの有効化・無効化
            targetBlock.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        }
    }
}
