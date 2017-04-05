using UnityEngine;
using System.Collections;

public class EnemyDummy : MonoBehaviour {

    public GameObject tagObj;
    public Renderer renderer;

	void Start () {
        UnitLocationsManager.Team2Units.Add(tagObj);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void TakeDamage(float enemyDamage)
    {
        //Debug.Log(enemyDamage);
        StartCoroutine(ColChanger());
    }

    IEnumerator ColChanger()
    {
        renderer.material.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        renderer.material.color = Color.white;
    }
}
