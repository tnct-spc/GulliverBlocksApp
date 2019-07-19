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

            void Start()
            {
                isEditer_Test = true;
                StartCoroutine(TestMove());
            }

            IEnumerator TestMove()
            {
                player_rigidbody = gameObject.AddComponent<Rigidbody>();
                player_rigidbody.useGravity = false;
                transform.position = Vector3.zero;
                transform.Rotate(0.0f, 0.0f, 0.0f);
                yield return null;

                Move_State = Forward;
                yield return new WaitForSeconds(1);

                Assert.AreEqual(transform.position.x, 0);
                Assert.AreEqual(transform.position.y, 0);
                Assert.AreNotEqual(0, transform.position.z);

                gameObject.SetActive(false);
                IsTestFinished = true;
            }
        }
    }
}
