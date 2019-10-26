using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace myGame {
    public class Diretion {
        public const int UP = 0;
        public const int DOWN = 2;
        public const int LEFT = -1;
        public const int RIGHT = 1;
    }

    public class RangeLimit {
        public const float horiLimit = 12.42f;
        public const float leftLimit = -3.0f;
        public const float rightLimit = 3.0f;
    }

    public class SceneController : System.Object, IUserAction, IAddAction, IStatusOptions {
        private static SceneController instance;
        private GameModel myGameModel;
        private GameController myGameController;

        public void heroMove(int dir) { myGameModel.heroMove(dir); }

        public void addRandomMovement(GameObject sourceObj, bool isActive) { myGameModel.addRandomMovement(sourceObj, isActive); }

        public void addDirectMovement(GameObject sourceObj) { myGameModel.addDirectMovement(sourceObj); }

        public int getHeroArea() { return myGameModel.getHeroArea(); }

        public void heroScore() { myGameController.heroScore(); }

        public void caughtHero() { myGameController.caughtHero(); }

        public static SceneController getInstance() {
            if (instance == null)
                instance = new SceneController();
            return instance;
        }

        internal void setGameModel(GameModel _myGameModel) {
            if (myGameModel == null) {
                myGameModel = _myGameModel;
            }
        }

        internal void setGameController(GameController _myGameController) {
            if (myGameController == null) {
                myGameController = _myGameController;
            }
        }


    }
}

