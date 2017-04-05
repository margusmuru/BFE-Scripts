using UnityEngine;
using System.Collections;

public class BasicBuildState : IBuildingState {

    private Building building;

    private float addHealthSave = 0;
    private float buildProgressRound;
    private bool buildDone = false;

    public BasicBuildState(Building building)
    {
        this.building = building;
    }

    public void Update()
    {
        if (!buildDone)
        {
            building.Spinner.transform.Rotate(0, 0, 30 * Time.deltaTime);

            AnimatorStateInfo currentState = building.animator.GetCurrentAnimatorStateInfo(0);
            float playbackTime = currentState.normalizedTime;

            //use state to set floatmenu´s healthring progress and BuildProgress
            building.floatMenu.SetHealth(building.CurHealth, building.MaxHealth);
            //add health
            buildProgressRound = Mathf.Round(100 * playbackTime);
            building.BuildProgress = buildProgressRound;

            if (addHealthSave != buildProgressRound)
            {
                building.CurHealth += building.MaxHealth / 100;
                addHealthSave = buildProgressRound;
            }

            if (!building.animator.GetCurrentAnimatorStateInfo(0).IsName("Take 001"))
            {
                buildDone = true;
                building.FinishBuilding();
                ToActiveState();
            }
        }
    }

    

    public void ToActiveState()
    {
        building.currentState = building.activeState;
        building.currentState.ToActiveState();
    }

    public void ToDeathState()
    {
        building.animator.speed = 0f;
        building.currentState = building.deathState;
        building.currentState.ToDeathState();
    }

    public void ToInActiveState()
    {

    }

    public void ToBuildState()
    {
        if (GameInfo.FastBuild)
            building.animator.speed = 8f;
    }

    public void ToUnitBuildState()
    {

    }


    public void ToSellState()
    {

    }
}
