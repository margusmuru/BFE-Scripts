using UnityEngine;
using System.Collections;

public class VehicleDeathState : IVehicleState {

    private Vehicle vehicle;

    public VehicleDeathState(Vehicle vehicle)
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

    }

    public void ToMoveState(IVehicleState returnState)
    {

    }

    public void ToVehicleDeathState()
    {

    }

    public void ToUnloadingState()
    {

    }

}
