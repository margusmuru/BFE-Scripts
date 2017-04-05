using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResourcesPile : MonoBehaviour {

    public float ResourcesLeft = 100;
    public bool OverRideUnitValues = false;
    public float OverRideResourcesAmount = 1000;
    public Image HealthRing;



    void Start()
    {
        
        if (OverRideUnitValues)
        {
            ResourcesLeft = OverRideResourcesAmount;
        }
        else
        {
            ResourcesLeft = UnitValues.ResourcePileMax;
        }

        UnitLocationsManager.ResourcesList.Add(gameObject);
        InvokeRepeating("SetHealthRing", 5, 5);
    }

    void SetHealthRing()
    {
        HealthRing.fillAmount = ResourcesLeft / UnitValues.ResourcePileMax;
    }

    public float GetResources(float amount)
    {
        if (amount < ResourcesLeft)
        {
            ResourcesLeft -= amount;
            SetHealthRing();
            return amount;
        }
        else
        {
            float tmp = ResourcesLeft;
            ResourcesLeft = 0;
            SetHealthRing();
            return tmp;
        }
    }

}
