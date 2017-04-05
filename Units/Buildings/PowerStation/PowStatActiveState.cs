using UnityEngine;
using System.Collections;
using System;

public class PowStatActiveState : IBuildingState
{
    private BuildingPowerStation building;

    public PowStatActiveState(BuildingPowerStation building)
    {
        this.building = building;
    }

    public void ToActiveState()
    {
        Debug.Log("PowerStation is now in Active state");
    }

    public void ToBuildState()
    {
         
    }

    public void ToDeathState()
    {
        building.currentState = building.deathState;
        building.currentState.ToDeathState();
    }

    public void ToInActiveState()
    {
        building.SteamEmitter.Stop();
        building.currentState = building.inactiveState;
        building.currentState.ToInActiveState();
    }

    public void Update()
    {
         
    }

    public void ToUnitBuildState()
    {

    }

    public void ToSellState()
    {
        building.currentState = building.sellState;
        building.currentState.ToSellState();
    }
}
