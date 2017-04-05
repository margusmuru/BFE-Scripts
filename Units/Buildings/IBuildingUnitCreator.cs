using UnityEngine;
using System.Collections;

public interface IBuildingUnitCreator {

    ArrayList BuildQueue { get; }
	
    bool UnitActive { get; set; }

    float CurBuildProgress { get; set; }

    void AddToQueue(int value);

    void RemoveFromQueue(int value);
}
