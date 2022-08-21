using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStats : MonoBehaviour
{
    public string charName;
    public int playerLevel = 1;
    public int currentXP;
    public int[] expToNextLevel;
    public int maxLevel = 100;
    public int baseEXP = 1000;

    public int currentHP;
    public int maxHP = 100;
    public int currentMP;
    public int maxMP = 30;
    public int[] mpLvlBonus;
    public int strength;
    public int defence;
    public int weaponPower;
    public int armorPower;
    public string equippedWpn;
    public string equippedArmr;
    public Sprite charImage;

    // Start is called before the first frame update
    void Start()
    {
        expToNextLevel = new int[maxLevel];
        expToNextLevel[1] = baseEXP;

        for (int i = 2; i < expToNextLevel.Length; i++)
        {
            expToNextLevel[i] = Mathf.FloorToInt(expToNextLevel[i - 1] * 1.05f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddExp(1000);
        }
    }

    public void AddExp(int expToAdd)
    {
        currentXP += expToAdd;
        if(playerLevel < maxLevel)
        {
            if (currentXP > expToNextLevel[playerLevel])
            {
                currentXP -= expToNextLevel[playerLevel];

                playerLevel++;

                // determine whether to add to strength or  defence based on odd or even
                if (playerLevel % 2 == 0)
                {
                    strength++;
                }
                else
                {
                    defence++;
                }

                maxHP = Mathf.FloorToInt(maxHP * 1.05f);
                currentHP = maxHP;

                maxMP = maxMP + mpLvlBonus[playerLevel];
                currentMP = maxMP;
            }
        }

        if (playerLevel >= maxLevel)
        {
            currentXP = 0;
        }
        GameMenu.instance.UpdateMainStats();
    }
}
