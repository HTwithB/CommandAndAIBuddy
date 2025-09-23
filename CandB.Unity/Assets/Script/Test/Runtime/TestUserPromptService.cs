using CandB.Script.Core;
using NUnit.Framework;
using UnityEngine;

namespace Script.Test.Runtime
{
    public class TestUserPromptService
    {
        [Test]
        public void TestParseMoveInstructionJson()
        {
            // ARRANGE
            var json = @"
{
    ""instruction"": [
    ]
}
";
            // ACT
            var result = JsonUtility.FromJson<MovePlan>(json);
            // ASSERT
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.instructions);
            Assert.AreEqual(0, result.instructions.Count);
        }
    }
}