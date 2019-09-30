using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace VrScene
{
    public class InputManager : MonoBehaviour
    {
        PlayerManager playermanager;
        GameManager gamemanager;
        BlockManager BlockManager;

        List<GameObject> fadeOutObjects = new List<GameObject>();

        public GameObject player;
        public GameObject gamesystem;

        int TouchCount = 0;

        // NonTwoEyesModeUIとその子要素
        public GameObject NonTwoEyesModeUI;
        public Toggle FlyingModeToggle;
        public GameObject FlyingButtons;
        public Toggle PlayBackButton;
        public GameObject PlayBackModeUI;
        public GameObject Seekbar;
        public Slider seekbarSlider;
        public GameObject TouchPanel;

        // DebugButton関連
        public GameObject BackToTheGame;
        public GameObject RuntimeHierarchy;
        public GameObject RuntimeInspector;

        // GeneralMenu関連
        public GameObject GeneralMenuButton;
        public GameObject GeneralMenuPanel;

        void Start()
        {
            playermanager = player.GetComponent<PlayerManager>();
            gamemanager = gamesystem.GetComponent<GameManager>();
            BlockManager = gamesystem.GetComponent<BlockManager>();

            seekbarSlider = Seekbar.GetComponent<Slider>();
            FlyingModeToggle.onValueChanged.AddListener(FlyingModeCheck);
            seekbarSlider.onValueChanged.AddListener(PlaceBlockBySeekBar);
            PlayBackButton.onValueChanged.AddListener(PlayBack);

            bool isPlayBackMode = false;
            if (GameManager.Mode == "PlayBack") isPlayBackMode = true;
            FlyingButtons.SetActive(isPlayBackMode);
            FlyingModeToggle.GetComponent<Toggle>().isOn = isPlayBackMode;
            FlyingModeCheck(isPlayBackMode);
            PlayBackButton.GetComponent<Toggle>().isOn = false;
            PlayBackModeUI.SetActive(false);
            seekbarSlider.maxValue = 100;
            InputTracking.disablePositionalTracking = true;
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

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (XRSettings.enabled == true)
                {
                    XRSettings.enabled = false;
                }

                else
                {
                    gamemanager.Back_To_Title_If_Android();
                }
            }

            NonTwoEyesModeUI.SetActive(!XRSettings.enabled);
            TouchPanel.SetActive(XRSettings.enabled);
        }

        private List<GameObject> FocusUI(GameObject parent, GameObject focusObject)
        {
            List<GameObject> fadeOutObjects = new List<GameObject>();
            foreach(Transform child in parent.transform)
            {
                if (child.name == focusObject.name) continue;
                fadeOutObjects.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }

            return fadeOutObjects;
        }

        private void ResetFocusUI(List<GameObject> fadeOutObjects)
        {
            fadeOutObjects.ForEach(obj => obj.SetActive(true));
        }

        // 再生モード関連
        public void PlayBack(bool isActive)
        {
            seekbarSlider.maxValue = BlockManager.BlocksCount;
            if (isActive)
            {
                if (seekbarSlider.value == seekbarSlider.maxValue) seekbarSlider.value = 0;
                if (BlockManager.isRepeating == false) BlockManager.RepeatPlaceBlocks();
                fadeOutObjects.Clear();
                fadeOutObjects.AddRange(FocusUI(NonTwoEyesModeUI, PlayBackModeUI));
            }
            else
            {
                ResetFocusUI(fadeOutObjects);
                fadeOutObjects.Clear();
                Seekbar.SetActive(false);
            }
        }

        public void PlaceBlockBySeekBar(float value)
        {
            seekbarSlider.maxValue = BlockManager.BlocksCount;
            BlockManager.PlaceBlocks(value);
        }

        public void OnClickAdvanceSkipButton()
        {
            seekbarSlider.value++;
        }

        public void OnClickBackSkipButton()
        {
            seekbarSlider.value--;
        }

        // GeneralMenu
        public void OnClicKGeneralMenuButton()
        {
            GeneralMenuPanel.SetActive(true);
            GeneralMenuButton.SetActive(false);
        }

        public void OnClickGeneralMenuCancelButton()
        {
            GeneralMenuButton.SetActive(true);
            GeneralMenuPanel.SetActive(false);
        }

        // VRのtoggle
        public void VR_ModeOn()
        {
            XRSettings.enabled = true;
        }

        public void VR_ModeOff()
        {
            XRSettings.enabled = false;
        }

        // DebugButton
        public void OnClickBackToTheGame()
        {
            this.BackToTheGame.SetActive(false);
            this.RuntimeHierarchy.SetActive(false);
            this.RuntimeInspector.SetActive(false);
            Debug.Log("Back to Game");
        }

        // BackTitleButton
        public void OnClickBackTitleButton()
        {
            SceneManager.LoadScene("Title");
        }

        //ダッシュに関する処理
        public void Touch()
        {
            TouchCount++;
            DashCheck();
            Player_Forward();
        }

        public void ReleaseTheTouch()
        {
            Player_StopForward();
            playermanager.isDefault_speed = true;
        }

        public async void DashCheck()
        {
            if (TouchCount > 1)
            {
                playermanager.isDefault_speed = false;
            }
            await Task.Delay(500);
            TouchCount = 0;
        }

        /*
         * ここから下の関数は、Player関連
         */

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
}