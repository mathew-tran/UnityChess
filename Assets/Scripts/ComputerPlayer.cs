using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    public static void Shuffle<T>(this IList<T> ts) 
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) 
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}
public enum DIFFICULTY
{
    EASY,
    NORMAL,
    SMART
}

// A struct to help determine if a piece should do a move, the piece reference, is the acting piece, 
// and the higher the score, the more likely a move will be used
public struct PossibleMove
{
    public Chessman piece;
    public int x;
    public int y;
    public int score;

}

// A bucket for each piece, all the moves they have available to use
public struct PossibleMovesForPiece
{
    public Chessman piece;
    public List<PossibleMove> moves;
}

// An AI controlled player
public class ComputerPlayer : MonoBehaviour
{
    public List<PossibleMovesForPiece> possibleMoves;
    PossibleMove chosenMove;

    GameObject gameReference;
    
    [Header("AI Settings")]
    public float chooseTime = 0.2f;
    public float moveTime = 0.5f;
    public int maxDepth = 1;

    public DIFFICULTY playerDifficulty;


    void Start()
    {
        gameReference = GameObject.FindGameObjectWithTag("GameController");
        possibleMoves = new List<PossibleMovesForPiece>();
    }

    public void GetPossibleMoves()
    {
        GameObject[] chessPieces = new GameObject[1];
        switch(gameReference.GetComponent<Game>().GetCurrentPlayer())
        {
            case ENUM_TEAM.BLACK:
            chessPieces = gameReference.GetComponent<Game>().playerBlackPieces;
            break;

            case ENUM_TEAM.WHITE:
            chessPieces = gameReference.GetComponent<Game>().playerWhitePieces;
            break;

            default:
                throw new System.Exception("ComputerPlayer.GetPossibleMoves current player did not have a valid team");
        }
        
        possibleMoves.Clear();
        foreach(var piece in chessPieces)
        {
            if (piece)
            {
                Chessman chesspiece = piece.GetComponent<Chessman>();
                if(chesspiece)
                {
                    if(chesspiece.IsInPlay)
                    {
                        PossibleMovesForPiece pfp = new PossibleMovesForPiece();
                        pfp.piece = chesspiece;
                        pfp.moves = chesspiece.GivePossibleMoves();
                        if(pfp.moves.Count != 0)
                        {
                            possibleMoves.Add(pfp);
                        }
                    }
                }
            }
        }
        
    }

    public void GetPossibleMoves(ENUM_TEAM team)
    {
        GameObject[] chessPieces = new GameObject[1];
        switch(team)
        {
            case ENUM_TEAM.BLACK:
            chessPieces = gameReference.GetComponent<Game>().playerBlackPieces;
            break;

            case ENUM_TEAM.WHITE:
            chessPieces = gameReference.GetComponent<Game>().playerWhitePieces;
            break;

            default:
                throw new System.Exception("ComputerPlayer.GetPossibleMoves current player did not have a valid team");
        }
        
        possibleMoves.Clear();
        foreach(var piece in chessPieces)
        {
            if (piece)
            {
                Chessman chesspiece = piece.GetComponent<Chessman>();
                if(chesspiece)
                {
                    if(chesspiece.IsInPlay)
                    {
                        PossibleMovesForPiece pfp = new PossibleMovesForPiece();
                        pfp.piece = chesspiece;
                        pfp.moves = chesspiece.GivePossibleMoves();
                        if(pfp.moves.Count != 0)
                        {
                            possibleMoves.Add(pfp);
                        }
                    }
                }
            }
        }
        
    }
    public PossibleMove SimpleMove()
    {
        PossibleMove move = new PossibleMove();

        move.score = -10;

        Helper.Shuffle(possibleMoves);
        foreach(var testmoveList in possibleMoves)
        {
            Helper.Shuffle(testmoveList.moves);
            foreach(var testmove in testmoveList.moves)
            {
                if(testmove.piece)
                {
                    if (move.score <= testmove.score)
                    {
                        move = testmove;
                    }
                }
            }
        }
        if (move.score == -10)
        {
            Debug.LogError("Could not find a move for computer");
        }
        return move;
    }
    public IEnumerator ChooseSimpleMove()
    {
        yield return new WaitForSeconds(chooseTime);
        GetPossibleMoves();
        yield return new WaitForSeconds(chooseTime);
        chosenMove = SimpleMove();
        chosenMove.piece.ShowMovePlates();
        
        Game.VerboseLog(gameReference.GetComponent<Game>().GetCurrentPlayer() + " Deciding...");
        yield return new WaitForSeconds(moveTime);
        Game.VerboseLog(gameReference.GetComponent<Game>().GetCurrentPlayer() + " Playing...");
        GameObject[] plates = GameObject.FindGameObjectsWithTag("MovePlate");
        foreach(var plate in plates)
        {
            MovePlate mp = plate.GetComponent<MovePlate>();
            if(mp)
            {               
                if(mp.matrixX == chosenMove.x && mp.matrixY == chosenMove.y)
                {
                     mp.ChoosePlate();    
                     break;                
                }
            }
        }
        yield return new WaitForSeconds(.1f);
    }

