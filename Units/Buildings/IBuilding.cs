using UnityEngine;
using System.Collections;

public interface IBuilding {

	string UnitName { get; }

    int UnitLevel { get; set; }

    float CurHealth { get; set; }

    float MaxHealth { get; set; }

    bool ObjDead { get; }

    float BuildProgress { get; set; }

    bool UnitActive { get; set; }

    void SellBuilding();

    void RepairBuilding();

    void UpgradeBuilding();

}
