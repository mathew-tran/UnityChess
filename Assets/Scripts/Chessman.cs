using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// The number will correlate to the index the sprite should be in.
public enum ENUM_PIECES_TYPE {

    NULL = -100,
    PAWN = 0,
    ROOK = 1,

    KNIGHT = 2,
    BISHOP = 3,
    QUEEN = 4,
    KING = 5,
}
public enum ENUM_PIECES_SCORE
{
    PAWN = 10,
    ROOK = 30,

    KNIGHT = 50,
    BISHOP = 80,
    QUEEN = 300,
    KING = 1000
}
[SerializeField]
public class Chessman : MonoBehaviour
{
    public GameObject controllerObject;
    public GameObject movePlate;

    private int xBoard = -1;
    private int yBoard = -1;
    public ENUM_TEAM team;

    public ENUM_PIECES_TYPE pieceType;

    public Sprite[] blackSprites;
    public Sprite[] whiteSprites;

    public bool IsInPlay = true;
    public bool IsFirstTime = true;

    public bool bHeadless = false;

    public static int SpriteIndex(ENUM_PIECES_TYPE type)
    {
        switch(type)
        {
            case ENUM_PIECES_TYPE.PAWN:
            return 0;            

            case ENUM_PIECES_TYPE.ROOK:
            return 1;            

            case ENUM_PIECES_TYPE.KNIGHT:
            return 2;    


            case ENUM_PIECES_TYPE.BISHOP:
            return 3;        

            case ENUM_PIECES_TYPE.QUEEN:
            return 4;
            
            case ENUM_PIECES_TYPE.KING:
            return 5;
            
            default:
                return -1;   
    }
    }
    //Activates the piece and puts it in play.
    public void ActivatePiece()
    {
        
        IsInPlay = true;
        SetCoords();
        if(pieceType == ENUM_PIECES_TYPE.NULL)
        {
            SetHieararchyGrouping(false);
            SetIsInPlay(false);                  
        }
        else
        {
            controllerObject = GameObject.FindGameObjectWithTag("GameController");

            SetCoords();

            if(!bHeadless)
            {
                switch(team)
                {
                    case ENUM_TEAM.BLACK:
                        this.GetComponent<SpriteRenderer>().sprite = blackSprites[(int)pieceType];
                        break;

                    case ENUM_TEAM.WHITE:
                        this.GetComponent<SpriteRenderer>().sprite = whiteSprites[(int)pieceType];
                        break;

                    default:
                        throw new System.Exception("Chessman.Activate used an invalid team");
                }
            }
            SetHieararchyGrouping(true);
        }
    }
    //Used to move objects in the inspector so it's easier to see what pieces are active or not.
    public void SetHieararchyGrouping(bool isActive = false)
    {
        if(isActive)
        {
            if(GameObject.FindGameObjectWithTag("ActivePieces") == null)
            {
                GameObject parentObj = new GameObject("_ActivePieces");
                parentObj.tag = "ActivePieces";
            }
            GameObject obj = GameObject.FindGameObjectWithTag("ActivePieces");
            gameObject.transform.SetParent(obj.transform);
        }
        else
        {
            if(GameObject.FindGameObjectWithTag("InActivePieces") == null)
            {
                GameObject parentObj = new GameObject("_InActivePieces");
                parentObj.tag = "InActivePieces";
            }
            GameObject obj = GameObject.FindGameObjectWithTag("InActivePieces");
            gameObject.transform.SetParent(obj.transform);
        }
    }

    public int GetScore()
    {
        switch(pieceType)
        {
            case ENUM_PIECES_TYPE.PAWN:
            return (int)ENUM_PIECES_SCORE.PAWN;            

            case ENUM_PIECES_TYPE.ROOK:
            return(int)ENUM_PIECES_SCORE.ROOK;            

            case ENUM_PIECES_TYPE.KNIGHT:
            return (int)ENUM_PIECES_SCORE.KNIGHT;            

            case ENUM_PIECES_TYPE.QUEEN:
            return (int)ENUM_PIECES_SCORE.QUEEN;
            
            case ENUM_PIECES_TYPE.BISHOP:
            return (int)ENUM_PIECES_SCORE.BISHOP;
            
            case ENUM_PIECES_TYPE.KING:
            return (int)ENUM_PIECES_SCORE.KING;
            
            default:
                throw new System.Exception("Chessman.SetAttackPlate attacked piece type is not using a valid ENUM");    
        }
    }

    public void SetIsInPlay(bool state)
    {
        IsInPlay = state;
        if(IsInPlay)
        {
            SetCoords();
        }
        else
        {
            SetOffBoardCoords();
        }
    }

