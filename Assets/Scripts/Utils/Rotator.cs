using UnityEngine;

public class Rotator : MonoBehaviour {
    
    [SerializeField] private float rotateSpeed = 1;
    [SerializeField] private bool clockwise;

    public void FixedUpdate() {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;

        if (rotateSpeed != 0)
            transform.localEulerAngles += new Vector3(0, 0, rotateSpeed * (clockwise ? 1 : -1));
    }
}
