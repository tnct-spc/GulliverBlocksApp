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
                touchStartPos = Input.mousePosition;
                if (ShootingRay().collider != null) ishit = true;
            }
            else if (Input.GetMouseButton(0))
            {
                touchEndPos = Input.mousePosition;
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
            }
        }

        private RaycastHit ShootingRay()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 1000);
            hitObject = hit.transform.root.gameObject;
            return hit;
        }

        private void MoveCamera()
        {
            float directionX = touchStartPos.x - touchEndPos.x;
            float directionY = touchStartPos.y - touchEndPos.y;
            gameObject.transform.Translate(directionX/20, directionY/20, 0);
            touchStartPos = Input.mousePosition;
        }

        private void MoveMap(GameObject mapObject, Vector3 vector)
        {
            mapObject.transform.Translate(vector);
            touchStartPos = Input.mousePosition;
        }

        private void CountFlame()
        {
            X += touchStartPos.x - touchEndPos.x;
            Y += touchStartPos.y - touchEndPos.y;
            if(Mathf.Abs(X) >= blockCoefficientXZ)
            {
                if (X > 0) MoveMap(hitObject, new Vector3(-1*blockCoefficientXZ, 0, 0));
                if (X < 0) MoveMap(hitObject, new Vector3(blockCoefficientXZ, 0, 0));
                X = 0;
            }
            else if(Mathf.Abs(Y) >= blockCoefficientXZ)
            {
                if (Y > 0) MoveMap(hitObject, new Vector3(0, 0, -1*blockCoefficientXZ));
                if (Y < 0) MoveMap(hitObject, new Vector3(0, 0, blockCoefficientXZ));
                Y = 0;
            }
        }
    }
}