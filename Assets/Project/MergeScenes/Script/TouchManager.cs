using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeScene
{
    public class TouchManager : MonoBehaviour
    {
        private Vector3 LastTouchPos;
        private bool isMap = false;
        float blockCoefficientXZ = 0.32f;
        Vector2 currentDiff = Vector2.zero;
        float LastDistance = 0;
        GameObject CurrentMapParent = null;
        int LastTouchCount = 0;

        void Update()
        {
            ChangeParentColor(this.CurrentMapParent, false);
            if (Input.touchCount == 1)
            { // タッチしてる指が1本のとき
                Touch t1 = Input.GetTouch(0);
                var tappedObject = ShootingRay(t1.position);
                Vector3 tappedPostion = Camera.main.ScreenToWorldPoint(t1.position);
                if (IsMapParent(tappedObject))
                {
                    ChangeParentColor(tappedObject, true);
                    CurrentMapParent = tappedObject;
                }
                if (this.LastTouchCount == 2||t1.phase == TouchPhase.Began)
                {
                    this.currentDiff = Vector2.zero;
                } else
                {
                    if (IsMapParent(tappedObject))
                    {
                        MoveMap(tappedObject, this.LastTouchPos,tappedPostion);
                    }
                    else
                    {
                        MoveCamera(this.LastTouchPos, tappedPostion);
                    }
                }
                this.LastTouchPos = Camera.main.ScreenToWorldPoint(t1.position);
                
            } else if (Input.touchCount == 2)
            {
                // タッチしてる指が2本のとき
                Touch t1 = Input.GetTouch(0);
                Touch t2 = Input.GetTouch(1);
                float distance = (t1.position - t2.position).magnitude;
                if (t1.phase != TouchPhase.Began && t2.phase != TouchPhase.Began)
                {
                    float cameraScale = this.gameObject.GetComponent<Camera>().orthographicSize;
                    cameraScale += (this.LastDistance - distance)/ 100;
                    cameraScale = Mathf.Max(0.2f, cameraScale);
                    this.gameObject.GetComponent<Camera>().orthographicSize = cameraScale;
                }
                this.LastDistance = distance;
            }
            this.LastTouchCount = Input.touchCount;
        }

        private GameObject ShootingRay(Vector3 position)
        {
            Ray ray = Camera.main.ScreenPointToRay(position);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 1000);
            if (hit.transform == null) return null;
            return hit.transform.gameObject;
        }

        private void ChangeParentColor(GameObject g, bool selected)
        {
            if (IsMapParent(g))
                g.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, (byte)(selected ? 70 : 120));
        }

        private bool IsMapParent(GameObject g)
        {
            return (g != null && g.tag == "map_parent");
        }
 
        private void MoveCamera(Vector3 lastPos, Vector3 currentPos)
        {
            float directionX = lastPos.x - currentPos.x;
            float directionY = lastPos.z - currentPos.z;
            gameObject.transform.Translate(directionX, directionY, 0);
        }

        private void MoveMap(GameObject mapObject, Vector3 lastPos, Vector3 currentPos)
        {
            this.currentDiff.x += currentPos.x - lastPos.x;
            this.currentDiff.y += currentPos.z - lastPos.z;
            mapObject.GetComponent<MapParent>().Move((int)(this.currentDiff.x / blockCoefficientXZ),(int)(this.currentDiff.y / blockCoefficientXZ));
            this.currentDiff.x %= blockCoefficientXZ;
            this.currentDiff.y %= blockCoefficientXZ;
        }
    }
}