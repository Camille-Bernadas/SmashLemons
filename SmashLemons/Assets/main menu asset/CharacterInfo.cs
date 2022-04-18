using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour
{
    public Image original;
    public int posX;
    public int posY;
    public int characterId;

    public Sprite getSprite()
    {
        return original.sprite;
    }
}
