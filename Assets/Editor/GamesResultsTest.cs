using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class GamesResultsTest
{
    [Test]
    public void CanPassRiver()
    {
        MagicRiverManager river = new MagicRiverManager();
        //Here we check if a player can move level if has 0 errors and 0 target errors
        Assert.IsTrue(river.CanPass(0, 0));
        //Here we check if a player can move level if has 1 errors and 0 target errors
        Assert.IsTrue(river.CanPass(1, 0));
        //Here we check if a player can move level if has 2 errors and 0 target errors
        Assert.IsFalse(river.CanPass(2, 0));
        //Here we check if a player can move level if has 1 errors and 1 target errors
        Assert.IsFalse(river.CanPass(1, 1));
        //Here we check if a player can move level if has 2 errors and 1 target errors
        Assert.IsFalse(river.CanPass(2, 1));
    }

    [Test]
    public void IsGoodEnoughTheDraw()
    {
        SandDrawingController sand = new SandDrawingController();
        Assert.IsTrue(sand.IsWellMade(60, 70));
        Assert.IsFalse(sand.IsWellMade(100, 71));
        Assert.IsTrue(sand.IsWellMade(80, 10));
        Assert.IsFalse(sand.IsWellMade(50, 10));
    }
}   
