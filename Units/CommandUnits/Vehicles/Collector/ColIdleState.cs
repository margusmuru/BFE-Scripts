using UnityEngine;
using System.Collections;

public class ColIdleState : IVehicleState {

    private Collector vehicle;

    public ColIdleState(Collector vehicle)
    {
        this.vehicle = vehicle;
    }

    public void UpdateState()
    {
        
    }

    public void ToCollectingState()
    {

    }

    public void ToIdleState()
    {
        NotificationSystem.SendNotification("Collector Idle", "A Collector is in idle state. \n You might want to assign a job for it.", Color.yellow, vehicle.transform.position);
    }

    public void ToMoveState(IVehicleState returnState)
    {
        vehicle.currentState = vehicle.moveState;
        vehicle.currentState.ToMoveState(null);
    }

    public void ToVehicleDeathState()
    {
        vehicle.currentState = vehicle.deathState;
        vehicle.currentState.ToVehicleDeathState();
    }

    public void ToUnloadingState()
    {

    }
}
