using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace TestEditMode
{
    public class TestEditMode
    {
        [Test]
        public void CheckAudioListener()
        {
            Assert.IsTrue(Camera.main.GetComponent<AudioListener>());
        }
    }
}
