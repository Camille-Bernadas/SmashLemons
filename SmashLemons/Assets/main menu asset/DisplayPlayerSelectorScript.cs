using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DisplayPlayerSelectorScript : MonoBehaviour
{
    public Image Original;
    public SelectScreenManager manager;

    public int posX;
    public int posY;

    public int playerID;

    public float timerToReset = 0f;

    void start()
    {
        PlayerInput.all[0].SwitchCurrentControlScheme("PlayerOne", Keyboard.current);
    }

    private void Update() {
        if (timerToReset > 0f)
        {
            timerToReset -= Time.deltaTime;
        }
    }

    public void onMove(InputAction.CallbackContext ctx)
    {
        if (timerToReset <= 0f)
        {
            timerToReset  = 0.1f;
            Vector2 navigation = ctx.ReadValue<Vector2>();
            if (navigation.x > navigation.y && navigation.x > 0f)
            {
                manager.onMove(playerID, 1, 0);
            }
            if (navigation.y > navigation.x && navigation.y > 0f)
            {
                manager.onMove(playerID, 0, 1);
            }
            if (navigation.x > navigation.y && navigation.y < 0f)
            {
                manager.onMove(playerID, 0, -1);
            }
            if (navigation.y > navigation.x && navigation.x < 0f)
            {
                manager.onMove(playerID, -1, 0);
            }
        }
    }

    public void Validate()
    {
        if (timerToReset <= 0f)
        {
            timerToReset  = 0.3f;
            manager.Validate(playerID);
        }
    }

    public void modifyImage(int characterId, Sprite replacement)
    {
        Original.sprite = replacement;
    }
}
