using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests
{
    public class TestGame
    {

        [UnitySetUp]
        public static IEnumerator Setup()
        {
            yield return new WaitForSeconds(0.2f);
        }
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        public static void CreateRequirements()
        {
            if(!GameObject.Find("_WinnerText"))
            {
                GameObject winnerText = new GameObject("_WinnerText");
                winnerText.AddComponent<Text>();
                winnerText.tag = "WinnerText";
            }

            if(!GameObject.Find("_RestartText"))
            {
                GameObject restartText = new GameObject("_RestartText");
                restartText.AddComponent<Text>();
                restartText.tag = "RestartText";
            }

            if(!GameObject.Find("_TurnText"))
            {
                GameObject turnText = new GameObject("_TurnText");
                turnText.AddComponent<Text>();
                turnText.tag = "TurnText";
            }
        }
        public static GameObject SetupWhitePlayer(bool aiDisabled = false)
        {
            GameObject myWhiteTeamObject = GameObject.Find("_WhitePlayer");
            
            if(!myWhiteTeamObject)
            {
                myWhiteTeamObject = new GameObject("_WhitePlayer");
                Team myTeam = myWhiteTeamObject.AddComponent<Team>();            

                ComputerPlayer player = myWhiteTeamObject.AddComponent<ComputerPlayer>();
                myTeam.AI = player;
                myTeam.IsAIDisabled = aiDisabled;

                myTeam.teamType = ENUM_TEAM.WHITE;
            }
            return myWhiteTeamObject;
        }
        
        public static GameObject SetupBlackPlayer(bool aiDisabled = false)
        {
            GameObject myBlackTeamObject = GameObject.Find("_BlackPlayer");
            
            if(!myBlackTeamObject)
            {
                myBlackTeamObject = new GameObject("_BlackPlayer");
                Team myTeam = myBlackTeamObject.AddComponent<Team>();

                ComputerPlayer player = myBlackTeamObject.AddComponent<ComputerPlayer>();
                myTeam.AI = player;
                myTeam.IsAIDisabled = aiDisabled;

                myTeam.teamType = ENUM_TEAM.BLACK;
            }
            return myBlackTeamObject;
        }

        public static Game SetupGameTestVariables()
        {
            GameObject myGameObject = GameObject.Find("_Controller");
            if(!myGameObject)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                Camera camera = cameraObject.AddComponent<Camera>();
                camera.orthographic = true;
                camera.tag = "MainCamera";
                camera.depth = -1;
                camera.enabled = true;
                camera.transform.position = new Vector3(0,0, -300.0f);
                camera.backgroundColor = new Color(0,0,0);

                GameObject board = new GameObject("_board");
                board.transform.localScale = new Vector3(3.0f, 3.0f, 1.0f);
                SpriteRenderer boardSprite = board.AddComponent<SpriteRenderer>();
                boardSprite.sprite = Resources.Load<Sprite>("Sprites/board");

                myGameObject = new GameObject("_Controller");
                Game myGame = myGameObject.AddComponent<Game>();

                History myHistory = myGameObject.AddComponent<History>();

                myGame.tag = "GameController";

                GameObject selectedPlate = Resources.Load("Objects/SelectedPlate") as GameObject;
                
                GameObject deathParticle =  new GameObject("_Deathparticle");
                deathParticle.tag = "DeathParticle";
                
                myGame.chessPiece = Resources.Load("Objects/ChestPiece") as GameObject;
                Debug.Log(myGame.chessPiece);
            }

            return myGameObject.GetComponent<Game>();

            
        }
        [UnityTest]
        public IEnumerator TestGameCanBePlayedThrough()
        {
            // Something isn't being cleaned up correctly right now..
            yield return new WaitForSeconds(1.2f);
            CreateRequirements();

            Game myGame = SetupGameTestVariables();
            
            myGame.TeamBlack = SetupBlackPlayer().GetComponent<Team>();
            myGame.TeamWhite = SetupWhitePlayer().GetComponent<Team>();                 


            myGame.TeamBlack.CreateTeamFormation(
                new ENUM_PIECES_TYPE[] {
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL
                },

                new ENUM_PIECES_TYPE[] {
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.KING
                }
            );

             myGame.TeamWhite.CreateTeamFormation(
                new ENUM_PIECES_TYPE[] {
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL
                },

                new ENUM_PIECES_TYPE[] {
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.KING,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.QUEEN
                }
            );


            yield return new WaitForSeconds(2.0f);
            myGame.Setup();
            yield return new WaitUntil(() => myGame.IsGameOver() == true);
            Assert.True(myGame.IsGameOver(), "Expect game to be over");
            Assert.True(myGame.round == 1, $"Expect game to end in one round, got {myGame.round} rounds instead");
        }
        [UnityTest]
        public IEnumerator TestGameRookDoesNotJumpThrough()
        {
            CreateRequirements();

            Game myGame = SetupGameTestVariables();
            
            myGame.TeamBlack = SetupBlackPlayer().GetComponent<Team>();
            myGame.TeamWhite = SetupWhitePlayer().GetComponent<Team>();            


            myGame.TeamBlack.CreateTeamFormation(
                new ENUM_PIECES_TYPE[] {
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL
                },

                new ENUM_PIECES_TYPE[] {
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.KING,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL
                }
            );

             myGame.TeamWhite.CreateTeamFormation(
                new ENUM_PIECES_TYPE[] {
                    ENUM_PIECES_TYPE.QUEEN,
                    ENUM_PIECES_TYPE.QUEEN,
                    ENUM_PIECES_TYPE.QUEEN,
                    ENUM_PIECES_TYPE.QUEEN,
                    ENUM_PIECES_TYPE.QUEEN,
                    ENUM_PIECES_TYPE.QUEEN,
                    ENUM_PIECES_TYPE.QUEEN,
                    ENUM_PIECES_TYPE.QUEEN
                },

                new ENUM_PIECES_TYPE[] {
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.KING,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL
                }
            );

            yield return new WaitForSeconds(2.0f);
            myGame.Setup();
            yield return new WaitUntil(() => myGame.IsGameOver() == true);
            Assert.True(myGame.round > 2, $"Expect game to be past two rounds as rook cannot jump through a piece, got {myGame.round} rounds instead");
        }
          [UnityTest]
        public IEnumerator TestGameRookGoesForObviousGoal()
        {
            // Something isn't being cleaned up correctly right now..
            yield return new WaitForSeconds(1.2f);
            CreateRequirements();

            Game myGame = SetupGameTestVariables();
            
            myGame.TeamBlack = SetupBlackPlayer().GetComponent<Team>();
            myGame.TeamWhite = SetupWhitePlayer().GetComponent<Team>();            


            myGame.TeamBlack.CreateTeamFormation(
                new ENUM_PIECES_TYPE[] {
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.ROOK,
                    ENUM_PIECES_TYPE.ROOK,
                    ENUM_PIECES_TYPE.ROOK,
                    ENUM_PIECES_TYPE.ROOK,
                    ENUM_PIECES_TYPE.ROOK,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.ROOK
                },

                new ENUM_PIECES_TYPE[] {
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.KING,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL
                }
            );

             myGame.TeamWhite.CreateTeamFormation(
                new ENUM_PIECES_TYPE[] {
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.KING,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.ROOK
                },

                new ENUM_PIECES_TYPE[] {
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL,
                    ENUM_PIECES_TYPE.NULL
                }
            );

            yield return new WaitForSeconds(2.0f);
            myGame.Setup();

            yield return new WaitUntil(() => myGame.IsGameOver() == true);
            Assert.True(myGame.round == 2, $"Expect to finish second round, because the player should take the king first rather than rook as it is higher in priority, but {myGame.round} rounds instead");
        }
    }
}
