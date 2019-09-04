using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeScene
{
    public class CameraManager : MonoBehaviour
    {
        private Vector3 touchStartPos;
        private Vector3 touchEndPos;
        private GameObject hitObject;
        private bool ishit = false;
        float blockCoefficientXZ = 0.32f;
        float X;
        float Y;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (ShootingRay().collider != null) ishit = true;
            }
            else if (Input.GetMouseButton(0))
            {
                touchEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (ishit)
                {
                    CountFlame();
                }
                else
                {
                    MoveCamera();
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                ishit = false;
                hitObject = null;
                X = 0f;
                Y = 0f;
            }
        }

        private RaycastHit ShootingRay()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 1000);
            if (hit.transform == null) return hit;
            hitObject = hit.transform.root.gameObject;
            return hit;
        }

        private void MoveCamera()
        {
            float directionX = touchStartPos.x - touchEndPos.x;
            float directionY = touchStartPos.z - touchEndPos.z;
            gameObject.transform.Translate(directionX, directionY, 0);
            touchStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private void MoveMap(GameObject mapObject, Vector3 vector)
        {
            mapObject.transform.Translate(vector);
            touchStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private void CountFlame()
        {
            if (hitObject == null) return;
            X += touchEndPos.x - touchStartPos.x;
            Y += touchEndPos.z - touchStartPos.z;
            MoveMap(hitObject, new Vector3(X - X%blockCoefficientXZ, 0, Y-Y%blockCoefficientXZ));
            X = X % blockCoefficientXZ;
            Y = Y % blockCoefficientXZ;
        }
    }
}