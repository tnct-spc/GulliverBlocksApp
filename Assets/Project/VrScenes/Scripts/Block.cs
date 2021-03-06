﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JsonFormats;
using System;

namespace VrScene
{
    public class Block : MonoBehaviour
    {
        GameObject test;
        public float x;
        public float y;
        public float z;
        public string ID;
        public double time;
        public string colorID;
        public string applied_colorID;
        public string map_id;


        public void SetBlockData(BlockInfo block)
        {
            x = block.x;
            y = block.y;
            z = block.z;
            ID = block.ID;
            time = block.time;
            colorID = block.colorID;
            applied_colorID = block.colorID;
            map_id = block.map_id;
        }

        public Vector3 GetPosition()
        {
            Vector3 position = new Vector3(0.32f*x, 0.384f*y, 0.32f*z);
            return position;
        }

        public void SetColor(string colorID, bool isSelected)
        {
            string colorName = "Color" + colorID;
            Material colorMaterial;
            if (isSelected)
            {
            colorMaterial = Resources.Load("SelectedColors/"+colorName) as Material;
            }
            else
            { 
            colorMaterial = Resources.Load("Materials/"+colorName) as Material;
            }
            GetComponent<Renderer>().material = colorMaterial;
            this.applied_colorID = colorID;
        }

        public void SetActive(bool f)
        {
            this.gameObject.SetActive(f);
        }

        public void Delete()
        {
            Destroy(this.gameObject);
        }

        public Texture texture;

        public void OnClickBlock()
        {
            GameObject canvas = GameObject.Find("Canvas");
            GameObject panel = canvas.transform.Find("ViewModeUI/ColorChangePanel").gameObject;

            if (panel.activeSelf)
            {
                panel.SetActive(false);
            }
            panel.SetActive(true);

            ColorChangePanel colorChangePanel = panel.GetComponent<ColorChangePanel>();

            
            if (colorChangePanel.lightUpObject == null)
            {
                this.SetColor(applied_colorID, true);
                colorChangePanel.lightUpObject = gameObject;
            }
            if (gameObject.GetComponent<Block>().ID != colorChangePanel.lightUpObject.GetComponent<Block>().ID)
            {
                colorChangePanel.lightUpObject.GetComponent<Block>().SetColor(colorChangePanel.lightUpObject.GetComponent<Block>().applied_colorID, false);
                //colorChangePanel.lightUpObject.GetComponent<Renderer>().material = colorChangePanel.lastBlockMaterial ;
                this.SetColor(applied_colorID, true); 


                colorChangePanel.lightUpObject = gameObject;
            }
            colorChangePanel.SetupColorChangePanel(gameObject);
        }
    }
}
