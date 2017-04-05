using UnityEngine;
using System.Collections;

public class BasicDeathState : IBuildingState {

    private Building building;
    private bool deathSequence = false;
    private float deathTimer = 0f;
    private bool explosionAction = false;
    private bool deathMovement = false;
    private Vector3 endLoc = new Vector3(0, 0, 0);

    public BasicDeathState(Building building)
    {
        this.building = building;
    }

    public void Update()
    {
        if (deathSequence)
        {
            if (Time.time > deathTimer && !explosionAction)
            {
                explosionAction = true;
                building.DeathActionsDelayed();
                deathTimer = Time.time + 30;
            }
            if (Time.time > deathTimer && !deathMovement)
            {
                deathMovement = true;
                building.FreeArea();
                endLoc = building.WreckSet.transform.position + new Vector3(0, -15, 0);
            }
            if (deathMovement)
            {
                building.WreckSet.transform.position = Vector3.Lerp(building.WreckSet.transform.position, endLoc, 0.1f * Time.deltaTime);
                if (building.WreckSet.transform.position.y < endLoc.y + 3)
                {
                    MonoBehaviour.Destroy(building.gameObject);
                }
            }
        }
    }

    public void ToActiveState()
    {

    }

    public void ToDeathState()
    {
        building.DeathActions();
        deathTimer = Time.time + 0.5f;
        deathSequence = true;
    }

    public void ToInActiveState()
    {

    }

    public void ToBuildState()
    {

    }

    public void ToUnitBuildState()
    {

    }


    public void ToSellState()
    {

    }

}
