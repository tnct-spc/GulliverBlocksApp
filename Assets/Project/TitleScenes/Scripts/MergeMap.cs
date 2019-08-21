using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeMap : MonoBehaviour
{
    public GameObject imagePref;
    private int verticalBlockNum = 48;
    private int horizontalBolckNum = 48;

    private void OnEnable()
    {
        GameObject canvas = GameObject.Find("Canvas");
        RectTransform content = canvas.transform.Find("MergePanel/Panel").gameObject.GetComponent<RectTransform>();
        for (int i = 0; i < verticalBlockNum * horizontalBolckNum; i++)
        {
            GameObject image = Instantiate(imagePref) as GameObject;
            image.transform.SetParent(content, false);
        }
    }
}
