using UnityEngine;
using System.Collections;

public class BarracksActiveState : IBuildingState {

    private Building building;

    public BarracksActiveState(BuildingBarracks building)
    {
        this.building = building;
    }

    public void Update()
    {

    }

    public void ToActiveState()
    {
        building.UnitActive = true;
    }

    public void ToDeathState()
    {
        building.UnitActive = false;
        building.currentState = building.deathState;
        building.currentState.ToDeathState();
    }

    public void ToInActiveState()
    {
        building.UnitActive = false;
        building.currentState = building.inactiveState;
        building.currentState.ToInActiveState();
    }

    public void ToBuildState()
    {

    }

    public void ToUnitBuildState()
    {
        building.currentState = building.unitBuildingState;
        building.currentState.ToUnitBuildState();
    }


    public void ToSellState()
    {
        building.UnitActive = false;
        building.currentState = building.sellState;
        building.currentState.ToSellState();
    }
}
