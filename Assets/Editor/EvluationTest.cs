using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class EvluationTest
{

    [Test]
    public void EvaluationDifficultyIsAllright()
    {
        EvaluationController evaluationController = new EvaluationController();
        evaluationController.SetAge(4);
        Assert.AreEqual(0, evaluationController.DifficultyLevel());
        evaluationController.SetAge(5);
        Assert.AreEqual(0, evaluationController.DifficultyLevel());
        evaluationController.SetAge(6);
        Assert.AreEqual(0, evaluationController.DifficultyLevel());
        evaluationController.SetAge(7);
        Assert.AreEqual(1, evaluationController.DifficultyLevel());
        evaluationController.SetAge(8);
        Assert.AreEqual(1, evaluationController.DifficultyLevel());
        evaluationController.SetAge(9);
        Assert.AreEqual(1, evaluationController.DifficultyLevel());
        evaluationController.SetAge(10);
        Assert.AreEqual(2, evaluationController.DifficultyLevel());
        evaluationController.SetAge(11);
        Assert.AreEqual(2, evaluationController.DifficultyLevel());
        evaluationController.SetAge(12);
        Assert.AreEqual(2, evaluationController.DifficultyLevel());
    }

    [Test]
    public void EvaluationPercentageAreOK()
    {
        EvaluationController evaluationController = new EvaluationController();
        Assert.AreEqual(50f, evaluationController.GetPercentage(6, 12));
        Assert.AreEqual(25f, evaluationController.GetPercentage(25, 100));
        Assert.AreEqual("33.33", evaluationController.GetPercentage(3, 9).ToString("0.00"));
        Assert.AreEqual("7.5", evaluationController.GetPercentage(75, 1000).ToString("0.0"));
    }
}
