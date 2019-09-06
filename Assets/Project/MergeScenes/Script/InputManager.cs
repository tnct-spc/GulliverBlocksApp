using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeScene
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] GameObject camera;

        private void Start()
        {
        }

        public void OnClickPlusButton()
        {
            camera.GetComponent<Camera>().orthographicSize -= 5;
        }

        public void OnClickMinusButton()
        {
            camera.GetComponent<Camera>().orthographicSize += 5;
        }
    }
}
