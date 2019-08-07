using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void OnClickBlock()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject panel = canvas.transform.Find("ColorChangePanel").gameObject;
        panel.SetActive(true);
        ColorChangePanel colorChangePanel = panel.GetComponent<ColorChangePanel>();
        colorChangePanel.SetupColorChangePanel(gameObject);
    }
}
