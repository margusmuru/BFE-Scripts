using UnityEngine;
using System.Collections;

public interface ISoldierState{

    void UpdateState();

    void ToIdleState();

    void ToRunState(float delay);

    void ToCoverState(GameObject coverObj);

    void ToAimInCoverState();

    void ToAimSittingState();

    void ToAimStandingState();

    void ToDeathState();

    void ToSitState();

}
