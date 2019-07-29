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

        bool modechecker=false;
        if (GameManager.Mode == "Play") modechecker = true;
        FlyingButtons.SetActive(modechecker);
        toggle.GetComponent<Toggle>().isOn = modechecker;
        FlyingModeCheck(modechecker);
        
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
    }

    public void FlyingModeCheck(bool isActive)
    {
        playermanager.Flying(isActive);
        FlyingButtons.SetActive(isActive);
    }


    public void Player_Forward()
    {
        playermanager.MoveForward = true;
    }

    public void Player_Right()
    {
        playermanager.MoveRight = true;
    }

    public void Player_Left()
    {
        playermanager.MoveLeft = true;
    }

    public void Player_Back()
    {
        playermanager.MoveBack = true;
    }
    public void Player_Up()
    {
        playermanager.MoveUp = true;
    }

    public void Player_Down()
    {
        playermanager.MoveDown = true;
    }

    public void Player_StopForward()
    {
        playermanager.MoveForward = false;
    }

    public void Player_StopBack()
    {
        playermanager.MoveBack = false;
    }

    public void Player_StopRight()
    {
        playermanager.MoveRight = false;
    }

    public void Player_StopLeft()
    {
        playermanager.MoveLeft = false;
    }

    public void Player_StopUp()
    {
        playermanager.MoveUp = false;
    }

    public void Player_StopDown()
    {
        playermanager.MoveDown = false;
    }

}
