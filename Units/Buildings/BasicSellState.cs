using UnityEngine;
using System.Collections;
using System;

public class BasicSellState : IBuildingState
{
    private Building building;
    private float sellTimer;
    private bool sellSequence = false;
    private bool staticDisabled = false;

    public BasicSellState(Building building)
    {
        this.building = building;
    }

    public void ToActiveState()
    {
         
    }

    public void ToBuildState()
    {
         
    }

    public void ToDeathState()
    {
         
    }

    public void ToInActiveState()
    {
         
    }

    public void ToSellState()
    {
        
        building.AnimatedSet.SetActive(true);
        building.animator.SetBool("Sell", true);
        sellTimer = Time.time + 1;
        sellSequence = true;
    }

    public void ToUnitBuildState()
    {
         
    }

    public void Update()
    {
        if (!staticDisabled && building.animator.GetCurrentAnimatorStateInfo(0).IsName("Selling"))
        {
            staticDisabled = true;
            building.StaticSet.SetActive(false);
        }
        if (sellSequence && Time.time > sellTimer &&
            !building.animator.GetCurrentAnimatorStateInfo(0).IsName("Selling"))
        {
            building.SellActions();
        }
    }
}
