﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JsonFormats;

namespace VrScene
{
    public class Block : MonoBehaviour
    {
        GameObject test;
        public float x;
        public float y;
        public float z;
        public string ID;
        public float time;
        public bool put;
        public string colorID;


        public void SetBlockData(BlockInfo block)
        {
            x = block.x;
            y = block.y;
            z = block.z;
            ID = block.ID;
            time = block.time;
            put = block.put;
            colorID = block.colorID;
        }

        public Vector3 GetPosition()
        {
            Vector3 position = new Vector3(0.32f*x, 0.384f*y, 0.32f*z);
            return position;
        }

        public Texture texture;

        public void SetColor(string colorID)
        {
            string colorName = "Color" + colorID;
            Material colorMaterial = Resources.Load(colorName) as Material;
            GetComponent<Renderer>().sharedMaterial = colorMaterial;
        }

        public void SetActive(bool f)
        {
            this.gameObject.SetActive(f);
        }
        public void OnClickBlock()
        {
            //Debug.Log(gameObject.GetComponent<Renderer>().material.GetTexture("_MainTex"));
            GameObject canvas = GameObject.Find("Canvas");
            GameObject panel = canvas.transform.Find("ColorChangePanel").gameObject;

            if (panel.activeSelf)
            {
                panel.SetActive(false);
            }
            panel.SetActive(true);

            ColorChangePanel colorChangePanel = panel.GetComponent<ColorChangePanel>();
            Material material = gameObject.GetComponent<Renderer>().material;
            Color color = material.color;
            print(color.ToString());
                color.r = color.r + 0.20 > 1 ? color.r + (float)0.20 - 1 : color.r + (float)0.20;
                color.b = color.b + 0.20 > 1 ? color.b + (float)0.20 - 1 : color.b + (float)0.20;
                color.g = color.g + 0.20 > 1 ? color.g + (float)0.20 - 1 : color.g + (float)0.20;
            print(color.ToString());

            if (colorChangePanel.lightUpObject == null)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color);
                material.SetTexture("_MainTex", texture);
                colorChangePanel.lightUpObject = gameObject;
            }
            if (gameObject.GetComponent<Block>().ID != colorChangePanel.lightUpObject.GetComponent<Block>().ID)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color);
                material.SetTexture("_MainTex", texture);
                colorChangePanel.lightUpObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                colorChangePanel.lightUpObject.GetComponent<Renderer>().material.SetTexture("_MainTex", null);
                colorChangePanel.lightUpObject = gameObject;
            }
            colorChangePanel.SetupColorChangePanel(gameObject);
        }
    }
}
