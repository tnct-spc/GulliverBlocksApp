using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangePanel : MonoBehaviour
{
    void OnEnable()
    {
        Debug.Log("click");
        List<Material> contentMaterials = new List<Material>();
        for(int i = 0; i < 10; i++)
        {
            string materialName = "Color" + i.ToString();
            Material material = Resources.Load(materialName, typeof(Material)) as Material;
            Debug.Log(material.name);
            contentMaterials.Add(material);
        }
        
    }
}
