using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class StoreTest
{
    [Test]
    public void CanBuyAnItem()
    {
        IslandShoppingManager shoppingManager = new IslandShoppingManager();
        Assert.IsTrue(shoppingManager.HasEnoughMoneyToBuy(12, 12));
        Assert.IsTrue(shoppingManager.HasEnoughMoneyToBuy(15, 20));
        Assert.IsFalse(shoppingManager.HasEnoughMoneyToBuy(12, 11));
    }
}
