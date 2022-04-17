using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPlayerSelectorScript : MonoBehaviour
{
    public Image Original;
    public Sprite Catherine;
    public Sprite Melog;
    public Sprite Kevin;
    public Sprite Balibump;
    public Sprite Random;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    void modifyImage(int characterId)
    {
        switch (characterId)
        {
            case 1:
                Original.sprite = Catherine;
                break;

            case 2:
                Original.sprite = Melog;
                break;

            case 3:
                Original.sprite = Kevin;
                break;

            case 4:
                Original.sprite = Balibump;
                break;

            default:
                Original.sprite = Random;
                break;
        }
    }
}
