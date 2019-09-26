using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveUsername: MonoBehaviour
{
    public InputField nameInputField;
    public Text text;

    void Start()
    {
        nameInputField = nameInputField.GetComponent<InputField>();
        text = text.GetComponent<Text>();
    }

    public void InputText()
    {
        text.text = nameInputField.text;
    }

    public void SaveDate()
    {
        PlayerPrefs.SetString("UserName", nameInputField.text);
        PlayerPrefs.Save();
    }

    public void LoadDate()
    {
        nameInputField.text = PlayerPrefs.GetString("UserName", "");
    }

    public void DeleteDate()
    {
        PlayerPrefs.DeleteKey("UserName");
        PlayerPrefs.DeleteAll();
    }

}
