using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeScene
{
    public class TouchManager : MonoBehaviour
    {
        int LastTouchCount = 0;
        GameObject CurrentMapParent = null;
 
        private Vector3 LastTouchPos;
        Vector2 currentDiff = Vector2.zero;
        float blockCoefficientXZ = 0.32f;
   
        bool IsRotate = false;
        Vector2 LastDirection;
        float LastAngle = 0f;
        float LastDistance = 0;

        void Update()
        {
            if (Input.touchCount == 0)
            {
                if (this.CurrentMapParent != null)
                {
                    this.CurrentMapParent.GetComponent<MapParent>().Rotate(0);
                }
            }
            ChangeParentColor(this.CurrentMapParent, false);
            if (Input.touchCount == 1)
            { // タッチしてる指が1本のとき
                this.IsRotate = false;
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
                if ( (t1.phase == TouchPhase.Began || t2.phase == TouchPhase.Began) &&(IsMapParent(ShootingRay(t1.position)) || IsMapParent(ShootingRay(t2.position))))
                {
                    this.IsRotate = true;
                    this.LastDirection = t1.position - t2.position;
                    this.LastAngle = 0f;
                    if (IsMapParent(ShootingRay(t1.position)))
                    {
                        this.CurrentMapParent = ShootingRay(t1.position);
                    }
                    else
                    {
                        this.CurrentMapParent = ShootingRay(t2.position);
                    }
                }

                if (this.IsRotate)
                {
                    var currentDirection = t1.position - t2.position;
                    float angle = Vector2.Angle(this.LastDirection, currentDirection);
                    angle *= Vector3.Cross(this.LastDirection, currentDirection).z > 0 ? -1 : 1;
                    if (Mathf.Abs(angle) >= 30f) {
                        var rotateDirection = angle > 0 ? 1 : -1;
                        this.LastDirection = currentDirection;
                        angle = 0f;
                        this.CurrentMapParent.GetComponent<MapParent>().Rotate(rotateDirection);
                    };
                    if (Mathf.Abs(angle) > 5f)
                    {
                        this.CurrentMapParent.transform.Rotate(new Vector3(0f, (angle - this.LastAngle) * 3, 0f));
                    }
                    this.LastAngle = angle;
                } else
                {
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