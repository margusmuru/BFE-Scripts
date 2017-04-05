using UnityEngine;
using System.Collections;

public class BarracksUnitBuildState : IBuildingState {

    private BuildingBarracks building;

    private bool buildingStarted = false;
    private float buildTime = 0;
    private float buildEndTime = 0;

    public BarracksUnitBuildState(BuildingBarracks building)
    {
        this.building = building;
    }

    public void Update()
    {
        //initial values for build progress
        if (building.BuildQueue.Count > 0 && !buildingStarted)
        {
            buildingStarted = true;
            building.CurBuildProgress = 0;
            buildTime = ChooseBuildTime((int)building.BuildQueue[0]);
            buildEndTime = Time.time + buildTime;
            building.Spinner.SetActive(true);
        }

        if (buildingStarted)
        {
            building.Spinner.transform.Rotate(0, 0, 30 * Time.deltaTime);

            if (buildEndTime > Time.time && !building.cancelReq)
            {
                //still building
                building.CurBuildProgress = (buildEndTime - Time.time) / buildTime;
            }
            else if (!building.cancelReq)
            {
                //building done
                building.StartCoroutine(InstantiateUnit((int)building.BuildQueue[0]));

                building.BuildQueue.RemoveAt(0);
                building.BuiltUnitCount++;

                buildingStarted = false;
            }
            else
            {
                //building canceled
                building.cancelReq = false;
                buildingStarted = false;

            }
        }

        if (building.BuildQueue.Count == 0)
            ToActiveState();

    }

    private float ChooseBuildTime(int unitID)
    {
        float buildTime = 0f;
        switch (building.UnitLevel)
        {
            case 1:
                buildTime = 12f;
                break;
            case 2:
                buildTime = 9f;
                break;
            case 3:
                buildTime = 6f;
                break;
            default:
                buildTime = 15f;
                break;
        }
        if (GameInfo.FastBuild)
            buildTime = 3;
        return buildTime;
    }

    IEnumerator InstantiateUnit(int unitID)
    {
        GameObject spawnLocation = null;
        while (spawnLocation == null)
        {
            spawnLocation = GetSpawnLocation();
            // no location found or unit inactive, lets wait
            if (spawnLocation == null ||
                (building.currentState != building.activeState && building.currentState != building.unitBuildingState))
            {
                NotificationSystem.SendNotification("Debug Alert!", "A Barracks cannot spawn an unit", Color.yellow, building.transform.position);
                yield return new WaitForSeconds(3);
            }
        }
        Object.Instantiate(building.UnitPrefabs[0], spawnLocation.transform.position, Quaternion.identity);
        if (building.levelMaster.HumanPlayer)
            NotificationSystem.SendNotification("Unit created", "A new unit has been created", Color.green, building.transform.position);
    }

    private GameObject GetSpawnLocation()
    {
        foreach (GameObject sp in building.spawnLocations)
        {
            if (!UnitLocationsManager.InUsedLocationsList(UnitLocationsManager.FindClosestPlaneTo(sp.transform.position).transform.position))
                return sp;
        }
        return null;
    }

    public void ToActiveState()
    {
        building.buildRunning = false;
        building.Spinner.SetActive(false);
        building.currentState = building.activeState;
        building.currentState.ToActiveState();
    }

    public void ToDeathState()
    {
        building.buildRunning = false;
        building.Spinner.SetActive(false);
        building.currentState = building.deathState;
        building.currentState.ToDeathState();
    }

    public void ToInActiveState()
    {
        building.buildRunning = false;
        building.Spinner.SetActive(false);
        //save remaining time
        buildEndTime -= Time.time;

        building.currentState = building.inactiveState;
        building.currentState.ToInActiveState();
    }

    public void ToBuildState()
    {

    }

    public void ToUnitBuildState()
    {
        building.buildRunning = true;
        if (buildingStarted)
        {
            buildEndTime += Time.time;
        }
    }

    public void ToSellState()
    {
        building.buildRunning = false;
        building.Spinner.SetActive(false);
        building.currentState = building.sellState;
        building.currentState.ToSellState();
    }
}
