using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject prefabPlayerData, prefabCaterine, prefabKevin, prefabBalibump, prefabMelog, prefabAI;
    [SerializeField]
    private Sprite chat, kevin, balibump, golem, aiSprite;

    [SerializeField]
    private InputActionAsset controls;


    private static readonly Color[] colors = {  new Color(1f, 0f, 0f, 0.2f), new Color(0f, 0f, 1f, 0.2f),
                                                new Color(1f, 1f, 0f, 0.2f), new Color(0f, 1f, 0f, 0.2f) };

    Camera camera;

    TextMeshProUGUI timer;

    Transform playerData;

    Canvas canvas, gameOverCanvas;
    public int amountPlayers = 4;
    public int amountAI = 2;
    GameObject[] playerDatas = new GameObject[4];
    Character[] players = new Character[4];
    float time = 0f;


    public bool togglePause = false;
    private string pauseStarter;


    private void Awake(){
        camera = FindObjectOfType<Camera>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        gameOverCanvas = GameObject.Find("GameOverCanvas").GetComponent<Canvas>();
        gameOverCanvas.gameObject.SetActive(false);
        timer = canvas.gameObject.transform.Find("Timer").GetComponent<TextMeshProUGUI>();
        playerData = canvas.gameObject.transform.Find("PlayerData");

    }

    private void Start() {
        amountPlayers = 0;
        amountAI = 0;
        bool[] actives = new bool[4];
        bool[] controlled = new bool[4];
        int[] characters = new int[4];
        
        for(int i = 0; i<4; i++){
            actives[i] = SaveData.getInstance().playersLoaded[i];
            controlled[i] = SaveData.getInstance().playerNotAI[i];
            if(actives[i]){
                if(controlled[i]){
                    amountPlayers++;
                }
                else{
                    amountAI++;
                }
            }
            characters[i] = SaveData.getInstance().charactersID[i];

        }
        for(int i = 0; i<4; i++){
            SaveData.getInstance().playersLoaded[i] = false;
            SaveData.getInstance().playerNotAI[i] = false;
            SaveData.getInstance().charactersID[i] = -1;
            
        }




        for (int i = 0; i < amountPlayers; i++) {
            string controlScheme = "PlayerOne";
            switch (i) {
                case 0:
                    break;
                case 1:
                    controlScheme = "PlayerTwo";
                    break;
                case 2:
                    controlScheme = "PlayerThree";
                    break;
                case 3:
                    controlScheme = "PlayerFour";
                    break;
                default:
                    break;
            }

            GameObject character;
            GameObject usedPrefab = prefabCaterine;

            switch (characters[i]) {
                case 0:
                    usedPrefab = prefabCaterine;
                    break;
                case 1:
                    usedPrefab = prefabCaterine;
                    break;
                case 2:
                    usedPrefab = prefabMelog;
                    break;
                case 3:
                    usedPrefab = prefabKevin;
                    break;
                case 4:
                    usedPrefab = prefabBalibump;
                    break;
                default:
                    break;
            }
        
            character = PlayerInput.Instantiate(usedPrefab, 0, controlScheme).gameObject;
            
            
            character.name = "Player" + (i + 1);
            players[i] = character.transform.GetComponent<Character>();
            players[i].Spawn();

            playerDatas[i] = Instantiate(prefabPlayerData, playerData);
            Outline outline = character.AddComponent<Outline>();

            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineColor = colors[i];
            outline.OutlineWidth = 1f;
            Image currentImage = playerDatas[i].transform.Find("Image").GetComponent<Image>();
            currentImage.sprite = chat;
            switch (characters[i]) {
                case 0:
                    currentImage.sprite = chat;
                    break;
                case 1:
                    currentImage.sprite = chat;
                    break;
                case 2:
                    currentImage.sprite = golem;
                    break;
                case 3:
                    currentImage.sprite = kevin;
                    break;
                case 4:
                    currentImage.sprite = balibump;
                    break;
                default:
                    break;
            }

            currentImage.color = new Color(1f, 1f, 1f, 0.95f);
            playerDatas[i].transform.GetComponent<Image>().color = colors[i];
        }
        for (int i = 0; i < amountAI; i++) {
            
            GameObject bot = Instantiate(prefabAI);
            bot.name = "BOT" + (i + 1);
            players[amountPlayers+i] = bot.transform.GetComponent<Character>();
            players[amountPlayers+i].Spawn();

            playerDatas[amountPlayers+i] = Instantiate(prefabPlayerData, playerData);
            Outline outline = bot.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineColor = new Color(0.1f, 0.1f, 0.1f);
            outline.OutlineWidth = 1f;
            Image currentImage = playerDatas[amountPlayers+i].transform.Find("Image").GetComponent<Image>();
            currentImage.sprite = aiSprite;
            currentImage.color = new Color(1f, 1f, 1f, 0.95f);
            playerDatas[amountPlayers+i].transform.GetComponent<Image>().color = colors[amountPlayers+i];
        }
        PlayerInput.all[0].SwitchCurrentControlScheme("PlayerOne", Keyboard.current);
        if(amountPlayers>1){
            PlayerInput.all[1].SwitchCurrentControlScheme("PlayerTwo", Keyboard.current);
        }
        camera.GetComponent<FollowingTargetsCamera>().SetUpList();
    }

    private void Update() {
        time += Time.deltaTime;
        timer.text = time.ToString("#.") + "s";
        UpdateHealth();
        UpdateLives();
        UpdateDead();
    }

    private void UpdateHealth() {
        for (int i = 0; i < amountPlayers+amountAI; i++) {
            float damage = players[i].getTakenDamage();
            playerDatas[i].transform.GetComponent<PlayerUIManager>().setDamage(damage);
        }
    }

    private void UpdateLives() {
        for (int i = 0; i < amountPlayers+amountAI; i++) {
            int lives = players[i].getRemainingLives();
            playerDatas[i].transform.GetComponent<PlayerUIManager>().setLives(lives);
        }
    }

    private void UpdateDead(){
        int surviving = 0;
        for (int i = 0; i < amountPlayers+amountAI; i++) {
            if(players[i].getRemainingLives() <= 0){
                camera.GetComponent<FollowingTargetsCamera>().RemoveTarget(players[i].transform);
                playerDatas[i].transform.Find("Image").GetComponent<Image>().color = new Color(0.5f,0.5f,0.5f,0.5f);
            }
            else{
                surviving++;
            }
        }
        if(surviving == 1){
            GameOver();
        }
    }

    private void GameOver(){
        int winner = 0;
        for (int i = 0; i < amountPlayers + amountAI; i++) {
            if (players[i].getRemainingLives() > 0) {
                winner = i;
                break;
            }
        };
        
        timer.text = "Game over";
        players[winner].setAlive(false);
        canvas.gameObject.SetActive(false);
        gameOverCanvas.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Game Over, "+players[winner].gameObject.name +" WON !";
        gameOverCanvas.gameObject.SetActive(true);
        long winnerPoints = players[winner].getPoints();
        if (SaveData.getInstance().leaderboard.isHigh(winnerPoints)) {
            SaveData.getInstance().lastScore = winnerPoints;
            StartCoroutine(WriteHighScore());
        }
        else{
            StartCoroutine(ReloadLevel());
        }
        
    }

    IEnumerator ReloadLevel() {//Loads Main Menu
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(5);

        SceneManager.LoadScene(0);
    }
    IEnumerator WriteHighScore() {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(5);

        SceneManager.LoadScene("newHighScore");
    }

    public void pause(InputAction.CallbackContext context){
        togglePause = !togglePause;
        if(togglePause){
            pauseStarter = context.control.ToString();
            startPause();
        }
        else{
            if(context.control.ToString() == pauseStarter){
                stopPause();
            }
            else{
                togglePause = !togglePause;
            }
            
        }
    }

    public void startPause() {
        Time.timeScale = 0f;
    }

    public void stopPause() {
        Time.timeScale = 1f;
    }



}
