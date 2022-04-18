using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPlayerSelectorScript : MonoBehaviour
{
    public Image Original;

    public int posX;
    public int posY;
    public int playerID;
    public bool isActive;
    public bool isPlayer;

    void Activated()
    {
        if(isActive)
        {
            if(isPlayer)
            {
                isPlayer = false;
            }
            else
            {
                isActive = false;
            }
        }
        else
        {
            isActive = true;
            isPlayer = true;
        }
    }

    public void modifyImage(int characterId, Sprite replacement)
    {
        Original.sprite = replacement;
    }
}
