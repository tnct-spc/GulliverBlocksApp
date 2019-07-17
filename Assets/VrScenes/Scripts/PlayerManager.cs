﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] int default_move_speed = 1;
    [SerializeField] int run_move_speed = 2;
    Rigidbody player_rigidbody;
    private bool isDefault_speed = true;
    const string Stop = "Stop";
    const string Forward = "Forward";
    const string Right = "Right";
    const string Left = "Left";
    const string Back = "Back";
    private string Move_State = Stop;

    void Start()
    {
        player_rigidbody = GetComponent<Rigidbody>();
        Input.gyro.enabled = true;
    }

    void Update ()
    {
        Rotate();
        Move();
    }

    public void Move()
    {
        switch (Move_State) {
            //Stop
            case Stop:
                Add_Velocity(Vector3.zero);
                break;
            //Forward
            case Forward:
                Add_Velocity(gameObject.transform.forward);
                break;
            //Right
            case Right:
                Add_Velocity(gameObject.transform.right);
                break;
            //Left
            case Left:
                Add_Velocity(gameObject.transform.right * -1);
                break;
            //Back
            case Back:
                Add_Velocity(gameObject.transform.forward * -1);
                break;

        }
    }

    public void Move_Forward()
    {
        Move_State = Forward;
    }

    public void Move_Right()
    {
        Move_State = Right;
    }

    public void Move_Left()
    {
        Move_State = Left;
    }

    public void Move_Back()
    {
        Move_State = Back;
    }

    public void StopMove()
    {
        Move_State = Stop;
    }

    public void Add_Velocity(Vector3 move_direction)
    {
        if(isDefault_speed) player_rigidbody.velocity = default_move_speed * move_direction;
        else player_rigidbody.velocity = run_move_speed * move_direction;
    }

    public void Rotate()
    {
        transform.rotation = Quaternion.AngleAxis(90.0f, Vector3.right) * Input.gyro.attitude * Quaternion.AngleAxis(180.0f, Vector3.forward);
    }
}
