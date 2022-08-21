using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public GameObject theMenu;
    public GameObject[] windows;

    private CharStats[] playerStats;

    public Text[] nameText, hpText, mpText, lvlText, xpText;
    public Slider[] xpSlider;
    public Image[] charImage;
    public GameObject[] charStatHolder;

    public GameObject[] statusButtons;

    public Text statusName, statusHP, statusMP, statusStrength, statusDef, statusWpnEq, statusWpnPwr, statusArmrEq, statusArmrPwr, statusXp;
    public Image statusImg;

    public ItemButton[] itemButtons;
    public string selectedItem;
    public Item activeItem;
    public Text itemName, itemDesc, useButtonText;

    public GameObject itemCharChoiceMenu;
    public Text[] itemCharChoiceNames;

    public Text goldText;

    public static GameMenu instance;

    public string mainMenuName;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            if (theMenu.activeInHierarchy)
            {
                //theMenu.SetActive(false);
                //GameManager.instance.gameMenuOpen = false;

                CloseMenu();
            }
            else
            {
                UpdateMainStats();
                theMenu.SetActive(true);
                
                GameManager.instance.gameMenuOpen = true;
            }

            AudioManager.instance.PlaySFX(5);
        }
    }

    public void UpdateMainStats() //updatejta statove trenutnog igra?a
    {
        playerStats = GameManager.instance.playerStats;

        for (int i = 0; i < playerStats.Length; i++)
        {
            if (playerStats[i].gameObject.activeInHierarchy) //ako je odredeni igrac aktivan
            {
                charStatHolder[i].SetActive(true); //aktiviraju se njegove informacije na po?etnom meniju

                nameText[i].text = playerStats[i].charName; 
                hpText[i].text = "HP: " + playerStats[i].currentHP + "/" + playerStats[i].maxHP;
                mpText[i].text = "MP: " + playerStats[i].currentMP + "/" + playerStats[i].maxMP;
                lvlText[i].text = "Lvl: " + playerStats[i].playerLevel;
                xpText[i].text = "" + playerStats[i].currentXP + "/" + playerStats[i].expToNextLevel[playerStats[i].playerLevel];
                xpSlider[i].maxValue = playerStats[i].expToNextLevel[playerStats[i].playerLevel];
                xpSlider[i].value = playerStats[i].currentXP;
                charImage[i].sprite = playerStats[i].charImage;
            }
            else
            {
                charStatHolder[i].SetActive(false); //ako je neaktivan, onda se ne vide info
            }
        }
        goldText.text = GameManager.instance.currentGold.ToString();
    }

    public void ToggleWindow(int windowNumber) //otvaranje i zatvaranje menija
    {
        UpdateMainStats(); //update statova prije svakog otvaranja

        for (int i = 0; i < windows.Length; i++)
        {
            if (i == windowNumber)
            {
                windows[i].SetActive(!windows[i].activeInHierarchy); //pritiskom na odre?eni gumb, zatvara se ili otvara taj prozor (Items, Stats...)
            }
            else
            {
                windows[i].SetActive(false); 
            }
        }

        itemCharChoiceMenu.SetActive(false);
    }

    public void CloseMenu() //zatvaranje menija na buttonu
    {
        for (int i = 0; i < windows.Length; i++)
        {
            windows[i].SetActive(false); //zatvaranje svakog prozora
        }
        theMenu.SetActive(false);
        GameManager.instance.gameMenuOpen = false;

        itemCharChoiceMenu.SetActive(false);
    }

    public void OpenStatus() //otvaranje Stats prozora
    {
        UpdateMainStats();
        //ažurirati info koji se prikazuje
        StatusChar(0); //prvotni prikaz statova prvog igraca

        for (int i = 0; i < statusButtons.Length; i++)
        {
            statusButtons[i].SetActive(playerStats[i].gameObject.activeInHierarchy); //aktivirati svaki gumb ciji igrac je aktivan
            statusButtons[i].GetComponentInChildren<Text>().text = playerStats[i].charName; //napisati ime igraca na svaki gumb
        }
    }

    public void StatusChar(int selected) { //prikaz svih statova o selektiranom igracu
        statusName.text = playerStats[selected].charName;
        statusHP.text = "" + playerStats[selected].currentHP + "/" + playerStats[selected].maxHP;
        statusMP.text = "" + playerStats[selected].currentMP + "/" + playerStats[selected].maxMP;
        statusStrength.text = playerStats[selected].strength.ToString();
        statusDef.text = playerStats[selected].defence.ToString();
        if (playerStats[selected].equippedWpn != "") //ako ima weapon onda se ispisuje, ako ne onda ostaje None
        {
            statusWpnEq.text = playerStats[selected].equippedWpn;
        }
        statusWpnPwr.text = playerStats[selected].weaponPower.ToString();
        if (playerStats[selected].equippedArmr != "") //ako ima armor onda se ispisuje, ako ne onda ostaje None
        {
            statusArmrEq.text = playerStats[selected].equippedArmr;
        }
        statusArmrPwr.text = playerStats[selected].armorPower.ToString();
        statusXp.text = (playerStats[selected].expToNextLevel[playerStats[selected].playerLevel] - playerStats[selected].currentXP).ToString(); //potreban XP za sljede?i level - trenutni XP igra?a
        statusImg.sprite = playerStats[selected].charImage;
    }

    public void ShowItems() //prikazivanje svih 40 itema
    {
        GameManager.instance.SortItems(); //sortira popis itema

        for (int i=0; i<itemButtons.Length; i++) //prolazi sve iteme
        {
            itemButtons[i].buttonValue = i; //dodjeljuje im vrijednosti

            if (GameManager.instance.itemsHeld[i] != "") // ako trenutni item u popisu nije prazan
            {
                itemButtons[i].buttonImage.gameObject.SetActive(true); //prikazuje se slika
                itemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[i]).itemSprite; //dodjeljuje se slika preko GetItemDetails() funkcije
                itemButtons[i].amountText.text = GameManager.instance.numberOfItems[i].ToString(); //dodjeljuje se koli?ina itema
            }
            else
            {
                itemButtons[i].buttonImage.gameObject.SetActive(false); //ina?e ostaje prazno mjesto
                itemButtons[i].amountText.text = "";
            }
        }
    }

    public void SelectItem(Item item) //uzima item te njegovim podacima ispunjava info panel
    {
        activeItem = item;

        if (activeItem.isItem)
        {
            useButtonText.text = "Use"; //itemi se useaju
        } else if (activeItem.isWeapon || activeItem.isArmor)
        {
            useButtonText.text = "Equip"; //weapons i armor se equipa
        }

        itemName.text = item.itemName;
        itemDesc.text = item.description;
    }

    public void DiscardItem() //klikom na gumb se briše koli?ina itema 1 po 1
    {
        if (activeItem != null) //ako aktivni item nije prazan
        {
            GameManager.instance.RemoveItem(activeItem.itemName);
        }
    }

    public void OpenItemCharChoice() //klikom na Use button otvara se izbor igra?a za kojeg želimo upotrijebit item
    {
        itemCharChoiceMenu.SetActive(true); //aktivira se meni s izborom igra?a

        for (int i=0; i < itemCharChoiceNames.Length; i++) //prelazi se sve opcije
        {
            itemCharChoiceNames[i].text = GameManager.instance.playerStats[i].charName; //umetanje teksta na button izbora
            itemCharChoiceNames[i].transform.parent.gameObject.SetActive(GameManager.instance.playerStats[i].gameObject.activeInHierarchy); //aktiviranje buttona ako je igra? aktivan
        }
    }

    public void CloseItemCharChoice() //zatvaranje menija za izbor igra?a pri odabiru itema
    {
        itemCharChoiceMenu.SetActive(false);
    }

    public void UseItem(int selectChar) //nakon što se odabere igra? na njemu se upotrebljava odabrani item
    {
        activeItem.Use(selectChar); //za aktivnog itema
        CloseItemCharChoice(); //zatvara se izbor igra?a
    }

    public void SaveGame() //za save button
    {
        GameManager.instance.SaveData();
        QuestManager.instance.SaveQuestData();
    }

    public void PlayButtonSound()
    {
        AudioManager.instance.PlaySFX(4);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(mainMenuName);

        Destroy(GameManager.instance.gameObject);
        Destroy(PlayerController.instance.gameObject);
        Destroy(AudioManager.instance.gameObject);
        Destroy(gameObject);
    }
}
