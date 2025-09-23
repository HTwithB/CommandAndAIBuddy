using System.Collections;
using CandB.Script.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace CandB.Script.Test.Runtime
{
    public class StageServiceTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void LoadMapTest()
        {
            // ARRANGE
            var sut = CreateSut();
            
            // ACT
            // sut.LoadMap();
            
            // ASSERT
            
        }
        
        private StageService CreateSut()
        {
            var sut = new StageService();
            sut.Setup();
            return sut;
        }
    }
    
}
