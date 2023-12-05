using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeUI : MonoBehaviour
{
    public Text challengeText;
    public void UpdateUI(ChallengeManager challengeReference)
    {
        if(challengeReference)
        {
            if(challengeText)
            {
                challengeText.text = $"CHALLENGE {challengeReference.index}";
            }
        }
    }   
}
