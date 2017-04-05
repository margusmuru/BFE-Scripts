using UnityEngine;
using System.Collections;
using System;

public class UnloadingState : IVehicleState
{
    private Collector vehicle;
    private bool unloadingStarted = false;
    private float nextColAddTime = 0;
    private BuildingFactory curFactory = null;

    public UnloadingState(Collector vehicle)
    {
        this.vehicle = vehicle;
    }

    public void UpdateState()
    {
        if (unloadingStarted)
        {
            vehicle.Spinner.transform.Rotate(0, 0, 30 * Time.deltaTime);
            if (Time.time > nextColAddTime && vehicle.collectedAmount > 0)
            {
                if (vehicle.collectedAmount > 10)
                {
                    curFactory.AddResources(10f);
                    vehicle.collectedAmount -= 10;
                }
                else
                {
                    curFactory.AddResources(vehicle.collectedAmount);
                    vehicle.collectedAmount = 0;
                }
                nextColAddTime = Time.time + 0.1f;
            }
        }
        if (vehicle.collectedAmount == 0)
            ToMoveState(vehicle.collectingState);
    }

    public void ToCollectingState()
    {
         
    }

    public void ToIdleState()
    {
        ExitThisStateActions();
        vehicle.currentState = vehicle.idleState;
        vehicle.currentState.ToIdleState();
    }

    public void ToMoveState(IVehicleState returnState)
    {
        ExitThisStateActions();
        //check if there are any resources left
        if (vehicle.myResources.GetComponent<ResourcesPile>().ResourcesLeft == 0)
        {
            //if unable to find another resourcepile, got to idle state
            if (!vehicle.FindResources())
            {
                ToIdleState();
            }
        }
        GameObject destPlane = UnitLocationsManager.FindLocation(vehicle.myResources.transform.position, vehicle.transform.position, 200, false);
        if (destPlane != null)
        {
            UnitLocationsManager.ClearLocFromUsedList(vehicle.destinationObject.transform.position);
            vehicle.destinationObject.transform.position = destPlane.transform.position;
            UnitLocationsManager.UsedUnitLocations.Add(destPlane.transform.position);
            vehicle.currentState = vehicle.moveState;
            vehicle.currentState.ToMoveState(returnState);
        }
        else
        {
            NotificationSystem.SendNotification("Destination Error", "Cannot reach to Resources", Color.red, vehicle.transform.position);
            ToIdleState();
        }
    }

    public void ToVehicleDeathState()
    {
        ExitThisStateActions();
        vehicle.currentState = vehicle.deathState;
        vehicle.currentState.ToVehicleDeathState();
    }

    public void ToUnloadingState()
    {
        if (vehicle.myFactory == null)
        {
            if (!vehicle.FindFactory())
            {
                NotificationSystem.SendNotification("Destination Error", "A Collector is unable to find a Refinery.\n Create a one and assign it to the Collector Manually", Color.red, vehicle.transform.position);
                ToIdleState();
            }
        }
        if (vehicle.myFactory != null && (vehicle.transform.position - vehicle.myFactory.transform.position).sqrMagnitude < 110)
        {
            curFactory = vehicle.myFactory.transform.parent.gameObject.GetComponent<BuildingFactory>();
            unloadingStarted = true;
            vehicle.Spinner.SetActive(true);
        }
    }

    private void ExitThisStateActions()
    {
        unloadingStarted = false;
        vehicle.Spinner.SetActive(false);
    }

}
