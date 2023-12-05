using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Graveyard : MonoBehaviour
{
    public ENUM_TEAM teamToWatch;
    public Transform uiParent;
    public GameObject graveyardItem;

    public Game gameReference;
    // Start is called before the first frame update

    public void UpdateGraveyard()
    {
        gameReference = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
        foreach (Transform child in uiParent) 
        {
            GameObject.Destroy(child.gameObject);
        }

        GameObject[] piecesToDisplay;
        if(teamToWatch == ENUM_TEAM.BLACK)
        {
            piecesToDisplay = gameReference.playerBlackPieces;
        }
        else
        {
            piecesToDisplay = gameReference.playerWhitePieces;
        }
        
        foreach(GameObject piece in piecesToDisplay)
        {
            if(piece)
            {
                Chessman cp = piece.GetComponent<Chessman>();
                if(cp)
                {
                    if(!cp.IsInPlay && cp.pieceType != ENUM_PIECES_TYPE.NULL)
                    {
                        GameObject obj = Instantiate(graveyardItem, Vector3.zero, Quaternion.identity);
                        obj.transform.SetParent(uiParent);
                        obj.GetComponent<RawImage>().texture = cp.GetComponent<SpriteRenderer>().sprite.texture;
                    }
                }
            }
        }
    }
    
}
