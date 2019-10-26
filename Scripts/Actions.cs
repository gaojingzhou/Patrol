using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAction : ScriptableObject
{
    public bool enable = true;
    public bool destroy = false;

    public GameObject gameObject { get; set; }
    public Transform transform { get; set; }
    public ISSActionCallback callBack { get; set; }

    protected SSAction() { }

    public virtual void Start()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Update()
    {
        throw new System.NotImplementedException();
    }
}

public enum SSActionEventType : int { Started, Completed }
public enum SSActionTargetType : int { Normal, Catching }   

public interface ISSActionCallback
{
    void SSActionEvent(SSAction source,
        SSActionEventType eventType = SSActionEventType.Completed,
        SSActionTargetType intParam = SSActionTargetType.Normal,     
        string strParam = null,
        Object objParam = null);
}

public class CCSequeneActions : SSAction, ISSActionCallback
{
    public List<SSAction> actionList;
    public int repeatTimes = -1;         
    public int subActionIndex = 0;        

    public static CCSequeneActions CreateSSAction(List<SSAction> _actionList, int _repeatTimes = 0)
    {
        CCSequeneActions action = ScriptableObject.CreateInstance<CCSequeneActions>();
        action.repeatTimes = _repeatTimes;
        action.actionList = _actionList;
        return action;
    }

    public override void Start()
    {
        foreach (SSAction action in actionList)
        {
            action.gameObject = this.gameObject;
            action.transform = this.transform;
            action.callBack = this;
            action.Start();
        }
    }

    public override void Update()
    {
        if (actionList.Count == 0)
            return;
        else if (subActionIndex < actionList.Count)
        {
            actionList[subActionIndex].Update();
        }
    }

    public void SSActionEvent(SSAction source, 
        SSActionEventType eventType = SSActionEventType.Completed, 
        SSActionTargetType intParam = SSActionTargetType.Normal, 
        string strParam = null, Object objParam = null)
    {
        source.destroy = false;
        this.subActionIndex++;
        if (this.subActionIndex >= actionList.Count)
        {
            this.subActionIndex = 0;
            if (repeatTimes > 0)
                repeatTimes--;
            if (repeatTimes == 0)
            {
                this.destroy = true;
                this.callBack.SSActionEvent(this);
            }
        }
    }
    void OnDestroy()
    {

    }


}


public class CCMoveToAction : SSAction
{
    public Vector3 target;
    public float speed;
    public bool isCatching;    

    public static CCMoveToAction CreateSSAction(Vector3 _target, float _speed, bool _isCatching)
    {
        CCMoveToAction action = ScriptableObject.CreateInstance<CCMoveToAction>();
        action.target = _target;
        action.speed = _speed;
        action.isCatching = _isCatching;
        return action;
    }

    public override void Update()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, target, speed);
        if (this.transform.position == target)
        {
            this.destroy = true;
            if (!isCatching)    
                this.callBack.SSActionEvent(this);
            else
                this.callBack.SSActionEvent(this, SSActionEventType.Completed, SSActionTargetType.Catching);
        }
    }
}



public class SSActionManager : MonoBehaviour
{

    private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
    private List<SSAction> waitingAdd = new List<SSAction>();
    private List<int> waitingDelete = new List<int>();

    protected void Update()
    {
        foreach (SSAction ac in waitingAdd)
        {
            actions[ac.GetInstanceID()] = ac;
        }
        waitingAdd.Clear();

        foreach (KeyValuePair<int, SSAction> kv in actions)
        {
            SSAction ac = kv.Value;
            if (ac.destroy)
                waitingDelete.Add(kv.Key);
            else if (ac.enable)
                ac.Update();
        }

        foreach (int key in waitingDelete)
        {
            SSAction ac = actions[key];
            actions.Remove(key);
            DestroyObject(ac);
        }
        waitingDelete.Clear();
    }

    public void runAction(GameObject gameObj, SSAction action, ISSActionCallback manager)
    {
        
        for (int i = 0; i < waitingAdd.Count; i++) //destroy actions
        {
            if (waitingAdd[i].gameObject.Equals(gameObj))
            {
                SSAction ac = waitingAdd[i];
                waitingAdd.RemoveAt(i);
                i--;
                DestroyObject(ac);
            }
        }
        foreach (KeyValuePair<int, SSAction> kv in actions)
        {
            SSAction ac = kv.Value;
            if (ac.gameObject.Equals(gameObj))
            {
                ac.destroy = true;
            }
        }

        action.gameObject = gameObj;
        action.transform = gameObj.transform;
        action.callBack = manager;
        waitingAdd.Add(action);
        action.Start();
    }
}


