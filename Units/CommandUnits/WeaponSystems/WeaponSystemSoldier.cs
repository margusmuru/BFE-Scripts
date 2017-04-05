using UnityEngine;
using System.Collections;
using System;

public class WeaponSystemSoldier : WeaponSystem, IWeaponSystem
{
    private RaycastHit hit;
    //sub emitters
    public GameObject BloodEmitter;
    public GameObject DustEmitter;

    public void Shoot()
    {
        if (Target)
        {
            if (Physics.Raycast(muzzlePos.transform.position, muzzlePos.transform.forward, out hit, weaponRange + 10, targetLayerMask))
            {
                if (hit.collider.gameObject == Target)
                {
                    NextFireTime = Time.time + reloadTime;
                    StartCoroutine(ShootSequence());
                }
                else
                {
                    NextFireTime = Time.time + reloadTime / 2;
                }
            }
        }
    }

    IEnumerator ShootSequence()
    {
        bool shotHit = false;
        float _curAccuracy = wepInaccuracy;

        muzzlePos.transform.localEulerAngles = new Vector3(UnityEngine.Random.Range(-_curAccuracy, _curAccuracy), 
            UnityEngine.Random.Range(-_curAccuracy, _curAccuracy), 0);
        if (Physics.Raycast(muzzlePos.transform.position, muzzlePos.transform.forward, out hit, 100, targetLayerMask))
        {
            if (hit.collider.gameObject == Target)
            {
                if (Target.name == "SoldierBody")
                {
                    BloodEmitter.SetActive(true);
                    DustEmitter.SetActive(false);
                }
                else
                {
                    BloodEmitter.SetActive(false);
                    DustEmitter.SetActive(true);
                }
                shotHit = true;
            }
            else
            {
                BloodEmitter.SetActive(false);
                DustEmitter.SetActive(true);
            }
        }

        effectSys.Play();
        PlaySound();

        yield return new WaitForSeconds(0.25f);
        if (Target && shotHit)
        {
            SetDamageValue(Target.name);
            //Target.transform.parent.gameObject.SendMessage("TakeDamage", weaponDam, SendMessageOptions.DontRequireReceiver);
            Target.transform.parent.gameObject.GetComponent<IDamageable>().TakeDamage(weaponDam);
        }
        yield return new WaitForSeconds(0.25f);
        effectSys.Stop();
        muzzlePos.transform.localEulerAngles = new Vector3(0, 0, 0);
    }
}
