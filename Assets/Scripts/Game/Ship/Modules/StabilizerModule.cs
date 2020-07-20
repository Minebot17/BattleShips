using System;
using UnityEngine;

public class StabilizerModule : MonoBehaviour {

    [SerializeField] private float addDrag;
    
    private void Start() {
        Rigidbody2D rigidbody = transform.parent.parent.GetComponent<Rigidbody2D>();
        if (!rigidbody)
            return;
        
        rigidbody.angularDrag += addDrag;
        rigidbody.drag += addDrag/2f;
    }
}
