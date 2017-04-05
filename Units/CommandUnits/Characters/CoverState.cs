using UnityEngine;
using System.Collections;
using System;

public class CoverState : ISoldierState
{
    private readonly Soldier soldier;
    public MonoBehaviour monoBehaviour;

    private bool inCover = false;
    private GameObject coverObj;
    private Vector3 coverDestination;

    public CoverState(Soldier soldier)
    {
        this.soldier = soldier;
        monoBehaviour = soldier;
    }

    public void ToCoverState(GameObject coverObj)
    {
        Debug.Log(coverObj);
        if (coverObj)
        {
            this.coverObj = coverObj;
            Vector3 _targetPoint = coverObj.transform.position;
            _targetPoint.y = soldier.coverLocator.transform.position.y;
            soldier.coverLocator.transform.LookAt(_targetPoint);

            RaycastHit hit;
            if (Physics.Raycast(soldier.coverLocator.transform.position, soldier.coverLocator.transform.forward, out hit, 5f, soldier.coverLayerMask))
            {
                coverDestination = CalculateCoverPoint(soldier.transform.position, hit.point);
                soldier.animator.SetInteger("stance", -1);
                soldier.transform.LookAt(coverDestination);
            }
            inCover = false;
        }     
    }

    public void ToIdleState()
    {
        
    }

    public void ToRunState(float delay)
    {
        inCover = false;
        soldier.animator.SetInteger("stance", -1);
        soldier.currentState = soldier.runState;
        soldier.currentState.ToRunState(delay);
    }

    public void UpdateState()
    {
        if (!inCover)
        {
            soldier.transform.position = Vector3.Lerp(soldier.transform.position, coverDestination, 2 * Time.deltaTime);
            if ((soldier.transform.position - coverDestination).sqrMagnitude < 0.1f)
            {
                soldier.animator.SetInteger("stance", 1);
                inCover = true;
            }
        }
        else
        {
            if (soldier.weaponSystem.Target)
            {
                soldier.currentState = soldier.aimInCoverState;
                soldier.currentState.ToAimInCoverState();
            }
        }
    }

    private Vector3 CalculateCoverPoint(Vector3 coverLoc, Vector3 unitLoc)
    {
        Vector3 coverPoint;
        float d = Mathf.Sqrt(Mathf.Pow((coverLoc.x - unitLoc.x), 2) + Mathf.Pow((coverLoc.z - unitLoc.z), 2));
        float r = 0.25f / d;
        coverPoint.x = r * coverLoc.x + (1 - r) * unitLoc.x;
        coverPoint.z = r * coverLoc.z + (1 - r) * unitLoc.z;
        coverPoint.y = 0.99f;
        return coverPoint;
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

    public void ToDeathState()
    {
         
    }

    public void ToSitState()
    {
         
    }
}
