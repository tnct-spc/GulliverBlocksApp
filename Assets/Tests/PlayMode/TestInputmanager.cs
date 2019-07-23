using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class TestInputmanager : MonoBehaviour
    {
        [UnityTest]
        public IEnumerator Start_Test_Inputmanager()
        {
            yield return new MonoBehaviourTest<TestPlayerMove_On_Inputmanager>();
        }

        private class TestPlayerMove_On_Inputmanager : InputManager, IMonoBehaviourTest
        {
            public bool IsTestFinished { get; private set; }
            private float waitTime = 1.0f;

            void Start()
            {
                player = gameObject;
                player.AddComponent<Rigidbody>();
                StartCoroutine(TestMove());
            }

            IEnumerator TestMove()
            {
                playermanager = player.AddComponent<PlayerManager>();
                Player_Change_isEditer_Test();
                playermanager.player_rigidbody = GetComponent<Rigidbody>();
                Player_Change_Rigidbody_useGravity();
                transform.position = Vector3.zero;
                transform.Rotate(0.0f, 0.0f, 0.0f);
                yield return null;

                Player_Move();
                yield return new WaitForSeconds(waitTime);

                Assert.AreEqual(transform.position.x, 0);
                Assert.AreEqual(transform.position.y, 0);
                Assert.AreNotEqual(0, transform.position.z);

                gameObject.SetActive(false);
                IsTestFinished = true;
            }
        }
    }
}
