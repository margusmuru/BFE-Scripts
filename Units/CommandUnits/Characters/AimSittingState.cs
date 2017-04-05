using UnityEngine;
using System.Collections;
using System;

public class AimSittingState : ISoldierState
{
    private readonly Soldier soldier;
    public MonoBehaviour monoBehaviour;

    public AimSittingState(Soldier soldier)
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
