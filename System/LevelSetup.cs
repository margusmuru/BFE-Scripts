using UnityEngine;
using System.Collections;

public class LevelSetup : MonoBehaviour {

    public GameObject[] Team1StartLoc;
    public GameObject[] Team2StartLoc;

    public GameObject[] FriendlyStartUnit;
    public GameObject[] EnemyStartUnit;

    void Start()
    {
        if (GameInfo.GameMode == "SinglePlayer")
        {
            SinglePlayerSetup();
        }
        else
        {
            MultiPlayerSetup();
        }
    }

    private void SinglePlayerSetup()
    {
        // choose start locations
        Random.seed = System.DateTime.Now.Second;
        int _locIndex1 = Random.Range(0, Team1StartLoc.Length);
        int _locIndex2 = Random.Range(0, Team2StartLoc.Length);
        GameObject _locPlane1 = UnitLocationsManager.FindLocation(Team1StartLoc[_locIndex1].transform.position, 10000, false);
        GameObject _locPlane2 = UnitLocationsManager.FindLocation(Team2StartLoc[_locIndex2].transform.position, 10000, false);
        
        // spawn start units
        if (_locPlane1 != null && _locPlane2 != null)
        {
            GameObject tmp1 = Instantiate(FriendlyStartUnit[0], _locPlane1.transform.position, Quaternion.identity) as GameObject;
            tmp1.SetActive(true);
            GameObject tmp2 = Instantiate(EnemyStartUnit[0], _locPlane2.transform.position, Quaternion.identity) as GameObject;
            tmp2.SetActive(true);
            // move player camera to the spawned unit
            CameraControls.SetNewCamPos(_locPlane1.transform.position);
        }
        else { Debug.Log("Starting unit spawn error"); }

        GameInfo.LevelReady = true;
    }

    private void MultiPlayerSetup()
    {

    }
}
