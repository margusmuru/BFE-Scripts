using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NotificationPrefab : MonoBehaviour {

    public Text Header;
    public Text Description;
    public Vector3 Location;
    public GameObject Objects;

    private bool moveLeftTrigger = false;
    private bool moveDownTrigger = false;
    private bool deathTrigger = false;
    private Vector3 destPos;
    private float lifeStart;
    private GameObject camObj;

    void Awake()
    {
        // get camera location
        camObj = Camera.main.transform.parent.transform.gameObject;
    }
    
    public void SetCamPos()
    {
        //trigger MoveToLeft
        lifeStart = 0;
        CameraControls.SetNewCamPos(Location);
    }

    public void SetText(string _header, string _description, Color _color, Vector3 _location)
    {
        //set visibility
        Objects.SetActive(true);
        //set color
        Header.color = _color;
        Description.color = _color;
        //set text
        Header.text = _header;
        Description.text = _description;
        // set location
        Location = _location;
        //set deathtrigger
        deathTrigger = true;
        lifeStart = Time.time;
    }

    public void CallMoveDown()
    {
        StartCoroutine("MoveDown");
    }

    IEnumerator MoveDown()
    {
        destPos = transform.position + new Vector3(0, -120, 0);
        moveDownTrigger = true;
        yield return new WaitForSeconds(0.5f);
        moveDownTrigger = false;
    }

    IEnumerator MoveLeft()
    {
        destPos = transform.position + new Vector3(340, 0, 0);
        moveLeftTrigger = true;
        yield return new WaitForSeconds(1);
        moveLeftTrigger = false;
        Objects.SetActive(false);
        // change the object location in notificationmanager
        NotificationSystem.NotificationObjects.Remove(gameObject);
        NotificationSystem.NotificationCarbage.Add(gameObject);
    }

    void Update()
    {
        if (moveLeftTrigger)
        {
            transform.position = Vector3.Lerp(transform.position, destPos, 2.5f * Time.deltaTime);
        }
        if (moveDownTrigger)
        {
            transform.position = Vector3.Lerp(transform.position, destPos, 2.5f * Time.deltaTime);
        }
        if (deathTrigger && lifeStart + 5 < Time.time)
        {
            deathTrigger = false;
            StartCoroutine("MoveLeft");
        }
    }
}
