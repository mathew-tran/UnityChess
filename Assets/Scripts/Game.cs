using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class Game : MonoBehaviour
{
    public GameObject chessPiece;

    public GameObject[,] tilePositions = new GameObject[8,8];
    public GameObject[] playerBlackPieces = new GameObject[16];
    public GameObject[] playerWhitePieces = new GameObject[16];

    [Header("Local Variables")]
    private ENUM_TEAM currentPlayer = ENUM_TEAM.BLACK;
    private ENUM_TEAM winningPlayer = ENUM_TEAM.BLACK;
    private bool bIsGameOver = false;

    public GameObject winScreen;
    
    public ChallengeManager challengeManager;
    public ShopManager shopManager;
    public Team TeamWhite;
    public Team TeamBlack;

    [Header("Game Debug Settings")]
    public bool HasVerboseLogging = false;

    public int round = 0;
    public bool isTakingScreenshots = true;
    // Start is called before the first frame update


    public static float OFFSET_MULTIPLIER = 0.66f;
    public static float OFFSET_CONSTANT = -2.3f;

    public AudioClip moveAudioClip;
    public AudioClip attackAudioClip;

    public AudioSource audioSource;

    private void Start() 
    {
        if(challengeManager)
        {
            challengeManager.Setup();    
        }
    }

    public void SetupShop()
    {
        if(shopManager)
        {            
            shopManager.OpenShop();            
        }
    }

    public void CloseShop()
    {
        if(shopManager)
        {
            shopManager.CloseShop();
        }
    }
    public void Setup()
    {
        if(challengeManager)
        {
            challengeManager.StartChallenge();
        }
        round = 0;
        currentPlayer = ENUM_TEAM.BLACK; // doing this so white actually plays first.
        bIsGameOver = false;

        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text ="";
        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().text = "";
        UpdateUI();
        
        SetupTeams(); 

        if(isTakingScreenshots)
        {
            if(Directory.Exists("saved"))
            {
                FileUtil.DeleteFileOrDirectory("saved");
                Directory.CreateDirectory("saved");
            }
        }
        
        NextTurn();
    }

    public static void VerboseLog(string message)
    {
        Game gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
        if(gameController.HasVerboseLogging)
        {
            Debug.Log(message);
        }
    }
    public void SetupTeams()
    {
        tilePositions = new GameObject[8,8];
        foreach(Chessman obj in GameObject.FindObjectsOfType<Chessman>())
        {
            Destroy(obj.gameObject);
        }
        playerWhitePieces = TeamWhite.CreateTeam(this, 0);
        playerBlackPieces = TeamBlack.CreateTeam(this, 6);
    }

    public GameObject Create(ENUM_TEAM team, ENUM_PIECES_TYPE pieceType, int x, int y)
    {
        GameObject obj = Instantiate(chessPiece, new Vector3(0,0,-1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>();
        cm.team = team;
        cm.pieceType = pieceType;
        cm.SetXBoard(x);
        cm.SetYBoard(y);

        if(cm.pieceType != ENUM_PIECES_TYPE.NULL)
        {
            SetPosition(obj);
        }
        cm.ActivatePiece();
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();
        tilePositions[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

    public int EvaluateBoard(ENUM_TEAM team)
    {
        int score = 0;
        foreach(var piece in playerBlackPieces)
        {
            if(piece)
            {
                if(piece.GetComponent<Chessman>().IsInPlay)
                {
                    score += piece.GetComponent<Chessman>().GetScore();
                }
            }
        }
        foreach(var piece in playerWhitePieces)
        {
            if(piece)
            {
                if(piece.GetComponent<Chessman>().IsInPlay)
                {
                    score -= piece.GetComponent<Chessman>().GetScore();
                }
            }
        }
        if(team == ENUM_TEAM.WHITE)
        {
          score = -score;
        }
        return score;
    }

    public void SetPositionEmpty (int x, int y)
    {
        tilePositions[x, y] = null;
    }

    public GameObject GetPosition(int x, int y)
    {
        return tilePositions[x, y];
    }

    public bool PositionOnBoard(int x , int y)
    {
        if ( x < 0 || y < 0 || x >= tilePositions.GetLength(0) || y >= tilePositions.GetLength(1)) return false;
        return true;
    }

    public ENUM_TEAM GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public bool IsGameOver()
    {
        return bIsGameOver;
    }

    public void NextPlayer()
    {
        if(currentPlayer == ENUM_TEAM.BLACK)
        {
            currentPlayer = ENUM_TEAM.WHITE;
        }
        else
        {
            currentPlayer = ENUM_TEAM.BLACK;
        }
        UpdateUI();
    }
    public void UpdateGraveyard()
    {
        GameObject[] graveyards = GameObject.FindGameObjectsWithTag("Graveyard");
        foreach(GameObject graveyard in graveyards)
        {
            graveyard.GetComponent<Graveyard>().UpdateGraveyard();
        }
    }
    public void NextTurn()
    {
        UpdateGraveyard();
        if(isTakingScreenshots)
        {
            ScreenCapture.CaptureScreenshot($"saved/{round.ToString()}.png", 4);
            Debug.Log($"Look at {Application.persistentDataPath}");
        }
        
        NextPlayer();
        if(!bIsGameOver)
        {            
            round++;
            if(currentPlayer == ENUM_TEAM.BLACK)
            {
                if(TeamBlack.isAIControlled())
                {
                    TeamBlack.RunAI();
                }
            }
            else
            {
                if(TeamWhite.isAIControlled())
                {
                    TeamWhite.RunAI();
                }
            }
        }

    }
    public void UpdateUI()
    {
        GameObject.FindGameObjectWithTag("TurnText").GetComponent<Text>().text = $"{currentPlayer.ToString()}'S TURN";
    }

    public void Update()
    {
        if (bIsGameOver && Input.GetMouseButtonDown(0))
        {
            bIsGameOver = false;

            ContinuePostMatch();
        }
    }

    public void Winner(ENUM_TEAM playerWinner)
    {
        bIsGameOver = true;
        Debug.Log($"Game over: {playerWinner.ToString()} is the winner");
        winningPlayer = playerWinner;
        StopCoroutine(FinishGame());
        StartCoroutine(FinishGame());

    }
    public void PlaySound(bool isAttack)
    {
        audioSource.clip = moveAudioClip;
        if(isAttack)
        {
            audioSource.clip = attackAudioClip;
        }
        audioSource.Play();
    }

    public IEnumerator FinishGame()
    {
        yield return new WaitForSeconds(1.0f);
        
        if(bIsGameOver) // This is to cover the AI rewinding.
        {
            GameObject.FindGameObjectWithTag("TurnText").GetComponent<Text>().text = "";
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = winningPlayer.ToString() + " IS THE WINNER";
            GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().text = "CLICK ANYWHERE TO CONTINUE";
            for(int i = 0 ; i < TeamWhite.teamInstance.Length; ++i)
            {
                if(TeamWhite.teamInstance[i])
                {
                    if (i <= 7)
                    {
                        if(TeamWhite.teamInstance[i].GetComponent<Chessman>().IsInPlay == false)
                        {
                            TeamWhite.firstRow[i] = ENUM_PIECES_TYPE.NULL;
                        }
                    }
                    else
                    {
                        if(TeamWhite.teamInstance[i].GetComponent<Chessman>().IsInPlay == false)
                        {
                            TeamWhite.secondRow[i-8] = ENUM_PIECES_TYPE.NULL;
                        }
                    }
                }
            }
        }           
    }

    public void Restart()
    {
        SceneManager.LoadScene("Game");
    }
    public void ContinuePostMatch()
    {
        if(winningPlayer != ENUM_TEAM.WHITE)
        {
            Restart();
        }
        else
        {
            SetupShop();
            challengeManager.GiveChallengeReward(); 
            challengeManager.NextChallenge(); 
            if(challengeManager.IsNotAtEndOfChallenge() == false)
            {                           
                CloseShop();
                winScreen.SetActive(true);                 
            }
        }
    }
    public void RevertWinner()
    {
        bIsGameOver = false;

        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = "";
        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().text = "";
        Debug.Log("Reverted winner");
    }

}
