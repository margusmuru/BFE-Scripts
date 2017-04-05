using UnityEngine;
using System.Collections;

public class Soldier : MonoBehaviour, ICommandUnit, IDamageable {

    public IdleState idleState;
    public RunState runState;
    public CoverState coverState;
    public AimInCoverState aimInCoverState;
    public AimStandingState aimStandingState;
    public AimSittingState aimSittingState;
    public DeathState deathState;
    public SitState sitState;
    public ISoldierState currentState;

    [HideInInspector]
    public AudioSource audioSource;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public AIPath aiPath;
    public LayerMask coverLayerMask = 0;
    public GameObject destinationObject;
    public GameObject tagObj;
    [HideInInspector]
    public bool selectionActive = false;
    private Renderer destMarkerRenderer;
    private LineRenderer lineRenderer;

    public GameObject selectionMarker;
    public GameObject coverLocator;

    [HideInInspector]
    public int RunHash;
    [HideInInspector]
    public int AimOnKneeHash;
    [HideInInspector]
    public int AimSquatingHash;
    [HideInInspector]
    public int AimSittingHash;

    public UnityEngine.UI.Image FireModeCircle;
    public Sprite FireModeCircleAgressive;
    public Sprite FireModeCirclePassive;
    public Sprite FireModeCircleHold;

    private ILevelMaster levelMaster;   
    public ILevelMaster LevelMasterRef
    {
        get { return levelMaster; }
        set { levelMaster = value; }
    }
    public string UnitFactionField = "Friendly";
    public IWeaponSystem weaponSystem;

    void Awake () {
        aiPath = GetComponent<AIPath>();
        animator = GetComponentInChildren<Animator>();

        idleState = new IdleState(this);
        runState = new RunState(this);
        coverState = new CoverState(this);
        aimInCoverState = new AimInCoverState(this);
        aimStandingState = new AimStandingState(this);
        aimSittingState = new AimSittingState(this);
        deathState = new DeathState(this);
        sitState = new SitState(this);

        audioSource = GetComponent<AudioSource>();
        lineRenderer = GetComponent<LineRenderer>();
        destMarkerRenderer = destinationObject.GetComponent<Renderer>();
        weaponSystem = GetComponentInChildren<IWeaponSystem>();

        RunHash = Animator.StringToHash("Base Layer.run");
        AimOnKneeHash = Animator.StringToHash("Base Layer.AimOnKnee");
        AimSquatingHash = Animator.StringToHash("Base Layer.AimSquatting");
        AimSittingHash = Animator.StringToHash("Base Layer.AimSitting");
        levelMaster = GameInfo.GetLevelMaster(UnitFactionField);
    }
	
    void Start()
    {
        
        //UnitLocationsManager.Team1Units.Add(tagObj);
        currentState = idleState;
        tagObj.tag = levelMaster.PlayerTag;
        //add unit
        levelMaster.AddUnit(tagObj, "soldier");
        //move character to closest plane
        Vector3 plane = UnitLocationsManager.FindClosestPlaneTo(transform.position).transform.position;
        UnitLocationsManager.UsedUnitLocations.Add(plane);
        MoveTo(plane);
    }

	void Update () {

        currentState.UpdateState();
        //if selection is active and unit is moving, calculate line renderer
        if (lineRenderer.enabled)
        {
            lineRenderer.SetPosition(0, transform.position + new Vector3(0, 0.3f, 0));
            lineRenderer.SetPosition(2, transform.position + new Vector3(0, 0.3f, 0));
            lineRenderer.SetPosition(1, destinationObject.transform.position);
        }
    }

    public void MoveTo(Vector3 location)
    {
        destinationObject.transform.position = location;
        if (currentState == idleState)
            currentState.ToRunState(0f);
        else
            currentState.ToRunState(0.4f);
        SetMarkervisibility(true);
    }

    // called by menuButtons
    public void SetFireMode(int _input)
    {
        switch (_input)
        {
            case 0:// agressive
                weaponSystem.FireMode = "agressive";
                FireModeCircle.overrideSprite = FireModeCircleAgressive;
                break;
            case 1:// passive
                weaponSystem.FireMode = "passive";
                FireModeCircle.overrideSprite = FireModeCirclePassive;
                weaponSystem.Target = null;
                weaponSystem.PriorityTarget = null;
                break;
            case 2: //hold
                weaponSystem.FireMode = "hold";
                FireModeCircle.overrideSprite = FireModeCircleHold;
                weaponSystem.Target = null;
                weaponSystem.PriorityTarget = null;
                break;
            default:
                break;
        }
    }

    public void SetSelection(bool value)
    {
        selectionMarker.SetActive(value);
        selectionActive = value;
        SetMarkervisibility(value);
    }

    public GameObject getDestinationObject()
    {
        return destinationObject;
    }

    public void RotateToFaceTheTarget()
    {
        Vector3 _targetPos = weaponSystem.Target.transform.position;
        _targetPos.y = transform.position.y;
        transform.LookAt(_targetPos);
    }

    public void SetMarkervisibility(bool value)
    {

        if (selectionActive && (currentState == runState || currentState == aimStandingState))
        {
            destMarkerRenderer.enabled = value;
            lineRenderer.enabled = value;
        }
        else
        {
            destMarkerRenderer.enabled = false;
            lineRenderer.enabled = false;
        }
    }

    public void TakeDamage(float value)
    {

    }
}
