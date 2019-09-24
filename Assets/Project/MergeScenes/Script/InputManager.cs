using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MergeScene
{
    public class InputManager : MonoBehaviour
    {
        public GameObject SubmitPanel;
        public GameObject OpenSubmitPanelButton;
        public GameObject BackTitlePanel;
        public GameObject SceneBackButton;


        private void Start()
        {
        }

        public void OnClickOpenMergePanelButton()
        {
            this.OpenSubmitPanelButton.SetActive(false);
            this.SubmitPanel.SetActive(true);
        }

        public void OnClickBackButton()
        {
            SubmitPanel.SetActive(false);
            OpenSubmitPanelButton.SetActive(true);
        }

        public void OnClickSceneBackButton()
        {
            BackTitlePanel.SetActive(true);
            SceneBackButton.SetActive(false);
        }

        public void OnClickCancelButton()
        {
            BackTitlePanel.SetActive(false);
            SceneBackButton.SetActive(true);
        }

        public void OnClickAcceptButton()
        {
            SceneManager.LoadScene("Title");
        }
    }
}
