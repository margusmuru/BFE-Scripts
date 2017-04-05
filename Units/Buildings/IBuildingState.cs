using UnityEngine;
using System.Collections;

public interface IBuildingState {

    void Update();

    void ToActiveState();

    void ToInActiveState();

    void ToDeathState();

    void ToBuildState();

    void ToUnitBuildState();

    void ToSellState();
}
