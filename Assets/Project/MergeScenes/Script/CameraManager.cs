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
        private bool isMap = false;
        float blockCoefficientXZ = 0.32f;
        float X;
        float Y;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                ShootingRay();
                isMap = (hitObject != null && hitObject.tag == "map_parent");
                if (isMap)
                {
                    hitObject.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 70);
                }
            }
            else if (Input.GetMouseButton(0))
            {
                touchEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (isMap)
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
                if (isMap)
                {
                    hitObject.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 120);

                }
                isMap = false;
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
            hitObject = hit.transform.gameObject;
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