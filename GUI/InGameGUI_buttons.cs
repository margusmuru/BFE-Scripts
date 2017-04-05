using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGameGUI_buttons : MonoBehaviour {

    InGameGUI InGameGUI;

    [Header("Buildings sub menu:")]
    [Header("=================================")]
    public GameObject BuildingsSubButtons;
    private Animator BuildSub;
    public bool buildSubOpen = false;
        
    [Header("BASE button: ")]
    public GameObject Btn_base_a;
    public GameObject Btn_base_p;
    public Text Btn_base_text;
    public Text Btn_base_e_text;

    [Header("POWERSTATION button: ")]
    public GameObject Btn_powerstation_a;
    public GameObject Btn_powerstation_p;
    public Text Btn_powerstation_text;
    public Text Btn_powerstation_e_text;

    [Header("BARRACKS button: ")]
    public GameObject Btn_barracks_a;
    public GameObject Btn_barracks_p;
    public Text Btn_barracks_text;
    public Text Btn_barracks_e_text;

    [Header("FACTORY button: ")]
    public GameObject Btn_factory_a;
    public GameObject Btn_factory_p;
    public Text Btn_factory_text;
    public Text Btn_factory_e_text;

    [Header("SPREAD SWICTH button: ")]
    public GameObject Btn_Centre_UnitMove;
    public GameObject Btn_UnitMove_Tight;
    public GameObject Btn_UnitMove_Wide;

    [Header("=================================")]
    [Header("Options sub menu:")]
    public GameObject OptionsSubButtons;
    private Animator OptionsSub;
    public bool optionsSubOpen = false;
    public Toggle AO_Switch;
    public Button FireModeAgressiveBtn;
    public Sprite FireModeAgressiveSprite;
    public Button FireModePassiveBtn;
    public Sprite FireModePassiveSprite;
    public Button FireModeHoldBtn;
    public Sprite FireModeHoldSprite;
    public Sprite FireModeDisabled;
    public Button AllowPauseBtn;
    public Sprite AllowPauseGreen;
    public Sprite AllowPauseRed;

    [Header("=================================")]
    [Header("Defences sub menu:")]
    public GameObject DefencesSubButtons;
    private Animator DefencesSub;
    public bool defencesSubOpen = false;

    void Awake()
    {
        InGameGUI = GetComponent<InGameGUI>();
        BuildSub = BuildingsSubButtons.GetComponent<Animator>();
        OptionsSub = OptionsSubButtons.GetComponent<Animator>();
    }

    //============================ sub menu switches =====================================================================

    // centre submenu
    public void ChangeMovementSpread(string _input)
    {
        if (_input == "wide")
        {
            Btn_UnitMove_Tight.SetActive(false);
            Btn_UnitMove_Wide.SetActive(true);
            InGameGUI.Spread = "wide";
        }
        if (_input == "tight")
        {
            Btn_UnitMove_Wide.SetActive(false);
            Btn_UnitMove_Tight.SetActive(true);
            InGameGUI.Spread = "tight";
        }
    }

    // show/hide buildings submenu
    public void BuildingsSubMenuSwitch(bool _disable)
    {
        if (!buildSubOpen && !_disable)
        {
            SetBuildingMenuButtons();
            buildSubOpen = true;
            BuildSub.SetBool("Show", true);
        }
        else if (buildSubOpen || _disable)
        {
            buildSubOpen = false;
            BuildSub.SetBool("Show", false);
        }
        // close unit menu
        InGameGUI.selectedBuilding.ClearData();

        // hide other ringmenus
        if (optionsSubOpen)
        {
            optionsSubOpen = false;
            OptionsSub.SetBool("Show", false);
        }
    }

    // show/hide options submenu
    // variable version for buttons in submenu
    public void OptionsSubMenuSwitch(bool _disable)
    {
        if (!optionsSubOpen && !_disable)
        {
            SetOptionsMenuButtons();
            optionsSubOpen = true;
            OptionsSub.SetBool("Show", true);
        }
        else if (optionsSubOpen || _disable)
        {
            optionsSubOpen = false;
            OptionsSub.SetBool("Show", false);
        }
        // close unit menu
        InGameGUI.selectedBuilding.ClearData();

        SetOptionsMenuButtons();

        //hide other ringmenus
        if (buildSubOpen)
        {
            buildSubOpen = false;
            BuildSub.SetBool("Show", false);
        }
    }

    public void OptionsSubMenuSwitch()
    {
        if (optionsSubOpen && !InGameGUI.levelMaster.AutoShowOptionsMenu)
        {
            optionsSubOpen = false;
            OptionsSub.SetBool("Show", false);
        }
        
    }

    //show/hide defences submenu
    public void DefencesSubMenuSwitch(bool _disable)
    {
        if (!defencesSubOpen && !_disable)
        {
            SetDefencesMenuButtons();
            defencesSubOpen = true;
            DefencesSub.SetBool("Show", true);
        }
        else if (defencesSubOpen || _disable)
        {
            defencesSubOpen = false;
            DefencesSub.SetBool("Show", false);
        }
        // close unit menu
        InGameGUI.selectedBuilding.ClearData();
    }


    //=====================================================================================================================

    //===================== sub menu button value changers =============================================================

    public void SetBuildingMenuButtons()
    {
        Btn_base_text.text = UnitValues.BasePrice.ToString();
        Btn_base_e_text.text = "-" + UnitValues.BasePower.ToString();

        Btn_powerstation_text.text = UnitValues.PowerStationPrice.ToString();
        Btn_powerstation_e_text.text = "+" + UnitValues.PowerStationPower.ToString();

        Btn_barracks_text.text = UnitValues.BarracksPrice.ToString();
        Btn_barracks_e_text.text = "-" + UnitValues.BarracksPower.ToString();

        Btn_factory_text.text = UnitValues.FactoryPrice.ToString();
        Btn_factory_e_text.text = "-" + UnitValues.FactoryPower.ToString();

        //Debug.Log(InGameGUI.levelMaster.UsedPowerCount + "/" + InGameGUI.levelMaster.TotalPowerCount);

        // BASE
        if (InGameGUI.levelMaster.MoneyCount >= UnitValues.BasePrice && !InGameGUI.IsBuildingBuilding)
        {
            Btn_base_a.SetActive(true);
            Btn_base_p.SetActive(false);
            if (InGameGUI.levelMaster.UsedPowerCount + UnitValues.BasePower > InGameGUI.levelMaster.TotalPowerCount)
                Btn_base_e_text.color = Color.red;
            else
                Btn_base_e_text.color = Color.green;
            Btn_base_text.color = Color.green;
        }
        else
        {
            Btn_base_a.SetActive(false);
            Btn_base_p.SetActive(true);
            Btn_base_text.color = Color.red;
            if (InGameGUI.levelMaster.UsedPowerCount + UnitValues.BasePower > InGameGUI.levelMaster.TotalPowerCount)
                Btn_base_e_text.color = Color.red;
            else
                Btn_base_e_text.color = Color.green;
            if (InGameGUI.IsBuildingBuilding)
            {
                Btn_base_text.color = Color.grey;
                Btn_base_e_text.color = Color.grey;
            }
        }

        // POWERSTATION
        if (InGameGUI.levelMaster.MoneyCount >= UnitValues.PowerStationPrice && !InGameGUI.IsBuildingBuilding && InGameGUI.levelMaster.BaseCount != 0)
        {
            Btn_powerstation_a.SetActive(true);
            Btn_powerstation_p.SetActive(false);
            Btn_powerstation_e_text.color = Color.green;
            Btn_powerstation_text.color = Color.green;
        }
        else
        {
            Btn_powerstation_a.SetActive(false);
            Btn_powerstation_p.SetActive(true);
            Btn_powerstation_e_text.color = Color.green;
            if (InGameGUI.IsBuildingBuilding || InGameGUI.levelMaster.BaseCount == 0)
            {
                Btn_powerstation_text.color = Color.grey;
                Btn_powerstation_e_text.color = Color.grey;
            }
            if (InGameGUI.levelMaster.MoneyCount < UnitValues.PowerStationPrice)
                Btn_powerstation_text.color = Color.red; 
        }

        //BARRACKS
        if (InGameGUI.levelMaster.MoneyCount >= UnitValues.BarracksPrice && !InGameGUI.IsBuildingBuilding && InGameGUI.levelMaster.PowerStationCount != 0 && InGameGUI.levelMaster.BaseCount != 0)
        {
            Btn_barracks_a.SetActive(true);
            Btn_barracks_p.SetActive(false);
            if (InGameGUI.levelMaster.UsedPowerCount + UnitValues.BarracksPower > InGameGUI.levelMaster.TotalPowerCount)
                Btn_barracks_e_text.color = Color.red;
            else
                Btn_barracks_e_text.color = Color.green; 
            Btn_barracks_text.color = Color.green;
        }
        else
        {
            Btn_barracks_a.SetActive(false);
            Btn_barracks_p.SetActive(true);
            if (InGameGUI.levelMaster.UsedPowerCount + UnitValues.BarracksPower > InGameGUI.levelMaster.TotalPowerCount)
                Btn_barracks_e_text.color = Color.red;
            else
                Btn_barracks_e_text.color = Color.green; 
            if (InGameGUI.IsBuildingBuilding || InGameGUI.levelMaster.BaseCount == 0 || InGameGUI.levelMaster.PowerStationCount == 0)
            {
                Btn_barracks_text.color = Color.grey;
                Btn_barracks_e_text.color = Color.grey;
            }
            if (InGameGUI.levelMaster.MoneyCount < UnitValues.BarracksPrice)
                Btn_barracks_text.color = Color.red;
        }

        //Factory
        if (InGameGUI.levelMaster.MoneyCount >= UnitValues.FactoryPrice && !InGameGUI.IsBuildingBuilding && InGameGUI.levelMaster.BaseCount != 0 && InGameGUI.levelMaster.PowerStationCount != 0)
        {
            Btn_factory_a.SetActive(true);
            Btn_factory_p.SetActive(false);
            if (InGameGUI.levelMaster.UsedPowerCount + UnitValues.FactoryPower > InGameGUI.levelMaster.TotalPowerCount)
                Btn_factory_e_text.color = Color.red;
            else
                Btn_factory_e_text.color = Color.green; 
            Btn_factory_text.color = Color.green;
        }
        else
        {
            Btn_factory_a.SetActive(false);
            Btn_factory_p.SetActive(true);
            if (InGameGUI.levelMaster.UsedPowerCount + UnitValues.FactoryPower > InGameGUI.levelMaster.TotalPowerCount)
                Btn_factory_e_text.color = Color.red;
            else
                Btn_factory_e_text.color = Color.green; 
            if (InGameGUI.IsBuildingBuilding || InGameGUI.levelMaster.BaseCount == 0 || InGameGUI.levelMaster.PowerStationCount == 0)
            {
                Btn_factory_text.color = Color.grey;
                Btn_factory_e_text.color = Color.grey;
            }
            if (InGameGUI.levelMaster.MoneyCount < UnitValues.FactoryPrice)
                Btn_factory_text.color = Color.red;
        }

    }

    public void SetDefencesMenuButtons()
    {

    }

    public void SetOptionsMenuButtons()
    {
        if (InGameGUI.commandUnits.Count > 0)
        {
            FireModeAgressiveBtn.image.overrideSprite = FireModeAgressiveSprite;
            FireModeAgressiveBtn.enabled = true;
            FireModePassiveBtn.image.overrideSprite = FireModePassiveSprite;
            FireModePassiveBtn.enabled = true;
            FireModeHoldBtn.image.overrideSprite = FireModeHoldSprite;
            FireModeHoldBtn.enabled = true;
        }
        else
        {
            FireModeAgressiveBtn.image.overrideSprite = FireModeDisabled;
            FireModeAgressiveBtn.enabled = false;
            FireModePassiveBtn.image.overrideSprite = FireModeDisabled;
            FireModePassiveBtn.enabled = false;
            FireModeHoldBtn.image.overrideSprite = FireModeDisabled;
            FireModeHoldBtn.enabled = false;
        }

        if (InGameGUI.levelMaster.PauseToEngage)
        {
            AllowPauseBtn.image.overrideSprite = AllowPauseGreen;
        }
        else
        {
            AllowPauseBtn.image.overrideSprite = AllowPauseRed;
        }
    }

    //==================================================================================================================

    
    //========================== buildings menu button commands ==========================================================
    public void ButtonBuildBase()
    {
        InGameGUI.PlaceMode = true;
        InGameGUI.CurBuild = 0;
    }

    public void ButtonBuildPowerStation()
    {
        InGameGUI.PlaceMode = true;
        InGameGUI.CurBuild = 1;
    }

    public void ButtonBuildBarracks()
    {
        InGameGUI.PlaceMode = true;
        InGameGUI.CurBuild = 2;
    }

    public void ButtonBuildFactory()
    {
        InGameGUI.PlaceMode = true;
        InGameGUI.CurBuild = 3;
    }

    //==================================================================================================================

    //========================== options menu button commands ==========================================================

    public void SetToggle()
    {
        //disable
        if (InGameGUI.levelMaster.AutoShowOptionsMenu)
        {
            InGameGUI.levelMaster.AutoShowOptionsMenu = false;
            AO_Switch.isOn = false;
        }
        //enable 
        else if (!InGameGUI.levelMaster.AutoShowOptionsMenu)
        {
            InGameGUI.levelMaster.AutoShowOptionsMenu = true;
            AO_Switch.isOn = true;
        }
    }

    public void SetFireMode(int _input)
    {
        foreach (GameObject go in InGameGUI.commandUnits)
        {
            go.SendMessage("SetFireMode", _input, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void SetPauseToEngage()
    {
        if (InGameGUI.levelMaster.PauseToEngage)
        {
            InGameGUI.levelMaster.PauseToEngage = false;
            AllowPauseBtn.image.overrideSprite = AllowPauseRed;
        }
        else
        {
            InGameGUI.levelMaster.PauseToEngage = true;
            AllowPauseBtn.image.overrideSprite = AllowPauseGreen;
        }
    }

    //==================================================================================================================
}