    public void SetOffBoardCoords()
    {
        this.transform.position = new Vector3(-200.0f,-200.0f, -1.0f);
    }
    public void SetCoords()
    {
        float x = xBoard;
        float y = yBoard;

        x = ApplyLocalChessCoordinate(x);
        y = ApplyLocalChessCoordinate(y);

        this.transform.position = new Vector3(x,y, -1.0f);
    }

    public float ApplyLocalChessCoordinate(float val)
    {
        val *= Game.OFFSET_MULTIPLIER;
        val += Game.OFFSET_CONSTANT;
        return val;
    }

    public int GetXBoard() { return xBoard; }
    public int GetYBoard() { return yBoard; }

    public void SetXBoard(int x) { xBoard = x;}
    public void SetYBoard(int y) { yBoard = y;}

    public void ShowMovePlates()
    {
        if(!controllerObject.GetComponent<Game>().IsGameOver() && controllerObject.GetComponent<Game>().GetCurrentPlayer() == team)
        {
            GameObject.FindGameObjectWithTag("SelectedPlate").transform.position = new Vector3(-300, -300, 0);
            DestroyMovePlates();
            InitiateMovePlates();
            GameObject.FindGameObjectWithTag("SelectedPlate").transform.position = new Vector3(this.transform.position.x, transform.transform.position.y, 0);
        }
    }
    public List<PossibleMove> GivePossibleMoves()
    {
        List<PossibleMove> moves = new List<PossibleMove>();
        moves = InitiateMovePlates();
        return moves;
        
    }
    private void OnMouseUp() {
        ShowMovePlates();
    }

