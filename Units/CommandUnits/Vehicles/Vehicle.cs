using UnityEngine;
using System.Collections;

public class Vehicle : MonoBehaviour, ICommandUnit {

    private ILevelMaster levelMaster;
    public ILevelMaster LevelMasterRef
    {
        get { return levelMaster; }
        set { levelMaster = value; }
    }
    [HideInInspector]
    public AIPath aiPath;
    [HideInInspector]
    public AudioSource audioSource;
    public GameObject destinationObject;
    public GameObject tagObj;
    public GameObject selectionMarker;
    [HideInInspector]
    public bool selectionActive = false;
    private Renderer destMarkerRenderer;
    private LineRenderer lineRenderer;

    public GameObject dynamicSet;
    public GameObject staticSet;
    
    public IVehicleState currentState;   
    public IVehicleState deathState;
    public IVehicleState moveState;

    public string UnitFactionField = "Friendly";

    public virtual void Awake()
    {
        aiPath = GetComponent<AIPath>();
        audioSource = GetComponent<AudioSource>();
        lineRenderer = GetComponent<LineRenderer>();
        destMarkerRenderer = destinationObject.GetComponent<Renderer>();
        deathState = new VehicleDeathState(this);
    }

    public virtual void Start ()
    {
        levelMaster = GameInfo.GetLevelMaster(UnitFactionField);
        tagObj.tag = levelMaster.PlayerTag;
    }
	
	public virtual void Update ()
    {
        currentState.UpdateState();
        //if selection is active and unit is moving, calculate line renderer
        if (lineRenderer.enabled)
        {
            lineRenderer.SetPosition(0, transform.position + new Vector3(0, 0.3f, 0));
            lineRenderer.SetPosition(2, transform.position + new Vector3(0, 0.3f, 0));
            lineRenderer.SetPosition(1, destinationObject.transform.position + new Vector3(0,1,0));
        }
	}

    public void MoveTo(Vector3 location)
    {
        destinationObject.transform.position = location;
        currentState.ToMoveState(null);
        SetMarkervisibility(true);
    }

    public void SetSelection(bool value)
    {
        selectionMarker.SetActive(value);
        selectionActive = value;
        SetMarkervisibility(value);
    }

    public void TakeDamage(float value)
    {

    }

    public GameObject getDestinationObject()
    {
        return destinationObject;
    }

    public void SetMarkervisibility(bool value)
    {
        
        if (selectionActive && currentState == moveState)
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
}
