using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using myGame;

public class GameController : MonoBehaviour {
    public delegate void GameScoreAction();
    public static event GameScoreAction myGameScoreAction;
    public delegate void GameOverAction();
    public static event GameOverAction myGameOverAction;
    private SceneController scene;

    void Start () {
        scene = SceneController.getInstance();
        scene.setGameController(this);
    }
	
    public void heroScore() {
        if (myGameScoreAction != null)
            myGameScoreAction();
    }

    public void caughtHero() {
        if (myGameOverAction != null)
            myGameOverAction();
    }
}
