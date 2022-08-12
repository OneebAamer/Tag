using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoostOrb : OrbBehaviour
{
    void OnTriggerEnter (Collider collider) {
        collider.gameObject.GetComponent<PlayerData>().playerSpeed = 12f;
        Destroy(this.gameObject);
    }
}
