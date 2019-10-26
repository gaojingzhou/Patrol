using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using myGame;


public class PatrolController : MonoBehaviour {
    private IAddAction addAction;
    private IStatusOptions op;

    public int ownIndex;
    public bool isCatching;    //if sense the hero

    private float range = 3.0f;

    void Start () {
        addAction = SceneController.getInstance() as IAddAction;
        op = SceneController.getInstance() as IStatusOptions;
        ownIndex = getOwnIndex();
        isCatching = false;
    }
	
	void Update () {
        checkNearByHero();
	}

    int getOwnIndex() { //get self index
        string name = this.gameObject.name;
        char cindex = name[name.Length - 1];
        int result = cindex - '0';
        return result;
    }

 
    void checkNearByHero () { //check if the hero is into it's range
        if (op.getHeroArea() == ownIndex) {   //step into the range 
            if (!isCatching) {
                isCatching = true;
                addAction.addDirectMovement(this.gameObject);
            }
        }
        else {
            if (isCatching) {   //stop catching
                op.heroScore();
                isCatching = false;
                addAction.addRandomMovement(this.gameObject, false);
            }
        }
    }

    void OnCollisionStay(Collision e) {
        if (e.gameObject.name.Contains("Patrol") || e.gameObject.name.Contains("fence") || e.gameObject.tag.Contains("FenceAround")) {
            isCatching = false;
            addAction.addRandomMovement(this.gameObject, false);
        }

        if (e.gameObject.name.Contains("Hero")) {
            op.caughtHero();
        }
    }
}
