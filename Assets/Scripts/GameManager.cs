using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public CharStats[] playerStats;

    public bool gameMenuOpen,  dialogActive, fadingBetweenAreas, shopActive, battleActive;

    public string[] itemsHeld;
    public int[] numberOfItems;
    public Item[] referenceItems;

    public int currentGold;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != null)
        {
            Destroy(instance);
        }

        DontDestroyOnLoad(gameObject);

        SortItems();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameMenuOpen || dialogActive || fadingBetweenAreas || shopActive || battleActive)
        {
            PlayerController.instance.canMove = false;
        }
        else
        {
            PlayerController.instance.canMove = true;
        }
    }

    public Item GetItemDetails(string itemToGrab) //dohva?anje podataka o itemu
    {
        for (int i=0; i<referenceItems.Length; i++) //prolaženje kroz sve iteme u prefabs
        {
            if (referenceItems[i].itemName == itemToGrab) //ako je prona?en item prema nazivu
            {
                return referenceItems[i]; //vra?a se
            }
        }   
        return null;
    }

    public void SortItems() // sortiranje tako da postoje?i itemi budu odma prvi po redu
    {
        bool itemAfterSpace = true;

        while (itemAfterSpace) //dok ima item nakon praznog mjesta radi
        {
            itemAfterSpace = false;
            for (int i = 0; i < itemsHeld.Length - 1; i++) //prolazi sve iteme koje sadrži igra?
            {
                if (itemsHeld[i] == "") //ako je prazno mjesto
                {
                    itemsHeld[i] = itemsHeld[i + 1]; //zamjeni mjesta sa sljede?im mjestom
                    itemsHeld[i + 1] = "";

                    numberOfItems[i] = numberOfItems[i + 1]; //isto tako zamjeni koli?inu itema sa sljede?im
                    numberOfItems[i + 1] = 0;

                    if (itemsHeld[i] != "") //ako zamijenjeni item nije prazan while petlja ide dalje
                    {
                        itemAfterSpace = true;
                    }
                }
            }
        }
    }

    public void AddItem(string itemToAdd) //dodavanje itema
    {
        int newItemPosition = 0;
        bool foundSpace = false;

        for (int i=0; i < itemsHeld.Length; i++) //prolazi kroz sve iteme igra?a
        {
            if (itemsHeld[i] == "" || itemsHeld[i] == itemToAdd) //ako nema itema na toj poziciji ili ako je taj item ve? u inventaru da mu se pove?a koli?ina
            {
                newItemPosition = i; //pamti se pozicija
                i = itemsHeld.Length; //zavrsava se petlja
                foundSpace = true; //prona?eno mjesto za dodavanje
            }
        }

        if (foundSpace)
        {
            bool itemExists = false;
            for (int i=0; i<referenceItems.Length; i++) //provjeravanje postoji li item u prefabs
            {
                if (referenceItems[i].itemName == itemToAdd) //ako postoji
                {
                    itemExists = true;

                    i = referenceItems.Length;
                }
            }

            if (itemExists)
            {
                itemsHeld[newItemPosition] = itemToAdd; //na novoj poziciji se dodaje traženi item
                numberOfItems[newItemPosition]++; //pove?avanje koli?ine
            }
            else
            {
                Debug.LogError(itemToAdd + " Does not exist!");
            }
        }
        GameMenu.instance.ShowItems(); //ažuriranje popisa
    }

    public void RemoveItem(string itemToRemove) //funkcija koju koristi Discard button
    {
        bool foundItem = false;
        int itemPosition = 0;

        for (int i=0; i<itemsHeld.Length; i++) //prolazi kroz sve iteme
        {
            if (itemsHeld[i] == itemToRemove) //ako je traženi item prona?en
            {
                foundItem = true;
                itemPosition = i; //pamti se pozicija itema

                i = itemsHeld.Length; //završava se petlja
            }
        }

        if (foundItem) //ako je prona?en
        {
            numberOfItems[itemPosition]--; //koli?ina se smanjuje
            if (numberOfItems[itemPosition] == 0) // ako je koli?ina 0
            {
                itemsHeld[itemPosition] = ""; //uklanja se item iz popisa
            }
            GameMenu.instance.ShowItems(); //ažurira se popis
        }
        else
        {
            Debug.LogError("Couldn't find " + itemToRemove); //ako je krivi unos imena itema
        }
    }

    public void SaveData() //save igre
    {
        PlayerPrefs.SetString("Current_Scene", SceneManager.GetActiveScene().name); //dobivanje imena trenutne scene
        PlayerPrefs.SetFloat("Player_Position_x", PlayerController.instance.transform.position.x); //save x pozicije igraca
        PlayerPrefs.SetFloat("Player_Position_y", PlayerController.instance.transform.position.y); //save y pozicije igraca
        PlayerPrefs.SetFloat("Player_Position_z", PlayerController.instance.transform.position.z); //save z pozicije igraca

        //save char info
        for (int i = 0; i<playerStats.Length; i++)
        {
            if (playerStats[i].gameObject.activeInHierarchy)
            {
                PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_active", 1);
            }
            else
            {
                PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_active", 0);
            }
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_level", playerStats[i].playerLevel);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_CurrentExp", playerStats[i].currentXP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_CurrentHP", playerStats[i].currentHP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_MaxHP", playerStats[i].maxHP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_CurrentMP", playerStats[i].currentMP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_MaxMP", playerStats[i].maxMP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_Strength", playerStats[i].strength);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_Defence", playerStats[i].defence);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_WpnPwr", playerStats[i].weaponPower);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_ArmrPwr", playerStats[i].armorPower);
            PlayerPrefs.SetString("Player_" + playerStats[i].charName + "_EquippedWpn", playerStats[i].equippedWpn);
            PlayerPrefs.SetString("Player_" + playerStats[i].charName + "_EquippedArmr", playerStats[i].equippedArmr);
            PlayerPrefs.SetInt("CurrentGold_", currentGold);
        }

        // inventar data
        for (int i=0; i<itemsHeld.Length; i++)
        {
            PlayerPrefs.SetString("ItemInInventory_" + i, itemsHeld[i]); //pohrana svakog itema
            PlayerPrefs.SetInt("ItemAmount_" + i, numberOfItems[i]); //pohrana svakog broja itema
        }
    }

    public void LoadData()
    {
        PlayerController.instance.transform.position = new Vector3(PlayerPrefs.GetFloat("Player_Position_x"), PlayerPrefs.GetFloat("Player_Position_y"), PlayerPrefs.GetFloat("Player_Position_z")); //load pozicije igraca

        for (int i = 0; i<playerStats.Length; i++)
        {
            if (PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_active") == 0)
            {
                playerStats[i].gameObject.SetActive(false);
            }
            else
            {
                playerStats[i].gameObject.SetActive(true);
            }

            playerStats[i].playerLevel = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_Level");
            playerStats[i].currentXP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_CurrentExp");
            playerStats[i].currentHP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_CurrentHP");
            playerStats[i].maxHP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_MaxHP");
            playerStats[i].currentMP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_CurrentMP");
            playerStats[i].maxMP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_MaxMP");
            playerStats[i].strength = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_Strength");
            playerStats[i].defence = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_Defence");
            playerStats[i].weaponPower = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_WpnPwr");
            playerStats[i].armorPower = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_ArmrPwr");
            playerStats[i].equippedWpn = PlayerPrefs.GetString("Player_" + playerStats[i].charName + "_EquippedWpn");
            playerStats[i].equippedArmr = PlayerPrefs.GetString("Player_" + playerStats[i].charName + "_EquippedArmr");
            currentGold = PlayerPrefs.GetInt("CurrentGold_");
        }

        for (int i=0; i < itemsHeld.Length; i++)
        {
            itemsHeld[i] = PlayerPrefs.GetString("ItemInInventory_" + i); //pohrana svakog itema
            numberOfItems[i] = PlayerPrefs.GetInt("ItemAmount_" + i); //pohrana svakog broja itema
        }
    }
}
