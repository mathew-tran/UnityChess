using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Shows which pieces you have on your team, creates purchase items for each object, whether you can buy / sell items
public class ShopManager : MonoBehaviour
{
    // Start is called before the first frame update

    public Game gameReference;
    public Transform uiParent;
    
    public GameObject purchaseItem;
    public GameObject chessManReference;
    public GameObject instancedChessmanReference;
    public GameObject nextRoundButton;

    public GameObject moneyUI;
    public ShopList shopList;


    public int Money = 0;

    private void Start()
    {
        MakeTransaction(0);
    }
    public void OpenShop()
    {
        gameObject.SetActive(true);
        ShowPlayerTeam();
        nextRoundButton.SetActive(true);
    }
    public void CloseShop()
    {
        nextRoundButton.SetActive(false);
        gameObject.SetActive(false);
    }
    public void NextRound()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>().Setup();
        CloseShop();
    }

    public bool MakeTransaction(int amount)
    {
        if(amount < 0)
        {
            if(amount + Money < 0)
            {
                return false;
            }
        }
        Money += amount;
        moneyUI.GetComponent<Text>().text = Money.ToString();
        return true;
    }

    public void ShowPlayerTeam()
    {
        nextRoundButton.GetComponent<Button>().onClick.RemoveAllListeners();
        nextRoundButton.GetComponent<Button>().onClick.AddListener( () => NextRound());
        foreach(Chessman obj in GameObject.FindObjectsOfType<Chessman>())
        {
            Destroy(obj.gameObject);
        }
        instancedChessmanReference = Instantiate(chessManReference);
        foreach (Transform child in uiParent) 
        {
            GameObject.Destroy(child.gameObject);
        }

        
        for(int i = 0; i < gameReference.TeamWhite.secondRow.Length; ++i)
        {
            CreatePieceOnShop(gameReference.TeamWhite.secondRow[i], i);
        }

        for(int i = 0; i < gameReference.TeamWhite.firstRow.Length; ++i)
        {
            CreatePieceOnShop(gameReference.TeamWhite.firstRow[i], i + gameReference.TeamWhite.secondRow.Length);
        }
    }
    

    public void CreatePieceOnShop(ENUM_PIECES_TYPE type, int teamIndex)
    {
        GameObject obj = Instantiate(purchaseItem, Vector3.zero, Quaternion.identity);
        obj.transform.SetParent(uiParent);

        PurchaseItem item = obj.GetComponent<PurchaseItem>();
        item.index = teamIndex;
        item.shopReference = this;

        item.buyButton.GetComponent<Button>().onClick.AddListener ( ()=> shopList.OpenShopList(teamIndex));
        item.sellButton.GetComponent<Button>().onClick.AddListener ( ()=> item.Sell());

        int index = Chessman.SpriteIndex(type);
        if(index != -1)
        {
            item.purchaseImage.GetComponent<Image>().sprite = instancedChessmanReference.GetComponent<Chessman>().whiteSprites[index];
            item.DisableBuyButton();
            if(type == ENUM_PIECES_TYPE.KING)
            {
               item.DisableSellButton();
            }
        }
        else
        {
            item.DisableImage();
            item.DisableSellButton();
        }
    }
}
