using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using myGame;

public class HeroController : MonoBehaviour {
    public int standOnArea = -1;

	
	void Update () {
        modifyStandOnArea();
	}

    void modifyStandOnArea() {
        float posX = this.gameObject.transform.position.x;
        float posZ = this.gameObject.transform.position.z;
        if (posZ >= RangeLimit.horiLimit) {
            if (posX < RangeLimit.leftLimit)
                standOnArea = 0;
            else if (posX > RangeLimit.rightLimit)
                standOnArea = 2;
            else
                standOnArea = 1;
        }
        else {
            if (posX < RangeLimit.leftLimit)
                standOnArea = 3;
            else if (posX > RangeLimit.rightLimit)
                standOnArea = 5;
            else
                standOnArea = 4;
        }
    }
}
