using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HighScoreInput : MonoBehaviour {

    TextMeshProUGUI[] texts;
    GameObject[] objects;
    int selectedLetter;
    private Vector2 inputs;
    private float timer;

    // Start is called before the first frame update
    void Start() {
        selectedLetter = 0;
        objects = new GameObject[3];
        texts = new TextMeshProUGUI[3];
        objects[0] = GameObject.Find("FirstLetter");
        texts[0] = objects[0].GetComponent<TextMeshProUGUI>();
        objects[1] = GameObject.Find("SecondLetter");
        texts[1] = objects[1].GetComponent<TextMeshProUGUI>();
        objects[2] = GameObject.Find("ThirdLetter");
        texts[2] = objects[2].GetComponent<TextMeshProUGUI>();
        placeSelector();

    }

    void placeSelector() {
        for (int i = 0; i < 3; i++) {
            if ((texts[i].fontStyle & FontStyles.Italic) != 0) {
                texts[i].fontStyle ^= FontStyles.Italic;
            }
        }
        texts[selectedLetter].fontStyle = FontStyles.Italic;
    }

    void getPreviousLetter() {
        selectedLetter -= 1;
        if (selectedLetter < 0) {
            selectedLetter = 2;
        }
        placeSelector();
    }
    void getNextLetter() {
        selectedLetter += 1;
        if (selectedLetter > 2) {
            selectedLetter = 0;
        }
        placeSelector();
    }

    string getPreviousValue() {
        string letterString = texts[selectedLetter].text;
        char letterChar = (char)letterString[0];
        if (letterChar == 'A') {
            letterChar = 'Z';
        } else {
            letterChar = (char)(((int)letterChar) - 1);
        }
        return letterChar.ToString();
    }

    string getNextValue() {
        string letterString = texts[selectedLetter].text;
        char letterChar = (char)letterString[0];
        if (letterChar == 'Z') {
            letterChar = 'A';
        } else {
            letterChar = (char)(((int)letterChar) + 1);
        }
        return letterChar.ToString();
    }

    string getString() {
        return texts[0].text + texts[1].text + texts[2].text;
    }

    private void Update() {
        if (timer > 0f) {
            timer -= Time.deltaTime;
        }
    }

    public void OnMove(InputAction.CallbackContext ctx) {
        if (timer <= 0f) {
            timer = 0.1f;
            Vector2 navigation = ctx.ReadValue<Vector2>();
            if (navigation.x > navigation.y && navigation.x > 0f) {
                getNextLetter();
            }
            if (navigation.y > navigation.x && navigation.y > 0f) {
                texts[selectedLetter].text = getPreviousValue();
            }
            if (navigation.x > navigation.y && navigation.y < 0f) {
                texts[selectedLetter].text = getNextValue();
            }
            if (navigation.y > navigation.x && navigation.x < 0f) {
                getPreviousLetter();
            }
        }


    }

    public void OnSelect(InputAction.CallbackContext ctx) {
        if (timer <= 0f && ctx.performed) {
            timer = 0.5f;
            SaveData.getInstance().lastEnteredSurname = getString();
            SaveData.getInstance().lastScore = (long) Random.Range(10,5000);
            SceneManager.LoadScene("LeaderBoard");
        }

    }
}