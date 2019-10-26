using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using myGame;

public class GameModel : SSActionManager, ISSActionCallback {

    private List<GameObject> patrolList;
    private List<int> patrolRemainList;

    private const float normalSpeed = 0.03f;
    private const float catchSpeed = 0.05f;
    public GameObject PatrolItem, HeroItem, sceneModelItem, canvasItem;

    private SceneController scene;
    private GameObject myHero, sceneModel, canvasAndText;

    public void SSActionEvent(SSAction source, //ssActionCallBack interface
    SSActionEventType eventType = SSActionEventType.Completed,
    SSActionTargetType intParam = SSActionTargetType.Normal,
    string strParam = null, Object objParam = null)
    {
        if (intParam == SSActionTargetType.Normal)
            addRandomMovement(source.gameObject, true);
        else
            addDirectMovement(source.gameObject);
    }


    void Awake() {
        PatrolFactory.getInstance().initItem(PatrolItem);
    }

    protected new void Start () {
        scene = SceneController.getInstance();
        scene.setGameModel(this);
        genHero();
        genPatrols();
        sceneModel = Instantiate(sceneModelItem);
        canvasAndText = Instantiate(canvasItem);
    }

    protected new void Update() {
        base.Update();
    }

  
    void genHero() { //get hero
        myHero = Instantiate(HeroItem);
    }

    void genPatrols() { //get patrols
        patrolList = new List<GameObject>(6);
        patrolRemainList = new List<int>(6);
        Vector3[] posSet = PatrolFactory.getInstance().getPosSet();
        for (int i = 0; i < 6; i++) {
            GameObject newPatrol = PatrolFactory.getInstance().getPatrol();
            newPatrol.transform.position = posSet[i];
            newPatrol.name = "Patrol" + i;
            patrolRemainList.Add(-2);
            patrolList.Add(newPatrol);
            addRandomMovement(newPatrol, true);
        }
    }

    public void heroMove(int dir) { //hero movement
        myHero.transform.rotation = Quaternion.Euler(new Vector3(0, dir * 90, 0));
        switch (dir) {
            case Diretion.UP:
                myHero.transform.position += new Vector3(0, 0, 0.1f);
                break;
            case Diretion.DOWN:
                myHero.transform.position += new Vector3(0, 0, -0.1f);
                break;
            case Diretion.LEFT:
                myHero.transform.position += new Vector3(-0.1f, 0, 0);
                break;
            case Diretion.RIGHT:
                myHero.transform.position += new Vector3(0.1f, 0, 0);
                break;
        }
    }


    public void addRandomMovement(GameObject sourceObj, bool isActive) {
        int index = getIndex(sourceObj);
        int dir = getdirection(index, isActive);
        patrolRemainList[index] = dir;

        sourceObj.transform.rotation = Quaternion.Euler(new Vector3(0, dir * 90, 0));
        Vector3 target = sourceObj.transform.position;
        switch (dir) {
            case Diretion.UP:
                target += new Vector3(0, 0, 1);
                break;
            case Diretion.DOWN:
                target += new Vector3(0, 0, -1);
                break;
            case Diretion.LEFT:
                target += new Vector3(-1, 0, 0);
                break;
            case Diretion.RIGHT:
                target += new Vector3(1, 0, 0);
                break;
        }
        addSingleMoving(sourceObj, target, normalSpeed, false);
    }

    int getIndex(GameObject sourceObj) {
        string name = sourceObj.name;
        char cindex = name[name.Length - 1];
        int result = cindex - '0';
        return result;
    }

    int getdirection(int index, bool isActive) {
        int dir = Random.Range(-1, 3);
        if (!isActive) {    //when hit
            while (patrolRemainList[index] == dir || isOutOfRange(index, dir)) {
                dir = Random.Range(-1, 3);
            }
        }
        else {              //when not hit
            while (patrolRemainList[index] == 0 && dir == 2 
                || patrolRemainList[index] == 2 && dir == 0
                || patrolRemainList[index] == 1 && dir == -1
                || patrolRemainList[index] == -1 && dir == 1
                || isOutOfRange(index, dir)) {
                dir = Random.Range(-1, 3);
            }
        }
        return dir;
    }
    

    //judge if patrol out range
    bool isOutOfRange(int index, int dir) {
        Vector3 patrolPos = patrolList[index].transform.position;
        float posX = patrolPos.x;
        float posZ = patrolPos.z;
        switch (index) {
            case 0:
                if (dir == 1 && posX + 1 > RangeLimit.leftLimit
                    || dir == 2 && posZ - 1 < RangeLimit.horiLimit)
                    return true;
                break;
            case 1:
                if (dir == 1 && posX + 1 > RangeLimit.rightLimit
                    || dir == -1 && posX - 1 < RangeLimit.leftLimit
                    || dir == 2 && posZ - 1 < RangeLimit.horiLimit)
                    return true;
                break;
            case 2:
                if (dir == -1 && posX - 1 < RangeLimit.rightLimit
                    || dir == 2 && posZ - 1 < RangeLimit.horiLimit)
                    return true;
                break;
            case 3:
                if (dir == 1 && posX + 1 > RangeLimit.leftLimit
                    || dir == 0 && posZ + 1 > RangeLimit.horiLimit)
                    return true;
                break;
            case 4:
                if (dir == 1 && posX + 1 > RangeLimit.rightLimit
                    || dir == -1 && posX - 1 < RangeLimit.leftLimit
                    || dir == 0 && posZ + 1 > RangeLimit.horiLimit)
                    return true;
                break;
            case 5:
                if (dir == -1 && posX - 1 < RangeLimit.rightLimit
                    || dir == 0 && posZ + 1 > RangeLimit.horiLimit)
                    return true;
                break;
        }
        return false;
    }

    public void addDirectMovement(GameObject sourceObj) { //catch movement
        int index = getIndex(sourceObj);
        patrolRemainList[index] = -2;
        sourceObj.transform.LookAt(sourceObj.transform);
        Vector3 oriTarget = myHero.transform.position - sourceObj.transform.position; //get original target
        Vector3 target = new Vector3(oriTarget.x / 4.0f, 0, oriTarget.z / 4.0f);
        target += sourceObj.transform.position;
        addSingleMoving(sourceObj, target, catchSpeed, true);
    }

    void addSingleMoving(GameObject sourceObj, Vector3 target, float speed, bool isCatching) {
        this.runAction(sourceObj, CCMoveToAction.CreateSSAction(target, speed, isCatching), this);
    }

    void addCombinedMoving(GameObject sourceObj, Vector3[] target, float[] speed, bool isCatching) {
        List<SSAction> acList = new List<SSAction>();
        for (int i = 0; i < target.Length; i++) {
            acList.Add(CCMoveToAction.CreateSSAction(target[i], speed[i], isCatching));
        }
        CCSequeneActions MoveSeq = CCSequeneActions.CreateSSAction(acList);
        this.runAction(sourceObj, MoveSeq, this);
    }

    
    public int getHeroArea() { //get hero area
        return myHero.GetComponent<HeroController>().standOnArea;
    }


}
