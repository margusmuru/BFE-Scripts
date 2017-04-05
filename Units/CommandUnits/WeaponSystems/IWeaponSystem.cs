using UnityEngine;
using System.Collections;

public interface IWeaponSystem {

    float NextFireTime { get; set; }

    GameObject Target { get; set; }

    GameObject PriorityTarget { get; set; }

    string FireMode { get; set; }

    void Shoot();

}

