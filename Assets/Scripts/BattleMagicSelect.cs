using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMagicSelect : MonoBehaviour
{
    public string spellName;
    public int spellCost;
    public Text nameText;
    public Text costText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Press()
    {
        if (BattleManager.instance.activeBattlers[BattleManager.instance.currentTurn].currentMP >= spellCost) // ima li igrac dovoljno MP za napad
        {
            BattleManager.instance.magicMenu.SetActive(false);
            BattleManager.instance.OpenTargetMenu(spellName); // prikazuje se target menu
            BattleManager.instance.activeBattlers[BattleManager.instance.currentTurn].currentMP -= spellCost; // smanjivanje MP za napad
        }
        else
        {
            // nnema dovoljno MP-a
            BattleManager.instance.battleNotice.theText.text = "Not enough MP!"; //dodijeli tekst obavijesti
            BattleManager.instance.battleNotice.Activate(); // aktiviraj obavijest
            BattleManager.instance.magicMenu.SetActive(false); //ugasi magic menu
        }
    }
}
