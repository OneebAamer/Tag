using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBoostOrb : OrbBehaviour
{
    void OnTriggerEnter (Collider collider) {
        collider.gameObject.GetComponent<PlayerMovement>().jumpHeight = 4f;
        Destroy(this.gameObject);
    }
}
