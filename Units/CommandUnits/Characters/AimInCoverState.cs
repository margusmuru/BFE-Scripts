using UnityEngine;
using System.Collections;
using System;

public class AimInCoverState : ISoldierState
{
    private readonly Soldier soldier;
    public MonoBehaviour monoBehaviour;

    private bool aiming = false;

    public AimInCoverState(Soldier soldier)
    {
        this.soldier = soldier;
        monoBehaviour = soldier;
    }

    public void UpdateState()
    {
        if (!soldier.weaponSystem.Target)
        {
            ToCoverState(null);
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

    public void ToAimInCoverState()
    {
        soldier.StopCoroutine(WaitForAnim());
        soldier.StartCoroutine(WaitForAnim());
    }

    IEnumerator WaitForAnim()
    {
        soldier.animator.SetBool("target", true);
        yield return new WaitForSeconds(1f);
        aiming = true;

    }

    public void ToAimSittingState()
    {
         
    }

    public void ToAimStandingState()
    {
         
    }

    public void ToCoverState(GameObject coverObj)
    {
        aiming = false;
        soldier.animator.SetBool("target", aiming);
        soldier.currentState = soldier.coverState;
    }

    public void ToDeathState()
    {
        aiming = false;
        soldier.animator.SetBool("target", aiming);
        soldier.currentState = soldier.deathState;
    }

    public void ToIdleState()
    {
         
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
