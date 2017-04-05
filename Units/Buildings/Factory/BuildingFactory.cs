using UnityEngine;
using System.Collections;

public class BuildingFactory : Building, IBuilding, IDamageable, IBuildingUnitCreator
{
    public ParticleSystem SteamEmitter;
    private ArrayList buildQueue;
    public ArrayList BuildQueue
    {
        get { return buildQueue; }
    }
    

    public int BuiltUnitCount = 0;
    public int CurBuildUnit; // id of currently build unit
    private float curBuildProgress;
    public float CurBuildProgress
    {
        get { return curBuildProgress; }
        set { curBuildProgress = value; }
    }

    public bool buildRunning = false; // true if currently building an unit
    public bool cancelReq; // trigger to cancel current building progress
    public GameObject[] UnitPrefabs;

    private float curResources = 0;
    private bool processingResources = false;
    private float processingTime = 0;

    public override void Awake()
    {
        base.Awake();
        activeState = new FactoryActiveState(this);
        inactiveState = new FactoryInActiveState(this);
        unitBuildingState = new FactoryUnitBuildState(this);
        buildState = new BasicBuildState(this);
        deathState = new BasicDeathState(this);
        sellState = new BasicSellState(this);
        buildQueue = new ArrayList();
        curBuildProgress = 1;
        originalMat = StaticSet.GetComponent<Renderer>().material;

        InvokeRepeating("CheckPower", 1f, 1f);
        InvokeRepeating("CheckQueue", 1f, 1f);
    }

    public override void Start()
    {
        base.Start();

        MaxHealth = UnitValues.FactoryHealth;
        levelMaster.AddBuilding(tagObj, UnitName);

        levelMaster.RemoveMoney(UnitValues.FactoryPrice);

        currentState = buildState;
        currentState.ToBuildState();

        FindClosePlanes(1);
    }

    public override void Update()
    {
        base.Update();
        currentState.Update();

        //resources processing
        if (!processingResources && curResources >= 100 && currentState != deathState)
        {
            processingResources = true;
            curResources -= 100;
            processingTime = Time.time + 2f;
            SteamEmitter.Play();
        }
        if (processingResources && Time.time > processingTime)
        {
            if (!UnitActive)
            {
                processingTime = Time.time + 2f;
                SteamEmitter.Stop();
            }
            else
            {
                if (curResources < 100)
                    SteamEmitter.Stop();
                else
                    SteamEmitter.Play();
                double moneyToAdd = 100 * (1 + UnitLevel * 0.2);
                levelMaster.AddMoney(Mathf.RoundToInt((float)moneyToAdd));
                processingResources = false;
            }
        }
    }

    private void CheckQueue()
    {
        if (currentState == activeState && buildQueue.Count > 0 && !buildRunning)
            currentState.ToUnitBuildState();
    }

    private void CheckPower()
    {
        if (levelMaster.UsedPowerCount > levelMaster.TotalPowerCount && currentState != inactiveState)
            currentState.ToInActiveState();
        else if (currentState == inactiveState && levelMaster.UsedPowerCount <= levelMaster.TotalPowerCount)
            currentState.ToActiveState();
    }


    // add an unit to the building queue
    public void AddToQueue(int unitID)
    {
        if (levelMaster.CollectorCount + levelMaster.ExpectedCollectorCount < levelMaster.MaxCollectorCount &&
            buildQueue.Count < 4)
        {
            switch (unitID)
            {
                case 0: //collector
                    buildQueue.Add(unitID);
                    levelMaster.ExpectedCollectorCount++;
                    levelMaster.RemoveMoney(UnitValues.CollectorPrice);
                    break;
                default:
                    Debug.Log("<color=red>unexpected unitID in factory: " + unitID + "</color>");
                    break;
            }
        }
    }


    // remove an unit from building queue and call cancelReq trigger if needed
    public void RemoveFromQueue(int buildIndex)
    {
        int _indexValue = -1; // save the unitID value at quueue-position buildIndex
        if (buildQueue.Count > buildIndex)
        {
            _indexValue = (int)buildQueue[buildIndex];
            buildQueue.RemoveAt(buildIndex);
            cancelReq = true;
            if (_indexValue == 0) //collector
            {
                levelMaster.ExpectedCollectorCount--;
                levelMaster.AddMoney(Mathf.RoundToInt(UnitValues.CollectorPrice));
            }
        }
        else
        {
            Debug.Log("<color=red>unexpexted queue index in factory: " + buildIndex + "</color>");
        }
    }

















    public override void FinishBuilding()
    {
        base.FinishBuilding();
        levelMaster.AddPower(UnitValues.FactoryPower, false);
    }

    public override void DeathActions()
    {
        base.DeathActions();
        levelMaster.RemovePower(UnitValues.FactoryPower, false);
    }

    public override void DeathActionsDelayed()
    {
        base.DeathActionsDelayed();
    }

    public override void SellActions()
    {
        base.SellActions();
        levelMaster.RemovePower(UnitValues.FactoryPower, false);
        levelMaster.AddMoney(Mathf.RoundToInt(UnitValues.FactoryPrice * 0.8f));
    }

    public override void UpgradeBuilding()
    {
        switch (UnitLevel)
        {
            case 0:
                levelMaster.AddCollectorMax();
                break;
            case 1:
                levelMaster.AddCollectorMax();
                break;
            case 2:
                levelMaster.AddCollectorMax();
                break;
            default:
                break;
        }

        base.UpgradeBuilding();
    }

    public void AddResources(float amount)
    {
        curResources += amount;
    }

}
