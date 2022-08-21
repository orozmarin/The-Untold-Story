using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Type")]
    public bool isItem;
    public bool isWeapon, isArmor;

    [Header("Item Details")]
    public string itemName;
    public string description;
    public int value;
    public Sprite itemSprite;

    [Header("Item Details")]
    public int amountToChange;
    public bool affectHP, affectMP, affectStr;

    [Header("Weapon/Armor Details")]
    public int weaponPwr;
    public int armorPwr;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*public void Use(int charToUseOn) //prilikom klika na Use
    {
        CharStats selectedChar = GameManager.instance.playerStats[charToUseOn]; //za odabranog igra?a

        if (isItem) //provjera je li item
        {
            if (affectHP) //ako je health item
            {
                selectedChar.currentHP += amountToChange;
                
                if (selectedChar.currentHP > selectedChar.maxHP) //ako prelazi max HP igra?a
                {
                    selectedChar.currentHP = selectedChar.maxHP;
                }
            }
            if (affectMP) //ako je MP item
            {
                selectedChar.currentMP += amountToChange;

                if (selectedChar.currentMP > selectedChar.maxMP)
                {
                    selectedChar.currentMP = selectedChar.maxMP;
                }
            }
            if (affectStr) //ako je strength item
            {
                selectedChar.strength += amountToChange;
            }
        }
        if (isWeapon) //ako je weapon
        {
            if(selectedChar.equippedWpn != "") //ako ima equipan weapon
            {
                GameManager.instance.AddItem(selectedChar.equippedWpn); //equipani weapon se dodaje u popis svih itema
            }
            selectedChar.equippedWpn = itemName; //equipa se novi weapon
            selectedChar.weaponPower = weaponPwr;
        }
        if (isArmor)
        {
            if(selectedChar.equippedArmr != "") //ako ima equipan armor
            {
                GameManager.instance.AddItem(selectedChar.equippedArmr); //equipani armor se dodaje u popis svih itema
            }
            selectedChar.equippedArmr = itemName; //equipa se novi armor
            selectedChar.armorPower = armorPwr;
        }
        GameManager.instance.RemoveItem(itemName); //removea se equipani item iz popisa itema

    }*/

    public void Use(int charToUseOn)
    {
        if (BattleManager.instance.battleActive)
        {
            charToUseOn = BattleManager.instance.currentTurn;
        }

        CharStats selectedChar = GameManager.instance.playerStats[charToUseOn];

        if (isItem)
        {
            if (selectedChar.currentHP != selectedChar.maxHP)
            {
                if (affectHP)
                {
                    selectedChar.currentHP += amountToChange;
                    if (selectedChar.currentHP > selectedChar.maxHP)
                    {
                        selectedChar.currentHP = selectedChar.maxHP;
                    }

                    if (BattleManager.instance.battleActive)
                    {
                        charToUseOn = BattleManager.instance.currentTurn;
                        BattleManager.instance.activeBattlers[charToUseOn].currentHP += amountToChange;
                        if (BattleManager.instance.activeBattlers[charToUseOn].currentHP > selectedChar.maxHP)
                        {
                            BattleManager.instance.activeBattlers[charToUseOn].currentHP = selectedChar.maxHP;
                        }
                    }
                }

                GameManager.instance.RemoveItem(itemName);
            }

            if (selectedChar.currentMP != selectedChar.maxMP)
            {
                if (affectMP)
                {
                    selectedChar.currentMP += amountToChange;
                    if (selectedChar.currentMP > selectedChar.maxMP)
                    {
                        selectedChar.currentMP = selectedChar.maxMP;
                    }

                    if (BattleManager.instance.battleActive)
                    {
                        charToUseOn = BattleManager.instance.currentTurn;
                        BattleManager.instance.activeBattlers[charToUseOn].currentMP += amountToChange;
                        if (BattleManager.instance.activeBattlers[charToUseOn].currentMP > selectedChar.maxMP)
                        {
                            BattleManager.instance.activeBattlers[charToUseOn].currentMP = selectedChar.maxMP;
                        }
                    }

                    GameManager.instance.RemoveItem(itemName);
                }
            }

            if (affectStr)
            {
                selectedChar.strength += amountToChange;

                GameManager.instance.RemoveItem(itemName);
            }
        }

        if (isWeapon)
        {
            if (selectedChar.equippedWpn != "")
            {
                GameManager.instance.AddItem(selectedChar.equippedWpn);
            }

            selectedChar.equippedWpn = itemName;
            selectedChar.weaponPower = weaponPwr;

            GameManager.instance.RemoveItem(itemName);
        }

        if (isArmor)
        {
            if (selectedChar.equippedArmr != "")
            {
                GameManager.instance.AddItem(selectedChar.equippedArmr);
            }

            selectedChar.equippedArmr = itemName;
            selectedChar.armorPower = armorPwr;

            GameManager.instance.RemoveItem(itemName);
        }
    }

}
