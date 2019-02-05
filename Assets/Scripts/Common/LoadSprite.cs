﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSprite
{
    public static Sprite GetSpriteFromSpriteSheet(string spritesheetRoute, string spriteName)
    {
        Debug.Log($"The route of the sprite is {spritesheetRoute} and the sprite to get is {spriteName}");

        var sprites = Resources.LoadAll<Sprite>(spritesheetRoute);

        var spriteDicctionary = new Dictionary<string, Sprite>();

        foreach (Sprite sprite in sprites)
        {
            spriteDicctionary.Add(sprite.name, sprite);
        }

        return spriteDicctionary[spriteName];
    }
}
