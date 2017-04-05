using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public interface ILevelMaster
{
    int UnitCount { get; }

    int CollectorCount { get; }

    int MaxCollectorCount { get; }

    int ExpectedCollectorCount { get; set; }

    int BaseCount { get; }

    int PowerStationCount { get; }

    int FactoryCount { get; }

    int BarracksCount { set; }

    int MoneyCount { get; }

    int XpCount { get; }

    int UsedPowerCount { get; }

    int TotalPowerCount { get; }

    bool HumanPlayer { get; }

    string PlayerTag { get; set; }

    void AddBuilding(GameObject _gameObj, string _input);

    void RemoveBuilding(GameObject _gameObj, string _input, int buildingLevel);

    void AddUnit(GameObject _gameObj, string _type);

    void RemoveUnit(GameObject _gameObj, string _type);

    void AddMoney(int _input);

    void RemoveMoney(int _input);

    void AddXP(int _input);

    void RemoveXP(int _input);

    void AddPower(int _input, bool _toTotal);

    void RemovePower(int _input, bool _fromTotal);

    void AddCollectorMax();

    void RemoveCollectorMax(int _count);

    bool IntmpAvoidList(Vector3 _input);

    ArrayList FriendlyUnits { get; }

    ArrayList EnemyUnits { get; }

    void SetCurrentBuildingConstructionState(bool value);
}
