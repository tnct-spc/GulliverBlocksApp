using UnityEngine;
using System.Collections;
using UnityEngine.XR;

namespace VrScene {
    public class MoveForwardInTwoEyesMode : MonoBehaviour
    {
        public Camera mainCamera;
        public float moveSpeed = 1.0f;

        bool isTouching = false;

        GameObject Player;
        PlayerManager PlayerManager;
        float yOffset;
        void Start()
        {
            Player = GameObject.Find("Player");
            PlayerManager = Player.GetComponent<PlayerManager>();
        }

        void Update()
        {
            yOffset = mainCamera.transform.position.y;
            moveSpeed = PlayerManager.default_move_speed;
            if (XRSettings.enabled)
            {
                float x = mainCamera.transform.eulerAngles.x;
            }
            if (isTouching) moveForward();
        }

        private void moveForward()
        {
            Vector3 direction = new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z).normalized * moveSpeed * Time.deltaTime;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, -mainCamera.transform.rotation.eulerAngles.y, 0));
            mainCamera.transform.Translate(rotation * direction);
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, yOffset, mainCamera.transform.position.z);
        }

        public void TouchDown()
        {
            isTouching = true;
        }

        public void TouchUp()
        {
            isTouching = false;
        }
    }
}