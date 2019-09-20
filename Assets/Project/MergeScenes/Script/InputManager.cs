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


        private void Start()
        {
        }

        public void OnClickOpenMergePanelButton()
        {
            this.OpenSubmitPanelButton.SetActive(false);
            this.SubmitPanel.SetActive(true);
        }

        public void OnClickBackButton(GameObject currentUI)
        {
            currentUI.SetActive(false);
            OpenSubmitPanelButton.SetActive(true);
        }
    }
}
