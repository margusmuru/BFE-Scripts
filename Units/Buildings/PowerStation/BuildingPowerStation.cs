using UnityEngine;
using System.Collections;

public class BuildingPowerStation : Building, IBuilding, IDamageable
{
    public ParticleSystem SteamEmitter;

    public override void Awake()
    {
        base.Awake();
        activeState = new PowStatActiveState(this);
        inactiveState = new PowStatInActiveState(this);
        buildState = new BasicBuildState(this);
        deathState = new BasicDeathState(this);
        sellState = new BasicSellState(this);
    }

    public override void Start()
    {
        base.Start();

        MaxHealth = UnitValues.PowerStationHealth;
        levelMaster.AddBuilding(tagObj, UnitName);

        levelMaster.RemoveMoney(UnitValues.PowerStationPrice);

        currentState = buildState;
        currentState.ToBuildState();

        FindClosePlanes(1);
    }

    public override void Update()
    {
        base.Update();
        currentState.Update();

    }

    public override void FinishBuilding()
    {
        SteamEmitter.Play();
        levelMaster.AddPower(UnitValues.PowerStationPower, true);
        base.FinishBuilding();
    }

    public override void DeathActions()
    {
        base.DeathActions();
        SteamEmitter.Stop();
        levelMaster.RemovePower(UnitValues.PowerStationPower, true);
    }

    public override void DeathActionsDelayed()
    {
        base.DeathActionsDelayed();
    }

    public override void SellActions()
    {
        base.SellActions();
        levelMaster.RemovePower(UnitValues.PowerStationPower, true);
        levelMaster.AddMoney(Mathf.RoundToInt(UnitValues.PowerStationPrice * 0.8f));
    }

    public override void UpgradeBuilding()
    {
        base.UpgradeBuilding();

    }
}
