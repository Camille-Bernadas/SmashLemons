using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SelectScreenManager : MonoBehaviour
{
    public int numberOfPlayer; //nombre de joueur
    public List<PlayerInterfaces> playerInterfaces = new List<PlayerInterfaces>(); //liste des personnages survoler/selectionner des joueurs (+autre info)
    public CharacterInfo[] CharacterList; // liste des cases contenant les personnage
    public DisplayPlayerSelectorScript[] PlayerList;
    public int maxX; //max ligne (on doit pouvoir faire mieux sans ces deux valeurs)
    public int maxY; //max colone
    CharacterInfo[,] charGrid;
    DisplayPlayerSelectorScript[] playerGrid;

    public GameObject playerPreviewCanvas;
    public GameObject characterCanvas;
    bool loadLevel;
    public bool allPlayersSelected;

    public Sprite random;
    public Sprite unactive;

    // Start is called before the first frame update
    void Start()
    {
        charGrid = new CharacterInfo[maxX, maxY];
        playerGrid = new DisplayPlayerSelectorScript[4];
        int x = 0;
        int y = 0;

        CharacterList = characterCanvas.GetComponentsInChildren<CharacterInfo>();
        PlayerList = playerPreviewCanvas.GetComponentsInChildren<DisplayPlayerSelectorScript>();

        
        for(int i = 0; i < PlayerList.Length; i++)
        {
            PlayerList[i].posX += -1;
            PlayerList[i].posY += i;
            playerGrid[i] = PlayerList[i];
        }

        for(int i = 0; i < CharacterList.Length; i++)
        {
            CharacterList[i].posX += x;
            CharacterList[i].posY += y;
            
            charGrid[x, y] = CharacterList[i];

            if (y < maxY -1)
            {
                y++;
            }
            else
            {
                y = 0;
                x++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!loadLevel)
        {
            for(int i = 0; i < playerInterfaces.Count; i++)
            {
                if(playerInterfaces[i].isActive)
                {
                    if(playerInterfaces[i].isPlayer)
                    {
                        if(playerInterfaces[i].timerToReset > 0f)
                        {
                            playerInterfaces[i].timerToReset -= Time.deltaTime;
                        }
                        //deselection
                        /*
                        if(Input.GetButtonUp("Fire2" + charManager.players[i].inputId))
                        {
                            playerInterfaces[i].characterValue == 0;
                        }
                        */

                        if(playerInterfaces[i].characterValue == 0)
                        {
                            //playerInterfaces[i].playerBase = charManager.players[i]; //set up pour l'interface
                            HandleSelectorPosition(playerInterfaces[i]); // find active portrait
                            //HandleSelectScreenInput(playerInterfaces[i], charManager.players[i].inputId);//handleInput/player-user et info de l'id inpute
                            HandleCharacterPreview(playerInterfaces[i], i); // gere la creation et la visualisation du portrait
                        }
                    }
                    else//pour un ia
                    {
                        //Set un personnage pour les ia ? ou Alors fermer certains selector
                        //pour le moment random
                        playerInterfaces[i].characterValue = -1;
                        HandleCharacterPreview(playerInterfaces[i], i);
                    }
                }
            }
        }

        if(allPlayersSelected)
        {
            Debug.Log("loading");
            StartCoroutine("LoadLevel");
            loadLevel = true;
        }
        else
        {
            if(AllSet())
            {
                allPlayersSelected = true;
            }
        }
    }

    bool AllSet() // verifie si tout les player on selectionner leur personnages
    {
        bool allSet = true;
        for(int i = 0; i < numberOfPlayer; i++)
        {
            if(playerInterfaces[i].characterValue == 0)
            {
                allSet = false;
                break;
            }
        }

        return allSet;
    }

    void HandleSelectScreenInput(PlayerInterfaces player, int playerId)
    {
        #region Grid Navigation

        float vertical = Input.GetAxis("Vertical" + playerId);

        if(vertical != 0)
        {
            if(!player.hitInputOnce)
            {
                if(vertical > 0)
                {
                    player.activeX = (player.activeX > -1) ? player.activeX -1: maxX-1;
                }
                else
                {
                    player.activeX = (player.activeX < maxX -1) ? player.activeX + 1 : -1;
                }

                player.timerToReset = 0;
                player.hitInputOnce = true;
            }
        }

        float horizontal = Input.GetAxis("Horizontal" + playerId);

        if(horizontal != 0)
        {
            if(horizontal > 0)
            {
                player.activeY = (player.activeY > 0) ? player.activeY -1: maxY-1;
            }
            else
            {
                player.activeY = (player.activeY < maxY -1) ? player.activeY + 1 : 0;
            }

            player.timerToReset = 0;
            player.hitInputOnce = true;
        }

        if(vertical == 0 && horizontal == 0)
        {
            player.hitInputOnce = false;
        }

        if(player.hitInputOnce)
        {
            player.timerToReset += Time.deltaTime;

            if (player.timerToReset > 0.8f)
            {
                player.hitInputOnce = false;
                player.timerToReset = 0;
            }
        }

        #endregion

        //selection
        if(Input.GetButtonUp("Fire1" + playerId))
        {
            // animation de selection si affichage perso 3d ?

            //
            //player.playerBase.playerPrefab = charManager.returnCharacterWithID(player.activeC.characterId).prefab;

            //player.playerBase.hasCharacter = true;
        }
    }

    void HandleSelectorPosition(PlayerInterfaces player)
    {
        player.SelectorPlayer.SetActive(true); //enable the selector
        player.activeC = charGrid[player.activeX, player.activeY]; // find the active character
        
        Vector2 selectorPosition = player.activeC.transform.localPosition;
        selectorPosition = selectorPosition + new Vector2(characterCanvas.transform.localPosition.x, characterCanvas.transform.localPosition.y);

        player.SelectorPlayer.transform.localPosition = selectorPosition;
    }

    void HandleCharacterPreview(PlayerInterfaces player, int i)
    {
        playerGrid[i].modifyImage(player.activeC.characterId, charGrid[player.activeX, player.activeY].getSprite());
        player.previewC = player.activeC;
    }

    
    IEnumerator LoadLevel()
    {
        //if any of the players is an AI, then assign a random character to the prefab
        for(int i = 0; i < playerInterfaces.Count; i++)
        {
            if(playerInterfaces[i].isActive)
            {
                if(!playerInterfaces[i].isPlayer)
                {
                    playerInterfaces[i].characterValue = Random.Range(1, 4);
                }
            }
        }

        //enregistrer la valeur de chaque playerInterfaces (isPlayer, isActive, characterValue) dans saveData
        PrepareData();

        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("SampleScene"/*remplir ici la scene a jeu*/);
    }

    void PrepareData()
    {
        if(SaveData.getInstance() != null)
        {
            for(int i = 0; i < playerInterfaces.Count; i++)
            {
                SaveData.getInstance().isActives[i] = playerInterfaces[i].isActive;
                SaveData.getInstance().isPlayers[i] = playerInterfaces[i].isPlayer;
                SaveData.getInstance().charaterId[i] = playerInterfaces[i].characterValue;
            }
        }
    }

    /*public void OnMove(InputAction.CallbackContext ctx)
    {
        switch(ctx.currentControlScheme)
        {
        }
        */
        /*
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
    }*/
    
    /*
    public void OnSelect(InputAction.CallbackContext ctx) {
        if (timer <= 0f && ctx.performed) {
            timer = 0.5f;
            SaveData.getInstance().lastEnteredSurname = getString();
            SaveData.getInstance().lastScore = (long) Random.Range(10,5000);
            SceneManager.LoadScene("LeaderBoard");
        }
    }
    */

    [System.Serializable]
    public class PlayerInterfaces
    {
        public CharacterInfo activeC;
        public CharacterInfo previewC;
        public GameObject SelectorPlayer;
        public Transform charVis;

        public int characterValue;
        public bool isPlayer;
        public bool isActive;
        
        public int activeX;
        public int activeY;

        public bool hitInputOnce;
        public float timerToReset;
    }
}
