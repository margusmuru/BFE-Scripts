using UnityEngine;
using System.Collections;
using System;

public class IdleState : ISoldierState
{
    private readonly Soldier soldier;
    public MonoBehaviour monoBehaviour;

    public IdleState(Soldier soldier)
    {
        this.soldier = soldier;
        monoBehaviour = soldier;
    }

    public void UpdateState()
    {
        if (soldier.weaponSystem.Target)
        {
            ToAimStandingState();
        }
    }

    public void ToIdleState()
    {
        
    }

    public void ToRunState(float delay)
    {
        soldier.animator.SetInteger("stance", -1);
        soldier.currentState = soldier.runState;
        soldier.currentState.ToRunState(delay);

    }

    public void ToCoverState(GameObject coverObj)
    {
    }

    public void ToAimInCoverState()
    {
         
    }

    public void ToAimSittingState()
    {
         
    }

    public void ToAimStandingState()
    {
        soldier.currentState = soldier.aimStandingState;
        soldier.currentState.ToAimStandingState();
    }

    public void ToDeathState()
    {
         
    }

    public void ToSitState()
    {
         
    }
}
