using UnityEngine;
using System.Collections;

public class UnitProductionAI : MonoBehaviour {

    private AI ai;

    public void Awake()
    {
        ai = GetComponent<AI>();
        InvokeRepeating("UpdateCycle", ai.StartDelay + 10, 3f);
    }

    private void UpdateCycle()
    {
        if (ai.EnableAI)
        {

            //check if it is possible to construct a collector
            if (ai.lm.FactoryArray.Count > 0 &&
                ai.lm.CollectorCount + ai.lm.ExpectedCollectorCount < ai.lm.MaxCollectorCount &&
                ai.lm.MoneyCount >= UnitValues.CollectorPrice)
            {
                //check if unit is active
                GameObject building = (GameObject)ai.lm.FactoryArray[0];
                BuildingFactory factory = 
                    building.transform.parent.gameObject.GetComponent<BuildingFactory>();
                if (factory.UnitActive)
                {
                    //build
                    factory.AddToQueue(0);
                    if (ai.lm.enableLog)
                        Debug.Log("UnitProduction: requested a collector");
                }
            }

            if (ai.lm.UnitCount < ai.MaxSoldierCount && ai.lm.BarracksArray.Count > 0 &&
                ai.lm.MoneyCount >= UnitValues.SoldierPrice)
            {
                GameObject building = (GameObject)ai.lm.BarracksArray[0];
                BuildingBarracks barracks =
                    building.transform.parent.gameObject.GetComponent<BuildingBarracks>();
                if (barracks.UnitActive && barracks.BuildQueue.Count < 4)
                {
                    //build
                    barracks.AddToQueue(0);
                    if (ai.lm.enableLog)
                        Debug.Log("UnitProduction: requested a soldier");
                }
            }



        }
    }
    
}
