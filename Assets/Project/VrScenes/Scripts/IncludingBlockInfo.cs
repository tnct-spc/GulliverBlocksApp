using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JsonFormats;

namespace VrScene
{
    public class IncludingBlockInfo : MonoBehaviour
    {
        public float x;
        public float y;
        public float z;
        public string ID;
        public float time;
        public bool put;
        public string colorID;

        public void SetBlockData(Block block)
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
            Vector3 position = new Vector3(x, y, z);
            return position;
        }

        public Texture texture;

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

            if (colorChangePanel.lightUpObject == null)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color);
                material.SetTexture("_MainTex", texture);
                colorChangePanel.lightUpObject = gameObject;
            }
            if (gameObject.GetComponent<IncludingBlockInfo>().ID != colorChangePanel.lightUpObject.GetComponent<IncludingBlockInfo>().ID)
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
