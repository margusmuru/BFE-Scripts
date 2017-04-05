using UnityEngine;
using System.Collections;

public class Collector : Vehicle, IDamageable {

    public GameObject FrontWheels;
    public GameObject RearWheels;
    public GameObject MiddleWheels;
    public GameObject Cargo;
    public GameObject Spinner;
    public GameObject staticSetCargo;
    
    public IVehicleState collectingState;
    public IVehicleState moveToFactoryState;
    public IVehicleState moveToResourcesState;
    public IVehicleState unloadingState;
    public IVehicleState idleState;

    public float collectedAmount = 0;
    [HideInInspector]
    public float maxCollectionAmount = 1000;
    [HideInInspector]
    public GameObject myResources;
    [HideInInspector]
    public GameObject myFactory;

    public override void Awake()
    {
        base.Awake();
        collectingState = new CollectingState(this);
        unloadingState = new UnloadingState(this);
        moveState = new ColMoveState(this);
        idleState = new ColIdleState(this);
    }

    public override void Start ()
    {
        base.Start();
        LevelMasterRef.AddUnit(tagObj, "collector");
        if (FindFactory() && FindResources())
        {
            Vector3 loc = UnitLocationsManager.FindLocation(myResources.transform.position, transform.position, 200, false).transform.position;
            if (loc != null)
                StartMoving(loc, collectingState);
            else
                currentState = idleState;

        }
        else
            currentState = idleState;
    }
	
	public override void Update ()
    {
        base.Update();
	}

    public bool FindFactory()
    {
        float closeDis = float.MaxValue;
        GameObject curFactory = null;
        foreach (GameObject obj in LevelMasterRef.FriendlyUnits)
        {
            if (obj.name.Contains("Factory"))
            {
                float curDis = (gameObject.transform.position - obj.transform.position).sqrMagnitude;
                if (curDis < closeDis)
                {
                    closeDis = curDis;
                    curFactory = obj;
                }
            }
        }
        if (curFactory != null)
        {
            myFactory = curFactory;
            return true;
        }
        else
        {
            myFactory = null;
            return false;
        }
    }

    public bool FindResources()
    {
        GameObject curPile = UnitLocationsManager.GetCloseResourcePile(transform.position);
        if (curPile != null)
        {
            myResources = curPile;
            return true;
        }
        else
        {
            myResources = null;
            return false;
        }
    }

    public void SetFactory(GameObject factory)
    {
        myFactory = factory;
        if (myResources != null && LevelMasterRef.HumanPlayer)
            NotificationSystem.SendNotification("Collector", "A Refinery has been set. \n Resources destination present.", Color.green, transform.position);
        else if (LevelMasterRef.HumanPlayer)
            NotificationSystem.SendNotification("Collector", "A Refinery has been set. \n Resources destination missing.", Color.yellow, transform.position);

        if (myFactory && myResources)
            StartAutomaticMovement();
    }

    public void SetResources(GameObject resources)
    {
        myResources = resources;
        if (myFactory != null && LevelMasterRef.HumanPlayer)
            NotificationSystem.SendNotification("Collector", "A ResorucePile has been set. \n Home Refinery present.", Color.green, transform.position);
        else if (LevelMasterRef.HumanPlayer)
            NotificationSystem.SendNotification("Collector", "A ResorucePile has been set \n Home Refinery missing.", Color.yellow, transform.position);
        if (myFactory && myResources)
            StartAutomaticMovement();
    }

    private void StartAutomaticMovement()
    {
        if (collectedAmount > maxCollectionAmount * 0.5f)
        {
            GameObject destPlane = UnitLocationsManager.FindLocation(myFactory.transform.position, transform.position, 100, false);
            if (destPlane != null)
            {
                StartMoving(destPlane.transform.position, unloadingState);
            }
        }
        else
        {
            GameObject destPlane = UnitLocationsManager.FindLocation(myResources.transform.position, 200, false);
            if (destPlane != null)
            {
                StartMoving(destPlane.transform.position, collectingState);
            }
        }
    }

    public void StartMoving(Vector3 destPlane, IVehicleState state)
    {
        UnitLocationsManager.ClearLocFromUsedList(destinationObject.transform.position);
        destinationObject.transform.position = destPlane;
        UnitLocationsManager.UsedUnitLocations.Add(destPlane);
        currentState = moveState;
        currentState.ToMoveState(state);
        if (LevelMasterRef.HumanPlayer)
            NotificationSystem.SendNotification("Collector", "Getting to work", Color.green, transform.position);
    }
}
