using UnityEngine;
using System.Collections;
using System;

public class BuildingBarracks : Building, IBuilding, IDamageable, IBuildingUnitCreator
{
    public GameObject[] spawnLocations;

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

    public override void Awake()
    {
        base.Awake();
        activeState = new BarracksActiveState(this);
        inactiveState = new BarracksInActiveState(this);
        unitBuildingState = new BarracksUnitBuildState(this);
        buildState = new BasicBuildState(this);
        deathState = new BasicDeathState(this);
        sellState = new BasicSellState(this);
        buildQueue = new ArrayList();
        curBuildProgress = 1;
        originalMat = StaticSet.GetComponent<Renderer>().material;

        //SetSpawnPositions();
        InvokeRepeating("CheckPower", 1f, 1f);
        InvokeRepeating("CheckQueue", 1f, 1f);
    }

    public override void Start()
    {
        base.Start();

        MaxHealth = UnitValues.BarracksHealth;
        levelMaster.AddBuilding(tagObj, UnitName);

        levelMaster.RemoveMoney(UnitValues.BarracksPrice);

        currentState = buildState;
        currentState.ToBuildState();

        FindClosePlanes(2);
    }

    public override void Update()
    {
        base.Update();
        currentState.Update();

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

    public override void FinishBuilding()
    {
        base.FinishBuilding();
        levelMaster.AddPower(UnitValues.BarracksPower, false);
    }

    public override void DeathActions()
    {
        base.DeathActions();
        levelMaster.RemovePower(UnitValues.BarracksPower, false);
    }

    public override void DeathActionsDelayed()
    {
        base.DeathActionsDelayed();
    }

    public override void SellActions()
    {
        base.SellActions();
        levelMaster.RemovePower(UnitValues.BarracksPower, false);
        levelMaster.AddMoney(Mathf.RoundToInt(UnitValues.BarracksPrice * 0.8f));
    }

    public void AddToQueue(int unitID)
    {
        if (buildQueue.Count < 4)
        {
            switch (unitID)
            {
                case 0: //soldier
                    buildQueue.Add(unitID);
                    levelMaster.RemoveMoney(UnitValues.SoldierPrice);
                    break;
                default:
                    Debug.Log("<color=red>unexpected unitID in barracks: " + unitID + "</color>");
                    break;
            }
        }
    }

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
            Debug.Log("<color=red>unexpexted queue index in barracks: " + buildIndex + "</color>");
        }
    }

    //deprecated
    public void SetSpawnPositions()
    {
        GameObject loc = null;
        foreach (GameObject go in spawnLocations)
        {
            loc = UnitLocationsManager.FindClosestPlaneTo(go.transform.position);
            go.transform.position = loc.transform.position;
        }
    }
}
