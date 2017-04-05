using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour {

    public static CameraControls Instance;
    public static bool cameraMoving = false;
    public InGameGUI _inGameGUI;

    private int triggerEdgeSize = 5;
    private float cameraMoveSpeed = 20;

    private Vector3 mouseOrigin;
    private bool isRotating = false;
    private Quaternion savedRotation;
    private Quaternion savedRotation1;

    private bool triggerCamPos = false;
    private Vector3 newCamPos;

    private Vector3 savedCamPos;
    private Quaternion savedCamRot;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        savedRotation = gameObject.transform.localRotation;
        savedRotation1 = gameObject.transform.parent.gameObject.transform.rotation;
    }

    public static void SetNewCamPos(Vector3 _input)
    {
        CameraControls.Instance.newCamPos = _input;
        CameraControls.Instance.triggerCamPos = true;
    }

    void Update()
    {
        if (!_inGameGUI.GamePaused && !triggerCamPos)
        {

            /* Camera controls */
            /* Pan camera around with arrow keys */
            if (Application.isEditor || !Screen.fullScreen) //no screen-edge trigger
            {
                if (Input.GetAxis("Horizontal") < 0)
                    { transform.parent.gameObject.transform.Translate(Vector3.left * Time.deltaTime * 20); }
                if (Input.GetAxis("Horizontal") > 0)
                    { transform.parent.gameObject.transform.Translate(Vector3.right * Time.deltaTime * 20); }
                if (Input.GetAxis("Vertical") < 0)
                    { transform.parent.gameObject.transform.Translate(Vector3.back * Time.deltaTime * 20); }
                if (Input.GetAxis("Vertical") > 0)
                    { transform.parent.gameObject.transform.Translate(Vector3.forward * Time.deltaTime * 20); }
            }
            else//with screen-edge trigger
            {
                if (Input.mousePosition.x < triggerEdgeSize || Input.GetAxis("Horizontal") < 0)
                    { transform.parent.gameObject.transform.Translate(Vector3.left * Time.deltaTime * 20); }
                if (Input.mousePosition.x > (Screen.width - triggerEdgeSize) || Input.GetAxis("Horizontal") > 0)
                    { transform.parent.gameObject.transform.Translate(Vector3.right * Time.deltaTime * 20); }
                if (Input.mousePosition.y < triggerEdgeSize || Input.GetAxis("Vertical") < 0)
                    { transform.parent.gameObject.transform.Translate(Vector3.back * Time.deltaTime * 20); }
                if (Input.mousePosition.y > (Screen.height - triggerEdgeSize) || Input.GetAxis("Vertical") > 0)
                    { transform.parent.gameObject.transform.Translate(Vector3.forward * Time.deltaTime * 20); }
            }
            /* zoom in and out with mousewheel */
            if ((Input.GetKey(KeyCode.KeypadPlus) || Input.GetAxis("Mouse ScrollWheel") > 0) && transform.position.y > 5)
            {
                transform.Translate(Vector3.down * Time.deltaTime * cameraMoveSpeed * 6, Space.World);
                transform.Translate(Vector3.forward * Time.deltaTime * cameraMoveSpeed * 3.5f, Space.Self);
            }
            if ((Input.GetKey(KeyCode.KeypadMinus) || Input.GetAxis("Mouse ScrollWheel") < 0) && transform.position.y < 40)
            {
                transform.Translate(Vector3.up * Time.deltaTime * cameraMoveSpeed * 6, Space.World);
                transform.Translate(Vector3.back * Time.deltaTime * cameraMoveSpeed * 3.5f, Space.Self);
            }

            //camera rotation
			if(Input.GetMouseButtonDown(2))
			{
				mouseOrigin = Input.mousePosition;
				isRotating = true;
			}
			if (!Input.GetMouseButton(2)) isRotating=false;
			if (isRotating)
			{
		        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
				transform.parent.gameObject.transform.RotateAround(transform.position, Vector3.up, pos.x * 4);	//left-right
				gameObject.transform.RotateAround(transform.position, transform.right, -pos.y * 4); //up-down
			}
			if(Input.GetKeyDown("r"))
			{
				gameObject.transform.localRotation=savedRotation;
				gameObject.transform.parent.gameObject.transform.rotation=savedRotation1;
			}
        }//if !GamePaused
        else if (triggerCamPos)
        {
            transform.parent.gameObject.transform.position = Vector3.Lerp(transform.parent.gameObject.transform.position, newCamPos, 2 * Time.deltaTime);
            //check distance
            if ((transform.parent.gameObject.transform.position - newCamPos).sqrMagnitude < 10)
            {
                triggerCamPos = false;
            }
        }

        // check if camera is being moved. 
        if (savedCamPos != transform.position || savedCamRot != transform.localRotation)
        {
            savedCamPos = transform.position;
            savedCamRot = transform.localRotation;
            cameraMoving = true;
        }
        else
        {
            cameraMoving = false;
        }


    } //void Update()
}
