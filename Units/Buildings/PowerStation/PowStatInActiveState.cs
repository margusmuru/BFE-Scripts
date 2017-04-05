using UnityEngine;
using System.Collections;
using System;

public class PowStatInActiveState : IBuildingState
{
    private BuildingPowerStation building;

    public PowStatInActiveState(BuildingPowerStation building)
    {
        this.building = building;
    }

    public void ToActiveState()
    {
        building.SteamEmitter.Play();
        building.currentState = building.activeState;
        building.currentState.ToActiveState();
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
        Debug.Log("PowerStation is now in InActive state");
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
