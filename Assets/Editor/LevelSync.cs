using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class LevelSync
    {
        // A Test behaves as an ordinary method
        [Test]
        public void LevelSyncSimplePasses()
        {
            LevelSaver saver = new LevelSaver();
            saver.CreateSaveBlock();
            saver.SetGameData();
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator LevelSyncWithEnumeratorPasses()
        {
            LevelSaver saver = new LevelSaver();
            saver.CreateSaveBlock();
            saver.SetGameData();         
            yield return saver.FormToReturn();
        }
    }
}
