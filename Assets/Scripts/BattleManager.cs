using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    public bool battleActive;
    public GameObject battleScene;
    public Transform[] playerPositions, enemyPositions;

    public BattleChar[] playerPrefabs, enemyPrefabs;

    public List<BattleChar> activeBattlers = new List<BattleChar>();

    public int currentTurn;
    public bool turnWaiting;
    public GameObject uiButtonsHolder;

    public BattleMove[] movesList;

    public GameObject enemyAttackEffect;

    public DamageNumber theDamageNumber;

    public Text[] playerName, playerHP, playerMP;

    public GameObject targetMenu;
    public BattleTargetButton[] targetButtons;

    public GameObject magicMenu;
    public BattleMagicSelect[] magicButtons;
    public BattleNotification battleNotice;

    public GameObject itemMenu;
    public ItemButton[] itemButtons;
    public Item activeItem;
    public Text itemName, itemDesc, useButtonText;

    public string gameOverScene;

    public int chanceToFlee = 35; // sansa za bijeg je 35%
    private bool fleeing;
    public int rewardXP;
    public string[] rewardItems;

    public bool cannotFlee;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(instance);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            
            BattleStart(new string[] {"Skeleton", "Wizard", "Dragonite"}, false);
        }

        if (battleActive)
        {
            if (turnWaiting)
            {
                if (activeBattlers[currentTurn].isPlayer)
                {
                    uiButtonsHolder.SetActive(true);
                }
                else
                {
                    uiButtonsHolder.SetActive(false);

                    //enemy should attack
                    StartCoroutine(EnemyMoveCo());
                }
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                NextTurn();
            }
        }
    }

    public void BattleStart(string[] enemiesToSpawn, bool setCannotFlee) //zapocinjanje bitke
    {
        if (!battleActive) //ako vec nije bitka u tijeku
        {
            battleActive = true; //zapocinje

            cannotFlee = setCannotFlee;

            GameManager.instance.battleActive = true; //onemogucavanje kretnje igraca

            transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z); //background slika za battle bude u kameri
            battleScene.SetActive(true); //background i meni se pale

            AudioManager.instance.PlayBGM(0);

            for (int i = 0; i<playerPositions.Length; i++) //prolazi po pozicijama te ih popunjava prvim aktivnim igracima
            {
                if (GameManager.instance.playerStats[i].gameObject.activeInHierarchy) //ako je igrac aktivan
                {
                    for (int j = 0; j < playerPrefabs.Length; j++) //prolazi kroz prefabs od igraca
                    {
                        if (playerPrefabs[j].charName == GameManager.instance.playerStats[i].charName) //pronadi igraca u prefabsima
                        {
                            BattleChar newPlayer = Instantiate(playerPrefabs[j], playerPositions[i].position, playerPositions[i].rotation); //kreiranje "novog" igraca za battle na redom pozicijama od 1 do 3
                            newPlayer.transform.parent = playerPositions[i]; // na kojoj poziciji ce se spawnat
                            activeBattlers.Add(newPlayer); // dodaj igraca u listu

                            CharStats thePlayer = GameManager.instance.playerStats[i]; //za aktiviranje menija
                            activeBattlers[i].currentHP = thePlayer.currentHP;
                            activeBattlers[i].maxHP = thePlayer.maxHP;
                            activeBattlers[i].currentMP = thePlayer.currentMP;
                            activeBattlers[i].maxMP = thePlayer.maxMP;
                            activeBattlers[i].strength = thePlayer.strength;
                            activeBattlers[i].defence = thePlayer.defence;
                            activeBattlers[i].wpnPwr = thePlayer.weaponPower;
                            activeBattlers[i].armPwr = thePlayer.armorPower;
                        }
                    }

                    
                }
            }

            for (int i = 0; i < enemiesToSpawn.Length; i++) //prolazi enemies koje smo stavili pri pozivu funkcije
            {
                if (enemiesToSpawn[i] != "")
                {
                    for (int j=0; j<enemyPrefabs.Length; j++) //prolazi prefabs enemiesa
                    {
                        if (enemyPrefabs[j].charName == enemiesToSpawn[i]) //ako je pronaden u prefabsima
                        {
                            BattleChar newEnemy = Instantiate(enemyPrefabs[j], enemyPositions[i].position, enemyPositions[i].rotation); //isto kao i kod igraca
                            newEnemy.transform.parent = enemyPositions[i];
                            activeBattlers.Add(newEnemy);
                        }
                    }
                }
            }

            turnWaiting = true;
            currentTurn = Random.Range(0, activeBattlers.Count); // random lik ima prvi potez
        }
        UpdateUIStats();
    }

    public void NextTurn()
    {
        currentTurn++; //povecava se potez svakim klikom
        if (currentTurn >= activeBattlers.Count) //ako je broj veci od broja aktivnih igraca resetira se
        {
            currentTurn = 0;
        }

        turnWaiting = true;

        UpdateBattle();
        UpdateUIStats();
    }

    public void UpdateBattle() //provjeravanje je li tko umro
    {
        bool allEnemiesDead = true;
        bool allPlayersDead = true;

        for (int i = 0; i < activeBattlers.Count; i++) { // prolazi kroz sve aktivne ratnike hahah
            if (activeBattlers[i].currentHP < 0) // ako je HP ispod 0
            {
                activeBattlers[i].currentHP = 0; // HP je 0 (ne moze biti u minusu)
            }
            if (activeBattlers[i].currentHP == 0) // ako je HP ravno 0
            {
                if (activeBattlers[i].isPlayer)
                {
                    activeBattlers[i].theSprite.sprite = activeBattlers[i].deadSprite;
                }
                else
                {
                    activeBattlers[i].EnemyFade();
                }
            }
            else
            {
                if (activeBattlers[i].isPlayer) // ako je trenutni aktivni ratnik igrac i ako ima vise od 0 HP
                {
                    allPlayersDead = false; // nisu svi igraci mrtvi
                    activeBattlers[i].theSprite.sprite = activeBattlers[i].aliveSprite;
                }
                else
                {
                    allEnemiesDead = false; // ili nisu svi neprijatelji mrtvi
                }
            }
        }
        if (allEnemiesDead || allPlayersDead) // nakon prolaska kroz petlju se provjerava boolove
        {
            if (allEnemiesDead)
            {
                // pobjeda
                StartCoroutine(EndBattleCo());
            }
            else
            {
                // poraz
                StartCoroutine(GameOverCo());
            }

            /*battleScene.SetActive(false);
            GameManager.instance.battleActive = false;
            battleActive = false;*/
        }
        else
        {
            while (activeBattlers[currentTurn].currentHP == 0) // skippa se mrtve igrace
            {
                currentTurn++;
                if (currentTurn >= activeBattlers.Count)
                {
                    currentTurn = 0;
                }
            }
        }
    }

    public IEnumerator EnemyMoveCo()
    {
        turnWaiting = false;
        yield return new WaitForSeconds(1f); // cekanje 1 sekunde
        EnemyAttack(); // nakon 1 sekunde pozovi funkciju
        yield return new WaitForSeconds(1f); // cekanje 1 sekunde
        NextTurn(); // nakon 1 sekunde pozovi funkciju
    }


    public void EnemyAttack()
    {
        List<int> players = new List<int>();
        for (int i = 0; i < activeBattlers.Count; i++)
        {
            if (activeBattlers[i].isPlayer && activeBattlers[i].currentHP > 0) // ako je igrac i ako je ziv
            {
                players.Add(i); // pozicije igraca koji su zivi u borbi
            }
        }
        int selectedTarget = players[Random.Range(0,players.Count)]; // odabir mete neprijatelja

        //activeBattlers[selectedTarget].currentHP -= 30;

        int selectAttack = Random.Range(0, activeBattlers[currentTurn].movesAvailable.Length); // random odabir napada u listi moves available
        int movePower = 0;
        for (int i = 0; i < movesList.Length; i++) // pretrazuje sve napade
        {
            if (movesList[i].moveName == activeBattlers[currentTurn].movesAvailable[selectAttack]) // ako enemy ima taj napad
            {
                Instantiate(movesList[i].theEffect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation); //instanciraj ga na selected targeta u battleu
                movePower = movesList[i].movePower;
            }
        }

        Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation);

        DealDamage(selectedTarget, movePower);
    }

    public void DealDamage(int target, int movePower)
    {
        float attackPower = activeBattlers[currentTurn].strength + activeBattlers[currentTurn].wpnPwr;
        float defPower = activeBattlers[target].defence + activeBattlers[target].armPwr;

        float damageCalc = (attackPower / defPower) * movePower * Random.Range(.9f, 1.1f);
        int damageToGive = Mathf.RoundToInt(damageCalc);
        Debug.Log(activeBattlers[currentTurn].charName + "is dealing " + damageCalc + "(" + damageToGive + ") damage to " + activeBattlers[target].charName);

        activeBattlers[target].currentHP -= damageToGive;

        Instantiate(theDamageNumber, activeBattlers[target].transform.position, activeBattlers[target].transform.rotation).SetDamage(damageToGive);

        UpdateUIStats();
    }

    public void UpdateUIStats()
    {
        for (int i=0; i< playerName.Length; i++)
        {
            if (activeBattlers.Count > 1)
            {
                if (activeBattlers[i].isPlayer)
                {
                    BattleChar playerData = activeBattlers[i];

                    playerName[i].gameObject.SetActive(true);
                    playerName[i].text = playerData.charName;
                    playerHP[i].text = Mathf.Clamp(playerData.currentHP, 0, int.MaxValue) + "/" + playerData.maxHP;
                    playerMP[i].text = Mathf.Clamp(playerData.currentMP, 0, int.MaxValue) + "/" + playerData.maxMP;
                }
                else
                {
                    playerName[i].gameObject.SetActive(false);
                }
            }
            else
            {
                playerName[i].gameObject.SetActive(false);
            }
        }
    }

    public void PlayerAttack (string moveName, int selectedTarget)
    {
        int movePower = 0;
        for (int i = 0; i < movesList.Length; i++) // pretrazuje sve napade
        {
            if (movesList[i].moveName == moveName) // ako enemy ima taj napad
            {
                Instantiate(movesList[i].theEffect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation); //instanciraj ga na selected targeta u battleu
                movePower = movesList[i].movePower;
            }
        }
        Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation); //prikaz effecta napada

        DealDamage(selectedTarget, movePower);

        uiButtonsHolder.SetActive(false);
        targetMenu.SetActive(false);

        NextTurn();
    }

    public void OpenTargetMenu(string moveName)
    {
        targetMenu.SetActive(true);

        List<int> enemies = new List<int>();

        for (int i=0; i < activeBattlers.Count; i++)
        {
            if (!activeBattlers[i].isPlayer) //provjera da nije igrac vec neprijatelj
            {
                enemies.Add(i); //dodaj u listu
            }
        }

        for (int i = 0; i < targetButtons.Length; i++)
        {
            if (enemies.Count > i && activeBattlers[enemies[i]].currentHP > 0) //za deaktiviranje buttona ako je manji broj neprijatelja
            {
                targetButtons[i].gameObject.SetActive(true);

                targetButtons[i].moveName = moveName;
                targetButtons[i].activeBattlerTarget = enemies[i];
                targetButtons[i].targetName.text = activeBattlers[enemies[i]].charName;
            }
            else
            {
                targetButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenMagicMenu()
    {
        magicMenu.SetActive(true);

        for (int i = 0; i < magicButtons.Length; i++)
        {
            if (activeBattlers[currentTurn].movesAvailable.Length > i) //provjera da ovaj button ima svoj magic napad
            {
                magicButtons[i].gameObject.SetActive(true); //aktivira se button u meniju
                magicButtons[i].spellName = activeBattlers[currentTurn].movesAvailable[i];
                magicButtons[i].nameText.text = magicButtons[i].spellName; //ime na buttonu

                for (int j = 0; j< movesList.Length; j++) // moramo proci moveslist da dobijemo cost od napada
                {
                    if (movesList[j].moveName == magicButtons[i].spellName) // ako su isti napadi onda dobij cost
                    {
                        magicButtons[i].spellCost = movesList[j].moveCost;
                        magicButtons[i].costText.text = magicButtons[i].spellCost.ToString();
                    }
                }
            }
            else
            {
                magicButtons[i].gameObject.SetActive(false); // sakrij sve ostale buttone
            }
        }
    }

    public void Flee()
    {
        if (cannotFlee)
        {
            battleNotice.theText.text = "Can not flee Boss Battle!";
            battleNotice.Activate();
        }
        else
        {
            int fleeSuccess = Random.Range(0, 100);
            if (fleeSuccess < chanceToFlee)
            {
                // end the battle
                // battleActive = false;
                // battleScene.SetActive(false);
                fleeing = true;
                StartCoroutine(EndBattleCo());
            }
            else
            {
                NextTurn();
                battleNotice.theText.text = "Couldn't escape!";
                battleNotice.Activate();
            }
        }
    }

    public void OpenItemMenu()
    {
        GameManager.instance.SortItems();
        itemMenu.SetActive(true);
    }

    public void ShowItems()
    {
        GameManager.instance.SortItems();

        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtons[i].buttonValue = i;

            if (GameManager.instance.itemsHeld[i] != "")
            {
                itemButtons[i].buttonImage.gameObject.SetActive(true);
                itemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[i]).itemSprite;
                itemButtons[i].amountText.text = GameManager.instance.numberOfItems[i].ToString();
            }
            else
            {
                itemButtons[i].buttonImage.gameObject.SetActive(false);
                itemButtons[i].amountText.text = "";
            }
        }
    }

    public void SelectItem(Item selectedItem)
    {
        activeItem = selectedItem;
        if (selectedItem.isItem)
        {
            useButtonText.text = "Use";
        }

        if (activeItem.isWeapon || activeItem.isArmor)
        {
            useButtonText.text = "Equip";
        }

        itemName.text = activeItem.itemName;
        itemDesc.text = activeItem.description;
    }

    public void UseItem()
    {
        activeItem.Use(currentTurn);
        GameManager.instance.SortItems();
        UpdateUIStats();
        CloseItemMenu();
    }

    public void CloseItemMenu()
    {
        itemMenu.SetActive(false);
    }

    public IEnumerator EndBattleCo()
    {
        battleActive = false;
        uiButtonsHolder.SetActive(false);
        targetMenu.SetActive(false);
        magicMenu.SetActive(false);

        yield return new WaitForSeconds(.5f);

        UIFade.instance.FadeToBlack();

        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < activeBattlers.Count; i++)
        {
            if (activeBattlers[i].isPlayer)
            {
                for (int j = 0; j < GameManager.instance.playerStats.Length; j++)
                {
                    if (activeBattlers[i].charName == GameManager.instance.playerStats[j].charName)
                    {
                        GameManager.instance.playerStats[j].currentHP = activeBattlers[i].currentHP;
                        GameManager.instance.playerStats[j].currentMP = activeBattlers[i].currentMP;
                    }
                }
            }

            Destroy(activeBattlers[i].gameObject);
        }

        UIFade.instance.FadeFromBlack();
        battleScene.SetActive(false);
        activeBattlers.Clear();
        currentTurn = 0;
        //GameManager.instance.battleActive = false;
        if (fleeing)
        {
            GameManager.instance.battleActive = false;
            fleeing = false;
        }
        else
        {
            BattleReward.instance.OpenRewardScreen(rewardXP, rewardItems);
        }

        AudioManager.instance.PlayBGM(FindObjectOfType<CamerController>().musicToPlay);
    }

    public IEnumerator GameOverCo()
    {
        battleActive = false;

        UIFade.instance.FadeToBlack();

        yield return new WaitForSeconds(1.5f);

        battleScene.SetActive(false);
        SceneManager.LoadScene(gameOverScene);

    }
}
