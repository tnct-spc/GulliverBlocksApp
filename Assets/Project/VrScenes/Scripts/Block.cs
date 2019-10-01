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



            /*
                        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
                        meshFilter.mesh.SetIndices(meshFilter.mesh.GetIndices(0), MeshTopology.Lines, 0);
            */
            Color color = material.color;
            Material lastMaterial = material;
            //color.r = Math.Abs(color.r - 0.30f);
            //color.b = Math.Abs(color.b - 0.30f);
            //color.g = Math.Abs(color.g - 0.30f);
            color.a = color.a * 0.3F;
            material.SetColor("_Color", color);
            
            if (colorChangePanel.lightUpObject == null)
            {
                //   material.EnableKeyword("_EMISSION");
                //  material.SetColor("_EmissionColor", lastColor);


                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;


                material.SetColor("_Color", color);
                //material.SetTexture("_MainTex", texture);
                colorChangePanel.lightUpObject = gameObject;
            }
            if (gameObject.GetComponent<Block>().ID != colorChangePanel.lightUpObject.GetComponent<Block>().ID)
            {
                // material.EnableKeyword("_EMISSION");
                // material.SetColor("_EmissionColor", lastColor);


                material = lastMaterial;

                material.SetTexture("_MainTex", texture);
                material.SetColor("_Color", color);
                //colorChangePanel.lightUpObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                colorChangePanel.lightUpObject.GetComponent<Renderer>().material.SetColor("_Color", colorChangePanel.lastBlockColor);
                //colorChangePanel.lightUpObject.GetComponent<Renderer>().material.SetTexture("_MainTex", null);

                colorChangePanel.lightUpObject = gameObject;
            }
            colorChangePanel.SetupColorChangePanel(gameObject);
        }
    }
}