    // Simulates board, and then goes back a turn.
    public IEnumerator DoAdvancedMove()
    {
        Game controller = gameReference.GetComponent<Game>();
        yield return new WaitForSeconds(0.01f);
        GetPossibleMoves();

        PossibleMove bestMoveSoFar = new PossibleMove();

        Helper.Shuffle(possibleMoves);
        bestMoveSoFar = new PossibleMove();
        bestMoveSoFar.score = -10000;
        // Simulate through all possible moves, then find the one with the best score and go with that.
         for(int i = 0; i < possibleMoves.Count; ++i)
         {
             foreach(var move in possibleMoves[i].moves)
            {
                if(move.piece != null)
                {
                    // Simulate move
                    MovePlate.ChoosePlateHidden(move.piece, move.x, move.y);
                    int score = controller.GetComponent<Game>().EvaluateBoard(controller.GetComponent<Game>().GetCurrentPlayer());
                    //yield return new WaitUntil(() => History.Instance.DoRewind == false);
                    
                    History.Instance.Rewind(false);

                    if(bestMoveSoFar.piece == null)
                    {
                        bestMoveSoFar = move;
                        bestMoveSoFar.score = score;
                        bestMoveSoFar.piece = move.piece;
                    }
                    else
                    {
                        if(bestMoveSoFar.score < score)
                        {
                            bestMoveSoFar = move;
                        }
                    }  
                }       
            
            }
         }

        yield return new WaitForSeconds(chooseTime);
        bestMoveSoFar.piece.ShowMovePlates();
        yield return new WaitForSeconds(moveTime);
        MovePlate.ChoosePlateHidden(bestMoveSoFar.piece, bestMoveSoFar.x, bestMoveSoFar.y, true);
        
        //yield return new WaitForSeconds(.1f);

    }
    public int EvaluateBoard(ENUM_TEAM player, bool GetCurrentPlayer = false)
    {
        Game controller = gameReference.GetComponent<Game>();
        if(GetCurrentPlayer)
        {
            controller.GetComponent<Game>().EvaluateBoard(controller.GetComponent<Game>().GetCurrentPlayer());
        }
        return controller.GetComponent<Game>().EvaluateBoard(player);
    }

    public List<PossibleMove> GetAllMoves(ENUM_TEAM team)
    {
        List<PossibleMove> moves = new List<PossibleMove>();
        GetPossibleMoves(team);
        foreach(PossibleMovesForPiece piece in possibleMoves)
        {
            foreach(PossibleMove move in piece.moves)
            {
                moves.Add(move);
            }
        }
        Helper.Shuffle(moves);
        return moves;
    }

