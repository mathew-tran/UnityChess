using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Challenge : MonoBehaviour
{
    public GameObject teamChallenge;
    public int reward = 1;

    // Sets the black team on the game manager
    public void SetBlackTeam()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Challenge");
        foreach(GameObject challengeObject in objects)
        {
            Destroy(challengeObject);
        }
        GameObject obj = Instantiate(teamChallenge, Vector3.zero, Quaternion.identity);
        GameObject controllerObject = GameObject.FindGameObjectWithTag("GameController");
        controllerObject.GetComponent<Game>().TeamBlack = obj.GetComponent<Team>();
        obj.tag = "Challenge";
    }

    public void GiveReward()
    {
        GameObject.FindGameObjectWithTag("ShopManager").GetComponent<ShopManager>().MakeTransaction(reward);
    }
}
