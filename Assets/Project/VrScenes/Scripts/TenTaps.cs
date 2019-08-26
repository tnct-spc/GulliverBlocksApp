using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

namespace VrScene
{

    public class TenTaps : MonoBehaviour
    {
        public GameObject BackToTheGame;
        public GameObject RuntimeHierarchy;
        public GameObject RuntimeInspector;

        int TapCount = 0;


        private void Update()
        {
            if (TapCount == 10)
            {
                this.BackToTheGame.SetActive(true);
                this.RuntimeInspector.SetActive(true);
                this.RuntimeHierarchy.SetActive(true);
                TapCount = 0;
            }
        }

        public void Tapped()
        {
            TapCount++;
        }
    }
}
