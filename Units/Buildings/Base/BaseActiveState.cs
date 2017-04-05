using UnityEngine;
using System.Collections;

public class BaseActiveState : IBuildingState {

    private Building building;

    public BaseActiveState(Building building)
    {
        this.building = building;
    }

    public void Update()
    {

    }

    public void ToActiveState()
    {
        Debug.Log("Base is in active state now");
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
