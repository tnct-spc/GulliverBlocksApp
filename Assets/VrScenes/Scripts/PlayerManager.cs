using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    const string Up = "Up";
    const string Down = "Down";
    private string Move_State = Stop;
    public Toggle toggle;
    public GameObject FlyingButtons;

    void Start()
    {
        player_rigidbody = GetComponent<Rigidbody>();
        Input.gyro.enabled = true;
        toggle.GetComponent<Toggle>().isOn = false;
    }

    void Update ()
    {
        Rotate();
        Move();

        if (toggle.GetComponent<Toggle>().isOn)
        {
            FlyingButtons.SetActive(true);
            player_rigidbody.useGravity = false;
        }
        else
        {
            FlyingButtons.SetActive(false);
            player_rigidbody.useGravity = true;
        }
    }

    public void Move()
    {
        switch (Move_State) {
            case Stop:
                Add_Velocity(Vector3.zero);
                break;

            case Forward:
                Add_Velocity(gameObject.transform.forward);
                break;
            
            case Right:
                Add_Velocity(gameObject.transform.right);
                break;
            
            case Left:
                Add_Velocity(gameObject.transform.right * -1);
                break;

            case Back:
                Add_Velocity(gameObject.transform.forward * -1);
                break;

            case Up:
                Add_Velocity(gameObject.transform.up);
                break;

            case Down:
                Add_Velocity(gameObject.transform.up * -1);
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

    public void Move_Up()
    {
        Move_State = Up;
    }

    public void Move_Down()
    {
        Move_State = Down;
    }

    public void Add_Velocity(Vector3 move_direction)
    {
        switch (Move_State) {
            case Up:
                move_direction.x = 0;
                move_direction.y = 1;
                move_direction.z = 0;
                break;
            case Down:
                move_direction.x = 0;
                move_direction.y = -1;
                move_direction.z = 0;
                break;

            default:
        move_direction.y = 0;
        move_direction = move_direction.normalized;
                break;
    }
        if(isDefault_speed) player_rigidbody.velocity = default_move_speed * move_direction;
        else player_rigidbody.velocity = run_move_speed * move_direction;
    }

    public void Rotate()
    {
        transform.rotation = Quaternion.AngleAxis(90.0f, Vector3.right) * Input.gyro.attitude * Quaternion.AngleAxis(180.0f, Vector3.forward);
    }
}