    // This code is taken from  https://github.com/SkylerAlvarez/Chess-AI-Unity/blob/master/Assets/Scripts/AlphaBeta.cs and included the MIT license in source because of it. Just to be safe.
    int AB(int depth, int alpha, int beta, bool max, ENUM_TEAM currentPlayer, ENUM_TEAM opposingPlayer)
    {
        if (depth == 0)
        {
            return EvaluateBoard(currentPlayer); // Actually just gets current player
        }
        if (max)
        {
            int score = -10000000;
            List<PossibleMove> allMoves = GetAllMoves(currentPlayer);
            
            //Helper.Shuffle(allMoves);
            
            for(int i = 0; i < allMoves.Count; ++i) 
            {
                PossibleMove move = allMoves[i];
                
                MovePlate.ChoosePlateHidden(move.piece, move.x, move.y);

                score = AB(depth - 1, alpha, beta, false, currentPlayer, opposingPlayer);

                History.Instance.Rewind(false);


                if (score > alpha)
                {
                    move.score = score;
                    if (move.score > chosenMove.score && depth == maxDepth)
                    {
                        chosenMove = move;
                    }
                    alpha = score;
                }
                if (score >= beta)
                {
                    break;
                }
            }
            return alpha;
        }
        else
        {
            int score = 10000000;
            List<PossibleMove> allMoves = GetAllMoves(opposingPlayer);
            for(int i = 0; i < allMoves.Count; ++i) 
            {
                PossibleMove move = allMoves[i];

                MovePlate.ChoosePlateHidden(move.piece, move.x, move.y);

                score = AB(depth - 1, alpha, beta, true, currentPlayer, opposingPlayer);

                History.Instance.Rewind(false);

                if (score < beta)
                {
                    move.score = score;
                    beta = score;
                }
                if (score <= alpha)
                {
                    break;
                }
            }
            return beta;
        }
    }

    public IEnumerator MakeMove()
    {
        
        yield return new WaitForSeconds(chooseTime);
        chosenMove = new PossibleMove();
        chosenMove.score = -100000;

        int depth = maxDepth;

//        depth = modifyDepth(playerDifficulty);

        if(shouldRandom(playerDifficulty))
        {
            GetPossibleMoves();
            Helper.Shuffle(possibleMoves);
            chosenMove = possibleMoves[0].moves[0];
        }        
        
        
        if(gameReference.GetComponent<Game>().GetCurrentPlayer() == ENUM_TEAM.BLACK)
        {
            Debug.Log("Team black attack");
            AB(depth, -100000000, 1000000000, true, ENUM_TEAM.BLACK, ENUM_TEAM.WHITE);
        }
        else
        {
            Debug.Log("Team white attack");
            AB(depth, -100000000, 1000000000, true, ENUM_TEAM.WHITE, ENUM_TEAM.BLACK);
        }
        
        
        yield return new WaitForSeconds(chooseTime);
        chosenMove.piece.ShowMovePlates();
        yield return new WaitForSeconds(moveTime);
        MovePlate.ChoosePlateHidden(chosenMove.piece, chosenMove.x, chosenMove.y, true);    
    }

    public int modifyDepth(DIFFICULTY playerdifficulty)
    {
        int result = Random.Range(0, 100);
        int depth = maxDepth;
        switch(playerDifficulty)
        {
            case DIFFICULTY.EASY:
                if(result < 80)
                {
                    depth = Random.Range(1, maxDepth);
                }
                break;

            case DIFFICULTY.NORMAL:
                if(result < 40)
                {
                    depth = Random.Range(1, maxDepth);
                }
            break;

            case DIFFICULTY.SMART:
                if(result < 20)
                {
                    depth = Random.Range(1, maxDepth);
                }
            break;
        }
        if (depth < 1)
        {
            depth = 1;
        }

        return depth;
    }

    public bool shouldRandom(DIFFICULTY playerDifficulty)
    {
        return false;
    }

    public void DoTurn()
    {
        StopCoroutine(MakeMove());
        StartCoroutine(MakeMove());        
    }
}
