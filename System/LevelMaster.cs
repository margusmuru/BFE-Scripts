using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelMaster : MonoBehaviour, ILevelMaster {

   
    public Material GreenMat;
    public Material GreenMat1;
    public Material GreenMat2;
    public Material RedMat;
    public Material RedMat1;
    public Material RedMat2;


    private List<Vector3> tmpAvoidList; // for unit location calculations


    public bool AutoShowOptionsMenu = true;
    public bool PauseToEngage = true;

    public GameObject DestMarkerLargePrefab;
    public GameObject DestMarkerCenterPrefab;
    public GameObject DestMarkerSmallPrefab;
    public ArrayList DestMarkersAvailable;
    public ArrayList DestMarkersUsed;

    public Animator AnimStats;
    public Text BuildingsCountText;
    public Text UnitsCountText;
    public Text MoneyCountText;
    public Text XpCountText;
    public Slider PowerSlider;
    public Image PowerSliderImage;

    //PUBLIC VALUES
    private int unitCount;
	public int UnitCount
	{
		get { return unitCount;}
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
    public int  BarracksCount
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

    public List<GameObject> PlaneList;
    public List<GameObject> ClosePlaneList;

    private bool humanPlayer;
    public bool HumanPlayer
    {
        get { return humanPlayer; }
    }


    void Awake()
    {
        humanPlayer = true;
        DestMarkersAvailable = new ArrayList();
        DestMarkersUsed = new ArrayList();

        tmpAvoidList = new List<Vector3>();

        playerTag = "Team1";

        PlaneList = new List<GameObject>();
        ClosePlaneList = new List<GameObject>();

        // assign start values for stats
        moneyCount = UnitValues.MoneyCount;
        MoneyCountText.text = moneyCount.ToString();
        totalPowerCount = 30;
        PowerSlider.maxValue = 30;
        PowerSlider.value = usedPowerCount;
        XpCountText.text = xpCount.ToString();
        UnitsCountText.text = unitCount.ToString();
        BuildingsCountText.text = BuildingSum().ToString();

        
    }

    void Start()
    {
        friendlyUnits = UnitLocationsManager.Team1Units;
        enemyUnits = UnitLocationsManager.Team2Units;
    }


    //=========================== Location Finding ===============================================================================================

    // Finds a location for Unit Buildings and sets plane colors
    // if _count == 1 then find a single close plane
    // if _count == 2 find a plane which has 6 free planes surrounding it
    public GameObject FindPlacementPlane(Vector3 _location, int _count, GameObject _prevPlane)
    {
        float threshold = 700;
        GameObject closestPlane=null;
        float curDis;
        float distance = Mathf.Infinity;
        foreach (GameObject go in UnitLocationsManager.PlacementPlanes)
        {
            curDis = (go.transform.position - _location).sqrMagnitude;
            //if closer than previous
            if (curDis < distance)
            {
                distance = curDis;
                closestPlane = go;
            }
        }
        //closest plane is now calculated
        if (_prevPlane != closestPlane)
        {
            //if mouse has moved to a different plane, we must clear previous color set and make a new one
            ClearPlaneColors();
            //find closest friendly unit
            GameObject closestFriend = null;
            float curDis1;
            float distance1 = 100000;
            foreach(GameObject go in FriendlyUnits)
            {
                curDis1 = (go.transform.position - closestPlane.transform.position).sqrMagnitude;
                if (curDis1 < distance1)
                {
                    distance1 = curDis1;
                    closestFriend = go;
                }
            }
            if (closestFriend == null) { return null; }
            //make a new planeList
            foreach (GameObject go in UnitLocationsManager.PlacementPlanes)
            {
                curDis = (go.transform.position - closestPlane.transform.position).sqrMagnitude;
                if (curDis <= 25*_count && closestFriend != null && (go.transform.position - closestFriend.transform.position).sqrMagnitude < threshold)
                {
                    Renderer rend = go.gameObject.GetComponent<Renderer>();
                    if (go.tag == "PlacementPlane_Open" && !UnitLocationsManager.InUsedLocationsList(go.transform.position) &&
                        !UnitLocationsManager.InCoverLocationsList(go.transform.position) && PlaneSectorsFree(go, true) == 7)
                    { 
                        rend.material = GreenMat;
                        //add the closest plane to closePlanelist
                        ClosePlaneList.Add(go);
                    }
                    else
                    { 
                        rend.material = RedMat; 
                    }
                    rend.enabled = true;
                    PlaneList.Add(go);
                }
                if (curDis > 25 * _count && curDis <= 50 * _count && closestFriend != null && (go.transform.position - closestFriend.transform.position).sqrMagnitude < threshold)
                {
                    Renderer rend = go.gameObject.GetComponent<Renderer>();
                    if (go.tag == "PlacementPlane_Open" && !UnitLocationsManager.InUsedLocationsList(go.transform.position) &&
                        !UnitLocationsManager.InCoverLocationsList(go.transform.position) && PlaneSectorsFree(go, true) == 7)
                    { 
                        rend.material = GreenMat1;
                        if (_count == 2)
                        { 
                            ClosePlaneList.Add(go); 
                        }
                    }
                    else
                    { 
                        rend.material = RedMat1; 
                    }
                    rend.enabled = true;
                    PlaneList.Add(go);
                }
                if (curDis > 50 * _count && curDis < 300 * _count && closestFriend != null && (go.transform.position - closestFriend.transform.position).sqrMagnitude < threshold)
                {
                    Renderer rend = go.gameObject.GetComponent<Renderer>();
                    if (go.tag == "PlacementPlane_Open" && !UnitLocationsManager.InUsedLocationsList(go.transform.position) &&
                        !UnitLocationsManager.InCoverLocationsList(go.transform.position) && PlaneSectorsFree(go, true) == 7)
                    { rend.material = GreenMat2; }
                    else
                    { rend.material = RedMat2; }

                    rend.enabled = true;
                    PlaneList.Add(go);
                }
            }
        }
        return closestPlane;
    }


  

    
    // Command units to move to calculated locations
    // Users: InGameGUI
    public Vector3 FindDestPlane(Vector3 _location, string _spread, Vector3 _prevPlane)
    {
        GameObject closestPlane = null; // initial closest plane
        Vector3 closestLoc = Vector3.one; // closest sector to mouse position
        bool _recalculateLocations = false; // true if a new locations calculation is required
        // sector locations from plane center
        


        // detect mouse movement; if it is pointing to a different location than previous frame
        float curDis;
        float distance = Mathf.Infinity;
        foreach (GameObject go in UnitLocationsManager.PlacementPlanes)
        {
            curDis = (go.transform.position - _location).sqrMagnitude;
            if (curDis < distance) { distance = curDis; closestPlane = go; }
        }

        // calculated closest plane. 
        if (_spread == "wide")
        {
            if (closestPlane.transform.position != _prevPlane)
                _recalculateLocations = true;
            closestLoc = closestPlane.transform.position;
        }
        else // "tight"
        {
            //test closure to a sector
            curDis = 0;
            distance = Mathf.Infinity;
            for (int i = 0; i < 7; i++)
            {
                curDis = ((closestPlane.transform.position + UnitLocationsManager.Sectors[i]) - _location).sqrMagnitude;
                if (curDis < distance) { distance = curDis; closestLoc = closestPlane.transform.position + UnitLocationsManager.Sectors[i]; }
            }
            if (closestLoc != _prevPlane)
                _recalculateLocations = true;
        }
        
        if (_recalculateLocations)
        {
            //print("recalculating");

            HideDestPlanes(); // move all used tiles to available tiles list and disable them;
            GameObject _recommendedSectorPlane = null;

            foreach(GameObject cu in InGameGUI.commandUnits)
            {
                bool _searchFailed = true; // if true, the closest plane is no-go, and gets added to the avoidlist 
                Vector3 _tmpLoc = Vector3.one;
                

                while (_searchFailed)
                {
                    float _curDis = 0;
                    float _distance = Mathf.Infinity;
                    GameObject _closestPlane = null;

                    // if no recommended plane or the unit is not a soldier, do a search, otherwise try the already found plane
                    if (_recommendedSectorPlane == null || !cu.name.Contains("Soldier"))
                    {
                        foreach (GameObject _plane in UnitLocationsManager.PlacementPlanes)
                        {
                            bool _doCalculations = false;
                            // calculate distance if not in avoidlist or global used-list and find the new closest plane
                            if (_spread == "wide" && !UnitLocationsManager.InUsedLocationsList(_plane.transform.position) &&
                                !IntmpAvoidList(_plane.transform.position))
                            {
                                // continue if plane is a standart plane or the unit is a soldier and can move to forest area
                                if (_plane.tag == "PlacementPlane_Open" || (cu.name.Contains("Soldier") && _plane.tag == "PlacementPlane_Forest_Open"))
                                    _doCalculations = true;
                            }
                            else if(!IntmpAvoidList(_plane.transform.position)) // "tight"
                            {
                                if (cu.name.Contains("Soldier") && (_plane.tag == "PlacementPlane_Open" || _plane.tag == "PlacementPlane_Forest_Open"))
                                    _doCalculations = true;
                                else if (!cu.name.Contains("Soldier") && _plane.tag == "PlacementPlane_Open")
                                    _doCalculations = true;
                            }

                            if (_doCalculations)
                            {
                                _curDis = (_plane.transform.position - _location).sqrMagnitude;
                                if (_curDis < _distance)
                                {
                                    _distance = _curDis;
                                    _closestPlane = _plane;
                                }
                            }
                        } 
                    }
                    else
                        _closestPlane = _recommendedSectorPlane;
                    // closest available plane has been calculated

                    // check if any sectors are in use. only search coverlist if the unit is a soldier and Spread == tight
                    int _freeSectors = 0;
                    if (cu.name.Contains("Soldier") && _spread == "tight") // soldiers can use a sector
                        _freeSectors = PlaneSectorsFree(_closestPlane, true);
                    else
                        _freeSectors = PlaneSectorsFree(_closestPlane, false);

                    if (_freeSectors == 0)
                    {
                        // all sectors are occupied and unable to use the plane. 
                        _searchFailed = true;
                        _recommendedSectorPlane = null;
                        tmpAvoidList.Add(_closestPlane.transform.position);
                    }
                    else
                    {
                        // there are free sectors available
                        if (cu.name.Contains("Soldier") && _spread == "tight") // soldiers can use a sector
                        {
                            // if there are more free sectors, recommend it for the next unit
                            if (_freeSectors - 1 > 0)
                                _recommendedSectorPlane = _closestPlane;
                            // find a free sector, return sector number
                            int _sectorID = GetFreeSectorID(_closestPlane, closestLoc);
                            // calculate location for destination
                            Vector3 _destLoc = _closestPlane.transform.position + UnitLocationsManager.Sectors[_sectorID];
                            // set up marker
                            tmpAvoidList.Add(_destLoc);
                            GameObject _tmpPlane = null;
                            foreach (GameObject _plane in DestMarkersAvailable)
                            {
                                if (_plane.tag == "SmallTile" && _sectorID != 0)
                                {
                                    _plane.transform.position = _destLoc;
                                    _plane.transform.localEulerAngles = new Vector3(0, -60 * (_sectorID - 1), 0);
                                }
                                if(_plane.tag == "CenterTile" && _sectorID == 0)
                                    _plane.transform.position = _destLoc;
                                if ((_plane.tag == "CenterTile" && _sectorID == 0) || (_plane.tag == "SmallTile" && _sectorID != 0))
                                {
                                    _plane.SetActive(true);
                                    DestMarkersUsed.Add(_plane);
                                    _tmpPlane = _plane;
                                    break;
                                }
                                    
                                
                            }
                            if (_tmpPlane != null)
                                DestMarkersAvailable.Remove(_tmpPlane);
                            else
                            {
                                if (_sectorID == 0)
                                {
                                    GameObject newPlane = Instantiate(DestMarkerCenterPrefab, closestLoc, Quaternion.identity) as GameObject;
                                    DestMarkersUsed.Add(newPlane);
                                }
                                else
                                {
                                    GameObject newPlane = Instantiate(DestMarkerSmallPrefab, closestLoc, Quaternion.identity) as GameObject;
                                    DestMarkersUsed.Add(newPlane);
                                }
                                
                            }
                            _searchFailed = false;
                        }
                        else if(_freeSectors == 7) // "WIDE" ... _spread != "tight" || vehicle
                        {
                            tmpAvoidList.Add(_closestPlane.transform.position);
                            GameObject _tmpPlane = null;
                            foreach (GameObject _plane in DestMarkersAvailable)
                            {
                                if (_plane.tag == "LargeTile")
                                {
                                    _plane.transform.position = _closestPlane.transform.position;
                                    _plane.SetActive(true);
                                    DestMarkersUsed.Add(_plane);
                                    _tmpPlane = _plane;
                                    break;
                                }
                            }
                            if (_tmpPlane != null)
                                DestMarkersAvailable.Remove(_tmpPlane);
                            else
                            {
                                GameObject newPlane = Instantiate(DestMarkerLargePrefab, _closestPlane.transform.position, Quaternion.identity) as GameObject;
                                DestMarkersUsed.Add(newPlane);
                            }
                            _searchFailed = false;
                        }
                        else
                        {
                            _searchFailed = true;
                            tmpAvoidList.Add(_closestPlane.transform.position);
                        }
                    }


                } // while "searchFailed
            } // foreach unit in commandlist

        }
        tmpAvoidList.Clear();
        return closestLoc;
    }



    //============== Helpers ====================================================

    //FindDestPlane helper
    public void HideDestPlanes()
    {
        foreach (GameObject go in DestMarkersUsed)
        {
            go.SetActive(false);
            DestMarkersAvailable.Add(go);
        }
        DestMarkersUsed.Clear();
    }

    

    // FindDestPlaneHelper
    // check if plane is in tmpAvoidList
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

    // FindDestPlane helper
    //check if any sectors of found plane are occupied
    private int PlaneSectorsFree(GameObject _plane, bool _checkCoverList)
    {
        if (_plane != null)
        {
            int _freeSectorsCount = 0;
            Vector3 _loc = _plane.transform.position;
            for (int i = 0; i < 7; i++)
            {
                Vector3 _curLoc = _loc + UnitLocationsManager.Sectors[i];
                // further testing depends on wether we need to test for coverlocations or not
                if (_checkCoverList)
                {
                    if (!UnitLocationsManager.InUsedLocationsList(_curLoc) &&
                        !IntmpAvoidList(_curLoc) && !UnitLocationsManager.InCoverLocationsList(_curLoc))
                    {
                        _freeSectorsCount++;
                    }
                }
                else // do not check coverList
                {
                    if (!UnitLocationsManager.InUsedLocationsList(_curLoc) && !IntmpAvoidList(_curLoc))
                    {
                        _freeSectorsCount++;
                    }
                }
                
            }
            return _freeSectorsCount; 
        }
        else
        {
            return 0;
        }
    }

    // FindDestPlane helper
    // return if of the free sector
    private int GetFreeSectorID(GameObject _plane, Vector3 _reqSector)
    {
        int _sectorID = 0;
        float _curDis = 0;
        float _distance = Mathf.Infinity;
        for (int i = 0; i < 7; i++)
        {
            Vector3 _curLoc = _plane.transform.position + UnitLocationsManager.Sectors[i];
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


    //clear plane colors
    public void ClearPlaneColors()
    {
        foreach (GameObject go in PlaneList)
        {
            go.gameObject.GetComponent<Renderer>().enabled = false;
        }
        PlaneList.Clear();
        ClosePlaneList.Clear();
    }

    private int BuildingSum()
    {
        return baseCount + powerstationCount + barracksCount + factoryCount;
    }

    //===================================================================================

    //=============================================================================================================================================








    //=============================== Unit, Building, Money, Resources, etc value changers =========================================================

    public void SetCurrentBuildingConstructionState(bool value)
    {
        InGameGUI.SetBuildingBuild(value);
    }

    // called by a building unit when instanciated
    public void AddBuilding(GameObject _gameObj, string _input)
    {
        AnimStats.SetTrigger("AddBuilding");
        switch (_input)
        {
            case "Military Base":
                baseCount++;
                break;
            case "Power Station":
                powerstationCount++;
                break;
            case "Barracks":
                barracksCount++;
                break;
            case "Refinery":
                factoryCount++;
                maxCollectorCount++;
                break;
            default:
                Debug.Log("Levelmaster cant find a building name specified: " + _input);
                break;
        }
        BuildingsCountText.text = BuildingSum().ToString();
        FriendlyUnits.Add(_gameObj);
    }
    // called when building is destroyed
    public void RemoveBuilding(GameObject _gameObj, string _input, int buildingLevel)
    {
        AnimStats.SetTrigger("RemoveBuilding");
        switch (_input)
        {
            case "Military Base":
                baseCount--;
                break;
            case "Power Station":
                powerstationCount--;
                break;
            case "Barracks":
                barracksCount--;
                break;
            case "Refinery":
                factoryCount--;
                maxCollectorCount -= buildingLevel + 1;
                break;
            default:
                Debug.Log("Levelmaster cant find a building name specified: " + _input);
                break;
        }
        BuildingsCountText.text = BuildingSum().ToString();
        FriendlyUnits.Remove(_gameObj);
    }

    // called when unit added (not building)
    public void AddUnit(GameObject _gameObj, string _type)
    {
        AnimStats.SetTrigger("AddUnit");
        switch (_type)
        {
            case "soldier":
                break;
            case "collector":
                collectorCount++;
                break;
            default:
                break;
        }
        // add sum
        unitCount++;
        UnitsCountText.text = unitCount.ToString();
        friendlyUnits.Add(_gameObj);
    }

    // called when unit dies
    public void RemoveUnit(GameObject _gameObj, string _type)
    {
        AnimStats.SetTrigger("RemoveUnit");
        switch (_type)
        {
            case "soldier":
                break;
            case "collector":
                collectorCount--;
                break;
            default:
                break;
        }
        unitCount--;
        UnitsCountText.text = unitCount.ToString();
        friendlyUnits.Remove(_gameObj);
    }

    // called when money is added
    public void AddMoney(int _input)
    {
        AnimStats.SetTrigger("AddMoney");
        moneyCount += _input;
        MoneyCountText.text = moneyCount.ToString();
    }

    //called when money is removed
    public void RemoveMoney(int _input)
    {
        AnimStats.SetTrigger("RemoveMoney");
        moneyCount -= _input;
        MoneyCountText.text = moneyCount.ToString();
    }

    // called when xp is added
    public void AddXP(int _input)
    {
        AnimStats.SetTrigger("AddXP");
        xpCount += _input;
        XpCountText.text = xpCount.ToString();
    }

    //called when xp is removed
    public void RemoveXP(int _input)
    {
        AnimStats.SetTrigger("RemoveXP");
        xpCount -= _input;
        XpCountText.text = xpCount.ToString();
    }
    
    //called when power is added
    public void AddPower(int _input, bool _toTotal)
    {
        AnimStats.SetTrigger("AddPower");
        // change values
        if (_toTotal)
        {
            totalPowerCount += _input;
            PowerSlider.maxValue = totalPowerCount;
            PowerSlider.value = usedPowerCount;
        }
        else
        {
            usedPowerCount += _input;
            // check for overflowing slider
            if(usedPowerCount > totalPowerCount) { PowerSlider.value = totalPowerCount; }
            else { PowerSlider.value = usedPowerCount; }
        }
        if (usedPowerCount > totalPowerCount) { PowerSliderImage.color = Color.red; }
        else { PowerSliderImage.color = Color.green; }
    }

    //called when power is removed
    public void RemovePower(int _input, bool _fromTotal)
    {
        AnimStats.SetTrigger("RemovePower");
        // change values
        if (_fromTotal)
        {
            totalPowerCount -= _input;
            PowerSlider.maxValue = totalPowerCount;
            //avoid overflowing slider
            if (usedPowerCount > totalPowerCount) { PowerSlider.value = totalPowerCount; }
            else {PowerSlider.value = usedPowerCount; }
            
        }
        else
        {
            usedPowerCount -= _input;
            if (usedPowerCount < 0) { usedPowerCount = 0; }
            PowerSlider.value = usedPowerCount;
        }
        if (usedPowerCount > totalPowerCount) { PowerSliderImage.color = Color.red; }
        else { PowerSliderImage.color = Color.green; }
    }
    
    // called when max number of collectors is increased by upgrading a factory
    public void AddCollectorMax()
    {
        maxCollectorCount++;
        Debug.Log("CollectorMaxCount = " + maxCollectorCount);
    }

    // called when max number of collectors is decreased by destroying a factory
    public void RemoveCollectorMax(int _count)
    {
        maxCollectorCount -= _count;
        if (maxCollectorCount < 0)
        {
            maxCollectorCount = 0;
        }
    }

    //================================================================================================================================================
}
