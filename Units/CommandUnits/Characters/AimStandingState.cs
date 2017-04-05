using UnityEngine;
using System.Collections;
using System;

public class AimStandingState : ISoldierState
{
    private readonly Soldier soldier;
    public MonoBehaviour monoBehaviour;

    private bool aiming = false;

    public void UpdateState()
    {
        if (!soldier.weaponSystem.Target)
        {
            ToIdleState();
        }
        else
        {
            //rotate unit to face target
            soldier.RotateToFaceTheTarget();

            if (aiming && Time.time > soldier.weaponSystem.NextFireTime)
            {
                soldier.weaponSystem.Shoot();
            }
        }
    }

    public AimStandingState(Soldier soldier)
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
        soldier.StartCoroutine(WaitForAnim());
    }

    IEnumerator WaitForAnim()
    {
        soldier.animator.SetBool("target", true);
        yield return new WaitForSeconds(0.5f);
        aiming = true;

    }

    public void ToCoverState(GameObject coverObj)
    {
         
    }

    public void ToDeathState()
    {
        aiming = false;
        soldier.animator.SetBool("target", aiming);
        soldier.currentState = soldier.deathState;
    }

    public void ToIdleState()
    {
        aiming = false;
        soldier.animator.SetBool("target", aiming);
        soldier.currentState = soldier.idleState;
    }

    public void ToRunState(float delay)
    {
        aiming = false;
        soldier.animator.SetBool("target", aiming);
        soldier.animator.SetInteger("stance", -1);
        soldier.currentState = soldier.runState;
        soldier.currentState.ToRunState(delay);
    }

    public void ToSitState()
    {
         
    }

}
