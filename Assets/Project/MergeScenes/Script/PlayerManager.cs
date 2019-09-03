using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeScene
{
    public class PlayerManager : MonoBehaviour
    {
        private Vector3 touchStartPos;
        private Vector3 touchEndPos;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Input.mousePosition;
                ShootingRay();
            }
            if (Input.GetMouseButton(0))
            {
                touchEndPos = Input.mousePosition;
                GetDirection();
            }
        }

        private void ShootingRay()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000)) {
                Debug.Log(hit.collider.gameObject.transform.root.name);
            }
        }

        private void GetDirection()
        {
            float directionX = touchStartPos.x - touchEndPos.x;
            float directionY = touchStartPos.y - touchEndPos.y;
            gameObject.transform.Translate(directionX/10, directionY/10, 0);
            touchStartPos = Input.mousePosition;
        }
    }
}