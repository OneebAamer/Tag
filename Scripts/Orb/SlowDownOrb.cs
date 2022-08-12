using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownOrb : OrbBehaviour
{
    void OnTriggerEnter (Collider collider) {
        collider.gameObject.GetComponent<PlayerData>().playerSpeed = 4f;
        Destroy(this.gameObject);
    }
}
