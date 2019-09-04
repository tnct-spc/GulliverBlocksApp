using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeScene
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] GameObject camera;
        CameraManager cameraManager;

        private void Start()
        {
            cameraManager = camera.GetComponent<CameraManager>();
        }

        public void OnClickPlusButton()
        {
            camera.transform.Translate(0, 0, -100);
        }

        public void OnClickMinusButton()
        {
            camera.transform.Translate(0, 0, -100);
        }
    }
}
