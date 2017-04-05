using UnityEngine;
using System.Collections;
using System;

public class DeathState : ISoldierState
{
    private readonly Soldier soldier;
    public MonoBehaviour monoBehaviour;

    public DeathState(Soldier soldier)
    {
        this.soldier = soldier;
        monoBehaviour = soldier;
    }

    public void ToAimInCoverState()
    {
         
    }

    public void ToAimSittingState()
    {
         
    }

    public void ToAimStandingState()
    {
         
    }

    public void ToCoverState(GameObject coverObj)
    {
         
    }

    public void ToDeathState()
    {
        soldier.LevelMasterRef.RemoveUnit(soldier.tagObj, "soldier");
    }

    public void ToIdleState()
    {
         
    }

    public void ToRunState(float delay)
    {
         
    }

    public void ToSitState()
    {
         
    }

    public void UpdateState()
    {
         
    }
}
