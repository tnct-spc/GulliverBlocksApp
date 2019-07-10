using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] int default_move_speed = 1;
    [SerializeField] int dash_move_speed = 2;
    Rigidbody player_rigidbody;

    void Start()
    {
        player_rigidbody = GetComponent<Rigidbody>();

    }

    public void Move(Vector3 move_direction)
    {
        player_rigidbody.velocity = default_move_speed * move_direction;
    }

    public void StopMove()
    {
        player_rigidbody.velocity = new Vector3(0, 0, 0);
    }

    public void Rotate()
    {

    }
}
