using UnityEngine;
using System.Collections;

public class NotificationSystem : MonoBehaviour {

    public static NotificationSystem Instance;
    public GameObject NotificationPrefab;

	// add new notifications to a queue, and display them as needed. display up to 4 at a time;
    private float displayTime = 5f;

    private ArrayList notHeader;
    private ArrayList notDescription;
    private ArrayList notColor;
    private ArrayList notLocation;

    public static ArrayList NotificationObjects;
    public static ArrayList NotificationCarbage;
    private GameObject RootCanvas;
    void Awake()
    {
        Instance = this;
    }

	void Start () {
        //initialize arrays
        notHeader = new ArrayList();
        notDescription = new ArrayList();
        notColor = new ArrayList();
        notLocation = new ArrayList();
        NotificationObjects = new ArrayList();
        NotificationCarbage = new ArrayList();
        StartCoroutine("NotificationChecker");
        RootCanvas = GameObject.FindGameObjectWithTag("InGameCanvasFolder");
	}
	
	void Update () 
    {
	       
	}

    public static void SendNotification(string _header, string _description, Color _color, Vector3 _location)
    {
        // add notification data to arraylists.
        Instance.notHeader.Add(_header);
        Instance.notDescription.Add(_description);
        Instance.notColor.Add(_color);
        Instance.notLocation.Add(_location);
    }

    IEnumerator NotificationChecker()
    {
        GameObject notObj = null;
        NotificationPrefab notObjScript;

        for (; ; )
        {
            if (Instance.notLocation.Count > 0 && NotificationObjects.Count < 4)
            {
                //move any previous notifications down
                foreach (GameObject go in NotificationObjects)
                {
                    go.gameObject.GetComponent<NotificationPrefab>().CallMoveDown();
                }
                //wait if needed
                if (NotificationObjects.Count > 0) { yield return new WaitForSeconds(0.5f); }

                //instanciate a new one if needed
                if (NotificationCarbage.Count > 0)
                {
                    notObj = (GameObject) NotificationCarbage[0];
                    notObj.transform.position = new Vector3(Screen.width - 10, Screen.height - 40, 0);
                    //change object location in arrays
                    NotificationCarbage.RemoveAt(0);
                    NotificationObjects.Add(notObj);
                }
                else
                {
                    notObj = Instantiate(NotificationPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                    notObj.transform.parent = RootCanvas.transform.parent.transform;
                    notObj.transform.position = new Vector3(Screen.width - 10, Screen.height - 40, 0);
                    //add to in-use objects list
                    NotificationObjects.Add(notObj);
                    
                }
                // update notification info
                notObjScript = notObj.GetComponent<NotificationPrefab>();
                notObjScript.SetText((string)notHeader[0], (string)notDescription[0], (Color)notColor[0], (Vector3)notLocation[0]);
                //remove displayed notification from the queue
                notLocation.RemoveAt(0);
                notColor.RemoveAt(0);
                notDescription.RemoveAt(0);
                notHeader.RemoveAt(0);
            }
            
            yield return new WaitForSeconds(0.5f);
        }

    }
}
