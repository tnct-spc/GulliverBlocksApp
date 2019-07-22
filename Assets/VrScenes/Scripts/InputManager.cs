using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public PlayerManager playermanager;
    public GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playermanager = player.GetComponent<PlayerManager>();
    }

    public void Player_Move()
    {
        playermanager.Move_Forward();
    }

    public void Player_Stop()
    {
        playermanager.StopMove();
    }

    public void Player_Change_speed()
    {
        playermanager.isDefault_speed = !playermanager.isDefault_speed;
    }

    public void Player_Change_Rigidbody_useGravity()
    {
        playermanager.player_rigidbody.useGravity = !playermanager.player_rigidbody.useGravity;
    }

    public void Player_Change_isEditer_Test()
    {
        playermanager.isEditer_Test = !playermanager.isEditer_Test;
    }
}
