using UnityEngine;
using System.Collections;

public class FactoryInActiveState : IBuildingState {

    private Building building;

    public FactoryInActiveState(BuildingFactory building)
    {
        this.building = building;
    }

    public void Update()
    {

    }

    public void ToActiveState()
    {
        building.StaticSet.GetComponent<Renderer>().material = building.originalMat;
        building.currentState = building.activeState;
        building.currentState.ToActiveState();
    }

    public void ToDeathState()
    {
        building.currentState = building.deathState;
        building.currentState.ToDeathState();
    }

    public void ToInActiveState()
    {
        building.UnitActive = false;
        building.StaticSet.GetComponent<Renderer>().material = building.disabledMat;
    }

    public void ToBuildState()
    {

    }

    public void ToUnitBuildState()
    {

    }

    public void ToSellState()
    {
        building.UnitActive = false;
        building.currentState = building.sellState;
        building.currentState.ToSellState();
    }
}
