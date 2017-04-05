using UnityEngine;
using System.Collections;

public class CollectingState : IVehicleState {

    private Collector vehicle;
    private bool collectingStarted = false;
    private float nextColAddTime = 0;
    private ResourcesPile curPile = null;
    public CollectingState(Collector vehicle)
    {
        this.vehicle = vehicle;
    }

    public void UpdateState()
    {
        if (collectingStarted)
        {
            vehicle.Spinner.transform.Rotate(0, 0, 30 * Time.deltaTime);
            if (Time.time > nextColAddTime && vehicle.collectedAmount < vehicle.maxCollectionAmount &&
                curPile.ResourcesLeft > 0)
            {
                nextColAddTime = Time.time + 0.1f;
                vehicle.collectedAmount += curPile.GetResources(10f);
                
            }
            if (vehicle.collectedAmount == vehicle.maxCollectionAmount || curPile.ResourcesLeft == 0)
                ToMoveState(vehicle.unloadingState);
        }
    }

    public void ToCollectingState()
    {
        //check if near resources and there are resources left
        if ((vehicle.myResources.transform.position - vehicle.transform.position).sqrMagnitude < 210)
        {
            if (vehicle.myResources.GetComponent<ResourcesPile>().ResourcesLeft > 0)
            {
                collectingStarted = true;
                //vehicle.dynamicSet.SetActive(false);
                //vehicle.staticSetCargo.SetActive(vehicle.collectedAmount != 0);
                //vehicle.staticSet.SetActive(vehicle.collectedAmount == 0);
                vehicle.Spinner.SetActive(true);
                curPile = vehicle.myResources.GetComponent<ResourcesPile>();
            }
            else if (vehicle.FindResources())
            {
                Vector3 loc = UnitLocationsManager.FindLocation(vehicle.myResources.transform.position, vehicle.transform.position, 200, false).transform.position;
                if (loc != null)
                    vehicle.StartMoving(loc, vehicle.collectingState);
                else
                    ToIdleState();
            }
            else
            {
                vehicle.myResources = null;
                ToIdleState();
            }
            
        }
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
        if (returnState != null)
        {
            if (vehicle.myFactory == null)
            {
                if (!vehicle.FindFactory())
                {
                    NotificationSystem.SendNotification("Destination Error", "A Collector is unable to find a Refinery.\n Create a one and assign it to the Collector Manually", Color.red, vehicle.transform.position);
                    ToIdleState();
                }
            }
            if (vehicle.myFactory != null)
            {
                GameObject destPlane = UnitLocationsManager.FindLocation(vehicle.myFactory.transform.position, vehicle.transform.position, 100, false);
                if (destPlane != null)
                {
                    UnitLocationsManager.ClearLocFromUsedList(vehicle.destinationObject.transform.position);
                    vehicle.destinationObject.transform.position = destPlane.transform.position;
                    UnitLocationsManager.UsedUnitLocations.Add(destPlane.transform.position);
                    vehicle.currentState = vehicle.moveState;
                    vehicle.currentState.ToMoveState(vehicle.unloadingState);
                }
                else
                {
                    NotificationSystem.SendNotification("Destination Error", "A Collector is unable to find a Refinery.\n Create a one and assign it to the Collector Manually", Color.red, vehicle.transform.position);
                    ToIdleState();
                }
                    
            }
        }
        else
        {
            vehicle.currentState = vehicle.moveState;
            vehicle.currentState.ToMoveState(null);
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

    }

    private void ExitThisStateActions()
    {
        collectingStarted = false;
        vehicle.Spinner.SetActive(false);
        curPile = null;
    }
}
