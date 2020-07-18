using UnityEngine;

public class Rotator : MonoBehaviour {
    
    [SerializeField] private float rotateSpeed = 1;
    [SerializeField] private bool clockwise;
    [SerializeField] private bool onlyServer = true;

    public void FixedUpdate() {
        if (onlyServer && !NetworkManagerCustom.singleton.IsServer)
            return;

        if (rotateSpeed != 0)
            transform.localEulerAngles += new Vector3(0, 0, rotateSpeed * (clockwise ? 1 : -1));
    }
}
