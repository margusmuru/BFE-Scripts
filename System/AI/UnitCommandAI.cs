using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitCommandAI : MonoBehaviour {

    private AI ai;

    public void Awake()
    {
        ai = GetComponent<AI>();
        InvokeRepeating("UpdateCycle", ai.StartDelay + 15, 3f);
    }

    private void UpdateCycle()
    {
        MoveUnitsFromBarracks();

    }

    private void MoveUnitsFromBarracks()
    {
        
        foreach (GameObject barracksTagObj in ai.lm.BarracksArray)
        {
            //BuildingBarracks barracks = barracksTagObj.transform.parent.gameObject.GetComponent<BuildingBarracks>();
            //check distance
            foreach (GameObject unitTagObj in ai.lm.SoldierArray)
            {
                GameObject destObj = unitTagObj.transform.parent.gameObject.GetComponent<ICommandUnit>().getDestinationObject();
                float curDis = (barracksTagObj.transform.position - destObj.transform.position).sqrMagnitude;
                if (curDis < 50)
                {
                    List<GameObject> avoidList = new List<GameObject>();
                    foreach (GameObject unit in ai.lm.SoldierArray)
                        avoidList.Add(unit.transform.parent.gameObject.GetComponent<ICommandUnit>().getDestinationObject());
                    foreach (GameObject barracks in ai.lm.BarracksArray)
                        avoidList.Add(barracks);
                    //find a new position for the unit.
                    Vector3 destination = UnitLocationsManager.GetLocationForUnit(CollectBuildings(), avoidList, 7, 30);
                    UnitLocationsManager.ClearLocFromUsedList(destObj.transform.position);
                    UnitLocationsManager.UsedUnitLocations.Add(destination);
                    unitTagObj.transform.parent.gameObject.GetComponent<ICommandUnit>().MoveTo(destination);
                    Debug.Log("unit moved away from barracks");
                }
            }
        }
    }

    private List<GameObject> CollectBuildings()
    {
        List<GameObject> buildings = new List<GameObject>();
        foreach (GameObject item in ai.lm.BaseArray)
            buildings.Add(item);
        foreach (GameObject item in ai.lm.PowerStationArray)
            buildings.Add(item);
        foreach (GameObject item in ai.lm.FactoryArray)
            buildings.Add(item);
        foreach (GameObject item in ai.lm.BarracksArray)
            buildings.Add(item);
        return buildings;
    }
}
