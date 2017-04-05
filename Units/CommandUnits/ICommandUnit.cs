using UnityEngine;
using System.Collections;

public interface ICommandUnit {

    ILevelMaster LevelMasterRef
    {
        get; set;
    }
    
    void SetSelection(bool value);

    void MoveTo(Vector3 location);

    GameObject getDestinationObject();

}
