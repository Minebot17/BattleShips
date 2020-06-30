using System;
using UnityEngine;

public class VelocityRotate : MonoBehaviour {

    private Rigidbody2D rigidbody2D;

    private void Start() {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        transform.localEulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(rigidbody2D.velocity.normalized.y, rigidbody2D.velocity.normalized.x) - 90f);
    }
}
