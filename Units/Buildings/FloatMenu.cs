using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatMenu : MonoBehaviour {
    
    public Image HealthCircle;
    public GameObject TextArea;
    public GameObject Star1;
    public GameObject Star2;
    public GameObject Star3;

    public Image Icon;
    public Sprite IconBase;
    public Sprite IconPowerStation;
    public Sprite IconFactory;
    public Sprite IconBarracks;

    void Start()
    {
        //gameObject.transform.localScale.Set(1, 1, 1);
        this.transform.localScale = Vector3.one;
    }

    public void SetIcon(string _input)
    {
        if (_input == "Military Base") { Icon.overrideSprite = IconBase; }
        if (_input == "Power Station") { Icon.overrideSprite = IconPowerStation; }
        if (_input == "Refinery") { Icon.overrideSprite = IconFactory; }
        if (_input == "Barracks") { Icon.overrideSprite = IconBarracks; }
    }

    public void SetHealth(float _curHealth, float _maxHealth)
    {
        if (_curHealth < 0) { _curHealth = 0; }
        if (_maxHealth < 0) { _maxHealth = 0; }
        HealthCircle.fillAmount = _curHealth / _maxHealth;
    }

    public void SetColor(string _input)
    {
        if (_input == "green")
        {
            HealthCircle.color = Color.green;
        }
        if(_input == "blue")
        {
            HealthCircle.color = Color.cyan;
        }
    }

    public void SetTextArea(bool _input)
    {
        TextArea.SetActive(_input);
    }

    public void SetUnitLevel(int _level)
    {
        if (_level >= 0)
        {
            Star1.SetActive(false);
            Star2.SetActive(false);
            Star3.SetActive(false);        
        }

        if (_level >= 1) { Star1.SetActive(true); }
        if (_level >= 2) { Star2.SetActive(true); }
        if (_level == 3) { Star3.SetActive(true); }
        
    }


}
