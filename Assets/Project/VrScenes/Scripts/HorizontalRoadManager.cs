using System.Collections.Generic;
using UnityEngine;

public class HorizontalRoadManager : MonoBehaviour
{
    private new Renderer renderer;

    private string CAR_3DMODEL_NAME = "Car";

    private float frontXPosition;
    private float backXPosition;
    private float rightSideCenterZPosition;
    private float leftSideCenterZPosition;
    private float width;
    private float X_RATIO = 0.32f;

    System.Random rnd = new System.Random();

    private int RANDOM_SPEED_MIN = 4;
    private int RANDOM_SPEED_MAX = 9;

    private List<(GameObject car, int speed)> rightSideCars = new List<(GameObject car, int speed)>();
    private List<(GameObject car, int speed)> leftSideCars = new List<(GameObject car, int speed)>();

    void Start()
    {
        renderer = this.GetComponent<Renderer>();

        frontXPosition = this.transform.position.x - renderer.bounds.size.x / 2;
        backXPosition = this.transform.position.x + renderer.bounds.size.x / 2;
        rightSideCenterZPosition = this.transform.position.z + renderer.bounds.size.z / 4;
        leftSideCenterZPosition = this.transform.position.z - renderer.bounds.size.z / 4;
        width = this.GetComponent<Renderer>().bounds.size.x / X_RATIO; // ブロック単位の深さ

        GenerateCar("right");
        GenerateCar("left");
    }

    void FixedUpdate()
    {
        for (int i = 0; i < rightSideCars.Count; i++)
        {
            // 道路をはみ出しているか判定
            if (rightSideCars[i].car.transform.position.x >= backXPosition)
            {
                // 生成時の座標に戻す
                rightSideCars[i].car.transform.position = new Vector3(
                    frontXPosition,
                    rightSideCars[i].car.transform.position.y,
                    rightSideCars[i].car.transform.position.z
                );
                // 車の速さを変更
                rightSideCars[i] = (car: rightSideCars[i].car, speed: rnd.Next(RANDOM_SPEED_MIN, RANDOM_SPEED_MAX));
                AddVelocity(rightSideCars[i].car, rightSideCars[i].speed, "right");
            }
        }

        for (int i = 0; i < leftSideCars.Count; i++)
        {
            // 道路をはみ出しているか判定
            if (leftSideCars[i].car.transform.position.x <= frontXPosition)
            {
                // 生成時の座標に戻す
                leftSideCars[i].car.transform.position = new Vector3(
                    backXPosition,
                    leftSideCars[i].car.transform.position.y,
                    leftSideCars[i].car.transform.position.z
                );
                // 車の速さを変更
                leftSideCars[i] = (car: leftSideCars[i].car, speed: rnd.Next(RANDOM_SPEED_MIN, RANDOM_SPEED_MAX));
                AddVelocity(leftSideCars[i].car, leftSideCars[i].speed, "left");
            }
        }
    }

    void GenerateCar(string side /*left or right*/)
    {
        GameObject car3dModel = (GameObject)Resources.Load(CAR_3DMODEL_NAME);
        int randomSpeed = rnd.Next(RANDOM_SPEED_MIN, RANDOM_SPEED_MAX + 1);

        if (side == "right")
        {
            GameObject carObject = Instantiate(car3dModel, new Vector3(frontXPosition, this.transform.position.y + renderer.bounds.size.y, rightSideCenterZPosition), Quaternion.Euler(0, 90, 0)) as GameObject;
            carObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f); // そのままだと大きすぎるので小さくする
            carObject.AddComponent<Rigidbody>();
            carObject.GetComponent<Rigidbody>().useGravity = false;
            carObject.transform.parent = transform;
            AddVelocity(carObject, randomSpeed, "right");
            rightSideCars.Add((car: carObject, speed: randomSpeed));
        }
        else if (side == "left")
        {
            GameObject carObject = Instantiate(car3dModel, new Vector3(backXPosition, this.transform.position.y + renderer.bounds.size.y, leftSideCenterZPosition), Quaternion.Euler(0, 270, 0)) as GameObject;
            carObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f); // そのままだと大きすぎるので小さくする
            carObject.AddComponent<Rigidbody>();
            carObject.GetComponent<Rigidbody>().useGravity = false;
            carObject.transform.parent = transform;
            AddVelocity(carObject, randomSpeed, "left");
            leftSideCars.Add((car: carObject, speed: randomSpeed));
        }
    }

    void AddVelocity(GameObject gameObject, int speed, string direction /*left, right*/)
    {
        // 4ブロック以上あれば動かす
        if (width >= 4)
        {
            switch (direction)
            {
                case "left":
                    gameObject.GetComponent<Rigidbody>().velocity = Vector3.left * speed / 10;
                    break;

                case "right":
                    gameObject.GetComponent<Rigidbody>().velocity = Vector3.right * speed / 10;
                    break;
            }
        }
    }
}
