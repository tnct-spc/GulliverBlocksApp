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

    public void Player_Forward()
    {
        playermanager.Move_Forward();
    }

    public void Player_Right()
    {
        playermanager.Move_Right();
    }

    public void Player_Left()
    {
        playermanager.Move_Left();
    }

    public void Player_Back()
    {
        playermanager.Move_Back();
    }

    public void Player_Stop()
    {
        playermanager.StopMove();
    }
}
