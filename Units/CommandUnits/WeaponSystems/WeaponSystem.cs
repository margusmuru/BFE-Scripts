using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class WeaponSystem : MonoBehaviour {

    public enum Weapon { soldier, antiArmor, heavy, sniper };
    public Weapon weaponType = Weapon.soldier;

    public Transform muzzlePos;
    public ParticleSystem effectSys;
    public LayerMask targetLayerMask = 0;

    private GameObject target;
    public GameObject Target
    {
        get { return target; }
        set { target = value; }
    }

    private string fireMode;
    public string FireMode
    {
        get { return fireMode; }
        set { fireMode = value; }
    }
    
    private float nextFireTime;
    public float NextFireTime
    {
        get { return nextFireTime; }
        set { nextFireTime = value; }
    }

    private GameObject priorityTarget;

    public GameObject PriorityTarget
    {
        get { return priorityTarget; }
        set { priorityTarget = value; }
    }

    public string enemyTag = "Team2";
    public float reloadTime = 1f;
    public float weaponDam = 25f;
    public float defaultDamage;
    public float wepInaccuracy = 10f;
    public float weaponRange = 25f;
    public bool engage = true;
    public ICommandUnit masterScript;

    private float nextTargetSearch;
    private ArrayList targetList;
    [HideInInspector]
    public AudioSource soundPlayer;

    public virtual void Awake()
    {
        masterScript = GetComponentInParent<ICommandUnit>();
        soundPlayer = muzzlePos.gameObject.GetComponent<AudioSource>();
    }

    public virtual void Start ()
    {
        nextFireTime = Time.time;
        nextTargetSearch = Time.time + 1;
        defaultDamage = weaponDam;
        targetList = masterScript.LevelMasterRef.EnemyUnits;
        fireMode = "agressive";
    }
	
	public virtual void Update ()
    {
        if (Time.time > nextTargetSearch)
        {
            FindTargets();
            nextTargetSearch = nextTargetSearch + 1;
        }
	}

    void FindTargets()
    {
        float distance = Mathf.Infinity;
        float curDis;
        GameObject _target = null;

        foreach (GameObject go in targetList)
        {
            // priorityTarget
            if (go == priorityTarget && (go.transform.position - transform.position).sqrMagnitude < weaponRange * weaponRange)
            {
                gameObject.transform.LookAt(go.transform);
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 1000, targetLayerMask))
                {
                    if (hit.collider.gameObject == go)
                    {
                        _target = go;
                        break;
                    }
                }
            }
            if (fireMode.Equals("agressive"))
            {
                curDis = (go.transform.position - transform.position).sqrMagnitude;
                //if object is in weaponrange and closer than previous target
                if (curDis < weaponRange * weaponRange && curDis < distance)
                {
                    //check if possible to shoot
                    gameObject.transform.LookAt(go.transform);
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, transform.forward, out hit, 1000, targetLayerMask))
                    {
                        if (hit.collider.gameObject == go)
                        {
                            distance = curDis;
                            _target = go;
                        }
                    }
                }
            }
        }
        if (_target)
        {
            if (!target)
            {
                nextFireTime = Time.time + reloadTime;
            }
            target = _target;
        }
        else
        {
            target = null;
        }

        //leave prioritytarget when it is dead, until then it will shoot it if possible
        if (priorityTarget && priorityTarget.tag == "undefined")
        {
            priorityTarget = null;
        }
    }

    public void SetDamageValue(string targetName)
    {
        switch (weaponType)
        {
            case Weapon.soldier:
                weaponDam = defaultDamage;
                break;
            case Weapon.antiArmor:
                if (targetName == "SoldierBody" || targetName == "Sniper" || targetName == "Heavy" || targetName == "AntiArmor")
                {
                    weaponDam = weaponDam / 4;
                }
                break;
            case Weapon.heavy:
                weaponDam = defaultDamage;
                break;
            case Weapon.sniper:
                weaponDam = defaultDamage;
                break;
            default:
                weaponDam = defaultDamage;
                break;
        }
    }

    public void PlaySound()
    {
        if (soundPlayer.isPlaying)
        {
            soundPlayer.Stop();
        }
        soundPlayer.Play();
    }

}
