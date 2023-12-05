using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestTeam
    {
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestTeamWithNoKingFailsOnCreateTeam()
        {
            Game myGame = TestGame.SetupGameTestVariables();

            myGame.TeamBlack = TestGame.SetupBlackPlayer().GetComponent<Team>();
            myGame.TeamWhite = TestGame.SetupWhitePlayer().GetComponent<Team>();
            myGame.TeamBlack.CreateDefaultTeamIfEmpty();
            for(int i = 0; i < myGame.TeamBlack.firstRow.Length; ++i)
            {
                if(myGame.TeamBlack.firstRow[i] == ENUM_PIECES_TYPE.KING)
                {
                    myGame.TeamBlack.firstRow[i] = ENUM_PIECES_TYPE.PAWN;
                }
            }
            for(int i = 0; i < myGame.TeamBlack.secondRow.Length; ++i)
            {
                if(myGame.TeamBlack.secondRow[i] == ENUM_PIECES_TYPE.KING)
                {
                    myGame.TeamBlack.secondRow[i] = ENUM_PIECES_TYPE.PAWN;
                }
            }           

            try
            {
                myGame.TeamBlack.CreateTeam(myGame, 0);
            }
            catch (System.Exception e)
            {
                //Debug.Log()
                Assert.True(e.Message.CompareTo("No king was found") == 0);                
                Assert.Pass("Test passed");
            }
            Assert.Fail("Expected failure message for no king found");

            yield return null;
        }
         [UnityTest]
        public IEnumerator TestTeamWithKingPassesOnCreateTeam()
        {
            Game myGame = TestGame.SetupGameTestVariables();

            myGame.TeamBlack = TestGame.SetupBlackPlayer().GetComponent<Team>();
            myGame.TeamWhite = TestGame.SetupWhitePlayer().GetComponent<Team>();
            myGame.TeamBlack.CreateDefaultTeamIfEmpty();         

            myGame.TeamBlack.CreateTeam(myGame, 0);

            yield return null;
        }
    }
}
