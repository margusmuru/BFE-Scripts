using UnityEngine;
using System.Collections;

public interface IVehicleState {

    void UpdateState();

    void ToMoveState(IVehicleState returnState);

    void ToCollectingState();

    void ToIdleState();

    void ToVehicleDeathState();

    void ToUnloadingState();
}
