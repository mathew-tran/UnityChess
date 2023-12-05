using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Created in the shop list, it's the UI items you use to buy
public class ShopItem : MonoBehaviour
{
    public ENUM_PIECES_TYPE type;

    public GameObject image;
    public int index = -1;

    public int price = 0;
    public Text text;

    public GameObject buttonObject;
    public ShopManager managerReference;
    public ShopList listReference;

    public void UpdateItem(ENUM_PIECES_TYPE ItemType, int itemPrice, Sprite sprite)
    {
        image.GetComponent<Image>().sprite = sprite;
        price = itemPrice;
        type = ItemType;
        text.text = $"PURCHASE FOR ${price.ToString()}";
        buttonObject.GetComponent<Button>().onClick.AddListener( ()=> Purchase());
        //image.GetComponent<
    }
    public void Purchase()
    {
        if(managerReference.MakeTransaction(-price))
        {
            Game gameReference = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
            
            if(index < gameReference.TeamWhite.secondRow.Length)
            {
                gameReference.TeamWhite.secondRow[index] = type;
            }
            else
            {
                gameReference.TeamWhite.firstRow[index - gameReference.TeamWhite.secondRow.Length] = type;
            }
            Debug.Log("TEST");

            managerReference.ShowPlayerTeam();
            listReference.CloseShopList();

        }
    }
}
