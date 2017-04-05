using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingManagerAI : MonoBehaviour {

    private AI ai;
    public GameObject BasePrefab;
    public GameObject PowerStationPrefab;
    public GameObject FactoryPrefab;
    public GameObject BarracksPrefab;

    public GameObject testObj;

    private bool initialFetchDone = false;
    List<GameObject> fetchedList;


    public void Awake()
    {
        ai = GetComponent<AI>();
        fetchedList = new List<GameObject>();
        InvokeRepeating("UpdateCycle", ai.StartDelay, 3f);
    }

    public void Update()
    {
        if (!initialFetchDone && GameInfo.LevelReady)
        {
            initialFetchDone = true;
            GameObject unit = (GameObject)ai.lm.FriendlyUnits[0];
            fetchedList = UnitLocationsManager.PreFetchLocationsV2(unit.transform.position, 50, 6);
            if (fetchedList.Count == 0)
                Debug.Log("<color=red> FATAL ERROR: fetchedList too short: </color>" + fetchedList.Count);
            else
                Debug.Log("FetcedList count: " + fetchedList.Count);
            /*
            for (int i = 0; i < fetchedList.Count; i++)
            {
                //if (i % 7 == 0 || i == 0)
                    Instantiate(testObj, fetchedList[i].transform.position, Quaternion.identity);
            }
            */

        }
    }

    private void UpdateCycle()
    {

        if (!ai.lm.IsBuilding && ai.EnableAI)
        {
            // build a base 
            if (ai.lm.BaseCount == 0 && ai.lm.MoneyCount > UnitValues.BasePrice)
            {
                ai.lm.SetCurrentBuildingConstructionState(true);
                BuildBase();
            }
            // a base is built
            else if (ai.lm.BaseArray.Count > 0)
            {
                // powerstation
                if (ai.lm.UsedPowerCount + 75 > ai.lm.TotalPowerCount
                    && ai.lm.MoneyCount > UnitValues.PowerStationPrice)
                {
                    ai.lm.SetCurrentBuildingConstructionState(true);
                    BuildPowerStation();
                }
                //build factory
                else if (ai.lm.FactoryArray.Count == 0 && ai.lm.MoneyCount > UnitValues.FactoryPrice)
                {
                    ai.lm.SetCurrentBuildingConstructionState(true);
                    BuildFactory();
                }
                //build barracks
                else if (ai.lm.BarracksArray.Count == 0 && ai.lm.MoneyCount > UnitValues.BarracksPrice)
                {
                    ai.lm.SetCurrentBuildingConstructionState(true);
                    BuildBarracks();
                }
                //Debug.Log("<color=blue> Factory Count: " + ai.lm.FactoryArray.Count + "</color>");
                //Debug.Log("<color=blue> Barracks Count: " + ai.lm.BarracksArray.Count + "</color>");
            }
        }
    }

    private void BuildBase()
    {
        Vector3 location = Vector3.one;
        //inital unit
        GameObject initialUnit = (GameObject) ai.lm.FriendlyUnits[0];
        //location = UnitLocationsManager.GetLocation(fetchedList, initialUnit.transform.position, 2, 10, 30, 20);
        location = fetchedList[0].transform.position;
        if (location != Vector3.one)
        {
            Instantiate(BasePrefab, location, Quaternion.identity);
        }
        else
        {
            ai.lm.SetCurrentBuildingConstructionState(false);
            if (ai.lm.enableLog)
                Debug.Log("<color=red>Fatal error:</color> Unable to find a location for Base construction");
        }
    }

    private void BuildPowerStation()
    {
        GameObject baseObj = (GameObject)ai.lm.BaseArray[0];
        Vector3 location = Vector3.one;
        Debug.Log(fetchedList.Count);
        Debug.Log(baseObj.transform.position);
        location = UnitLocationsManager.GetLocation(fetchedList, baseObj.transform.position, 1, 10, 30, 10);
        if (location != Vector3.one)
        {
            Instantiate(PowerStationPrefab, location, Quaternion.identity);
        }
        else
        {
            ai.lm.SetCurrentBuildingConstructionState(false);
            if (ai.lm.enableLog)
                Debug.Log("<color=red>Fatal error:</color> Unable to find a location for PowerStation construction");
        }
    }

    private void BuildFactory()
    {
        GameObject baseObj = (GameObject)ai.lm.BaseArray[0];
        GameObject resourcePile = UnitLocationsManager.GetCloseResourcePile(baseObj.transform.position);
        Vector3 location = Vector3.one;

        if (resourcePile != null)
        {
            //get friendly units and sort them by distance to base
            List<GameObject> friendlyUnits = SortGameObjects(ai.lm.FriendlyUnits, baseObj);
            foreach (GameObject obj in friendlyUnits)
            {
                location =
                UnitLocationsManager.GetLocationForFactory(fetchedList, resourcePile.transform.position, obj.transform.position, 2, 10, 30, 10);
                if (location != Vector3.one)
                    break;
            }
        }

        if (location != Vector3.one)
        {
            Instantiate(FactoryPrefab, location, Quaternion.identity);
        }
        else
        {
            ai.lm.SetCurrentBuildingConstructionState(false);
            if (ai.lm.enableLog)
            {
                if (resourcePile == null)
                    Debug.Log("<color=yellow>Warning:</color> Unable to find resources");
                else if (location == Vector3.one)
                    Debug.Log("<color=red>Fatal error:</color> Unable to find a location for Refinery construction");
            }    
        }
    }

    private void BuildBarracks()
    {
        GameObject baseObj = (GameObject)ai.lm.BaseArray[0];
        Vector3 location = fetchedList[7].transform.position;
        //location = UnitLocationsManager.GetLocation(fetchedList, baseObj.transform.position, 2, 10, 30, 20);
        if (location != Vector3.one)
        {
            Instantiate(BarracksPrefab, location, Quaternion.identity);
        }
        else
        {
            ai.lm.SetCurrentBuildingConstructionState(false);
            if (ai.lm.enableLog)
                Debug.Log("<color=red>Fatal error:</color> Unable to find a location for Barracks construction");
        }
    }

    //deprecated
    private Vector3 GetRandomSecondaryLocFromFetchedList()
    {
        Vector3 loc = Vector3.one;
        int secondaryCount = (fetchedList.Count - 7) / 7;
        int rand = Random.Range(1, secondaryCount + 1);
        loc = fetchedList[rand * 7].transform.position;
        return loc;
    }


    private List<GameObject> SortGameObjects(List<GameObject> searchList, GameObject refLoc)
    {
        List<GameObject> newList = new List<GameObject>();
        foreach (GameObject obj in searchList)
            newList.Add(obj);
        newList.Sort(delegate (GameObject c1, GameObject c2) {
            return (refLoc.transform.position - c1.transform.position).sqrMagnitude.CompareTo
                        (((refLoc.transform.position - c2.transform.position).sqrMagnitude));
        });
        return newList;
    }
    private List<GameObject> SortGameObjects(ArrayList searchList, GameObject refLoc)
    {
        List<GameObject> newList = new List<GameObject>();
        foreach (GameObject obj in searchList)
            newList.Add(obj);
        newList.Sort(delegate (GameObject c1, GameObject c2) {
            return (refLoc.transform.position - c1.transform.position).sqrMagnitude.CompareTo
                        (((refLoc.transform.position - c2.transform.position).sqrMagnitude));
        });
        return newList;
    }

}
