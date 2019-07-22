using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    [SerializeField] int default_move_speed = 1;
    [SerializeField] int run_move_speed = 2;
    public Rigidbody player_rigidbody;
    public bool isDefault_speed = true;
    public readonly static string Stop = "Stop";
    public readonly static string Forward = "Forward";
    public readonly static string Right = "Right";
    public readonly static string Left = "Left";
    public string Move_State = Stop;
    public bool isEditer_Test = false;

    void Awake()
    {
        player_rigidbody = gameObject.AddComponent<Rigidbody>();
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
            case "Stop":
                Add_Velocity(Vector3.zero);
                break;
            case "Forward":
                Add_Velocity(gameObject.transform.forward);
                break;
        }
    }

    public void Move_Forward()
    {
        Move_State = Forward;
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
        if(!isEditer_Test) transform.rotation = Quaternion.AngleAxis(90.0f, Vector3.right) * Input.gyro.attitude * Quaternion.AngleAxis(180.0f, Vector3.forward);
    }
}
