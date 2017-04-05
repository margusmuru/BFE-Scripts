using UnityEngine;
using System.Collections;

public class BuildingBase : Building, IBuilding, IDamageable {

    public override void Awake()
    {
        base.Awake();
        activeState = new BaseActiveState(this);
        buildState = new BasicBuildState(this);
        deathState = new BasicDeathState(this);
        sellState = new BasicSellState(this);
    }

    public override void Start ()
    {
        base.Start();

        MaxHealth = UnitValues.BaseHealth;
        levelMaster.AddBuilding(tagObj, UnitName);
        
        levelMaster.RemoveMoney(UnitValues.BasePrice);

        currentState = buildState;
        currentState.ToBuildState();

        FindClosePlanes(2);
    }
	
	public override void Update ()
    {
        base.Update();
        currentState.Update();
    
	}

    public override void FinishBuilding()
    {
        base.FinishBuilding();
        levelMaster.AddPower(UnitValues.BasePower, false);
    }

    public override void DeathActions()
    {
        base.DeathActions();
        levelMaster.RemovePower(UnitValues.BasePower, false);
    }

    public override void DeathActionsDelayed()
    {
        base.DeathActionsDelayed();
    }

    public override void SellActions()
    {
        base.SellActions();
        levelMaster.RemovePower(UnitValues.BasePower, false);
        levelMaster.AddMoney(Mathf.RoundToInt(UnitValues.BasePrice * 0.8f));
    }

    public override void UpgradeBuilding()
    {
        base.UpgradeBuilding();

    }
}
