using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public static Shop instance;

    public GameObject shopMenu;
    public GameObject buyMenu, sellMenu;

    public Text goldText;
    public string[] itemsForSale;

    public ItemButton[] buyItemButtons;
    public ItemButton[] sellItemButtons;

    public Item selectedItem;
    public Text buyItemName, buyItemDesc, buyItemValue;
    public Text sellItemName, sellItemDesc, sellItemValue;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && !shopMenu.activeInHierarchy)
        {
            OpenShop();
        }
    }

    public void OpenShop() //otvaranje shop menija
    {
        shopMenu.SetActive(true);
        OpenBuyMenu(); //automatski otvaranje buy menija

        GameManager.instance.shopActive = true; //onemogucavanje kretnje
        goldText.text = GameManager.instance.currentGold.ToString() + "g"; //ispis golda
    }

    public void CloseShop() //zatvaranje shop menija pritiskom na exit
    {
        shopMenu.SetActive(false);
        GameManager.instance.shopActive = false;
    }

    public void OpenBuyMenu()
    {
        buyItemButtons[0].Press(); //prikaze se name i desc prvog itema

        buyMenu.SetActive(true); //pali
        sellMenu.SetActive(false); //gasi

        for (int i = 0; i < buyItemButtons.Length; i++) //prolazi sve iteme za prodaju
        {
            buyItemButtons[i].buttonValue = i; //dodjeljuje im vrijednosti

            if (itemsForSale[i] != "") // ako trenutni item u popisu nije prazan
            {
                buyItemButtons[i].buttonImage.gameObject.SetActive(true); //prikazuje se slika
                buyItemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(itemsForSale[i]).itemSprite; //dodjeljuje se slika preko GetItemDetails() funkcije
                buyItemButtons[i].amountText.text = ""; //dodjeljuje se kolicina itema
            }
            else
            {
                buyItemButtons[i].buttonImage.gameObject.SetActive(false); //inace ostaje prazno mjesto
                buyItemButtons[i].amountText.text = "";
            }
        }
    }

    public void OpenSellMenu()
    {
        sellItemButtons[0].Press(); //prikaze se name i desc prvog itema
        
        buyMenu.SetActive(false); //gasi
        sellMenu.SetActive(true); //pali

        GameManager.instance.SortItems(); // isto sortiranje kao i kod prikaza inventara

        ShowSellItems();
    }

    private void ShowSellItems()
    {
        for (int i = 0; i < sellItemButtons.Length; i++) //prolazi sve iteme
        {
            sellItemButtons[i].buttonValue = i; //dodjeljuje im vrijednosti

            if (GameManager.instance.itemsHeld[i] != "") // ako trenutni item u popisu nije prazan
            {
                sellItemButtons[i].buttonImage.gameObject.SetActive(true); //prikazuje se slika
                sellItemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[i]).itemSprite; //dodjeljuje se slika preko GetItemDetails() funkcije
                sellItemButtons[i].amountText.text = GameManager.instance.numberOfItems[i].ToString(); //dodjeljuje se koli?ina itema
            }
            else
            {
                sellItemButtons[i].buttonImage.gameObject.SetActive(false); //ina?e ostaje prazno mjesto
                sellItemButtons[i].amountText.text = "";
            }
        }
    }

    public void SelectBuyItem(Item buyItem) //ispis naziva, opisa i valuea itema za kupnju
    {
        selectedItem = buyItem;
        buyItemName.text = selectedItem.itemName;
        buyItemDesc.text = selectedItem.description;
        buyItemValue.text = "Value: " + selectedItem.value + "g";
    }

    public void SelectSellItem(Item sellItem) //ispis naziva, opisa i valuea itema za prodaju
    {
        selectedItem = sellItem;
        sellItemName.text = selectedItem.itemName;
        sellItemDesc.text = selectedItem.description;
        sellItemValue.text = "Value: " + Mathf.FloorToInt(selectedItem.value * .5f).ToString() + "g";
    }

    public void BuyItem() //kupovina itema
    {
        if (selectedItem != null) //safety checkk
        {

            if (GameManager.instance.currentGold >= selectedItem.value) //imamo li dovoljno golda
            {
                GameManager.instance.currentGold -= selectedItem.value; //umanjuje se stanje racuna

                GameManager.instance.AddItem(selectedItem.itemName); // dodaje se item
            }
            goldText.text = GameManager.instance.currentGold.ToString() + "g"; //azurira se stanje racuna

        }
    }

    public void SellItem() //prodaja itema
    {
        if (selectedItem != null)
        {
            GameManager.instance.currentGold += Mathf.FloorToInt(selectedItem.value * .5f); //azurira se stanje racuna (itemu opada vrijednost)
            GameManager.instance.RemoveItem(selectedItem.itemName); //uklanja se item iz inventara
        }
        goldText.text = GameManager.instance.currentGold.ToString() + "g"; //azurira se stanje racuna

        ShowSellItems(); //azurira se prikaz inventara
    }
}
