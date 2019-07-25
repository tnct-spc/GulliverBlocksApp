using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    PlayerManager playermanager;
    GameObject player;

    GameManager gamemanager;
    GameObject gamesystem;

    public Toggle toggle;
    public GameObject FlyingButtons;

    bool forward, back, right, left, up, down;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playermanager = player.GetComponent<PlayerManager>();

        gamesystem = GameObject.Find("GameSystem");
        gamemanager = gamesystem.GetComponent<GameManager>();

        toggle.onValueChanged.AddListener(FlyingModeCheck);

        FlyingButtons.SetActive(false);
        toggle.GetComponent<Toggle>().isOn = false;
        
    }

    private void Update()
    {
        if (Input.GetKey("w")) Player_Forward();
        else if (Input.GetKeyUp("w")) Player_StopForward();

        else if (Input.GetKey("d")) Player_Right();
        else if (Input.GetKeyUp("d")) Player_StopRight();

        else if (Input.GetKey("a")) Player_Left();
        else if (Input.GetKeyUp("a")) Player_StopLeft();

        else if (Input.GetKey("s")) Player_Back();
        else if (Input.GetKeyUp("s")) Player_StopBack();

        if (Input.GetKeyDown(KeyCode.Escape)) gamemanager.Back_To_Title_If_Android();

        Player_MoveDirectionCheck();

    }

    public void FlyingModeCheck(bool isActive)
    {
        playermanager.Flying(isActive);
        FlyingButtons.SetActive(isActive);
    }

    void Player_MoveDirectionCheck()
    {
        Vector3 moveDirection = Vector3.zero;
        if (forward) moveDirection.z = 1;
        if (back) moveDirection.z = -1;
        if (right) moveDirection.x = 1;
        if (left) moveDirection.x = -1;
        if (up) moveDirection.y = 1;
        if (down) moveDirection.y = -1;

        playermanager.Add_Velocity(moveDirection);
    }
    public void Player_Forward()
    {
        forward = true;
        playermanager.MoveZ = true;
    }

    public void Player_Right()
    {
        right = true;
        playermanager.MoveX = true;
    }

    public void Player_Left()
    {
        left = true;
        playermanager.MoveX = true;
    }

    public void Player_Back()
    {
        back = true;
        playermanager.MoveZ = true;
    }
    public void Player_Up()
    {
        up = true;
        playermanager.MoveY = true;
    }

    public void Player_Down()
    {
        down = true;
        playermanager.MoveY = true;
    }

    public void Player_StopForward()
    {
        forward = false;
        playermanager.MoveZ = false;
    }

    public void Player_StopBack()
    {
        back = false;
        playermanager.MoveZ = false;
    }

    public void Player_StopRight()
    {
        right = false;
        playermanager.MoveX = false;
    }

    public void Player_StopLeft()
    {
        left = false;
        playermanager.MoveX = false;
    }

    public void Player_StopUp()
    {
        up = false;
        playermanager.MoveY = false;
    }

    public void Player_StopDown()
    {
        down = false;
        playermanager.MoveY = false;
    }

}
