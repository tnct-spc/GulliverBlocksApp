using System.Collections;
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
        public float time;
        public string colorID;


        public void SetBlockData(BlockInfo block)
        {
            x = block.x;
            y = block.y;
            z = block.z;
            ID = block.ID;
            time = block.time;
            colorID = block.colorID;
        }

        public Vector3 GetPosition()
        {
            Vector3 position = new Vector3(0.32f*x, 0.384f*y, 0.32f*z);
            return position;
        }

        public void SetColor(string colorID)
        {
            string colorName = "Color" + colorID;
            Material colorMaterial = Resources.Load("Materials/"+colorName) as Material;
            GetComponent<Renderer>().material = colorMaterial;
            this.colorID = colorID;
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
            Material material = gameObject.GetComponent<Renderer>().material;

            Material lastMaterial = material;
            
            if (colorChangePanel.lightUpObject == null)
            {
                colorChangePanel.lastBlockMaterial = lastMaterial;
                this.SetColor((int.Parse(colorID) + 13).ToString());
                colorChangePanel.lightUpObject = gameObject;
            }
            if (gameObject.GetComponent<Block>().ID != colorChangePanel.lightUpObject.GetComponent<Block>().ID)
            {
                colorChangePanel.lightUpObject.GetComponent<Block>().SetColor((int.Parse(colorChangePanel.lightUpObject.GetComponent<Block>().colorID) - 13).ToString());
                //colorChangePanel.lightUpObject.GetComponent<Renderer>().material = colorChangePanel.lastBlockMaterial ;
                this.SetColor((int.Parse(colorID) + 13).ToString()); 


                colorChangePanel.lightUpObject = gameObject;
                colorChangePanel.lastBlockMaterial = lastMaterial;
            }
            colorChangePanel.SetupColorChangePanel(gameObject);
        }
    }
}
