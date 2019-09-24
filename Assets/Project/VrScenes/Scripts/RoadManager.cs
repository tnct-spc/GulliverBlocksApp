using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    private new Renderer renderer;

    private string CAR_3DMODEL_NAME = "Car";

    private float frontZPosition;
    private float backZPosition;
    private float rightSideCenterXPosition;
    private float leftSideCenterXPosition;

    System.Random rnd = new System.Random();

    private int RANDOM_SPEED_MIN = 5;
    private int RANDOM_SPEED_MAX = 10;

    private List<(GameObject car, int speed)> rightSideCars = new List<(GameObject car, int speed)>();
    private List<(GameObject car, int speed)> leftSideCars = new List<(GameObject car, int speed)>();

    void Start()
    {
        renderer = this.GetComponent<Renderer>();

        frontZPosition = this.transform.position.z - renderer.bounds.size.z / 2;
        backZPosition = this.transform.position.z + renderer.bounds.size.z / 2;
        rightSideCenterXPosition = this.transform.position.x + renderer.bounds.size.x / 4;
        leftSideCenterXPosition = this.transform.position.x - renderer.bounds.size.x / 4;

        GenerateCar("right");
        GenerateCar("left");
    }

    void FixedUpdate()
    {
        for (int i = 0; i < rightSideCars.Count; i++)
        {
            // 道路をはみ出しているか判定
            if (rightSideCars[i].car.transform.position.z <= frontZPosition)
            {
                // 生成時の座標に戻す
                rightSideCars[i].car.transform.position = new Vector3(
                    rightSideCars[i].car.transform.position.x,
                    rightSideCars[i].car.transform.position.y,
                    backZPosition
                );
                // 車の速さを変更
                rightSideCars[i] = (car: rightSideCars[i].car, speed: rnd.Next(RANDOM_SPEED_MIN, RANDOM_SPEED_MAX));
                rightSideCars[i].car.GetComponent<Rigidbody>().velocity = Vector3.back * rightSideCars[i].speed / 10;
            }
        }

        for (int i = 0; i < leftSideCars.Count; i++)
        {
            // 道路をはみ出しているか判定
            if (leftSideCars[i].car.transform.position.z >= backZPosition)
            {
                // 生成時の座標に戻す
                leftSideCars[i].car.transform.position = new Vector3(
                    leftSideCars[i].car.transform.position.x,
                    leftSideCars[i].car.transform.position.y,
                    frontZPosition
                );
                // 車の速さを変更
                leftSideCars[i] = (car: leftSideCars[i].car, speed: rnd.Next(RANDOM_SPEED_MIN, RANDOM_SPEED_MAX));
                leftSideCars[i].car.GetComponent<Rigidbody>().velocity = Vector3.forward * leftSideCars[i].speed / 10;
            }
        }
    }

    void GenerateCar(string side /*left or right*/)
    {
        GameObject car3dModel = (GameObject)Resources.Load(CAR_3DMODEL_NAME);
        int randomSpeed = rnd.Next(RANDOM_SPEED_MIN, RANDOM_SPEED_MAX+1);
        
        if(side == "right")
        {
            GameObject carObject = Instantiate(car3dModel, new Vector3(rightSideCenterXPosition, this.transform.position.y + renderer.bounds.size.y, backZPosition), Quaternion.Euler(0, 180, 0)) as GameObject;
            carObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f); // そのままだと大きすぎるので小さくする
            carObject.AddComponent<Rigidbody>();
            carObject.GetComponent<Rigidbody>().useGravity = false;
            carObject.GetComponent<Rigidbody>().velocity = Vector3.back * randomSpeed / 10;
            rightSideCars.Add((car: carObject, speed: randomSpeed));
        }
        else if(side == "left")
        {
            GameObject carObject = Instantiate(car3dModel, new Vector3(leftSideCenterXPosition, this.transform.position.y + renderer.bounds.size.y, frontZPosition), Quaternion.identity) as GameObject;
            carObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f); // そのままだと大きすぎるので小さくする
            carObject.AddComponent<Rigidbody>();
            carObject.GetComponent<Rigidbody>().useGravity = false;
            carObject.GetComponent<Rigidbody>().velocity = Vector3.forward * randomSpeed / 10;
            leftSideCars.Add((car: carObject, speed: randomSpeed));
        }
    }
}
