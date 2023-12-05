using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CommandLog
{
    public Chessman chesspiece;
    public Vector2 oldPosition;

    public Vector2 newPosition;
    public Chessman deadPiece;
}

public class History : MonoBehaviour
{
    public Stack<CommandLog> LogHistory;

    private static History _instance;

    public static History Instance { get { return _instance; }}

    public bool DoRewind;
    public bool ManualRewind = false;

    private void Awake() 
    { 
        if (_instance != null && _instance != this) 
        { 
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        LogHistory = new Stack<CommandLog>();
    } 
    public void AddLog(CommandLog log)
    {
        LogHistory.Push(log);
        //Debug.Log($"{log.chesspiece}: {log.oldPosition.ToString()}, {log.newPosition.ToString()}");
    }

    private void Update() {
        if(ManualRewind)
        {
            DoRewind = true;
            ManualRewind = false;
        }
    }
    public void Rewind(bool changePlayer = true)
    {
        DoRewind = true;
        Game controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
        
        if(LogHistory.Count > 0)
        {
            if (controller.IsGameOver())
            {
                controller.RevertWinner();
            }
            CommandLog log = LogHistory.Pop();


            controller.GetComponent<Game>().SetPositionEmpty(
            (int)log.newPosition.x,
            (int)log.newPosition.y);
            
            log.chesspiece.SetXBoard((int)log.oldPosition.x);
            log.chesspiece.SetYBoard((int)log.oldPosition.y);
            log.chesspiece.SetCoords();
            controller.GetComponent<Game>().SetPosition(log.chesspiece.gameObject);


            // bring dead piece back to life.
            if(log.deadPiece != null)
            {
                log.deadPiece.SetIsInPlay(true);         
                controller.GetComponent<Game>().SetPosition(log.deadPiece.gameObject);      
            }
            //Next player is coincidentally, the last player.
            if(changePlayer)
            {
                controller.NextPlayer();
            }
            Chessman.DestroyMovePlates();
            GameObject.FindGameObjectWithTag("SelectedPlate").transform.position = new Vector3(-300, -300, 0);

        }
        DoRewind = false;
    }
}
