using UnityEngine;
using System.Collections;

public class ColMoveState : IVehicleState {

    private Collector vehicle;
    private float curSpeed = 0;
    private float desSpeed = 3;
    private float defaultSpeed = 3;
    private float moveStartTime;
    public IVehicleState returnState;
    public ColMoveState(Collector vehicle)
    {
        this.vehicle = vehicle;
    }

    public void UpdateState()
    {
        if (vehicle.aiPath.speed != desSpeed)
        {
            vehicle.aiPath.speed = Mathf.Lerp(curSpeed, desSpeed, Time.time - moveStartTime);
        }
        if (desSpeed != 0 && (vehicle.transform.position - vehicle.destinationObject.transform.position).sqrMagnitude < 5f)
        {
            //ToIdleState();
            moveStartTime = Time.time;
            curSpeed = defaultSpeed;
            desSpeed = 0;
        }
        else if (vehicle.aiPath.speed == 0)
        {
            if (returnState == vehicle.idleState)
                ToIdleState();
            if (returnState == vehicle.collectingState)
                ToCollectingState();
            if (returnState == vehicle.unloadingState)
                ToUnloadingState();
        }
        
        vehicle.FrontWheels.transform.Rotate(vehicle.aiPath.speed * 70 * Time.deltaTime, 0, 0, Space.Self);
        vehicle.RearWheels.transform.Rotate(vehicle.aiPath.speed * 70 * Time.deltaTime, 0, 0, Space.Self);
        if (vehicle.MiddleWheels != null)
        {
            vehicle.MiddleWheels.transform.Rotate(vehicle.aiPath.speed * 70 * Time.deltaTime, 0, 0, Space.Self);
        }
    }

    public void ToCollectingState()
    {
        ExitThisStateActions();
        vehicle.currentState = vehicle.collectingState;
        vehicle.currentState.ToCollectingState();
    }

    public void ToIdleState()
    {
        ExitThisStateActions();
        vehicle.currentState = vehicle.idleState;
        vehicle.currentState.ToIdleState();
    }

    private void ExitThisStateActions()
    {
        vehicle.aiPath.canMove = false;
        vehicle.aiPath.canSearch = false;
        vehicle.dynamicSet.SetActive(false);
        vehicle.SetMarkervisibility(false);
        if (vehicle.staticSetCargo != null)
        {
            vehicle.staticSet.SetActive(vehicle.collectedAmount == 0);
            vehicle.staticSetCargo.SetActive(vehicle.collectedAmount != 0);
        }
        else
            vehicle.staticSet.SetActive(true);

    }

    public void ToMoveState(IVehicleState returnState)
    {
        curSpeed = 0.1f;
        desSpeed = defaultSpeed;
        moveStartTime = Time.time;
        vehicle.aiPath.canMove = true;
        vehicle.aiPath.canSearch = true;
        vehicle.audioSource.Play();
        
        vehicle.dynamicSet.SetActive(true);
        if (vehicle.Cargo != null)
            vehicle.Cargo.SetActive(vehicle.collectedAmount != 0);
        vehicle.staticSet.SetActive(false);
        if (vehicle.staticSetCargo != null)
            vehicle.staticSetCargo.SetActive(false);

        if (returnState == null)
            this.returnState = vehicle.idleState;
        else
            this.returnState = returnState;
    }

    public void ToVehicleDeathState()
    {
        ExitThisStateActions();
        vehicle.currentState = vehicle.deathState;
        vehicle.currentState.ToVehicleDeathState();
    }

    public void ToUnloadingState()
    {
        ExitThisStateActions();
        vehicle.currentState = vehicle.unloadingState;
        vehicle.currentState.ToUnloadingState();
    }
}
