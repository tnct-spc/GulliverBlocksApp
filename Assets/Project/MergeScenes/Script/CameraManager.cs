using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeScene
{
    public class CameraManager : MonoBehaviour
    {
        private Vector3 touchStartPos;
        private Vector3 touchEndPos;
        private bool ishit = false;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Input.mousePosition;
                if (ShootingRay().collider != null) ishit = true;
            }
            else if (Input.GetMouseButton(0))
            {
                if (!ishit)
                {
                    touchEndPos = Input.mousePosition;
                    MoveCamera();
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                ishit = false;
            }
        }

        private RaycastHit ShootingRay()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 1000);
            return hit;
        }

        private void MoveCamera()
        {
            float directionX = touchStartPos.x - touchEndPos.x;
            float directionY = touchStartPos.y - touchEndPos.y;
            gameObject.transform.Translate(directionX/10, directionY/10, 0);
            touchStartPos = Input.mousePosition;
        }
    }
}