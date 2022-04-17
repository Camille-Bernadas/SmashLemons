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
    public int maxX; //max ligne (on doit pouvoir faire mieux sans ces deux valeurs)
    public int maxY; //max colone
    CharacterInfo[,] charGrid;

    public GameObject characterCanvas;
    bool loadLevel;
    public bool allPlayersSelected;
    
    /*
    #region Singleton
    public static SelectScreenManager instance;
    public static SelectScreenManager getInstance()
    {
        return instance;
    }
    #endregion

    void Awake()
    {
        instance = this;
    }
    */

    // Start is called before the first frame update
    void Start()
    {
        //charactermanager ?

        charGrid = new CharacterInfo[maxX, maxY];
        int x = 0;
        int y = 0;

        CharacterList = characterCanvas.GetComponentsInChildren<CharacterInfo>();

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
                        //deselection
                        /*
                        if(Input.GetButtonUp("Fire2" + charManager.players[i].inputId))
                        {
                            playerInterfaces[i].playerBase.hasCharacter = false;
                        }
                        */

                        if(playerInterfaces[i].characterValue == 0)
                        {
                            //playerInterfaces[i].playerBase = charManager.players[i]; //set up pour l'interface
                            HandleSelectorPosition(playerInterfaces[i]); // find active portrait
                            //HandleSelectScreenInput(playerInterfaces[i], charManager.players[i].inputId);//handleInput/player-user et info de l'id inpute
                            HandleCharacterPreview(playerInterfaces[i]); // gere la creation et la visualisation du portrait
                        }
                    }
                    else//pour un ia
                    {
                        //Set un personnage pour les ia ? ou Alors fermer certains selector
                        //charManager.players[i].hasCharacter = true;
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

    bool AllSet()
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

    void HandleSelectScreenInput(PlayerInterfaces player, string playerId)
    {
        #region Grid Navigation

        float vertical = Input.GetAxis("Vertical" + playerId);

        if(vertical != 0)
        {
            if(!player.hitInputOnce)
            {
                if(vertical > 0)
                {
                    player.activeX = (player.activeX > 0) ? player.activeX -1: maxX-1;
                }
                else
                {
                    player.activeX = (player.activeX < maxX -1) ? player.activeX + 1 : 0;
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
        
        //player.selector.modifyImage(player.activeC.characterId);
        
        Vector2 selectorPosition = player.activeC.transform.localPosition;
        selectorPosition = selectorPosition + new Vector2(characterCanvas.transform.localPosition.x, characterCanvas.transform.localPosition.y);

        player.SelectorPlayer.transform.localPosition = selectorPosition;
    }

    void HandleCharacterPreview(PlayerInterfaces player)
    {
        //difference preview / active, changement de personnage
        /*
        if(player.previewC != player.activeC)
        {
            if(player.characterValue != null)
            {
                Destroy(player.createdCharacter);
            }

            GameObject go = Instantiate(CharacterManager.getInstance().returnCharacterWithID(player.activeC.characterId).prefab, player.charVisPos.position, Quaternion.identity) as GameObject;
             
            player.createdCharacter = go;

            player.previewC = player.activeC;

            if(!string.Equals(player.playerBase.playerId, charManager.players[0].playedId))
            {
                player.createdCharacter.GetComponent<StateManager>().lookRight = false;
            }
        }*/
    }

    
    IEnumerator LoadLevel()
    {
        //if any of the players is an AI, then assign a random character to the prefab
        /*
        for(int i = 0; i < charManager.players.Count; i++)
        {
            if(charManager.players[i].playerType == PlayerBase.PlayerType.ai)
            {
                if(charManager.players[i].playerPrefab == null)
                {
                    int ranValue = Random.range(0, CharacterList.Length);
                    charManager.players[i].playerPrefab = charManager.returnCharacterWithID(CharacterList[ranValue].characterId).prefab;
                
                }
            }
        }
        */

        //enregistrer la valeur de chaque playerInterfaces (isPlayer, isActive, characterValue) dans saveData
        PrepareData();

        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("SampleScene"/*remplir ici la scene a jeu*/);
    }

    void PrepareData()
    {
        /*if (SaveData.Instance != null)
        {
            for(int i = 0; i < playerInterfaces.Count; i++)
            {
                //ici enregistrer pour chaque joueur dans saveData
                bool x = playerInterfaces[i].isActive;
                bool y = playerInterfaces[i].isPlayer;
                int z = playerInterfaces[i].characterValue;
            }
        }*/
    }

    [System.Serializable]
    public class PlayerInterfaces
    {
        public CharacterInfo activeC;
        public CharacterInfo previewC;
        public GameObject SelectorPlayer;
        public GameObject Player;
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
