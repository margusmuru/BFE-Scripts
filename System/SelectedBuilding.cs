using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectedBuilding : MonoBehaviour {

    //currently selected building
    private GameObject selBuild;
	public GameObject SelBuild
	{
		get { return selBuild;}
		set 
        { 
            selBuild = value;
            GetData();
        }
	}

    #region selectedBuildData
    private IBuilding build; //reference to building script
    private IBuildingUnitCreator buildCreator;

    public string UnitName;
    public int UnitLevel;
    public float curHealth;
    public float maxhealth;
    #endregion
    // references to GUI objects
    #region general
    public GameObject BuildingMenu;
    public GameObject IconPanel;
    public Image IconImage;
    public Sprite[] BuildingIcons;

    public GameObject Panel2;
    public GameObject Panel3;
    public Image HealthRing;
    public Text HealthText;
    public Text HealthText1;
    public Image HealthBar;
    public Text NameText;
    public Image Star1;
    public Image Star2;
    public Image Star3;
    #endregion
    // unit build buttons
    public Button[] UnitBuildButtons;
    // unit queue buttons
    public Button[] QueueButtons;
    public Button[] QueueOverLay;
    private int savedQueueCount;

    // factory
    //private Building_Factory factoryScript;
    public Sprite[] FactoryActiveFriendly;
    public Sprite[] FactoryPassiveFriendly;
    public Sprite[] FactoryActiveEnemy;
    public Sprite[] FactoryPassiveEnemy;
    // barracks
    //private Building_Barracks barracksScript;
    public Sprite[] BarracksActiveFriendly;
    public Sprite[] BarracksPassiveFriendly;
    public Sprite[] BarracksActiveEnemy;
    public Sprite[] BarracksPassiveEnemy;

    public ILevelMaster levelMaster;


    private bool dataRecieved = false;
    private bool panelsVisible = true;

    void Start()
    {
        //get levelmaster
        levelMaster = GameInfo.GetLevelMaster("Friendly");
    }

    void Update()
    {
        if (selBuild != null && dataRecieved && !build.ObjDead)
        {
            if (build.BuildProgress == 100)
            {
                if (!panelsVisible)
                {
                    SetPanels(true);
                }
                curHealth = build.CurHealth;
                HealthText1.text = curHealth.ToString() + "/" + maxhealth.ToString();
                //calculate healtbar value
                if (curHealth > 0) { HealthBar.fillAmount = curHealth / maxhealth; }
                else { HealthBar.fillAmount = 0; }

                if (buildCreator != null)
                {
                    #region factory
                    if (UnitName.Equals("Refinery"))
                    {
                        //collector
                        if (levelMaster.CollectorCount + buildCreator.BuildQueue.Count < levelMaster.MaxCollectorCount
                            && buildCreator.UnitActive && levelMaster.MoneyCount >= UnitValues.CollectorPrice)
                        {
                            if (GameInfo.PlayerUnit.Equals("Friendly"))
                                UnitBuildButtons[0].image.overrideSprite = FactoryActiveFriendly[0];
                            else
                                UnitBuildButtons[0].image.overrideSprite = FactoryActiveEnemy[0];
                        }
                        else
                        {
                            if (GameInfo.PlayerUnit.Equals("Friendly"))
                                UnitBuildButtons[0].image.overrideSprite = FactoryPassiveFriendly[0];
                            else
                                UnitBuildButtons[0].image.overrideSprite = FactoryPassiveEnemy[0];
                        }


                        // queue button visibility and icons
                        if (savedQueueCount != buildCreator.BuildQueue.Count)
                        {
                            savedQueueCount = buildCreator.BuildQueue.Count;
                            SetQueueButtonsVisibility(savedQueueCount);
                        }
                        if (buildCreator.BuildQueue.Count > 0)
                            QueueOverLay[0].image.fillAmount = buildCreator.CurBuildProgress;
                    }
                    #endregion
                    
                    #region barracks
                    if (UnitName.Equals("Barracks"))
                    {
                        if (buildCreator.BuildQueue.Count < 4 && buildCreator.UnitActive
                             && levelMaster.MoneyCount >= UnitValues.SoldierPrice)
                        {
                            if (GameInfo.PlayerUnit.Equals("Friendly"))
                                UnitBuildButtons[0].image.overrideSprite = BarracksActiveFriendly[0];
                            else
                                UnitBuildButtons[0].image.overrideSprite = BarracksActiveEnemy[0];
                        }
                        else
                        {
                            if (GameInfo.PlayerUnit.Equals("Friendly"))
                                UnitBuildButtons[0].image.overrideSprite = BarracksPassiveFriendly[0];
                            else
                                UnitBuildButtons[0].image.overrideSprite = BarracksPassiveEnemy[0];
                        }

                        // queue button visibility and icons
                        if (savedQueueCount != buildCreator.BuildQueue.Count)
                        {
                            savedQueueCount = buildCreator.BuildQueue.Count;
                            SetQueueButtonsVisibility(savedQueueCount);
                        }
                        if (buildCreator.BuildQueue.Count > 0)
                            QueueOverLay[0].image.fillAmount = buildCreator.CurBuildProgress;
                    }
                    #endregion

                }
            }
            else
            {
                HealthRing.fillAmount = build.BuildProgress;
                HealthText.text = Mathf.Round(build.BuildProgress).ToString() + "%";
            }
        }
        else if(dataRecieved)
        {
            ClearData();
        }
    }

    public void GetData()
    {
        if (selBuild != null)
        {
            // set reference
            build = selBuild.GetComponent<IBuilding>();
            dataRecieved = true;
            // get data
            UnitName = build.UnitName;
            NameText.text = UnitName;
            UnitLevel = build.UnitLevel;
            // get health
            curHealth = build.CurHealth;
            maxhealth = build.MaxHealth;
            // set star visibility
            SetUnitLevel(UnitLevel);
            //set panels visibility
            if (UnitName.Equals("Refinery") || UnitName.Equals("Barracks"))
                buildCreator = selBuild.GetComponent<IBuildingUnitCreator>();
            else
                buildCreator = null;
            SetPanels(build.BuildProgress == 100);
            BuildingMenu.SetActive(true);
        }
    }
    
    public void ClearData()
    {
        BuildingMenu.SetActive(false);
        dataRecieved = false;
        //build = null;
        selBuild = null;
        //SetPanels(false);
    }
    
    public void SetPanels(bool buildComplete)
    {
        Panel3.SetActive(buildComplete);
        HealthRing.enabled = !buildComplete;
        HealthText.enabled = !buildComplete;
        panelsVisible = buildComplete;

        if (!buildComplete)
        {
            Panel2.SetActive(false);
            SetBuildButtonsVisibility(false, 0);
            IconPanel.SetActive(false);
        }
        else
        {
            IconPanel.SetActive(true);
            SetBuildButtonsVisibility(false, 6);
            if (buildCreator != null)
                SetQueueButtonsVisibility(buildCreator.BuildQueue.Count);
            else
                SetQueueButtonsVisibility(0);

            if (UnitName.Equals("Military Base"))
                IconImage.overrideSprite = BuildingIcons[0];
            if (UnitName.Equals("Power Station"))
                IconImage.overrideSprite = BuildingIcons[1];
            if (UnitName.Equals("Refinery")) 
            {
                IconImage.overrideSprite = BuildingIcons[2];
                SetBuildButtonsVisibility(true, 1);
                Panel2.SetActive(true);
            }
            if (UnitName.Equals("Barracks"))
            {
                IconImage.overrideSprite = BuildingIcons[3];
                SetBuildButtonsVisibility(true, 1);
                Panel2.SetActive(true);
            }
        }
    }

    public void SetUnitLevel(int _level)
    {
        if (_level >= 0)
        {
            Star1.enabled = false;
            Star2.enabled = false;
            Star3.enabled = false;
        }
        if (_level >= 1) { Star1.enabled = true; }
        if (_level >= 2) { Star2.enabled = true; }
        if (_level == 3) { Star3.enabled = true; }
    }

 
    public void SellBuilding()
    {
        build.SellBuilding();
        ClearData();
    }

    public void UpgradeBuilding()
    {
        build.UpgradeBuilding();
        GetData();
    }

    public void BuildUnit(int input)
    {
        if (buildCreator.UnitActive)
        {
            buildCreator.AddToQueue(input);
            Debug.Log("Added to queue: " + input);
        }
            
    }

    public void CancelUnitBuild(int _input)
    {
        buildCreator.RemoveFromQueue(_input);
    }

    private void SetBuildButtonsVisibility(bool _input, int _count)
    {
        if (!_input)
            _count = 6;
        for (int i = 0; i < _count; i++)
            UnitBuildButtons[i].gameObject.SetActive(_input);
    }

    private void SetQueueButtonsVisibility(int _count)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i < _count)
            {
                QueueButtons[i].gameObject.SetActive(true);
                // set queue icons
                if (UnitName == "Refinery")
                {
                    var tmp = (int)buildCreator.BuildQueue[i];

                    if (GameInfo.PlayerUnit == "Friendly")
                        QueueButtons[i].image.overrideSprite = FactoryActiveFriendly[tmp];
                    else
                        QueueButtons[i].image.overrideSprite = FactoryActiveEnemy[tmp];
                }
                if (UnitName == "Barracks")
                {
                    var tmp = (int)buildCreator.BuildQueue[i];

                    if (GameInfo.PlayerUnit == "Friendly")
                        QueueButtons[i].image.overrideSprite = BarracksActiveFriendly[tmp];
                    else
                        QueueButtons[i].image.overrideSprite = BarracksActiveEnemy[tmp];
                }
                QueueOverLay[i].gameObject.SetActive(true);
                QueueOverLay[i].image.fillAmount = 1;
            }
            else
            {
                QueueButtons[i].gameObject.SetActive(false);
                QueueOverLay[i].gameObject.SetActive(false);
            }
            
        }
    }

    
}
