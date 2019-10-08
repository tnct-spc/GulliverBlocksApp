using System.Collections.Generic;
using UnityEngine;

public class VerticalRoadManager : MonoBehaviour
{
    private new Renderer renderer;

    private string CAR_3DMODEL_NAME = "Car";

    private float frontZPosition;
    private float backZPosition;
    private float rightSideCenterXPosition;
    private float leftSideCenterXPosition;
    private float depth;
    private float Z_RATIO = 0.32f;

    System.Random rnd = new System.Random();

    private int RANDOM_SPEED_MIN = 2;
    private int RANDOM_SPEED_MAX = 7;

    private List<(GameObject car, int speed)> rightSideCars = new List<(GameObject car, int speed)>();
    private List<(GameObject car, int speed)> leftSideCars = new List<(GameObject car, int speed)>();

    void Start()
    {
        renderer = this.GetComponent<Renderer>();

        frontZPosition = this.transform.position.z - renderer.bounds.size.z / 2;
        backZPosition = this.transform.position.z + renderer.bounds.size.z / 2;
        rightSideCenterXPosition = this.transform.position.x + renderer.bounds.size.x / 4;
        leftSideCenterXPosition = this.transform.position.x - renderer.bounds.size.x / 4;
        depth = this.GetComponent<Renderer>().bounds.size.z / Z_RATIO; // ブロック単位の深さ

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
                AddVelocity(rightSideCars[i].car, rightSideCars[i].speed, "back");
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
                AddVelocity(leftSideCars[i].car, leftSideCars[i].speed, "forward");
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
            carObject.transform.parent = transform;
            AddVelocity(carObject, randomSpeed, "back");
            rightSideCars.Add((car: carObject, speed: randomSpeed));
        }
        else if(side == "left")
        {
            GameObject carObject = Instantiate(car3dModel, new Vector3(leftSideCenterXPosition, this.transform.position.y + renderer.bounds.size.y, frontZPosition), Quaternion.identity) as GameObject;
            carObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f); // そのままだと大きすぎるので小さくする
            carObject.AddComponent<Rigidbody>();
            carObject.GetComponent<Rigidbody>().useGravity = false;
            carObject.transform.parent = transform;
            AddVelocity(carObject, randomSpeed, "forward");
            leftSideCars.Add((car: carObject, speed: randomSpeed));
        }
    }

    void AddVelocity(GameObject gameObject, int speed, string direction /*forward, back*/)
    {
        // 4ブロック以上あれば動かす
        if (depth >= 4)
        {
            switch (direction)
            {
                case "forward":
                    gameObject.GetComponent<Rigidbody>().velocity = Vector3.forward * speed / 10;
                    break;

                case "back":
                    gameObject.GetComponent<Rigidbody>().velocity = Vector3.back * speed / 10;
                    break;
            }
        }
    }
}
