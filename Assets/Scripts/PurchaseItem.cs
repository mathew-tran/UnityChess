using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseItem : MonoBehaviour
{
    public GameObject buyButton;
    public GameObject sellButton;

    public GameObject purchaseImage;


    public ShopManager shopReference;

    public GameObject shopItemUI;


    public int index = -1;
    // Start is called before the first frame update

    public void DisableSellButton()
    {
        sellButton.SetActive(false);
    }
    public void DisableBuyButton()
    {
        buyButton.SetActive(false);
    }

    public void DisableImage()
    {
        purchaseImage.SetActive(false);
    }
    
    public void Sell()
    {
        Game gameReference = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
        int refundAmount = 0;
        if(index < gameReference.TeamWhite.secondRow.Length)
        {
            refundAmount += GetCost(gameReference.TeamWhite.secondRow[index]);
            gameReference.TeamWhite.secondRow[index] = ENUM_PIECES_TYPE.NULL;            
        }
        else
        {
            refundAmount += GetCost(gameReference.TeamWhite.firstRow[index - gameReference.TeamWhite.secondRow.Length]);
            gameReference.TeamWhite.firstRow[index - gameReference.TeamWhite.secondRow.Length] = ENUM_PIECES_TYPE.NULL;            
        }

        shopReference.MakeTransaction(refundAmount);

        shopReference.ShowPlayerTeam();
        
        
    }

    public static int GetCost(ENUM_PIECES_TYPE type)
    {
        switch(type)
        {
            case ENUM_PIECES_TYPE.PAWN:
            return 1;            

            case ENUM_PIECES_TYPE.ROOK:
            return 3;            

            case ENUM_PIECES_TYPE.KNIGHT:
            return 2;

            case ENUM_PIECES_TYPE.BISHOP:
            return 2;        

            case ENUM_PIECES_TYPE.QUEEN:
            return 4;

            
            default:
                return -1;   
        }
    }
}
