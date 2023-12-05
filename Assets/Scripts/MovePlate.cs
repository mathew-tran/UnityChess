using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A plate that is spawned to allow the player to move and attack
public class MovePlate : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject controller;

    GameObject chessPieceReference = null;

    public int matrixX;
    public int matrixY;

    public bool attack = false;
    // false is movement, true is attacking

    public void Start() {
        if(attack)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    public static void ChoosePlateHidden(Chessman piece, int x, int y, bool confirm = false, bool startNextTurn = true)
    {
        Game controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
        string enemy = "";
        CommandLog log = new CommandLog();
        bool isAttack = false;
        
            GameObject cp = controller.GetComponent<Game>().GetPosition(x, y);

            if(cp)
            {

                enemy = cp.GetComponent<Chessman>().GetChessInfoString();
                if (cp.GetComponent<Chessman>().pieceType == ENUM_PIECES_TYPE.KING)
                {
                    if(cp.GetComponent<Chessman>().team == ENUM_TEAM.BLACK)
                    {
                        controller.GetComponent<Game>().Winner(ENUM_TEAM.WHITE);
                    }
                    else
                    {
                        controller.GetComponent<Game>().Winner(ENUM_TEAM.BLACK);
                    }
                }
                GameObject deathParticle = GameObject.FindGameObjectWithTag("DeathParticle");
                deathParticle.transform.position = new Vector3(cp.transform.position.x, cp.transform.position.y, deathParticle.transform.position.z);
                //deathParticle.GetComponent<ParticleSystem>().Play();
                cp.GetComponent<Chessman>().SetIsInPlay(false);
                log.deadPiece = cp.GetComponent<Chessman>();
                isAttack = true;
            }
           

        controller.GetComponent<Game>().SetPositionEmpty(
        piece.GetXBoard(),
        piece.GetYBoard());

        log.chesspiece = piece;

        Vector2 oldPosition = new Vector2(piece.GetXBoard(), piece.GetYBoard());
        log.oldPosition = oldPosition;

        piece.SetXBoard(x);
        piece.SetYBoard(y);       

        Vector2 newPosition = new Vector2(piece.GetXBoard(), piece.GetYBoard());
        log.newPosition = newPosition;

        string message = piece.GetChessInfoString() + " moves from " + ConvertVectorToChessCoord(oldPosition) + 
        " to " + ConvertVectorToChessCoord(newPosition);
        Game.VerboseLog(message);

        if(enemy.Length > 0)
        {
           Game.VerboseLog(enemy + " destroyed by: " + piece.GetChessInfoString());
        }

        //Add this to log to enable rewind.
        History.Instance.AddLog(log);
        
        controller.GetComponent<Game>().SetPosition(piece.gameObject);
        if(confirm)
        {
            piece.SetCoords();
            if(startNextTurn)
            {
                controller.GetComponent<Game>().NextTurn();
                controller.GetComponent<Game>().PlaySound(isAttack);
            }
            piece.IsFirstTime = false;
        }

        Chessman.DestroyMovePlates();
        GameObject.FindGameObjectWithTag("SelectedPlate").transform.position = new Vector3(-300, -300, 0);
    }
    public void ChoosePlate(bool confirm = false)
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        string enemy = "";
        CommandLog log = new CommandLog();
        
        if (attack)
        {
            GameObject cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY);

            enemy = cp.GetComponent<Chessman>().GetChessInfoString();
            if (cp.GetComponent<Chessman>().pieceType == ENUM_PIECES_TYPE.KING)
            {
                if(cp.GetComponent<Chessman>().team == ENUM_TEAM.BLACK)
                {
                    controller.GetComponent<Game>().Winner(ENUM_TEAM.WHITE);
                }
                else
                {
                    controller.GetComponent<Game>().Winner(ENUM_TEAM.BLACK);
                }
            }
            GameObject deathParticle = GameObject.FindGameObjectWithTag("DeathParticle");
            deathParticle.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, deathParticle.transform.position.z);
            //deathParticle.GetComponent<ParticleSystem>().Play();
            cp.GetComponent<Chessman>().SetIsInPlay(false);
            log.deadPiece = cp.GetComponent<Chessman>();
           
        }

        Chessman movingChessman = chessPieceReference.GetComponent<Chessman>();
        controller.GetComponent<Game>().SetPositionEmpty(
        movingChessman.GetXBoard(),
        movingChessman.GetYBoard());

        log.chesspiece = movingChessman;

        Vector2 oldPosition = new Vector2(movingChessman.GetXBoard(), movingChessman.GetYBoard());
        log.oldPosition = oldPosition;

        movingChessman.SetXBoard(matrixX);
        movingChessman.SetYBoard(matrixY);
        movingChessman.SetCoords();

        Vector2 newPosition = new Vector2(movingChessman.GetXBoard(), movingChessman.GetYBoard());
        log.newPosition = newPosition;

        string message = movingChessman.GetChessInfoString() + " moves from " + ConvertVectorToChessCoord(oldPosition) + 
        " to " + ConvertVectorToChessCoord(newPosition);
        Game.VerboseLog(message);

        if(enemy.Length > 0)
        {
           Game.VerboseLog(enemy + " destroyed by: " + movingChessman.GetChessInfoString());
        }

        //Add this to log to enable rewind.
        History.Instance.AddLog(log);

        controller.GetComponent<Game>().SetPosition(chessPieceReference);

        if(confirm)
        {
            controller.GetComponent<Game>().NextTurn();
            movingChessman.IsFirstTime = false;
            controller.GetComponent<Game>().PlaySound(attack);
        }

        Chessman.DestroyMovePlates();
        GameObject.FindGameObjectWithTag("SelectedPlate").transform.position = new Vector3(-300, -300, 0);
    }

    public static string ConvertVectorToChessCoord(Vector2 position)
    {
        string message = "(";
        message += (Mathf.FloorToInt(position.y)+ 1).ToString();
        message += (char)('A' + System.Convert.ToChar(Mathf.FloorToInt(position.x)));  
        message += ")";

        return message;
    }
    public void OnMouseUp() {
        ChoosePlate(true);
    }

    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    public void SetChessPieceReference(GameObject obj) {
        {
            chessPieceReference = obj;
        }
    }
}
