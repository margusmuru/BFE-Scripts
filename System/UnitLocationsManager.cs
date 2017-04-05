using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UnitLocationsManager : MonoBehaviour
{
    public static GameObject[] PlacementPlanes;
    public static List<Vector3> UsedUnitLocations;
    public static List<Vector3> UsedCoverLocations;
    public static Vector3[] Sectors = {
        new Vector3(0, 0, 0),
        new Vector3(0f, 0f, -2.3f),
        new Vector3(1.95f, 0f, -1.14f),
        new Vector3(1.95f, 0f, 1.15f),
        new Vector3(0f, 0f, 2.3f),
        new Vector3(-1.95f, 0f, 1.15f),
        new Vector3(-1.95f, 0f, -1.15f)
    };

    public static ArrayList ResourcesList;
    public static ArrayList Team1Units;
    public static ArrayList Team2Units;



    void Awake()
    {
        Team1Units = new ArrayList();
        Team2Units = new ArrayList();

        ResourcesList = new ArrayList();
        UsedUnitLocations = new List<Vector3>();
        UsedCoverLocations = new List<Vector3>();

        CollectPlanes();
        CollectResources();
    }

    //========================== Collect Map Items =============================================================================================

    private void CollectPlanes()
    {
        var list1 = GameObject.FindGameObjectsWithTag("PlacementPlane_Open");
        var list2 = GameObject.FindGameObjectsWithTag("PlacementPlane_Forest_Open");
        var list3 = GameObject.FindGameObjectsWithTag("PlacementPlane_Held");
        var list4 = GameObject.FindGameObjectsWithTag("PlacementPlane_Forest_Held");
        PlacementPlanes = new GameObject[list1.Length + list2.Length + list3.Length + list4.Length];
        list1.CopyTo(PlacementPlanes, 0);
        list2.CopyTo(PlacementPlanes, list1.Length);
        list3.CopyTo(PlacementPlanes, list1.Length + list2.Length);
        list4.CopyTo(PlacementPlanes, list1.Length + list2.Length + list3.Length);
        Debug.Log("LevelMaster L48: PlacementPlanes in the scene: " + PlacementPlanes.Length);
    }

    private void CollectResources()
    {
        var list = GameObject.FindGameObjectsWithTag("Resource");
        foreach (GameObject go in list)
        {
            ResourcesList.Add(go);
        }
    }

        //========================== Unit location management in global lists =======================================================================

    // clears given location from UsedUnitDestinations list
    public static bool ClearLocFromUsedList(Vector3 _loc)
    {
        bool _success = false;
        foreach (Vector3 _listItem in UsedUnitLocations)
        {
            if (_listItem == _loc)
            {
                UsedUnitLocations.Remove(_listItem);
                _success = true;
                break;
            }
        }
        return _success;
    }

    // clears given location from UsedUnitCoverLocations list
    public static bool ClearLocFromUsedCoverList(Vector3 _loc)
    {
        bool _success = false;
        foreach (Vector3 _listItem in UsedCoverLocations)
        {
            if (_listItem.x == _loc.x && _listItem.y == _loc.y && _listItem.z == _loc.z)
            {
                UsedCoverLocations.Remove(_listItem);
                _success = true;
                break;
            }
        }
        return _success;
    }

    //FindDestPlane helper
    // checks if given vector3 is in UsedUnitLocations list
    public static bool InUsedLocationsList(Vector3 _input)
    {
        bool _inAvoidList = false;
        foreach (Vector3 _loc in UnitLocationsManager.UsedUnitLocations)
        {
            if (_loc.x == _input.x && _loc.y == _input.y && _loc.z == _input.z)
            {
                _inAvoidList = true;
                break;
            }
        }
        return _inAvoidList;
    }

    //FindDestPlane Helper
    // check if a location is in UsedCoverLocationsList
    public static bool InCoverLocationsList(Vector3 _input)
    {
        bool _inCoverList = false;
        foreach (Vector3 _loc in UnitLocationsManager.UsedCoverLocations)
        {
            if (_loc.x == _input.x && _loc.y == _input.y && _loc.z == _input.z)
            {
                _inCoverList = true;
                break;
            }
        }
        return _inCoverList;
    }



    /**
     * find closest resourePile with resources to given location
    */
    public static GameObject GetCloseResourcePile(Vector3 _location)
    {
        GameObject _closestPile = null; // closest plane to search start location
        float _curDis;
        float _distance = Mathf.Infinity;
        // find the closest free plane to given location
        foreach (GameObject go in ResourcesList)
        {
            _curDis = (go.transform.position - _location).sqrMagnitude;
            if (_curDis < _distance && go.GetComponent<ResourcesPile>().ResourcesLeft > 0)
            {
                _closestPile = go;
                _distance = _curDis;
            }
        }
        return _closestPile;
    }





    /**
     * FindClosestOpenPlaneTo finds the closest plane that has "PlacementPlane_Open" state
     * location - Vector3 location for the starting point
    */
    public static GameObject FindClosestOpenPlaneTo(Vector3 location)
    {
        GameObject _closestPlane = null;
        float _curDis;
        float _distance = Mathf.Infinity;

        foreach (GameObject go in PlacementPlanes)
        {
            if (go.tag == "PlacementPlane_Open")
            {
                _curDis = (go.transform.position - location).sqrMagnitude;
                if (_curDis < _distance)
                {
                    _closestPlane = go;
                    _distance = _curDis;
                }
            }
        }
        return _closestPlane;
    }

    /**
     * returns any closest plane to given vector 3
    */
    public static GameObject FindClosestPlaneTo(Vector3 location)
    {
        GameObject closestPlane = null;
        float curDis;
        float distance = Mathf.Infinity;

        foreach (GameObject go in PlacementPlanes)
        {
            curDis = (go.transform.position - location).sqrMagnitude;
            if (curDis < distance)
            {
                closestPlane = go;
                distance = curDis;
            }
        }
        return closestPlane;
    }

    /**
     * FindClosestFreePlaneTo finds closest free plane to given location.
     * Takes account to units positions
    */
    public static GameObject FindClosestFreePlaneTo(Vector3 location)
    {
        GameObject _closestPlane = null;
        float _curDis;
        float _distance = Mathf.Infinity;

        foreach (GameObject go in PlacementPlanes)
        {
            if (go.tag == "PlacementPlane_Open" && PlaneSectorsFreeCount(go, true) == 7)
            {
                _curDis = (go.transform.position - location).sqrMagnitude;
                if (_curDis < _distance)
                {
                    _closestPlane = go;
                    _distance = _curDis;
                }
            }
        }
        return _closestPlane;
    }

    /**
     * return int of how many sectors of the given plane are free
     * checks all LevelMasterAI lists, coverList check is optional
    */
    private static int PlaneSectorsFreeCount(GameObject _plane, bool _checkCoverList)
    {
        if (_plane != null)
        {
            int _freeSectorsCount = 0;
            Vector3 _loc = _plane.transform.position;
            for (int i = 0; i < 7; i++)
            {
                Vector3 _curLoc = _loc + Sectors[i];
                // further testing depends on wether we need to test for coverlocations or not
                if (_checkCoverList)
                {
                    if (!InUsedLocationsList(_curLoc) && !InCoverLocationsList(_curLoc))
                    {
                        _freeSectorsCount++;
                    }
                }
                else // do not check coverList
                {
                    if (!InUsedLocationsList(_curLoc))
                    {
                        _freeSectorsCount++;

                    }
                }

            }
            return _freeSectorsCount;
        }
        else
            return 0;
    }

    /**
     * returns placementplane
     * users: collector, ..
    */
    public static GameObject FindLocation(Vector3 target, float distance, bool checkCoverList)
    {
        GameObject closestPlane = null; // closest plane to target
        float curDis;

        foreach (GameObject go in PlacementPlanes)
        {
            // use triple_check to reduce processing time. if plane tag is not free, discard, then if the whole plane is occupied, discard, then check for occupied sectors.
            if (go.tag == "PlacementPlane_Open" && !InUsedLocationsList(go.transform.position) 
                && PlaneSectorsFreeCount(go, checkCoverList) == 7)
            {
                curDis = (target - go.transform.position).sqrMagnitude;
                if (curDis < distance)
                {
                    closestPlane = go;
                    distance = curDis;
                }
            }
        } // foreach
        return closestPlane;
    }
    public static GameObject FindLocation(Vector3 target, Vector3 source, float distance, bool checkCoverList)
    {
        GameObject closestPlane = null; // closest plane to target
        float curDis;
        float closeDis = float.MaxValue;

        foreach (GameObject go in PlacementPlanes)
        {
            if (go.tag == "PlacementPlane_Open" && (go.transform.position - target).sqrMagnitude < distance &&
                !InUsedLocationsList(go.transform.position) && PlaneSectorsFreeCount(go, checkCoverList) == 7)
            {
                curDis = (source - go.transform.position).sqrMagnitude;
                if (curDis < closeDis)
                {
                    closeDis = curDis;
                    closestPlane = go;
                }
            }
        } // foreach

        return closestPlane;
    }


    //==================== AI ========================================================================================
    //deprecated
    public static List<GameObject> PreFetchLocations(Vector3 refLoc, float range)
    {
        List<GameObject> inRange = GetPlanesInRange(refLoc, 0, range, 0);
        //sort inRange list
        inRange.Sort(delegate (GameObject c1, GameObject c2) {
            return (refLoc - c1.transform.position).sqrMagnitude.CompareTo
                        (((refLoc - c2.transform.position).sqrMagnitude));
        });
        //copy inRange >> searchList
        List<GameObject> searchList = new List<GameObject>();
        foreach (GameObject obj in inRange)
            searchList.Add(obj);

        List < List < GameObject >> randomSets = new List<List<GameObject>>();
        //keeps track of 7-plane areas center-planes. avoids possibility of two subsets being the same
        List<GameObject> subSet = new List<GameObject>();
        //look for suitable combinations
        //we need 2 7-plane areas in given range
        for (int i = 0; i < 10; i++)
        {
            List<GameObject> curInRange = new List<GameObject>();
            //one random set
            foreach (GameObject plane in inRange)
            {
                //get closest 7 plane list
                List<GameObject> returnedList = GetClosest7PlaneArea(searchList, plane);
                if (returnedList.Count == 7 && !subSet.Contains(returnedList[0]))
                {
                    subSet.Add(returnedList[0]);
                    foreach (GameObject item in returnedList)
                    {
                        curInRange.Add(item);
                        searchList.Remove(item);
                    } 
                }
            }
            //save set
            if (curInRange.Count >= 14)
                randomSets.Add(curInRange);
            //repopulate searchList
            searchList.Clear();
            foreach (GameObject obj in inRange)
                searchList.Add(obj);
        }
        //return a random set
        return randomSets[Random.Range(0, randomSets.Count - 1)];
    }

    public static List<GameObject> PreFetchLocationsV2(Vector3 refLoc, float range, int randomness)
    {
        List<GameObject> searchList = GetPlanesInRange(refLoc, 0, range, 0);
        List<List<GameObject>> randomSets = new List<List<GameObject>>();

        List<GameObject> primaryAvoidList = new List<GameObject>();
        for (int i = 0; i < randomness; i++)
        {
            List<GameObject> curList = new List<GameObject>();
            List<GameObject> returnedList = GetClosest7PlaneArea(searchList, primaryAvoidList, false, refLoc);

            if (returnedList.Count == 7)
            {
                primaryAvoidList.Add(returnedList[0]);
                foreach (GameObject o in returnedList)
                    curList.Add(o);
                //secondary positions
                List<GameObject> centersToAvoid = new List<GameObject>();
                List<List<GameObject>> subSets = new List<List<GameObject>>();
                for (int j = 0; j < randomness; j++)
                {
                    List<GameObject> subReturnedList = GetClosest7PlaneArea(searchList, returnedList, centersToAvoid, true, returnedList[0]);

                    if (subReturnedList.Count == 7)
                    {
                        centersToAvoid.Add(subReturnedList[0]);
                        subSets.Add(subReturnedList);
                    }
                }
                //choose one of the sublists to add to the curlist
                if (subSets.Count > 0)
                {
                    int rand = Random.Range(0, subSets.Count - 1);
                    foreach (GameObject item in subSets[rand])
                        curList.Add(item);
                }

            }
            //add complete curList to randomSets
            if (curList.Count >= 14)
                randomSets.Add(curList);
        }

        if (randomSets.Count > 0)
            return randomSets[Random.Range(0, (randomSets.Count - 1))];
        else
            return new List<GameObject>();
    }

    private static List<GameObject> GetClosest7PlaneArea(List<GameObject> searchList, GameObject refLoc)
    {
        List<GameObject> list = new List<GameObject>();
        //sort
        searchList.Sort(delegate (GameObject c1, GameObject c2) {
            return (refLoc.transform.position - c1.transform.position).sqrMagnitude.CompareTo
                        (((refLoc.transform.position - c2.transform.position).sqrMagnitude));
        });
        //get area
        foreach (GameObject obj in searchList)
        {
            list = CountNeighbours(searchList, obj);
            if (list.Count == 6)
            {
                list.Insert(0, obj);
                break;
            }
            else
                list.Clear();
        }
        return list;
    }
    private static List<GameObject> GetClosest7PlaneArea(List<GameObject> searchList, Vector3 refLoc)
    {
        List<GameObject> list = new List<GameObject>();
        //sort
        searchList.Sort(delegate (GameObject c1, GameObject c2) {
            return (refLoc - c1.transform.position).sqrMagnitude.CompareTo
                        (((refLoc - c2.transform.position).sqrMagnitude));
        });
        //get area
        foreach (GameObject obj in searchList)
        {
            list = CountNeighbours(searchList, obj);
            if (list.Count == 6)
            {
                list.Insert(0, obj);
                break;
            }
            else
                list.Clear();
        }
        return list;
    }
    private static List<GameObject> GetClosest7PlaneArea(List<GameObject> searchList, List<GameObject> avoidList, bool avoidNeighbours, GameObject refLoc)
    {
        List<GameObject> list = new List<GameObject>();
        //sort
        searchList.Sort(delegate (GameObject c1, GameObject c2) {
            return (refLoc.transform.position - c1.transform.position).sqrMagnitude.CompareTo
                        (((refLoc.transform.position - c2.transform.position).sqrMagnitude));
        });
        //get area
        foreach (GameObject obj in searchList)
        {
            if (!avoidList.Contains(obj))
            {
                if (avoidNeighbours)
                    list = CountNeighbours(searchList, avoidList, obj);
                else
                    list = CountNeighbours(searchList, obj);

                if (list.Count == 6)
                {
                    list.Insert(0, obj);
                    break;
                }
                else
                    list.Clear();
            }
        }
        return list;
    }
    private static List<GameObject> GetClosest7PlaneArea(List<GameObject> searchList, List<GameObject> avoidList, List<GameObject> centerAvoidList, bool avoidNeighbours, GameObject refLoc)
    {
        List<GameObject> list = new List<GameObject>();
        //sort
        searchList.Sort(delegate (GameObject c1, GameObject c2) {
            return (refLoc.transform.position - c1.transform.position).sqrMagnitude.CompareTo
                        (((refLoc.transform.position - c2.transform.position).sqrMagnitude));
        });
        //get area
        foreach (GameObject obj in searchList)
        {
            if (!avoidList.Contains(obj) && !centerAvoidList.Contains(obj))
            {
                if (avoidNeighbours)
                    list = CountNeighbours(searchList, avoidList, obj);
                else
                    list = CountNeighbours(searchList, obj);

                if (list.Count == 6)
                {
                    list.Insert(0, obj);
                    break;
                }
                else
                    list.Clear();
            }
        }
        return list;
    }
    private static List<GameObject> GetClosest7PlaneArea(List<GameObject> searchList, List<GameObject> avoidList, bool avoidNeighbours, Vector3 refLoc)
    {
        List<GameObject> list = new List<GameObject>();
        //sort
        searchList.Sort(delegate (GameObject c1, GameObject c2) {
            return (refLoc - c1.transform.position).sqrMagnitude.CompareTo
                        (((refLoc - c2.transform.position).sqrMagnitude));
        });
        //get area
        foreach (GameObject obj in searchList)
        {
            if (!avoidList.Contains(obj))
            {
                if (avoidNeighbours)
                    list = CountNeighbours(searchList, avoidList, obj);
                else
                    list = CountNeighbours(searchList, obj);

                if (list.Count == 6)
                {
                    list.Insert(0, obj);
                    break;
                }
                else
                    list.Clear();
            }
        }
        return list;
    }

    //used by GetCLosest7PlaneArea,
    private static List<GameObject> CountNeighbours(List<GameObject> searchList, GameObject target)
    {
        List<GameObject> list = new List<GameObject>();
        foreach (GameObject obj in searchList)
        {
            if (obj != target)
            {
                float curDis = (target.transform.position - obj.transform.position).sqrMagnitude;
                if (curDis < 50)
                    list.Add(obj);
                if (list.Count == 6)
                    break;
            }
        }
        return list;
    }
    private static List<GameObject> CountNeighbours(List<GameObject> searchList, List<GameObject> avoidList, GameObject target)
    {
        List<GameObject> list = new List<GameObject>();
        foreach (GameObject obj in searchList)
        {
            if (obj != target && !avoidList.Contains(obj))
            {
                float curDis = (target.transform.position - obj.transform.position).sqrMagnitude;
                if (curDis < 50)
                    list.Add(obj);
                if (list.Count == 6)
                    break;
            }
        }
        return list;
    }

    private static List<GameObject> CountNeighboursWithTag(List<GameObject> searchList, GameObject target, string tag)
    {
        List<GameObject> list = new List<GameObject>();
        foreach (GameObject obj in searchList)
        {
            if (obj != target && obj.tag.Equals(tag))
            {
                float curDis = (target.transform.position - obj.transform.position).sqrMagnitude;
                if (curDis < 50)
                    list.Add(obj);
                if (list.Count == 6)
                    break;
            }
        }
        return list;
    }



    /**
     * AI buildings. 
    */
    public static Vector3 GetLocation(List<GameObject> avoidList, Vector3 location, int areaSize, float minDistance, float maxDistance, int randomCount)
    {
        GameObject closestPlane = null; // closest plane to search start location. becomes the result later
        // contains all planes that are between required distances and in required state
        List<GameObject>  planesInRange = GetPlanesInRange(location, minDistance, maxDistance, 7);
        List<GameObject>  searchList = new List<GameObject>(); // if areaSize==7 we use this to search for planes and remove searhed ones from this list, since we cant
                                                               //remove them from  planesInRange due to need of neighbours search
        List<GameObject>  suitablePlanes = new List<GameObject>(); // planes that meet requirements and can be selected for the result

        if (areaSize == 1 &&  planesInRange.Count > 0)
        {
            // find randomCount number of close planes to location in  planesInRange
            // when a plane is added, add it to  suitablePlanes
            while ( suitablePlanes.Count < randomCount &&  planesInRange.Count > 0)
            {
                GameObject obj = GetClosestObjInList(planesInRange, avoidList, location);
                if (obj != null)
                {
                    suitablePlanes.Add(obj);
                    planesInRange.Remove(obj);
                }
                else
                {
                    Debug.Log("error: " + planesInRange.Count + " " + suitablePlanes.Count + " " + minDistance + " " + maxDistance);
                    break;
                }
            }
        }
        else if (planesInRange.Count > 7)// areasize == 7
        {
            // copy  planesInRange content to  searchList to begin searching
            foreach (GameObject  plane in  planesInRange)
                searchList.Add(plane);
            // start searching for closest planes.
            while ( suitablePlanes.Count < randomCount &&  searchList.Count > 0)
            {
                GameObject resultPlane = GetClosestObjInList(searchList, avoidList, location);
                // we will not test that plane again, so remove it from  searchList
                searchList.Remove( resultPlane);
                //do another full search to find neighbours for this plane
                if (CountNeighbours(planesInRange, avoidList, resultPlane).Count == 6)
                {
                    suitablePlanes.Add(resultPlane);
                    break;
                }
            }//while
        } //else (areasize == 7)
        // choose randomly a suitable plane from the list
        if (randomCount > 0 && suitablePlanes.Count > 1)
        {
            Random.seed = (int)System.DateTime.Now.Ticks;
             closestPlane =  suitablePlanes[Random.Range(0,  suitablePlanes.Count - 1)];
        }
        else if (suitablePlanes.Count > 0)
            closestPlane = suitablePlanes[0];
        //return
        if (closestPlane == null)
            return Vector3.one;
        else
            return closestPlane.transform.position;
    }
    public static Vector3 GetLocation(List<GameObject> avoidList, Vector3 far, Vector3 near, int areaSize, float minDistance, float maxDistance, int randomCount)
    {
        GameObject closestPlane = null; // closest plane to search start location. becomes the result later
        // contains all planes that are between required distances and in required state
        List<GameObject> planesInRange = GetPlanesInRange(near, minDistance, maxDistance, 7);
        List<GameObject> searchList = new List<GameObject>(); // if areaSize==7 we use this to search for planes and remove searhed ones from this list, since we cant
                                                              //remove them from  planesInRange due to need of neighbours search
        List<GameObject> suitablePlanes = new List<GameObject>(); // planes that meet requirements and can be selected for the result

        if (areaSize == 1 && planesInRange.Count > 0)
        {
            // find randomCount number of close planes to location in  planesInRange
            // when a plane is added, add it to  suitablePlanes
            while (suitablePlanes.Count < randomCount && planesInRange.Count > 0)
            {
                GameObject obj = GetClosestObjInList(planesInRange, avoidList, near, far);
                if (obj != null)
                {
                    suitablePlanes.Add(obj);
                    planesInRange.Remove(obj);
                }
                else
                {     
                    Debug.Log("error: " + planesInRange.Count + " " + suitablePlanes.Count +  " " + minDistance + " " + maxDistance);
                    break;
                }
                    
            }
        }
        else if (planesInRange.Count > 7)// areasize == 7
        {
            // copy  planesInRange content to  searchList to begin searching
            foreach (GameObject plane in planesInRange)
                searchList.Add(plane);
            // start searching for closest planes.
            while (suitablePlanes.Count < randomCount && searchList.Count > 0)
            {
                float distance = Mathf.Infinity;
                float curDis = 0;
                GameObject resultPlane = null; // plane currently tested for neighbours
                foreach (GameObject plane in searchList)
                {
                    curDis = (plane.transform.position - far).sqrMagnitude + (plane.transform.position - near).sqrMagnitude;
                    if (curDis < distance && !avoidList.Contains(plane))
                    {
                        distance = curDis;
                        resultPlane = plane;
                    }
                }
                // we will not test that plane again, so remove it from  searchList
                searchList.Remove(resultPlane);
                //do another full search to find neighbours for this plane
                int neighboursCount = 0;
                foreach (GameObject plane in planesInRange)
                {
                    if (plane != resultPlane)
                    {
                        curDis = (plane.transform.position - resultPlane.transform.position).sqrMagnitude;
                        if (curDis < 50 && !avoidList.Contains(plane))
                            neighboursCount++;
                    }
                    if (neighboursCount == 6)
                    {
                        // if enough neighbours, plane is suitable and we can add it to the  suitablePlanes list
                        suitablePlanes.Add(resultPlane);
                        break;
                    }
                } //foreach                  
            }//while
        } //else (areasize == 7)
        // choose randomly a suitable plane from the list
        if (randomCount > 0 && suitablePlanes.Count > 1)
        {
            Random.seed = (int)System.DateTime.Now.Ticks;
            closestPlane = suitablePlanes[Random.Range(0, suitablePlanes.Count - 1)];
        }
        else if (suitablePlanes.Count > 0)
            closestPlane = suitablePlanes[0];
        //return
        if (closestPlane == null)
            return Vector3.one;
        else
            return closestPlane.transform.position;
    }
    public static Vector3 GetLocationForFactory(List<GameObject> avoidList, Vector3 far, Vector3 near, int freeNeighbours, float minDistance, float maxDistance, int randomCount)
    {
        GameObject closestPlane = null; // closest plane to search start location. becomes the result later
        // contains all planes that are between required distances and in required state
        List<GameObject> planesInRange = GetPlanesInRange(near, minDistance, maxDistance, 7);
        List<GameObject> searchList = new List<GameObject>(); // if areaSize==7 we use this to search for planes and remove searhed ones from this list, since we cant
                                                              // copy  planesInRange content to  searchList to begin searching
        foreach (GameObject plane in planesInRange)
            searchList.Add(plane);
        //remove them from  planesInRange due to need of neighbours search
        List<GameObject> suitablePlanes = new List<GameObject>(); // planes that meet requirements and can be selected for the result

        // find randomCount number of close planes to location in  planesInRange
        // when a plane is added, add it to  suitablePlanes
        while (suitablePlanes.Count < randomCount && searchList.Count > 0)
        {
            GameObject obj = GetClosestObjInList(searchList, avoidList, near, far);
            int count = CountNeighboursWithTag(planesInRange, obj, "PlacementPlane_Open").Count;
            if (obj != null && count >= freeNeighbours)
            {
                suitablePlanes.Add(obj);
                searchList.Remove(obj);
            }
            else
            {
                Debug.Log("error: " + searchList.Count + " " + suitablePlanes.Count + " " + minDistance + " " + maxDistance);
                break;
            }
        }
        // choose randomly a suitable plane from the list
        if (randomCount > 0 && suitablePlanes.Count > 1)
        {
            Random.seed = (int)System.DateTime.Now.Ticks;
            closestPlane = suitablePlanes[Random.Range(0, suitablePlanes.Count - 1)];
        }
        else if (suitablePlanes.Count > 0)
            closestPlane = suitablePlanes[0];
        //return
        if (closestPlane == null)
            return Vector3.one;
        else
            return closestPlane.transform.position;
    }

    /**
     * make a group that contains all suitable planes, taking account minDistance, 
     * maxDistance and required free sectors count
     * sorted
    */
    private static List<GameObject> GetPlanesInRange(Vector3 location, float minDistance, float maxDistance, int requiredFreeSectors)
    {
        List<GameObject> planesInRange = new List<GameObject>();
        foreach (GameObject plane in PlacementPlanes)
        {
            if (plane.tag == "PlacementPlane_Open")
            {
                float curDis = (plane.transform.position - location).sqrMagnitude;
                if (curDis < maxDistance * maxDistance && 
                    curDis > minDistance * minDistance &&
                    PlaneSectorsFreeCount(plane, true) >= requiredFreeSectors)
                    planesInRange.Add(plane);
            }
        }
        //sort inRange list
        planesInRange.Sort(delegate (GameObject c1, GameObject c2) {
            return (location - c1.transform.position).sqrMagnitude.CompareTo
                        (((location - c2.transform.position).sqrMagnitude));
        });
        return planesInRange;
    }

    /**
     * Get Close object in given List of GameObjects
    */
    private static GameObject GetClosestObjInList(List<GameObject> objList, Vector3 location)
    {
        float _distance = Mathf.Infinity;
        float _curDis = 0;
        GameObject _resultPlane = null;
        foreach (GameObject _plane in objList)
        {
            _curDis = (_plane.transform.position - location).sqrMagnitude;
            if (_curDis < _distance)
            {
                _distance = _curDis;
                _resultPlane = _plane;
            }
        }
        return _resultPlane;
    }
    private static GameObject GetClosestObjInList(List<GameObject> objList, List<GameObject> avoidList, Vector3 location)
    {
        float _distance = Mathf.Infinity;
        float _curDis = 0;
        GameObject _resultPlane = null;
        foreach (GameObject _plane in objList)
        {
            _curDis = (_plane.transform.position - location).sqrMagnitude;
            if (_curDis < _distance && !avoidList.Contains(_plane))
            {
                _distance = _curDis;
                _resultPlane = _plane;
            }
        }
        return _resultPlane;
    }
    private static GameObject GetClosestObjInList(List<GameObject> objList, Vector3 locNear, Vector3 locFar)
    {
        GameObject _curPlane = null;
        float _sumDis = Mathf.Infinity;

        foreach (GameObject _plane in objList)
        {
            float _curCloseDis = (_plane.transform.position - locNear).sqrMagnitude;
            float _curFarDis = (_plane.transform.position - locFar).sqrMagnitude;
            float _curSumDis = _curCloseDis + _curFarDis;

            if (_curSumDis < _sumDis)
            {
                _sumDis = _curSumDis;
                _curPlane = _plane;
            }
        }
        return _curPlane;
    }
    private static GameObject GetClosestObjInList(List<GameObject> objList, List<GameObject> avoidList, Vector3 locNear, Vector3 locFar)
    {
        GameObject _curPlane = null;
        float _sumDis = Mathf.Infinity;

        foreach (GameObject _plane in objList)
        {
            if (!avoidList.Contains(_plane))
            {
                float _curCloseDis = (_plane.transform.position - locNear).sqrMagnitude;
                float _curFarDis = (_plane.transform.position - locFar).sqrMagnitude;
                float _curSumDis = _curCloseDis + _curFarDis;

                if (_curSumDis < _sumDis)
                {
                    _sumDis = _curSumDis;
                    _curPlane = _plane;
                }
            }
        }
        return _curPlane;
    }

    /**
     * AI Units
    */
    public static Vector3 GetLocationForUnit(List<GameObject> refUnits, List<GameObject> avoidUnits, float minDis, float maxDis)
    {
        Vector3 loc = Vector3.one;
        List<GameObject> closePlanes = new List<GameObject>();
        
        //collect nearby planes
        foreach (GameObject plane in PlacementPlanes)
        {
            foreach (GameObject refUnit in refUnits)
            {
                float planeDis = (plane.transform.position - refUnit.transform.position).sqrMagnitude;
                if (planeDis > minDis * minDis && planeDis < maxDis * maxDis &&
                    plane.tag.Equals("PlacementPlane_Open") && !UsedUnitLocations.Contains(plane.transform.position))
                    closePlanes.Add(plane);
            }
        }
        Debug.Log("GetLOcationForUnits: closeplanes: " + closePlanes.Count);
        //get location, that is the most far away from avoidUnits
        float globalDis = 0;
        foreach (GameObject plane in closePlanes)
        {
            //get distance to nearest avoidUnit
            float bestDis = 0;
            foreach (GameObject avoidUnit in avoidUnits)
            {
                float curDis = (plane.transform.position - avoidUnit.transform.position).sqrMagnitude;
                if (curDis < bestDis)
                    bestDis = curDis;
            }
            if (bestDis > globalDis)
                loc = plane.transform.position;
        }

        return loc;
    }
}
