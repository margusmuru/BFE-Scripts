using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InGameGUI : MonoBehaviour {

    public static InGameGUI Instance;
    public LevelMaster levelMaster;

    public static string enemyTag;

    public bool GamePaused = false;
    public LayerMask placementLayerMask;
    public Texture2D selHighLight;

    private GameObject lastHitObj;
    private GameObject savedLastHitObj;
    private Material savedCoverMaterial = null;
    private GameObject savedCoverObj = null;

    private Rect selRect;
    private Vector3 startClick = -Vector3.one;

    public static ArrayList commandUnits;
    
    public bool PlaceMode;
    public Material ProxyGreenMat;
    public Material ProxyRedMat;
    public Material CoverGlowMat;
    public GameObject[] UnitProxyList;
    public GameObject[] UnitBuildList;
    public GameObject[] EnemyProxyList;
    public GameObject[] EnemyBuildList;

    public static string Spread = "wide";
    private string savedSpread = "";
    private Vector3 lastLocation = Vector3.one;
    private Vector3 lastSavedLocation = Vector3.one;
    private Vector3 savedMousePos = Vector3.one;
    private bool forceUnitLocCalc = false;

    // static variable that tells if a building is currently being built
    private bool isBuildingBuilding;
    public bool IsBuildingBuilding
    {
        get { return isBuildingBuilding; }
        //set { isBuildingBuilding = value; }
    }

    //counter for proxylist and final build list. tells which obj to build
    public int CurBuild;
    //currently spawned proxy obj
    private GameObject curBuildProxy;
    //bool which is true if build conditions are met
    private bool canBuild = false;
    private GameObject lastPlane;
    private GameObject savedLastPlane = null;
    public SelectedBuilding selectedBuilding;
    public InGameGUI_buttons buttonsScript;
    //private Vector3 savedTargetLoc;
    private RaycastHit hit;

    void Awake()
    {
        Instance = this;
        selectedBuilding = GetComponent<SelectedBuilding>();
        buttonsScript = GetComponent<InGameGUI_buttons>();
    }

	void Start () 
    {
        commandUnits = new ArrayList();

        levelMaster = GameObject.FindGameObjectWithTag("LevelMaster").GetComponent<LevelMaster>();
        enemyTag = levelMaster.PlayerTag == "Team1" ? "Team2" : "Team1";
	}

	
	// Update is called once per frame
	void Update () 
    {
        //Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        // unit move command wide/tight spread
        if (commandUnits.Count > 0 && !buttonsScript.Btn_Centre_UnitMove.activeSelf)
        {
            buttonsScript.ChangeMovementSpread(Spread);
            buttonsScript.Btn_Centre_UnitMove.SetActive(true);
        }
        else if (commandUnits.Count == 0 && buttonsScript.Btn_Centre_UnitMove.activeSelf)
        {
            buttonsScript.ChangeMovementSpread(Spread);
            buttonsScript.Btn_Centre_UnitMove.SetActive(false);
        }
        if (commandUnits.Count > 0 && Input.GetKeyDown("f"))
        {
            buttonsScript.ChangeMovementSpread((Spread == "wide") ? "tight" : "wide");
            forceUnitLocCalc = true;
        }

        if ((Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2) || PlaceMode || commandUnits.Count != 0) 
            && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000, placementLayerMask))
        {
            //save reference to last hit obj
            lastHitObj = hit.collider.gameObject;
            // close any ringmenu submenus if clicked in the world
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && (buttonsScript.buildSubOpen))
            {
                buttonsScript.BuildingsSubMenuSwitch(true);
            }
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && (buttonsScript.optionsSubOpen))
            {
                buttonsScript.OptionsSubMenuSwitch(true);
            }

            #region PLACEMODE
            if (PlaceMode)
            {
                //choose how large area the scipt must search
                if (CurBuild == 0 || CurBuild == 2) 
                {
                    lastPlane = levelMaster.FindPlacementPlane(hit.point, 2, lastPlane);
                }
                else
                {
                    lastPlane = levelMaster.FindPlacementPlane(hit.point, 1, lastPlane); 
                }
                //there is no build proxy, so we create one
                if(lastPlane != null && curBuildProxy == null)
                {
                    if (GameInfo.PlayerUnit == "Friendly")
                    {
                        curBuildProxy = Instantiate(UnitProxyList[CurBuild], lastPlane.transform.position, Quaternion.identity) as GameObject;
                    }
                    if (GameInfo.PlayerUnit == "Enemy")
                    {
                        curBuildProxy = Instantiate(EnemyProxyList[CurBuild], lastPlane.transform.position, Quaternion.identity) as GameObject;
                    }
                }
                //there is already a proxy, so we just move it to a new location
                else if(lastPlane != null && curBuildProxy)
                {
                    curBuildProxy.transform.position = lastPlane.transform.position;
                }
                // BUILD CONDITIONS
                // 7 tile buildings
                //        base         barracks
                if(CurBuild == 0 || CurBuild == 2 && curBuildProxy != null)
                {
                    if (levelMaster.ClosePlaneList.Count == 7)
                    {
                        curBuildProxy.GetComponent<Renderer>().material = ProxyGreenMat;
                        canBuild = true;
                    }
                    else
                    {
                        curBuildProxy.GetComponent<Renderer>().material = ProxyRedMat;
                        canBuild = false;
                    }
                }
                else if ( curBuildProxy != null )// powerstation , factory
                {
                    if (levelMaster.ClosePlaneList.Count == 1)
                    {
                        curBuildProxy.GetComponent<Renderer>().material = ProxyGreenMat;
                        canBuild = true;
                    }
                    else
                    {
                        curBuildProxy.GetComponent<Renderer>().material = ProxyRedMat;
                        canBuild = false;
                    }
                }
                //place the selected building if conditions are met
                if (lastPlane != null && Input.GetMouseButtonDown(0) && canBuild)
                {
                    if (GameInfo.PlayerUnit == "Friendly")
                    {
                        Instantiate(UnitBuildList[CurBuild], lastPlane.transform.position, Quaternion.identity);
                    }
                    if (GameInfo.PlayerUnit == "Enemy")
                    {
                        Instantiate(EnemyBuildList[CurBuild], lastPlane.transform.position, Quaternion.identity);
                    }
                    isBuildingBuilding = true;
                }//if lastplane != null
                if (Input.GetMouseButtonDown(1) || (lastPlane != null && Input.GetMouseButtonDown(0) && canBuild))
                {
                    PlaceMode = false;
                    Destroy(curBuildProxy);
                    levelMaster.ClearPlaneColors();
                }
            }
            #endregion

            #region DESTINATION SET
            if (!PlaceMode && commandUnits.Count > 0)
            {
                if (forceUnitLocCalc)
                {
                    forceUnitLocCalc = false;
                    lastLocation = Vector3.one;
                }
                if (Input.mousePosition == savedMousePos)
                {
                    // force location recalculation if Spread is changed
                    lastLocation = levelMaster.FindDestPlane(hit.point, Spread, lastLocation);
                    forceUnitLocCalc = false;
                }
                savedMousePos = Input.mousePosition;
            }

            #endregion

            #region RIGHT MOUSE BUTTON COMMANDS
            if (Input.GetMouseButtonDown(1) && !PlaceMode)
            {
                if (lastHitObj.tag != enemyTag)
                {
                    // give commands to units
                    for (int i = 0; i < commandUnits.Count; i++)
                    {
                        GameObject _go = (GameObject)commandUnits[i];
                        GameObject _caster = (GameObject)levelMaster.DestMarkersUsed[i];
                        Vector3 _loc = _caster.transform.position;
                        UnitLocationsManager.ClearLocFromUsedList(_go.GetComponent<ICommandUnit>().getDestinationObject().transform.position);
                        UnitLocationsManager.ClearLocFromUsedCoverList(_go.GetComponent<ICommandUnit>().getDestinationObject().transform.position);
                        _go.GetComponent<ICommandUnit>().MoveTo(_loc);
                        UnitLocationsManager.UsedUnitLocations.Add(_loc);
                  
                    }
                    forceUnitLocCalc = true;
                }
                if (lastHitObj.tag.Equals("Resource"))
                {
                    foreach (GameObject go in commandUnits)
                    {
                        go.SendMessage("SetResources", lastHitObj, SendMessageOptions.DontRequireReceiver);
                    }
                }
                if (lastHitObj.tag.Equals(levelMaster.PlayerTag))
                {
                    foreach (GameObject go in commandUnits)
                    {
                        go.SendMessage("SetFactory", lastHitObj, SendMessageOptions.DontRequireReceiver);
                    }
                }



                else // (lastHitObj.tag == enemyTag)
                {
                    //set target to attack
                    foreach (GameObject go in commandUnits)
                    {
                        go.SendMessage("ReqPriorityTarget", lastHitObj, SendMessageOptions.DontRequireReceiver);
                    }
                } 
            }
            #endregion

            #region UNIT SELECTION
            if (Input.GetMouseButtonDown(0) && !PlaceMode && lastHitObj.layer == 11) //layer 11 == "Units"
            {
                /* clicked on a friendly unit */
                if (lastHitObj.tag == GameInfo.PlayerTag)
                {
                    //check if already in the commandList
                    if ( !CheckCommandList(lastHitObj) )
                    {
                        /* if not adding units, clear old commandLIst */
                        if ( !Input.GetKey("left shift") )
                        {
                            ClearUnitSelection();
                        }
                        /* then add the new unit */
                        commandUnits.Add(lastHitObj.transform.parent.gameObject);
                        lastHitObj.transform.parent.gameObject.GetComponent<ICommandUnit>().SetSelection(true);
                    }
                    lastLocation = Vector3.one;
                }
                // show unit options menu
                if (levelMaster.AutoShowOptionsMenu && commandUnits.Count != 0 && !buttonsScript.optionsSubOpen)
                {
                    buttonsScript.OptionsSubMenuSwitch(false);
                }
            }
            else if (PlaceMode || (Input.GetMouseButtonDown(0) && lastHitObj.tag != "Cover" && !EventSystem.current.IsPointerOverGameObject()))/* clicked on something else than own unit */
            {
                ClearUnitSelection();
                levelMaster.HideDestPlanes();
            }
            #endregion

            #region BUILDING SELECTION
            if (Input.GetMouseButtonDown(0) && !PlaceMode && !EventSystem.current.IsPointerOverGameObject() &&
                lastHitObj.tag == GameInfo.PlayerTag && lastHitObj.layer == 12) // layer 12 == "Buildings"
            {
                // if clicked a friendly building
                
                selectedBuilding.SelBuild = lastHitObj.transform.parent.gameObject;
            }
            else if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                selectedBuilding.ClearData();
            }
            #endregion
        }

        #region SELECTION RECTANGLE
        /* selection rectangle */
        /* record the position when mousebutton is pressed down */
        if (Input.GetMouseButtonDown(0))
        {
            startClick=Input.mousePosition;
        }
        /*when mousebutton is released and mouseposition has changed; not just clicked */
		else if(Input.GetMouseButtonUp(0) && Input.mousePosition!=startClick)
		{
			//invert negative size values
            if(selRect.width<0)
			{
				selRect.x+=selRect.width;
				selRect.width=-selRect.width;
			}
			if(selRect.height<0)
			{
				selRect.y+=selRect.height;
				selRect.height=-selRect.height;
			}
            /* makes sure drawn rect is not too small; so could be a small draged click */
			if(selRect.height>10 && selRect.width>10)
			{
				//disable previous selection
                ClearUnitSelection();

                /*get all friendly units */
				foreach (GameObject go in levelMaster.FriendlyUnits)
	            {
		            Vector3 camPos = GetComponent<Camera>().WorldToScreenPoint(go.transform.position);
					camPos.y=InvertMouseY(camPos.y);

					//filter out all commandable units, add them to the list and make selection active
					if((go.name=="UnitBody" || go.name=="SoldierBody") && go.layer==11 && selRect.Contains(camPos))
					{
						if(go != null)
                        {
                            commandUnits.Add(go.transform.parent.gameObject);
                            go.transform.parent.gameObject.GetComponent<ICommandUnit>().SetSelection(true);
                        }
					}
	            }
                // show unit options menu
                if (levelMaster.AutoShowOptionsMenu && commandUnits.Count != 0 && !buttonsScript.optionsSubOpen)
                {
                    buttonsScript.OptionsSubMenuSwitch(false);
                }  
              
			} //if selRect size
            startClick =-Vector3.one;
            lastLocation = Vector3.one;
        }
        #endregion


        if (Input.GetMouseButton(0))
		{
			selRect= new Rect(startClick.x, InvertMouseY(startClick.y), Input.mousePosition.x-startClick.x, InvertMouseY(Input.mousePosition.y)-InvertMouseY(startClick.y));
		}
        savedLastHitObj = lastHitObj;
	}

    private bool CheckCommandList(GameObject _lastHitObj)
    {
        foreach (GameObject go in commandUnits)
        {
            if (go == _lastHitObj)
            {
                return true;
            }
        }
        return false;
    }

    /*function to invert mouseposition for selection rectangle */
    private float InvertMouseY(float y)
    {
        return Screen.height - y;
    }

    /*Draw GUI 2D texture over selection rectangle */
    private void OnGUI()
    {
        if (startClick != -Vector3.one)
        {
            GUI.color = new Color(1, 1, 1, 0.5f);
            GUI.DrawTexture(selRect, selHighLight);
            GUI.color = new Color(1, 1, 1, 1);
        }
    }

    private void ClearUnitSelection()
    {
        //disable previous selection
        foreach (var go in commandUnits)
        {
            //make sure the unit is still available and not dead
            if (go != null)
            {
                GameObject _tmp = go as GameObject;
                _tmp.GetComponent<ICommandUnit>().SetSelection(false);
            }
        }
        /* clear old commandlist */
        commandUnits.Clear(); 
    }

    //used by building builder
    public static void SetBuildingBuild(bool _input)
    {
        InGameGUI.Instance.isBuildingBuilding = _input;
        InGameGUI.Instance.buttonsScript.SetBuildingMenuButtons();
    }

    
}
