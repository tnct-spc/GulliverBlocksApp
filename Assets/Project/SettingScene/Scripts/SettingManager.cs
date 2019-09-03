using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public Toggle UseGyroToggle;
    public Toggle NotUseGyroToggle;
    public static bool UseGyro = true;
    void Start()
    {
        UseGyroToggle.onValueChanged.AddListener(OnUseGyroToggleValueChanged);
        UseGyroToggle.isOn = UseGyro;
    }

    public void OnUseGyroToggleValueChanged(bool value)
    {
        UseGyro = value;
    }
}
