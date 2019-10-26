using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStatusText : MonoBehaviour {
    private int score = 0;
    private int textType;  

	void Start () {
        distinguishText();
	}

    void distinguishText() {
        if (gameObject.name.Contains("Score"))
            textType = 0;
        else
            textType = 1;
    }

    void OnEnable() {
        GameController.myGameScoreAction += gameScore;
        GameController.myGameOverAction += gameOver;
    }

    void OnDisable() {
        GameController.myGameScoreAction -= gameScore;
        GameController.myGameOverAction -= gameOver;
    }

    void gameScore() {
        if (textType == 0) {
            score++;
            this.gameObject.GetComponent<Text>().text = "Score: " + score;
        }
    } 

    void gameOver() {
        if (textType == 1)
            this.gameObject.GetComponent<Text>().text = "Game Over!";
    }
}