    public static void DestroyMovePlates()
    {
        GameObject [] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for(int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]);
        }
    }

    public List<PossibleMove> InitiateMovePlates()
    {
        List<PossibleMove> moves = new List<PossibleMove>();
        switch(this.pieceType)
        {
            case ENUM_PIECES_TYPE.BISHOP:
                moves.AddRange(LineMovePlate(-1, 1));
                moves.AddRange(LineMovePlate(1, -1));
                moves.AddRange(LineMovePlate(-1, -1));
                moves.AddRange(LineMovePlate(1, 1));
                break;

            case ENUM_PIECES_TYPE.QUEEN:
                moves.AddRange(LineMovePlate(1, 0));
                moves.AddRange(LineMovePlate(0, 1));

                moves.AddRange(LineMovePlate(-1, 0));
                moves.AddRange(LineMovePlate(0, -1));

                moves.AddRange(LineMovePlate(-1, 1));
                moves.AddRange(LineMovePlate(1, -1));

                moves.AddRange(LineMovePlate(-1, -1));
                moves.AddRange(LineMovePlate(1, 1));
                break;

            case ENUM_PIECES_TYPE.KING:
                moves = SurroundMovePlate();
                break;

            case ENUM_PIECES_TYPE.KNIGHT:
                moves = LMovePlate();
                break;

            case ENUM_PIECES_TYPE.ROOK:
                // RIGHT
                moves.AddRange(LineMovePlate(1, 0));
                // UP 
                moves.AddRange(LineMovePlate(0, 1));

                // LEFT
                moves.AddRange(LineMovePlate(-1, 0));

                // DOWN
                moves.AddRange(LineMovePlate(0, -1));
                break;

            case ENUM_PIECES_TYPE.PAWN:                
                if(team == ENUM_TEAM.BLACK)
                {
                    moves = PawnMovePlate(xBoard, yBoard - 1);
                    if (IsFirstTime)
                    {
                        moves.Add(PointMovePlate(xBoard, yBoard - 2, false));                        
                    }
                }
                else
                {
                    moves = PawnMovePlate(xBoard, yBoard + 1);
                    if (IsFirstTime)
                    {
                        moves.Add(PointMovePlate(xBoard, yBoard + 2, false));                        
                    }
                }
                
                break;
            default:
                throw new System.Exception("Chessman.InitiateMovePlates did not use a valid piece type ENUM");

        }

        // if piece is null for some reason, remove it, because the move probably wasn't that important -- may want to provide a better readon
        moves.RemoveAll(x => x.piece == null);
        return moves;
    }

    public List<PossibleMove> LineMovePlate(int xIncrement, int yIncrement)
    {
        List<PossibleMove> moves = new List<PossibleMove>();

        Game sc = controllerObject.GetComponent<Game>();
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while (sc.PositionOnBoard(x,y) && sc.GetPosition(x,y) == null)
        {
            moves.Add(MovePlateSpawn(x,y, false));
            x += xIncrement;
            y += yIncrement;
        }
        if (sc.PositionOnBoard(x,y) && sc.GetPosition(x,y).GetComponent<Chessman>().team != team)
        {
            moves.Add(SetAttackPlate(new Vector2Int(x,y)));
        }
        return moves;
    }
    
    public List<PossibleMove> LMovePlate()
    {
        List<PossibleMove> moves = new List<PossibleMove>();

        moves.Add(PointMovePlate(xBoard + 1, yBoard + 2));
        moves.Add(PointMovePlate(xBoard - 1, yBoard + 2));

        moves.Add(PointMovePlate(xBoard + 2, yBoard + 1));
        moves.Add(PointMovePlate(xBoard + 2, yBoard - 1));

        moves.Add(PointMovePlate(xBoard + 1, yBoard - 2));
        moves.Add(PointMovePlate(xBoard - 1, yBoard - 2));

        moves.Add(PointMovePlate(xBoard - 2, yBoard - 1));
        moves.Add(PointMovePlate(xBoard - 2, yBoard + 1));

        return moves;
    }
    
    public List<PossibleMove> SurroundMovePlate()
    {
        List<PossibleMove> moves = new List<PossibleMove>();

        moves.Add(PointMovePlate(xBoard, yBoard + 1));
        moves.Add(PointMovePlate(xBoard, yBoard - 1));

        moves.Add(PointMovePlate(xBoard + 1, yBoard - 1));
        moves.Add(PointMovePlate(xBoard + 1, yBoard + 1));

        moves.Add(PointMovePlate(xBoard + 1, yBoard));
        moves.Add(PointMovePlate(xBoard - 1, yBoard));

        moves.Add(PointMovePlate(xBoard - 1, yBoard - 1));
        moves.Add(PointMovePlate(xBoard - 1, yBoard + 1));

        return moves;
    }

    public PossibleMove PointMovePlate(int x, int y, bool canAttack = true)
    {
        PossibleMove move = new PossibleMove();
        
        Game sc = controllerObject.GetComponent<Game>();
        if(sc.PositionOnBoard(x,y))
        {
            GameObject cp = sc.GetPosition(x,y);

            if(cp == null)
            {
                move = MovePlateSpawn(x,y, false);
            }
            else if(cp.GetComponent<Chessman>().team != team)
            {
                if(canAttack)
                {
                    move = SetAttackPlate(new Vector2Int(x,y));
                }
            }

        }
        return move;
    }

    public List<PossibleMove> PawnMovePlate(int x, int y)
    {
        List<PossibleMove> moves = new List<PossibleMove>();
        Game sc = controllerObject.GetComponent<Game>();
        if (sc.PositionOnBoard(x,y))
        {
            if(sc.GetPosition(x,y) == null)
            {
                moves.Add(MovePlateSpawn(x,y, false));
            }

            moves.Add(SetAttackPlate(new Vector2Int(x + 1, y)));
            moves.Add(SetAttackPlate(new Vector2Int(x - 1, y)));                     
        }
        return moves; 
    }

    // Specifies the score, when piece can attack, call this.
    public PossibleMove SetAttackPlate(Vector2Int vector)
    {
        PossibleMove move = new PossibleMove();
        Game sc = controllerObject.GetComponent<Game>();
        if(sc.PositionOnBoard(vector.x, vector.y) && sc.GetPosition(vector.x, vector.y) != null && sc.GetPosition(vector.x, vector.y).GetComponent<Chessman>().team != team)
        {
            move = MovePlateSpawn(vector.x, vector.y, true);
            move.score = sc.GetPosition(vector.x, vector.y).GetComponent<Chessman>().GetScore();
        }
        return move;
    }

    public string GetChessInfoString()
    {
        return team.ToString() + " " + pieceType.ToString();
    }

    public PossibleMove MovePlateSpawn(int matrixX, int matrixY, bool isAttack = true)
    {
        PossibleMove pm = new PossibleMove();
        pm.x = matrixX;
        pm.y = matrixY;
        
        if(this != null)
        {
            pm.piece = this;
            float x = matrixX;
            float y = matrixY;

            x = ApplyLocalChessCoordinate(x);
            y = ApplyLocalChessCoordinate(y);
            
            GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);
            MovePlate mpScript = mp.GetComponent<MovePlate>();
            mp.transform.SetParent(this.transform);

            if(isAttack)
            {
                mpScript.attack = true;
            }
            
            mpScript.SetChessPieceReference(gameObject);
            mpScript.SetCoords(matrixX, matrixY);
        }
        return pm;
    }

}
