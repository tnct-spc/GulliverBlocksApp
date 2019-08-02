using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

public class SeekBarMover : MonoBehaviour
{
    Slider SeekBar;

    InputManager InputManager;
    BlockManager BlockManager;
    int i = 1;

    void Start()
    {
        SeekBar = GetComponent<Slider>();
        SeekBar.maxValue = 100;
        SeekBar.value = 0;
    }

    
    void Update()
    {
        if (SeekBar.value == 100 || SeekBar.value == 0) i *= -1;
        SeekBar.value += i;
    }
}
