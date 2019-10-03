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

        public void OnClickOpenMergePanelButton()
        {
            this.OpenSubmitPanelButton.SetActive(false);
            this.SubmitPanel.SetActive(true);
        }

        public void OnClickCancelSubmitButton()
        {
            SubmitPanel.SetActive(false);
            OpenSubmitPanelButton.SetActive(true);
        }

        public void OnClickSceneBackButton()
        {
            BackTitlePanel.SetActive(true);
            SceneBackButton.SetActive(false);
        }

        public void OnClickCancelSceneBackButton()
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
