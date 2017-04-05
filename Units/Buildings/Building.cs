using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Pathfinding;
using System.Collections.Generic;

public class Building : MonoBehaviour
{
    public enum Clip { construction, ambient, burning };
    public AudioClip[] soundClips;
    private AudioSource soundPlayer;

    public GameObject Spinner;
    public Transform FloatMenuLocator;
    public GameObject FloatMenuPrefab;
    public GameObject tagObj;

    public Animator animator;
    public GameObject AnimatedSet;
    public GameObject StaticSet;
    public GameObject WreckSet;
    public Detonator Explosion;
    [HideInInspector]
    public GameObject FloatMenu;
    [HideInInspector]
    public FloatMenu floatMenu;
    private GameObject RootCanvas;

    public ILevelMaster levelMaster;

    public string UnitNameField;
    public string UnitFactionField = "Friendly";
    private string unitName;
    public string UnitName
    {
        get { return unitName; }
    }

    private float curHealth;
    public float CurHealth
    {
        get { return curHealth; }
        set { curHealth = value; }
    }

    private float maxHealth;
    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    private int unitLevel;
    public int UnitLevel
    {
        get { return unitLevel; }
        set { unitLevel = value; }
    }

    private bool unitActive;
    public bool UnitActive
    {
        get { return unitActive; }
        set { unitActive = value; }
    }

    private bool objDead;
    public bool ObjDead
    {
        get { return objDead; }
    }

    private float buildProgress;
    public float BuildProgress
    {
        get { return buildProgress; }
        set { buildProgress = value; }
    }


    public IBuildingState activeState;
    public IBuildingState inactiveState;
    public IBuildingState unitBuildingState;
    public IBuildingState buildState;
    public IBuildingState deathState;
    public IBuildingState sellState;
    public IBuildingState currentState;
    
    public List<GameObject> ClosePlanes;

    public Material disabledMat;
    [HideInInspector]
    public Material originalMat;

    public virtual void Awake()
    {
        RootCanvas = GameObject.FindGameObjectWithTag("InGameCanvasFolder");
        FloatMenu = Instantiate(FloatMenuPrefab, transform.position, Quaternion.identity) as GameObject;
        FloatMenu.transform.parent = RootCanvas.transform;
        floatMenu = FloatMenu.gameObject.GetComponent<FloatMenu>();
        floatMenu.SetIcon(UnitName);
        //update floatmenu graphics
        floatMenu.SetHealth(CurHealth, MaxHealth);
        soundPlayer = gameObject.GetComponent<AudioSource>();
        unitName = UnitNameField;
        
    }

    public virtual void Start()
    {
        ClosePlanes = new List<GameObject>();
        AstarPath.active.UpdateGraphs(tagObj.GetComponent<Collider>().bounds);
        SetFloatMenuPos();
        //get levelmaster
        levelMaster = GameInfo.GetLevelMaster(UnitFactionField);
        //get tag
        tagObj.tag = levelMaster.PlayerTag;
        ClosePlanes = new List<GameObject>();
        AstarPath.active.UpdateGraphs(tagObj.GetComponent<Collider>().bounds);
    }

    public virtual void Update()
    {
        if (CameraControls.cameraMoving && currentState != deathState)
            SetFloatMenuPos();

    }

    public void PlaySound(int _clipID)
    {
        soundPlayer.Stop();
        if (soundClips.Length >= _clipID + 1 && soundClips[_clipID] != null)
        {
            soundPlayer.clip = soundClips[_clipID];
            soundPlayer.Play();
        }
    }

    public void SetFloatMenuPos()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(FloatMenuLocator.position);
        //if point in screenspace, make the floating window visible, else hide it
        if (screenPos.x < 0 || screenPos.y > Screen.height)
            FloatMenu.SetActive(false);
        else
        {
            if (FloatMenu.activeSelf == false)
                FloatMenu.SetActive(true);
            FloatMenu.transform.position = screenPos;
        }
    }

    public void FindClosePlanes(int _count)
    {
        float distance = (_count == 1 ? 25f : 50f);
        foreach (GameObject go in UnitLocationsManager.PlacementPlanes)
        {
            float curDis = (go.transform.position - gameObject.transform.position).sqrMagnitude;
            if (curDis < distance)
            {
                go.tag = "PlacementPlane_Held";
                ClosePlanes.Add(go);
            }
        }
    }

    //deprecated
    public void KeepPlanesHeld()
    {
        foreach (GameObject go in ClosePlanes)
            go.tag = "PlacementPlane_Held";
    }

    public void FreeArea()
    {
        foreach (GameObject go in ClosePlanes)
            go.tag = "PlacementPlane_Open";
        tagObj.layer = 0;
        AstarPath.active.UpdateGraphs(tagObj.GetComponent<Collider>().bounds);
    }

    public void TakeDamage(float value)
    {
        if (currentState != deathState)
        {
            //avoid negative values and floatmenu image overflow
            if (value > curHealth)
                curHealth = 0;
            else
                curHealth -= value;
            //apply changes to floatmenu
            floatMenu.SetHealth(curHealth, maxHealth);
            //if health is 0, switch to deathState
            if (curHealth <= 0)
            {
                currentState.ToDeathState();
            }
        }
    }

    public virtual void FinishBuilding()
    {
        //correct health
        if (CurHealth > (MaxHealth - 50))
            CurHealth = MaxHealth;

        StaticSet.SetActive(true);
        //Destroy(AnimatedSet);
        AnimatedSet.SetActive(false);

        //InGameGUI.SetBuildingBuild(false);
        levelMaster.SetCurrentBuildingConstructionState(false);

        floatMenu.SetColor("green");
        floatMenu.SetTextArea(true);

        floatMenu.SetHealth(CurHealth, MaxHealth);
        buildProgress = 100;
        Spinner.SetActive(false);

        PlaySound(1);
    }

    public virtual void DeathActions()
    {
        tagObj.tag = "undefined";
        objDead = true;
        levelMaster.RemoveBuilding(tagObj, UnitName, unitLevel);
        PlaySound((int)Building.Clip.burning);
        Explosion.Explode();
    }

    public virtual void DeathActionsDelayed()
    {
        if (AnimatedSet != null)
            Destroy(AnimatedSet);
        WreckSet.SetActive(true);
        StaticSet.SetActive(false);
        Spinner.SetActive(false);
        Destroy(FloatMenu);
    }

    public virtual void SellActions()
    {
        tagObj.tag = "undefined";
        objDead = true;
        FreeArea();
        levelMaster.RemoveBuilding(tagObj, UnitName, unitLevel);
        Spinner.SetActive(false);
        Destroy(FloatMenu);
        Destroy(gameObject);
    }

    public void SellBuilding()
    {
        currentState.ToSellState();
    }

    public void RepairBuilding()
    {

    }

    public virtual void UpgradeBuilding()
    {
        switch (unitLevel)
        {
            case 0:
                unitLevel = 1;
                floatMenu.SetUnitLevel(1);
                break;
            case 1:
                unitLevel = 2;
                floatMenu.SetUnitLevel(2);
                break;
            case 2:
                unitLevel = 3;
                floatMenu.SetUnitLevel(3);
                break;
            default:
                break;
        }
    }
    
}

