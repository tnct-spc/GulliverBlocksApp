using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class InputManager : MonoBehaviour
{
    PlayerManager playermanager;
    GameObject player;

    GameManager gamemanager;
    GameObject gamesystem;
    BlockManager blockManager;

    public Toggle FlyingModeToggle;
    public GameObject FlyingButtons;
    public Toggle PlayButton;
    public GameObject PlayModeUI;
    public GameObject ResetButton;
    public Slider SeekBar;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playermanager = player.GetComponent<PlayerManager>();

        gamesystem = GameObject.Find("GameSystem");
        gamemanager = gamesystem.GetComponent<GameManager>();
        blockManager = gamesystem.GetComponent<BlockManager>();

        FlyingModeToggle.onValueChanged.AddListener(FlyingModeCheck);
        SeekBar.onValueChanged.AddListener(PlaceBlockBySeekBar);
        PlayButton.onValueChanged.AddListener(Play);

        bool isPlayMode = false;
        if (GameManager.Mode == "Vr") isPlayMode = true;
        FlyingButtons.SetActive(isPlayMode);
        FlyingModeToggle.GetComponent<Toggle>().isOn = isPlayMode;
        FlyingModeCheck(isPlayMode);
        PlayButton.GetComponent<Toggle>().isOn = false;
        PlayModeUI.SetActive(isPlayMode);
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

        SeekBar.maxValue = blockManager.BlockCount;
        SeekBar.value = blockManager.BlockNumber;
        if (SeekBar.value == SeekBar.maxValue) PlayButton.GetComponent<Toggle>().isOn = false;
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

    public void Play(bool isActive)
    {
        if (isActive)
        {
            blockManager.CheckHasEndedPlaceBlock();

            blockManager.isPlacingBlock = true;

            ResetButton.SetActive(false);
            
        }
        else
        {
            ResetButton.SetActive(true);
        }

        if (isActive == false) blockManager.isPlacingBlock = false;
    }

    public void DestroyBlocks()
    {
        PlayButton.GetComponent<Toggle>().isOn = false;
        blockManager.DestroyBlocks();
    }

    public void PlaceBlockBySeekBar(float value)
    {
        blockManager.PlaceBlockBySeekBar(value);
    }

}
