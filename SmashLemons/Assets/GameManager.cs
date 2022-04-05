using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject prefabPlayerData, prefabCaterine, prefabKevin, prefabBalibump, prefabMelog;
    [SerializeField]
    private Sprite chat, kevin, balibump, golem;

    [SerializeField]
    private InputActionAsset controls;


    private static readonly Color[] colors = {  new Color(1f, 0f, 0f, 0.2f), new Color(0f, 0f, 1f, 0.2f),
                                                new Color(1f, 1f, 0f, 0.2f), new Color(0f, 1f, 0f, 0.2f) };

    Camera camera;

    TextMeshProUGUI timer;

    Transform playerData;

    Canvas canvas, gameOverCanvas;

    GameObject[] playerDatas = new GameObject[4];
    Character[] players = new Character[4];
    float time = 0f;
    public int amountPlayers = 4;

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
        for (int i = 0; i < amountPlayers; i++) {
            string controlScheme = "PlayerOne";
            switch(i){
                case 0:;
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
                    controlScheme = "PlayerOne";
                    break;
            }
            //players[i] = Instantiate(prefabCaterine).transform.GetComponent<Character>();
            GameObject character = PlayerInput.Instantiate(prefabCaterine, 0, controlScheme).gameObject;
            character.name = "Player" + (i + 1);
            players[i] = character.transform.GetComponent<Character>();
            players[i].Spawn();

            playerDatas[i] = Instantiate(prefabPlayerData, playerData);
            Image currentImage = playerDatas[i].transform.Find("Image").GetComponent<Image>();
            currentImage.sprite = chat;
            currentImage.color = new Color(1f, 1f, 1f, 0.95f);
            playerDatas[i].transform.GetComponent<Image>().color = colors[i];
        }
        PlayerInput.all[0].SwitchCurrentControlScheme("PlayerOne", Keyboard.current);
        PlayerInput.all[1].SwitchCurrentControlScheme("PlayerTwo", Keyboard.current);
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
        for (int i = 0; i < amountPlayers; i++) {
            float damage = players[i].getTakenDamage();
            playerDatas[i].transform.GetComponent<PlayerUIManager>().setDamage(damage);
        }
    }

    private void UpdateLives() {
        for (int i = 0; i < amountPlayers; i++) {
            int lives = players[i].getRemainingLives();
            playerDatas[i].transform.GetComponent<PlayerUIManager>().setLives(lives);
        }
    }

    private void UpdateDead(){
        int surviving = 0;
        for (int i = 0; i < amountPlayers; i++) {
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
        for (int i = 0; i < amountPlayers; i++) {
            if (players[i].getRemainingLives() > 0) {
                winner = i;
                break;
            }
        }
        ;
        timer.text = "Game over";
        players[winner].setAlive(false);
        canvas.gameObject.SetActive(false);
        gameOverCanvas.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Game Over, "+players[winner].gameObject.name +" WON !";
        gameOverCanvas.gameObject.SetActive(true);
        StartCoroutine(ReloadLevel());
    }

    IEnumerator ReloadLevel() {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(5);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
