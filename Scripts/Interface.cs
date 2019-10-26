using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUserAction
{
    void heroMove(int dir);
}

public interface IAddAction
{
    void addRandomMovement(GameObject sourceObj, bool isActive);
    void addDirectMovement(GameObject sourceObj);
}

public interface IStatusOptions
{
    int getHeroArea();
    void heroScore();
    void caughtHero();
}