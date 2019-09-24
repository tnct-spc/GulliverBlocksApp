using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    private new Renderer renderer;

    private string CAR_3DMODEL_NAME = "Audi S3";

    private float frontZPosition;
    private float backZPosition;
    private float rightSideCenterXPosition;
    private float leftSideCenterXPosition;
    private float height;

    System.Random rnd = new System.Random();

    private int RANDOM_SPEED_MIN = 5;
    private int RANDOM_SPEED_MAX = 10;

    private List<(GameObject car, int speed)> rightSideCars = new List<(GameObject car, int speed)>();
    private List<(GameObject car, int speed)> leftSideCars = new List<(GameObject car, int speed)>();

    void Start()
    {
        renderer = this.GetComponent<Renderer>();

        frontZPosition = this.transform.position.z + renderer.bounds.size.z / 2;
        backZPosition = this.transform.position.z - renderer.bounds.size.z / 2;
        rightSideCenterXPosition = this.transform.position.x + renderer.bounds.size.x / 4;
        leftSideCenterXPosition = this.transform.position.x - renderer.bounds.size.x / 4;

        GenerateCar("right");
        GenerateCar("left");
    }

    void FixedUpdate()
    {
        for (int i = 0; i < rightSideCars.Count; i++)
        {
            if (rightSideCars[i].car.transform.position.z <= frontZPosition)
            {
                rightSideCars[i].car.transform.position = new Vector3(
                    rightSideCars[i].car.transform.position.x,
                    rightSideCars[i].car.transform.position.y,
                    backZPosition
                );
                rightSideCars[i] = (car: rightSideCars[i].car, speed: rnd.Next(RANDOM_SPEED_MIN, RANDOM_SPEED_MAX));
            }
            else
            {
                rightSideCars[i].car.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, rightSideCars[i].speed / 100);
            }
        }

        for (int i = 0; i < leftSideCars.Count; i++)
        {
            if (leftSideCars[i].car.transform.position.z >= backZPosition)
            {
                leftSideCars[i].car.transform.position = new Vector3(
                    leftSideCars[i].car.transform.position.x,
                    leftSideCars[i].car.transform.position.y,
                    frontZPosition
                );
                leftSideCars[i] = (car: leftSideCars[i].car, speed: rnd.Next(RANDOM_SPEED_MIN, RANDOM_SPEED_MAX));
            }
            else
            {
                leftSideCars[i].car.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, leftSideCars[i].speed / 100);
            }
        }
    }

    void GenerateCar(string side /*left or right*/)
    {
        GameObject car3dModel = (GameObject)Resources.Load(CAR_3DMODEL_NAME);
        int RandomSpeed = rnd.Next(RANDOM_SPEED_MIN, RANDOM_SPEED_MAX+1);
        
        if(side == "right")
        {
            GameObject carObject = Instantiate(car3dModel, new Vector3(rightSideCenterXPosition, this.transform.position.y + renderer.bounds.size.y, backZPosition), Quaternion.identity) as GameObject;
            carObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f); // そのままだと大きすぎるので小さくする
            carObject.AddComponent<Rigidbody>();
            carObject.GetComponent<Rigidbody>().useGravity = false;
            rightSideCars.Add((car: carObject, speed: RandomSpeed));
        }
        else if(side == "left")
        {
            GameObject carObject = Instantiate(car3dModel, new Vector3(leftSideCenterXPosition, this.transform.position.y + renderer.bounds.size.y, frontZPosition), Quaternion.identity) as GameObject;
            carObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f); // そのままだと大きすぎるので小さくする
            carObject.AddComponent<Rigidbody>();
            carObject.GetComponent<Rigidbody>().useGravity = false;
            leftSideCars.Add((car: carObject, speed: RandomSpeed));
        }
    }
}
