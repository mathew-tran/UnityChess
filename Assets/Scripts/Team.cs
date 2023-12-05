using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENUM_TEAM {
    WHITE,
    BLACK
}

public class Team : MonoBehaviour
{
    public ENUM_TEAM teamType;


    [Header("AI Settings")]
    public ComputerPlayer AI;
    public bool IsAIDisabled = true;

    [Header("Team Formation")]
    public ENUM_PIECES_TYPE[] firstRow;
    public ENUM_PIECES_TYPE[] secondRow;

    public GameObject[] teamInstance;

    public void CreateTeamFormation(ENUM_PIECES_TYPE[] fRow, ENUM_PIECES_TYPE[] sRow)
    {
        firstRow = fRow;
        secondRow = sRow;
    }
    public void CreateDefaultTeamIfEmpty()
    {
        if(firstRow == null|| secondRow == null || firstRow.Length == 0 || secondRow.Length == 0)
        {
            if(teamType == ENUM_TEAM.BLACK)
            {
                CreateBasicBlackTeamFormation();
            }
            else
            {
                CreateBasicWhiteTeamFormation();
            }
        }
    }
    public void CreateBasicBlackTeamFormation()
    {
        firstRow = new ENUM_PIECES_TYPE[]
        {
            ENUM_PIECES_TYPE.PAWN,
            ENUM_PIECES_TYPE.PAWN,
            ENUM_PIECES_TYPE.PAWN,
            ENUM_PIECES_TYPE.PAWN,
            ENUM_PIECES_TYPE.PAWN,
            ENUM_PIECES_TYPE.PAWN,
            ENUM_PIECES_TYPE.PAWN,
            ENUM_PIECES_TYPE.PAWN,
        };
        secondRow = new ENUM_PIECES_TYPE[]
        {
            ENUM_PIECES_TYPE.ROOK,
            ENUM_PIECES_TYPE.KNIGHT,
            ENUM_PIECES_TYPE.BISHOP,
            ENUM_PIECES_TYPE.QUEEN,
            ENUM_PIECES_TYPE.KING,
            ENUM_PIECES_TYPE.BISHOP,
            ENUM_PIECES_TYPE.KNIGHT,
            ENUM_PIECES_TYPE.ROOK
        };
    }

    public void CreateBasicWhiteTeamFormation()
    {
        firstRow = new ENUM_PIECES_TYPE[]
        {
            ENUM_PIECES_TYPE.ROOK,
            ENUM_PIECES_TYPE.KNIGHT,
            ENUM_PIECES_TYPE.BISHOP,
            ENUM_PIECES_TYPE.QUEEN,
            ENUM_PIECES_TYPE.KING,
            ENUM_PIECES_TYPE.BISHOP,
            ENUM_PIECES_TYPE.KNIGHT,
            ENUM_PIECES_TYPE.ROOK
        };
        secondRow = new ENUM_PIECES_TYPE[]
        {
            ENUM_PIECES_TYPE.PAWN,
            ENUM_PIECES_TYPE.PAWN,
            ENUM_PIECES_TYPE.PAWN,
            ENUM_PIECES_TYPE.PAWN,
            ENUM_PIECES_TYPE.PAWN,
            ENUM_PIECES_TYPE.PAWN,
            ENUM_PIECES_TYPE.PAWN,
            ENUM_PIECES_TYPE.PAWN,
        };
    }
    public GameObject[] CreateTeam(Game game, int yOffset)
    {
        CreateDefaultTeamIfEmpty();
        GameObject[] team = new GameObject[16];
        for(int i = 0; i < firstRow.Length; ++i)
        {
            team[i] = game.Create(teamType, firstRow[i], i, yOffset);
            if(team[i] != null)
            {
                team[i].name = team[i].GetComponent<Chessman>().GetChessInfoString() +  i.ToString();
            }
        }

        for(int i = 0; i < secondRow.Length; ++i)
        {
            team[i+8] = game.Create(teamType, secondRow[i], i, yOffset + 1);
            if(team[i + 8] != null)
            {
                team[i+8].name = team[i+8].GetComponent<Chessman>().GetChessInfoString() +  (i+8).ToString();
            }
        }

        teamInstance = team;

        if(!IsKingAlive())
        {
            throw new System.Exception("No king was found");
        }
        

        return team;
    }

    public bool IsKingAlive()
    {
        for (int i = 0 ; i < teamInstance.Length; ++i)
        {
            if(teamInstance[i])
            {
                if (teamInstance[i].GetComponent<Chessman>().pieceType == ENUM_PIECES_TYPE.KING)
                {
                    return true;
                }
            }
            
        }
        return false;
    }

    public bool isAIControlled()
    {
        return GetComponent<ComputerPlayer>() != null && !IsAIDisabled;
    }
    public void RunAI()
    {
        GetComponent<ComputerPlayer>().DoTurn();
    }
}

