using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AI : MonoBehaviour {

    public bool EnableAI = true;
    public int MaxSoldierCount = 12;
    public float StartDelay = 3f;
    [HideInInspector]
    public BuildingManagerAI buildMngr;
    [HideInInspector]
    public LevelMasterAI lm;

    [Header("Enemy Stats Display")]
    public Text energyText;
    public Text moneyText;
    public Text xpText;
    public Text buildingsText;
    public Text unitsText;
    public Text collectorText;

    public void Awake()
    {
        buildMngr = GetComponent<BuildingManagerAI>();
        lm = GetComponent<LevelMasterAI>();

        InvokeRepeating("UpdateCycle", 1, 1);
    }

    public void Start()
    {

    }

    private void UpdateCycle()
    {
        if (lm.displayEnemyStats)
        {
            energyText.text = "Power: " + lm.UsedPowerCount + "/" + lm.TotalPowerCount;
            moneyText.text = "Money: " + lm.MoneyCount;
            xpText.text = "XP: " + lm.XpCount;
            buildingsText.text = "Buildings: " + lm.BuildingSum();
            unitsText.text = "Units: " + lm.UnitCount;
            collectorText.text = "Collectors: " + lm.CollectorCount + "/" + lm.ExpectedCollectorCount + "/" + lm.MaxCollectorCount;
        }
        else
        {
            energyText.text = "";
            moneyText.text = "";
            xpText.text = "";
            buildingsText.text = "";
            unitsText.text = "";
            collectorText.text = "";
        }


    }
    
    
    
    /*
    void Start()
    {
        StartCoroutine("Brain");
    }

    IEnumerator Brain()
    {
        // wait while level is being set up
        while(!GameInfo.LevelReady)
        {
            yield return new WaitForSeconds(1);
        }
        yield return new WaitForSeconds(1);
        //main AI sequence
        while(EnableAI && !GameInfo.GameEnded)
        {
            // construct buildings
            BuildingConstructionManager();

            // construct units
            UnitConstructionManager();

            // manage unit movement
            UnitMovementManager();


            yield return new WaitForSeconds(brainFrequency);
        }
    }

    //========================== Managers ====================================================================

    private void BuildingConstructionManager()
    {
        // build a base 
        if (!IsBuilding && BaseCount == 0 && LevelMaster.EnemyUnits.Count > 0 && MoneyCount > UnitValues.BasePrice)
        {
            IsBuilding = true;
            BuildBase();
        }

        // powerstation
        if (!IsBuilding && UsedPowerCount + 100 > TotalPowerCount && BaseArray.Count > 0 && MoneyCount > UnitValues.PowerStationPrice)
        {
            IsBuilding = true;
            BuildPowerStation();
        }

        //build factory
        if (!IsBuilding && FactoryCount == 0 && MoneyCount > UnitValues.FactoryPrice)
        {
            IsBuilding = true;
            BuildFactory();
        }

        //build barracks
        if (!IsBuilding && BaseArray.Count > 0 && BarracksArray.Count == 0 && MoneyCount > UnitValues.BarracksPrice)
        {
            IsBuilding = true;
            BuildBarracks();
        }
    }

    private void UnitConstructionManager()
    {
        //build collector
        if (FactoryArray.Count > 0 && CollectorCount + ExpectedCollectorCount < MaxCollectorCount && MoneyCount > UnitValues.CollectorPrice)
        {
            BuildCollector();
        }

        //build a soldier
        if (BaseArray.Count > 0 && BarracksArray.Count > 0 && MoneyCount > UnitValues.SoldierPrice)
        {
            BuildUnit(0);
        }
    }

    private void UnitMovementManager()
    {
        //check last attack time to behave accordingly
        if (lastResponceTime > lastAttackTime )
        {
            // no new attacks
            //keep area near barracks clear
            MoveUnitsAwayFromBarracks();

        }
        else
        {
            // AI should respond to attack
            RespondToAttack(lastAttackLocation);
        }
        
        
    }





    //=========================== Unit movement methods ====================================================

    private void MoveUnitsAwayFromBarracks()
    {
        foreach (GameObject _barrack in BarracksArray)
        {
            foreach (GameObject _soldier in SoldierArray)
            {
                // check if an idle unit is near the barracks
                if ((_soldier.transform.position - _barrack.transform.position).sqrMagnitude < 50 && _soldier.GetComponent<C_Soldier>().movementState == "idle")
                {
                    // choose location to refer to
                    Vector3 refLoc = GetBuildingWithLeastGuards(); 
                    // get unit destination location
                    Vector3 newLoc = GetUnitDestination(refLoc, 10, 40, 5, 1, 1);
                    // command unit to move
                    _soldier.GetComponent<C_Soldier>().MoveTo(newLoc, false);
                    // write new location to database
                    UsedUnitLocations.Add(newLoc);
                }
            }
        }
    }


    private void RespondToAttack(Vector3 attackLocation)
    {

        if (attackLocation != Vector3.one)
        {
            // check area for enemy count
            int attackForceCount = GetNearbyEnemiesCount(attackLocation, 60);

            // find nearest idle soldiers
            // check both movement and engage values
            List<GameObject> availableUnits = GetIdleUnits(attackLocation, 200, Mathf.RoundToInt(attackForceCount * 1.5f));

            // find a location near attack place for idle soldiers
            // use a gameobject with raycast to check fireing possibilities
            // ? concider cover objects

            



        }

        lastResponceTime = Time.time;
        lastResponceLocation = attackLocation;
    }

 



    //=========================== Builder methods ===========================================================

    private void BuildBase()
    {
        GameObject obj = (GameObject)LevelMaster.EnemyUnits[0];
        Vector3 _location = GetLocation(obj.transform.position, 7, 0, 60, 5);
        if (_location != null)
        {
            Instantiate(EnemyBuildingList[0], _location, Quaternion.identity);
        }
        else
        {
            IsBuilding = false;
            print("no location for base");
        }
    }

    private void BuildPowerStation()
    {
        GameObject _baseObj = null; 
        Vector3 _location = Vector3.one; 

        if (BaseArray.Count > 0) { _baseObj = (GameObject)BaseArray[0]; }
        else { if (enableLog) { print("AI: BuildPowerStation: NO BASE"); } }

        if (_baseObj != null) { _location = GetLocation(_baseObj.transform.position, 1, 15, 60, 5); }
        if (_location != Vector3.one)
        {
            Instantiate(EnemyBuildingList[1], _location, Quaternion.identity);
        }
        else
        {
            IsBuilding = false;
            print("no location for powerstation");
        }
    }

    private void BuildFactory()
    {
        GameObject _baseObj = null;
        GameObject _resourcePile = null; 
        Vector3 _location = Vector3.one;
        
        if (BaseArray.Count > 0) {_baseObj = (GameObject)BaseArray[0]; }
        else { if (enableLog) { print("AI: BuildFactory: NO BASE"); }}

        if (_baseObj != null) { _resourcePile = GetCloseResourcePile(_baseObj.transform.position); }

        if (_resourcePile != null && _resourcePile != null) { _location = GetLocation(_baseObj.transform.position, _resourcePile.transform.position, 1, 60, 3); }
        else { if (enableLog) { print("AI: BuildFactory: NO RESOURCES"); } }

        if (_location != Vector3.one) { Instantiate(EnemyBuildingList[2], _location, Quaternion.identity); }
        else
        {
            IsBuilding = false;
            print("no location for factory");

        }
    }

    private void BuildBarracks()
    {
        GameObject obj = (GameObject)LevelMaster.EnemyUnits[0];
        Vector3 _location = GetLocation(obj.transform.position, 7, 10, 60, 5);
        if (_location != null)
        {
            Instantiate(EnemyBuildingList[3], _location, Quaternion.identity);
        }
        else
        {
            IsBuilding = false;
            print("no location for barracks");
        }
    }

    private void BuildCollector()
    {
        GameObject _chosenFactory = null;
        float _bestRating = -1000;

        foreach (GameObject go in LevelMasterAI.FactoryArray)
        {
            Building_Factory _factory = go.GetComponent<Building_Factory>();
            if (_factory.BuildQueue.Count > 0)
            {
                continue; // break, if the factory is already building something
            }
            // get data
            int _buildCount = _factory.BuiltUnitCount;
            int _level = _factory.UnitLevel;
            GameObject _closePile = GetCloseResourcePile(go.transform.position);
            float _resourcesLeft = _closePile.GetComponent<ResourcesPile>().ResourcesLeft;
            
            // calculate rating
            float _rating = 1 - _buildCount + _level + UnitValues.ResourcePileMax / _resourcesLeft;
            if (_rating > _bestRating)
            {
                _bestRating = _rating;
                _chosenFactory = go;
            }
        }
        if (_chosenFactory != null)
        {
            _chosenFactory.GetComponent<Building_Factory>().AddToQueue(0);
            print("AI: BuildCollector: Construction of a Collector has been started.");
        }
        else
        {
            print("AI: BuildCollector: Could not find a factory");
        }
    }

    // _type -s
    // 0 - soldier
    private void BuildUnit(int _type)
    {
        GameObject _barracks = (GameObject)BarracksArray[0];
        Building_Barracks _barracksScript = _barracks.GetComponent<Building_Barracks>();
        if (_barracksScript.BuildQueue.Count < 4 && 
            _barracksScript.BuildQueue.Count + UnitCount - CollectorCount + -1 < MaxSoldierCount)
        {
            _barracksScript.AddToQueue(0);
        }
    }
    */
    //============================================================================================================
}
