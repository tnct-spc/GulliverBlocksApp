using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SettingScene
{
    public class Tentap : MonoBehaviour
    {
        int TapCount = 0;
        public GameObject ShowObject;

        private void Update()
        {
            if (TapCount == 10)
            {
                ShowObject.SetActive(true);
                TapCount = 0;
            }
        }

        public void Tapped()
        {
            TapCount++;
            StartCoroutine("waittime");
        }

        IEnumerator waittime()
        {
            int NowTapCount = TapCount;
            yield return new WaitForSeconds(1);
            if (NowTapCount == TapCount) TapCount = 0;
        }
    }
}
