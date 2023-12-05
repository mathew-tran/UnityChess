using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{
    public Challenge[] challenges;
    public int index = 0;
    bool bIsGameOver = false;

    public delegate void OnStartChallengeDelegate(ChallengeManager challengeReference);
    public static OnStartChallengeDelegate startChallengeDelegate;
    public ChallengeUI challengeUI;

    // Start is called before the first frame update

    public bool IsNotAtEndOfChallenge()
    {
        return index < challenges.Length;
    }
    public void Setup() 
    {
        index = 0;
        startChallengeDelegate += challengeUI.UpdateUI;
    }
    public void NextChallenge()
    {
        index++;
    }
    public void StartChallenge()
    {
        Debug.Log("Starting challenge");
        GameObject controllerObject = GameObject.FindGameObjectWithTag("GameController");
        if(IsNotAtEndOfChallenge())
        {
            challenges[index].SetBlackTeam();
            startChallengeDelegate(this);
        }
        else
        {
            bIsGameOver = true;
        }
    }

    public void GiveChallengeReward()
    {
        challenges[index].GiveReward();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
