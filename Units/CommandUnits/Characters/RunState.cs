using UnityEngine;
using System.Collections;
using System;

public class RunState : ISoldierState {

    private readonly Soldier soldier;
    public MonoBehaviour monoBehaviour;

    public RunState(Soldier soldier)
    {
        monoBehaviour = soldier;
        this.soldier = soldier;
    }

    public void UpdateState()
    {
        if ((soldier.transform.position - soldier.destinationObject.transform.position).sqrMagnitude < 0.8f)
        {
            //check area for cover
            GameObject coverObj = TestForCovers();
            if (coverObj == null)
            {
                ToIdleState();
            }
            else
            {
                ToCoverState(coverObj);
            }
            
        }
    }

    public void ToIdleState()
    {
        soldier.currentState = soldier.idleState;
        soldier.aiPath.canMove = false;
        soldier.aiPath.canSearch = false;
        soldier.SetMarkervisibility(false);
        soldier.animator.SetInteger("stance", 0);
    }

    public void ToRunState(float delay)
    {
        soldier.StopCoroutine(WaitForAnim(0f));
        soldier.StartCoroutine(WaitForAnim(delay));
    }

    IEnumerator WaitForAnim(float delay)
    {
        yield return new WaitForSeconds(delay);
        soldier.aiPath.canMove = true;
        soldier.aiPath.canSearch = true;
    }

    public void ToCoverState(GameObject coverObj)
    {
        soldier.currentState = soldier.coverState;
        soldier.currentState.ToCoverState(coverObj);
        soldier.aiPath.canMove = false;
        soldier.aiPath.canSearch = false;
    }

    public GameObject TestForCovers()
    {
        GameObject _coverObj = null;
        Quaternion _coverOrigRot = soldier.coverLocator.transform.rotation;
        soldier.coverLocator.transform.eulerAngles = new Vector3(0, 0, 0);
        for (int i = 0; i < 6; i++)
        {
            soldier.coverLocator.transform.eulerAngles = soldier.coverLocator.transform.eulerAngles + new Vector3(0, 60, 0);
            RaycastHit hit;
            if (Physics.Raycast(soldier.coverLocator.transform.position, soldier.coverLocator.transform.forward, out hit, 1.8f, soldier.coverLayerMask))
            {
                if (hit.collider.gameObject.CompareTag("Cover"))
                {
                    _coverObj = hit.collider.gameObject;
                    break;
                }
            }
        }
        soldier.coverLocator.transform.rotation = _coverOrigRot;
        return _coverObj;
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
