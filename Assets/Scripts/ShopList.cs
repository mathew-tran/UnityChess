using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This is spawned up when you click buy on an open position in the chess board
public class ShopList : MonoBehaviour
{
    public Transform uiParent;

    public GameObject shopItemReference;

    public ShopManager managerReference;

    public int chosenIndex;

    // Start is called before the first frame update
    public void OpenShopList(int index)
    {
        chosenIndex = index;
        managerReference = GameObject.FindGameObjectWithTag("ShopManager").GetComponent<ShopManager>();

        gameObject.SetActive(true);
        foreach (Transform child in uiParent) 
        {
            GameObject.Destroy(child.gameObject);
        }

        ENUM_PIECES_TYPE[] typesToSell = {
            ENUM_PIECES_TYPE.PAWN,
            ENUM_PIECES_TYPE.KNIGHT,
            ENUM_PIECES_TYPE.ROOK,
            ENUM_PIECES_TYPE.QUEEN            
        };

        foreach(var type in typesToSell)
        {
            CreateShopItem(type);
        }
    }
    public void CloseShopList()
    {
        gameObject.SetActive(false);
    }

    public void CreateShopItem(ENUM_PIECES_TYPE type)
    {
        GameObject obj = Instantiate(shopItemReference, Vector3.zero, Quaternion.identity, uiParent);
        ShopItem item = obj.GetComponent<ShopItem>();
        item.UpdateItem(type,
            PurchaseItem.GetCost(type),
            managerReference.instancedChessmanReference.GetComponent<Chessman>().whiteSprites[Chessman.SpriteIndex(type)]
        );
        item.index = chosenIndex;
        item.managerReference = managerReference;
        item.listReference = this;


        
    }
}
