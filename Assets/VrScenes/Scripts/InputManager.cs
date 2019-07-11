using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerManager playermanager;
    GameObject player;

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
}
