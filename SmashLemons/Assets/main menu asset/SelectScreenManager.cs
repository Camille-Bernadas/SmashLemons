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
    public int maxX;
    public int maxY;
    CharacterInfo[,] charGrid;
    DisplayPlayerSelectorScript[] playerGrid;

    public GameObject playerPreviewCanvas;
    public GameObject characterCanvas;
    public GameObject SelectorPlayerDefault;
    bool loadLevel;

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
            PlayerList[i].posX = -1;
            PlayerList[i].posY = i;
            playerGrid[i] = PlayerList[i];
        }

        for(int i = 0; i < CharacterList.Length; i++)
        {
            CharacterList[i].posX = x;
            CharacterList[i].posY = y;
            
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
                        if(playerInterfaces[i].characterValue == 0)
                        {
                            if(playerInterfaces[i].activeX > -1)
                            {
                                HandleSelectorPosition(playerInterfaces[i]); // find active portrait
                                HandleCharacterPreview(playerInterfaces[i], i); // gere la creation et la visualisation du portrait
                            }   
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

        if(AllSet())
        {
            StartCoroutine("LoadLevel");
            loadLevel = true;
        }
    }

    bool AllSet() // verifie si tout les player on selectionner leur personnages
    {
        bool allSet = true;
        for(int i = 0; i < playerInterfaces.Count; i++)
        {
            if(playerInterfaces[i].characterValue == 0)
            {
                allSet = false;
                break;
            }
        }

        return allSet;
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
                    playerInterfaces[i].characterValue = Random.Range(1, 3);
                }
            }
        }

        PrepareData();

        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("SampleScene"/*remplir ici la scene a jeu*/);
    }

    void PrepareData()//enregistrer la valeur de chaque playerInterfaces (isPlayer, isActive, characterValue) dans saveData
    {
        if(SaveData.getInstance() != null)
        {
            for(int i = 0; i < playerInterfaces.Count; i++)
            {
                SaveData.getInstance().playersLoaded[i] = playerInterfaces[i].isActive;
                SaveData.getInstance().playerNotAI[i] = playerInterfaces[i].isPlayer;
                SaveData.getInstance().charactersID[i] = playerInterfaces[i].characterValue;
            }
        }
    }

    public void onMove(int playerID, int horizontal, int vertical)
    {
        if(playerInterfaces[playerID].isPlayer == true)
        {
            if(vertical != 0) // verifie si modification sur l'axe Y
            {
                if(vertical < 0) // verifie si haut ou base
                {
                    playerInterfaces[playerID].activeX = (playerInterfaces[playerID].activeX > -1) ? playerInterfaces[playerID].activeX -1: maxX-1;
                    if(playerInterfaces[playerID].activeY > 3 && playerInterfaces[playerID].activeX == -1) // assure que sur la premiere ligne les valeurs ne depasse pas la taille max de joueur(3)
                    {
                        playerInterfaces[playerID].activeY = 3;
                    }
                }
                else
                {
                    playerInterfaces[playerID].activeX = (playerInterfaces[playerID].activeX < maxX -1) ? playerInterfaces[playerID].activeX +1 : -1;
                    if(playerInterfaces[playerID].activeY > 3 && playerInterfaces[playerID].activeX == -1)
                    {
                        playerInterfaces[playerID].activeY = 3;
                    }
                }
            }

            if(horizontal != 0)// verifie si modification sur l'axe x
            {
                if(horizontal < 0) // verifie si gauche ou droite
                {
                    if(playerInterfaces[playerID].activeX > -1)
                    {
                        playerInterfaces[playerID].activeY = (playerInterfaces[playerID].activeY > 0) ? playerInterfaces[playerID].activeY -1: maxY-1;;
                    }
                    else
                    {
                        playerInterfaces[playerID].activeY = (playerInterfaces[playerID].activeY > 0) ? playerInterfaces[playerID].activeY -1 : 3;
                    }
                }
                else
                {
                    if(playerInterfaces[playerID].activeX > -1)
                    {
                        playerInterfaces[playerID].activeY = (playerInterfaces[playerID].activeY < maxY -1) ? playerInterfaces[playerID].activeY + 1 : 0;
                    }
                    else
                    {
                        playerInterfaces[playerID].activeY = (playerInterfaces[playerID].activeY < 3) ? playerInterfaces[playerID].activeY + 1 : 0;
                    }
                }
            }
            Debug.Log(playerInterfaces[playerID].activeX + " , " + playerInterfaces[playerID].activeY);
        }
    }

    public void Validate(int playerID)
    {
        if(playerInterfaces[playerID].isPlayer == true)
        {
            if(playerInterfaces[playerID].activeX < 0)
            {
                if(playerInterfaces[playerID].activeY != 0)
                {
                    int targetID = playerInterfaces[playerID].activeY;
                    Debug.Log("ID : " + playerID);
                    Activated(playerInterfaces[targetID], targetID);
                }
            }
            else
            {
                playerInterfaces[playerID].characterValue = charGrid[playerInterfaces[playerID].activeX, playerInterfaces[playerID].activeY].characterId;
            }
        }
        else
        {
            Activated(playerInterfaces[playerID], playerID);
        }
    }

    public void Cancel(int playerID)
    {
        if(playerInterfaces[playerID].characterValue != 0)
        {
            playerInterfaces[playerID].characterValue = 0;
        }
        else
        {
            if(playerID == 0)
            {
                SceneManager.LoadScene("SampleScene"/*remplir ici la scene de menu pour retour en arriere*/);
            }
        }
    }

    void Activated(PlayerInterfaces player, int playerID)
    {
        player.activeX = 0;
        player.activeY = 0;
        player.activeC = charGrid[player.activeX, player.activeY];
        if(player.isActive)
        {
            player.SelectorPlayer.SetActive(true); //enable the selector

            Vector2 selectorPosition = SelectorPlayerDefault.transform.localPosition;
            selectorPosition = selectorPosition + new Vector2( SelectorPlayerDefault.transform.localPosition.x,  SelectorPlayerDefault.transform.localPosition.y);
            
            player.SelectorPlayer.transform.localPosition = selectorPosition;
            if(player.isPlayer)
            {
                player.isPlayer = false;
            }
            else
            {
                player.isActive = false;
                player.characterValue = 0;
                playerGrid[playerID].modifyImage(player.activeC.characterId, unactive);
            }
        }
        else
        {
            player.isActive = true;
            player.isPlayer = true;
        }
    }

    [System.Serializable]
    public class PlayerInterfaces
    {
        public CharacterInfo activeC;
        public CharacterInfo previewC;
        public GameObject SelectorPlayer;

        public int characterValue;
        public bool isPlayer;
        public bool isActive;
        
        public int activeX;
        public int activeY;
    }
}
