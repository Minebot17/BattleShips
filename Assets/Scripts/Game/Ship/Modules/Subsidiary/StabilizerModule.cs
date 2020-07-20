using System;
using UnityEngine;

public class StabilizerModule : MonoBehaviour {

    [SerializeField] private float addDrag;
    
    private void OnEnable() {
        Rigidbody2D rigidbody = transform.parent.parent.GetComponent<Rigidbody2D>();
        if (!rigidbody)
            return;
        
        rigidbody.angularDrag += addDrag;
        rigidbody.drag += addDrag/2f;
    }

    private void OnDisable() {
        Rigidbody2D rigidbody = transform.parent.parent.GetComponent<Rigidbody2D>();
        if (!rigidbody)
            return;
        
        rigidbody.angularDrag -= addDrag;
        rigidbody.drag -= addDrag/2f;
    }
}
