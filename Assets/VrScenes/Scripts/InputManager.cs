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
        if (Input.GetKey("w")) playermanager.Move_Forward();
        else if (Input.GetKeyUp("w")) playermanager.StopMove();

        else if (Input.GetKey("d")) playermanager.Move_Right();
        else if (Input.GetKeyUp("d")) playermanager.StopMove();

        else if (Input.GetKey("a")) playermanager.Move_Left();
        else if (Input.GetKeyUp("a")) playermanager.StopMove();

        else if (Input.GetKey("s")) playermanager.Move_Back();
        else if (Input.GetKeyUp("s")) playermanager.StopMove();

        if (Input.GetKeyDown(KeyCode.Escape)) gamemanager.Back_To_Title_If_Android();

    }

   public void FlyingModeCheck(bool isActive)
    {
        playermanager.Flying(isActive);
        FlyingButtons.SetActive(isActive);
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
    public void Player_Up()
    {
        playermanager.Move_Up();
    }

    public void Player_Down()
    {
        playermanager.Move_Down();
    }

}
