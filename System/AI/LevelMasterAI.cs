using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelMasterAI : MonoBehaviour, ILevelMaster
{

    public bool enableLog = false;
    public bool displayEnemyStats = true;
    //PUBLIC VALUES
    private int unitCount;
    public int UnitCount
    {
        get { return unitCount; }
        //set { unitCount = value;}
    }
    private int collectorCount;
    public int CollectorCount
    {
        get { return collectorCount; }
        //set { collectorCount = value; }
    }
    private int maxCollectorCount;
    public int MaxCollectorCount
    {
        get { return maxCollectorCount; }
        //set { maxCollectorCount = value; }
    }
    private int expectedCollectorCount;
    public int ExpectedCollectorCount
    {
        get { return expectedCollectorCount; }
        set { expectedCollectorCount = value; }
    }
    private int baseCount;
    public int BaseCount
    {
        get { return baseCount; }
        //set { baseCount = value; }
    }
    private int powerstationCount;
    public int PowerStationCount
    {
        get { return powerstationCount; }
        //set { powerstationCount = value; }
    }
    private int factoryCount;
    public int FactoryCount
    {
        get { return factoryCount; }
        //set { powerstationCount = value; }
    }
    private int barracksCount;
    public int BarracksCount
    {
        //get { return barracksCount; }
        set { barracksCount = value; }
    }
    private int moneyCount;
    public int MoneyCount
    {
        get { return moneyCount; }
        //set { moneyCount = value; }
    }
    private int xpCount;
    public int XpCount
    {
        get { return xpCount; }
        //set { xpCount = value; }
    }
    private int usedPowerCount;
    public int UsedPowerCount
    {
        get { return usedPowerCount; }
        //set { powerCount = value; }
    }
    private int totalPowerCount;
    public int TotalPowerCount
    {
        get { return totalPowerCount; }
        //set { totalPowerCount = value; }
    }

    private string playerTag;
    public string PlayerTag
    {
        get { return playerTag; }
        set { playerTag = value; }
    }

    private ArrayList tmpAvoidList; // for unit location calculations
    [HideInInspector]
    public Vector3[] Sectors = {
        new Vector3(0, 0, 0),
        new Vector3(0f, 0f, -2.3f),
        new Vector3(1.95f, 0f, -1.14f),
        new Vector3(1.95f, 0f, 1.15f),
        new Vector3(0f, 0f, 2.3f),
        new Vector3(-1.95f, 0f, 1.15f),
        new Vector3(-1.95f, 0f, -1.15f)
    };

    // Unit Arrays
    public ArrayList BaseArray;
    public ArrayList PowerStationArray;
    public ArrayList BarracksArray;
    public ArrayList FactoryArray;

    public ArrayList CollectorArray;

    public ArrayList SoldierArray;
    // true if any AI building is under construction
    public bool IsBuilding = false; 
    // info about last attack against the AI units
    public Vector3 lastAttackLocation = Vector3.one;
    public float lastAttackTime = 0;
    public float lastResponceTime = 30f;
    public Vector3 lastResponceLocation = Vector3.one;

    public GameObject AI_Defece_Raycaster;

    private ArrayList friendlyUnits;
    public ArrayList FriendlyUnits
    {
        get { return friendlyUnits; }
    }

    private ArrayList enemyUnits;
    public ArrayList EnemyUnits
    {
        get { return enemyUnits; }
    }
    private bool humanPlayer;
    public bool HumanPlayer
    {
        get { return humanPlayer; }
    }
    void Awake()
    {
        humanPlayer = false;
        tmpAvoidList = new ArrayList();

        playerTag = "Team2";

        BaseArray = new ArrayList();
        PowerStationArray = new ArrayList();
        BarracksArray = new ArrayList();
        FactoryArray = new ArrayList();

        CollectorArray = new ArrayList();

        SoldierArray = new ArrayList();

        // assign start values for stats
        moneyCount = UnitValues.MoneyCount;
        totalPowerCount = 30;

        
    }

    void Start()
    {
        friendlyUnits = UnitLocationsManager.Team2Units;
        enemyUnits = UnitLocationsManager.Team1Units;
    }

    //===========================================================================================================================================






    //============================ Unit Finding Methods =========================================================================================

    

    /** Get number of friendly AI units in given radius of given location
     * _avoid - string that contains unit names to avoid
     */
    public int GetNearbyFriendliesCount(Vector3 _location, float _maxDistance)
    {
        int _unitCount = 0;
        foreach (GameObject _unit in FriendlyUnits)
        {
            float _curDis = (_unit.transform.position - _location).sqrMagnitude;
            if (_curDis < _maxDistance)
            {
                _unitCount++;
            }
        }
        return _unitCount;
    }

    /** Get number of friendly AI units in given radius of given location
     * _avoid - string that contains unit names to avoid
     */
    public int GetNearbyEnemiesCount(Vector3 _location, float _maxDistance)
    {
        int _unitCount = 0;
        foreach (GameObject _unit in EnemyUnits)
        {
            float _curDis = (_unit.transform.position - _location).sqrMagnitude;
            if (_curDis < _maxDistance)
            {
                _unitCount++;
            }
        }
        return _unitCount;
    }

    /**
    *   returns location of the building which has the least number of units nearby
    */
    public Vector3 GetBuildingWithLeastGuards()
    {
        GameObject curChoice = null;
        int curCount = 1000; // very large number
        List<GameObject> builidngsList = new List<GameObject>();
        //construct list of all buildings
        foreach(GameObject _obj in BaseArray) { builidngsList.Add(_obj); }
        foreach (GameObject _obj in PowerStationArray) { builidngsList.Add(_obj); }
        foreach (GameObject _obj in FactoryArray) { builidngsList.Add(_obj); }
        foreach (GameObject _obj in BarracksArray) { builidngsList.Add(_obj); }

        //base
        foreach (GameObject _building in builidngsList)
        {
            int nearbyCount = GetNearbyFriendliesCount(_building.transform.position, 300);
            if (nearbyCount < curCount)
            {
                curCount = nearbyCount;
                curChoice = _building;
            }
        }


        return curChoice.transform.position;
    }
    /*
    /**
    * find number of idle units that are near a location
   
    public List<GameObject> GetIdleUnits(Vector3 _location, float _maxDistance, int _expectedCount)
    {
        List<GameObject> availableUnits = new List<GameObject>();
        List<GameObject> suitableUnits = new List<GameObject>();
        float curSearchDistance = _maxDistance / 10;
        


        //copy all idle and enough close units to "availableUnits" list
        foreach (GameObject _unit in SoldierArray)
        {
            C_Soldier _soldierScript = _unit.GetComponent<C_Soldier>();
            if ((_unit.transform.position - _location).sqrMagnitude < _maxDistance && _soldierScript.movementState == "idle" &&
                    _soldierScript.wS.target == null)
            {
                availableUnits.Add(_unit);
            }
        }

        // search for closest unit and add i to "suitableUnits" list until max search distance has reached or enough units have been found
        while (curSearchDistance < _maxDistance || suitableUnits.Count < _expectedCount)
        {
            float closeDis = Mathf.Infinity;
            GameObject closeUnit = null;
            foreach (GameObject _unit in availableUnits)
            {
                float curDis = (_unit.transform.position - _location).sqrMagnitude;
                if (curDis < closeDis)
                {
                    closeDis = curDis;
                    closeUnit = _unit;
                }
            }
            // add unit to suitable units list, which will be returned
            suitableUnits.Add(closeUnit);
            // remove unit from available units list
            availableUnits.Remove(closeUnit);
            // encrease curretn search distance
            curSearchDistance += _maxDistance / 10;
        }
        
        return suitableUnits;
    }

    /** 
    * get list of enemy units in range
    */
    // method here!


    //===========================================================================================================================================





    //============================ Location finding methods =====================================================================================

    /** GetBuildingLocation finds a suitable location for a building
     * Uses my special algoritm to find closest suitable plane to given location
     * location - defines the starting point of the search. 
     * areaSize - defines how large area is needed, 1 plane or 7 planes. 
     * minDistance - defines the minimum distance from the starting location
     * maxDistance - defines tha maximum distance from starting point where the resulting plane may be
     * randomCount - defines how many suitable location we must find before choosing one
     * requiredFreeSectors - number of free sectors required. mainly 1 or 7
     * locNear - location, near where a location is searched
     * locFar - location, that is far away and used for reference
     * areaSize = 1;
    *
    /** For Units */
        /*
    public Vector3 GetUnitDestination(Vector3 location, float minDistance, float maxDistance, int randomCount, int requiredFreeSectors, int allowedNearbies)
    {
        // closest plane to search start location. becomes the result later
        Vector3 _closestLoc = Vector3.one; 
        // contains all planes that are between required distances and in required state
        List<GameObject> _planesInRange = GetPlanesInRange(location, minDistance, maxDistance, requiredFreeSectors, allowedNearbies);

        List<GameObject> _searchList = new List<GameObject>(); // if areaSize==7 we use this to search for planes and remove searhed ones from this list, since we cant
                                                               //remove them from _planesInRange due to need of neighbours search
        List<Vector3> _suitableLocs = new List<Vector3>(); // planes that meet requirements and can be selected for the result


        if (_planesInRange.Count > 0)
        {
            // find randomCount number of close planes to location in _planesInRange
            // when a plane is added, add it to _suitablePlanes
            while (_suitableLocs.Count < randomCount && _planesInRange.Count > 0)
            {
                GameObject _resultPlane = GetClosestObjInList(_planesInRange, location);
                // remove found plane from searchlist so it will not be chosen again
                _planesInRange.Remove(_resultPlane);
                // add plane to suitable planes list
                _suitableLocs.Add(_resultPlane.transform. position + Sectors[GetFreeSectorID(_resultPlane)]);

            }

        }

        // choose randomly a suitable plane from the list
        if (randomCount > 0)
        {
            Random.seed = (int)System.DateTime.Now.Ticks;
            _closestLoc = _suitableLocs[Random.Range(0, _suitableLocs.Count)];
        }

        return _closestLoc;
    }
    */
    /* For Units 
    public GameObject GetUnitDestination(Vector3 locNear, Vector3 locFar, float minDistance, float maxDistance, int randomCount, int requiredFreeSectors)
    {
        GameObject _closestPlane = null; // closest plane to search start location. becomes the result later
        // contains all planes that are between required distances and in required state
        List<GameObject> _planesInRange = GetPlanesInRange(locNear, minDistance, maxDistance, requiredFreeSectors); 

        List<GameObject> _suitablePlanes = new List<GameObject>(); // planes that meet requirements and can be selected for the result

        
        // choose suitable planes
        while (_suitablePlanes.Count < randomCount && _planesInRange.Count > 0)
        {
            GameObject _curPlane = GetClosestObjInList(_suitablePlanes, locNear, locFar);
            _suitablePlanes.Add(_curPlane);
            _planesInRange.Remove(_curPlane);
        }

        // choose randomly a suitable plane from the list
        if (randomCount > 0)
        {
            Random.seed = (int)System.DateTime.Now.Ticks;
            _closestPlane = _suitablePlanes[Random.Range(0, _suitablePlanes.Count)];
        }

        return _closestPlane;
    }
    
   

    /** For Buildings 
    public Vector3 GetLocation(Vector3 location, int areaSize, float minDistance, float maxDistance, int randomCount)
    {
        GameObject _closestPlane = null; // closest plane to search start location. becomes the result later
        // contains all planes that are between required distances and in required state
        List<GameObject> _planesInRange = GetPlanesInRange(location, minDistance, maxDistance, 7);  
        List<GameObject> _searchList = new List<GameObject>(); // if areaSize==7 we use this to search for planes and remove searhed ones from this list, since we cant
                                                               //remove them from _planesInRange due to need of neighbours search
        List<GameObject> _suitablePlanes = new List<GameObject>(); // planes that meet requirements and can be selected for the result


        if (areaSize == 1 && _planesInRange.Count > 0)
        {
            // find randomCount number of close planes to location in _planesInRange
            // when a plane is added, add it to _suitablePlanes
            while (_suitablePlanes.Count < randomCount && _planesInRange.Count > 0)
            {
                _suitablePlanes.Add(GetClosestObjInList(_planesInRange, location));
            }

        }
        else // areasize == 7
        {
            // copy _planesInRange content to _searchList
            foreach (GameObject _plane in _planesInRange) { _searchList.Add(_plane); }

            // start searching for closest planes.
            while (_suitablePlanes.Count < randomCount && _searchList.Count > 0)
            {
                float _distance = Mathf.Infinity;
                float _curDis = 0;
                GameObject _resultPlane = null; // plane currently tested for neighbours
                foreach (GameObject _plane in _searchList)
                {
                    _curDis = (_plane.transform.position - location).sqrMagnitude;
                    if (_curDis < _distance)
                    {
                        _distance = _curDis;
                        _resultPlane = _plane;
                    }
                }
                // we will not test that plane again, so remove it from _searchList
                _searchList.Remove(_resultPlane);

                //do another full search to find neighbours for this plane
                int _neighboursCount = 0;

                foreach (GameObject _plane in _planesInRange)
                {
                    if (_plane != _resultPlane)
                    {
                        _curDis = (_plane.transform.position - _resultPlane.transform.position).sqrMagnitude;
                        if (_curDis < 50) { _neighboursCount++; }
                    }
                    if (_neighboursCount == 6) { break; }
                }
                // if enough neighbours, plane is suitable and we can add it to the _suitablePlanes list
                if (_neighboursCount == 6)
                {
                    _suitablePlanes.Add(_resultPlane);
                }
            }

        }

        // choose randomly a suitable plane from the list
        if (randomCount > 0)
        {
            Random.seed = (int)System.DateTime.Now.Ticks;
            _closestPlane = _suitablePlanes[Random.Range(0, _suitablePlanes.Count)];
        }

        return _closestPlane.transform.position;
    }
    
    /** For Buildings 
    public Vector3 GetLocation(Vector3 locNear, Vector3 locFar, float minDistance, float maxDistance, int randomCount)
    {
        GameObject _closestPlane = null; // closest plane to search start location. becomes the result later
        // contains all planes that are between required distances and in required state
        List<GameObject> _planesInRange = GetPlanesInRange(locNear, minDistance, maxDistance, 7);
        List<GameObject> _suitablePlanes = new List<GameObject>(); // planes that meet requirements and can be selected for the result

        
        // choose suitable planes
        while (_suitablePlanes.Count < randomCount && _planesInRange.Count > 0)
        {
            GameObject _curPlane = GetClosestObjInList(_planesInRange, locNear, locFar);
            _suitablePlanes.Add(_curPlane);
            _planesInRange.Remove(_curPlane);
        }

        // choose randomly a suitable plane from the list
        if (randomCount > 0)
        {
            Random.seed = (int)System.DateTime.Now.Ticks;
            _closestPlane = _suitablePlanes[Random.Range(0, _suitablePlanes.Count)];
        }

        return _closestPlane.transform.position;
    }
    
   


    
    
    



    //============================== Helper Methods For location finding ========================================================================

    /**
    * make a group that contains all suitable planes, taking account minDistance, maxDistance and required free sectors count
    
    private List<GameObject> GetPlanesInRange(Vector3 location, float minDistance, float maxDistance, int requiredFreeSectors, int allowedNearbies)
    {
        List<GameObject> _planesInRange = new List<GameObject>();
        foreach (GameObject _plane in LevelMaster.PlacementPlanes)
        {
            {
                if (_plane.tag == "PlacementPlane_Open")
                {
                    float _curDis = (_plane.transform.position - location).sqrMagnitude;
                    if (_curDis < maxDistance * maxDistance && _curDis > minDistance * minDistance &&
                        PlaneSectorsFreeCount(_plane, true, true) >= requiredFreeSectors &&
                        GetNearbyFriendliesCount(_plane.transform.position, 40) < allowedNearbies)
                    {
                        _planesInRange.Add(_plane);
                    }
                }
            }
        }
        return _planesInRange;
    }
    private List<GameObject> GetPlanesInRange(Vector3 location, float minDistance, float maxDistance, int requiredFreeSectors)
    {
        List<GameObject> _planesInRange = new List<GameObject>();
        foreach (GameObject _plane in LevelMaster.PlacementPlanes)
        {
            if (_plane.tag == "PlacementPlane_Open")
            {
                float _curDis = (_plane.transform.position - location).sqrMagnitude;
                if (_curDis < maxDistance * maxDistance && _curDis > minDistance * minDistance &&
                    PlaneSectorsFreeCount(_plane, true, true) >= requiredFreeSectors)
                {
                    _planesInRange.Add(_plane);
                }
            }
        }
        return _planesInRange;
    }
    */
    private GameObject GetClosestObjInList(List<GameObject> objList, Vector3 location)
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
        // remove found plane from searchlist so it will not be chosen again
        objList.Remove(_resultPlane);
        
        return _resultPlane;
    }
    private GameObject GetClosestObjInList(List<GameObject> objList, Vector3 locNear, Vector3 locFar)
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
    
    /**
     * return number of the free sector (if any) on given plane
     * if _reqSector is given, searches for the closest sector to given location on the plane
    */
    private int GetFreeSectorID(GameObject _plane, Vector3 _reqSector)
    {
        int _sectorID = -1;
        float _curDis = 0;
        float _distance = Mathf.Infinity;
        for (int i = 0; i < 7; i++)
        {
            Vector3 _curLoc = _plane.transform.position + Sectors[i];
            if (!UnitLocationsManager.InUsedLocationsList(_curLoc) &&
                !IntmpAvoidList(_curLoc) && !UnitLocationsManager.InCoverLocationsList(_curLoc))
            {
                _curDis = (_reqSector - _curLoc).sqrMagnitude;
                if (_curDis < _distance)
                {
                    _distance = _curDis;
                    _sectorID = i;
                }
            }
        }
        return _sectorID;
    }
    private int GetFreeSectorID(GameObject _plane)
    {
        int _sectorID = -1;
        for (int i = 0; i < 7; i++)
        {
            Vector3 _curLoc = _plane.transform.position + Sectors[i];
            if (!UnitLocationsManager.InUsedLocationsList(_curLoc) && !IntmpAvoidList(_curLoc) &&
                !UnitLocationsManager.InCoverLocationsList(_curLoc))
            {
                _sectorID = i;
                break;
            }
        }
        return _sectorID;
    }


    /**
     * check if plane is in tmpAvoidList
    */
    public bool IntmpAvoidList(Vector3 _input)
    {
        bool _inAvoidList = false;
        foreach (Vector3 _loc in tmpAvoidList)
        {
            if (_loc.x == _input.x && _loc.y == _input.y && _loc.z == _input.z)
            {
                _inAvoidList = true;
                break;
            }
        }
        return _inAvoidList;
    }
    //===============================================================

    //==========================================================================================================================================










    //=============================== Unit, Building, Money, Resources, etc value changers =========================================================

    public int BuildingSum()
    {
        return baseCount + powerstationCount + barracksCount + factoryCount;
    }

    public void SetCurrentBuildingConstructionState(bool value)
    {
        IsBuilding = value;
    }

    // called by a building unit when instanciated
    public void AddBuilding(GameObject _gameObj, string _input)
    {
        if (enableLog) { print("LevelMasterAI: AddBuilding: " + _input + " added"); }
        switch (_input)
        {
            case "Military Base":
                baseCount++;
                BaseArray.Add(_gameObj);
                break;
            case "Power Station":
                powerstationCount++;
                PowerStationArray.Add(_gameObj);
                break;
            case "Barracks":
                barracksCount++;
                BarracksArray.Add(_gameObj);
                break;
            case "Refinery":
                factoryCount++;
                maxCollectorCount++;
                FactoryArray.Add(_gameObj);
                break;
            default:
                break;
        }
        FriendlyUnits.Add(_gameObj);
    }

    // called when building is destroyed
    public void RemoveBuilding(GameObject _gameObj, string _input, int buildingLevel)
    {
        if (enableLog) { print("LevelMasterAI: RemoveBuilding: " + _input + " removed"); }
        switch (_input)
        {
            case "Military Base":
                baseCount--;
                BaseArray.Remove(_gameObj);
                break;
            case "Power Station":
                powerstationCount--;
                PowerStationArray.Remove(_gameObj);
                break;
            case "Barracks":
                barracksCount--;
                BarracksArray.Remove(_gameObj);
                break;
            case "Refinery":
                factoryCount--;
                maxCollectorCount -= buildingLevel + 1;
                FactoryArray.Remove(_gameObj);
                break;
            default:
                break;
        }
        FriendlyUnits.Remove(_gameObj);
    }

    // called when unit added (not building)
    public void AddUnit(GameObject _gameObj, string _type)
    {
        switch (_type)
        {
            case "soldier":
                SoldierArray.Add(_gameObj);
                break;
            case "collector":
                CollectorArray.Add(_gameObj);
                collectorCount++;
                break;
            default:
                break;
        }
        // add sum
        unitCount++;
        friendlyUnits.Add(_gameObj);
        if (enableLog) { print("LevelMasterAI: AddUnit: " + _type + " added. Total UnitCount: " + unitCount); }
    }

    public void RemoveUnit(GameObject _gameObj, string _type)
    {
        if (enableLog) { print("LevelMasterAI: RemoveUnit: unit removed"); }
        switch (_type)
        {
            case "soldier":
                SoldierArray.Remove(_gameObj);
                break;
            case "collector":
                CollectorArray.Remove(_gameObj);
                collectorCount--;
                break;
            default:
                break;
        }
        unitCount--;
        friendlyUnits.Remove(_gameObj);
    }

    // called when money is added
    public void AddMoney(int _input)
    {
        moneyCount += _input;
    }

    //called when money is removed
    public void RemoveMoney(int _input)
    {
        moneyCount -= _input;
    }

    // called when xp is added
    public void AddXP(int _input)
    {
        xpCount += _input;
    }

    //called when xp is removed
    public void RemoveXP(int _input)
    {
        xpCount -= _input;
    }

    //called when power is added
    public void AddPower(int _input, bool _toTotal)
    {
        // change values
        if (_toTotal)
        {
            totalPowerCount += _input;
        }
        else
        {
            usedPowerCount += _input;
        }
        if (enableLog) { print("LevelMasterAI: AddPower: " + _input + " added to total?= " + _toTotal + " usedPowerCount: " + usedPowerCount + ", TotalPowerCount: " + totalPowerCount); }
    }

    //called when power is removed
    public void RemovePower(int _input, bool _fromTotal)
    {
        // change values
        if (_fromTotal)
        {
            totalPowerCount -= _input;

        }
        else
        {
            usedPowerCount -= _input;
            if (usedPowerCount < 0) { usedPowerCount = 0; }
        }
        if (enableLog) { print("LevelMasterAI: RemovePower: " + _input + " removed from total?= " + _fromTotal + " usedPowerCount: " + usedPowerCount + ", TotalPowerCount: " + totalPowerCount); }
    }

    // called when max number of collectors is increased by upgrading a factory
    public void AddCollectorMax()
    {
        maxCollectorCount++;
        if (enableLog) { print("LevelMasterAI: AddCollectorMax: increased to " + maxCollectorCount); }
    }

    // called when max number of collectors is decreased by destroying a factory
    public void RemoveCollectorMax(int _count)
    {
        maxCollectorCount -= _count;
        if (maxCollectorCount < 0)
        {
            maxCollectorCount = 0;
        }
        if (enableLog) { print("LevelMasterAI: RemoveCollectorMax: decreased to " + maxCollectorCount); }
    }

    //================================================================================================================================================
}
