using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class TestPlayermanager : MonoBehaviour
    {
        [UnityTest]
        public IEnumerator Start_Test_Playermanager()
        {
            yield return new MonoBehaviourTest<TestPlayerMove>();
        }

        private class TestPlayerMove : PlayerManager, IMonoBehaviourTest
        {
            public bool IsTestFinished { get; private set; }
            private float waitTime = 1.0f;

            void Start()
            {
                isEditer_Test = true;
                gameObject.AddComponent<Rigidbody>();
                StartCoroutine(TestMove());
            }

            IEnumerator TestMove()
            {
                player_rigidbody = gameObject.GetComponent<Rigidbody>();
                player_rigidbody.useGravity = false;
                transform.position = Vector3.zero;
                transform.Rotate(0.0f, 0.0f, 0.0f);
                yield return null;

                Move_Forward();
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
