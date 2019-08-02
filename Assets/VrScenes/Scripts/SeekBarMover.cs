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
    public Slider SeekBar;

    InputManager InputManager;
    BlockManager BlockManager;
    
    void Start()
    {
        SeekBar = GetComponent<Slider>();
        SeekBar.maxValue = 100;
        SeekBar.value = 50;
    }

    
    void Update()
    {
        
    }
}
