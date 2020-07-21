using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EnemyPointer : MonoBehaviour {

    public GameObject Target;
    
    [SerializeField] private RectTransform rect;
    [SerializeField] private GameObject camera;
    [SerializeField] private Image image;
    [SerializeField] private CommonState cState;

    private void Start() {
        camera = CameraFollower.singleton.gameObject;
        cState = Players.GetPlayer(Target.GetComponent<NetworkIdentity>()).GetState<CommonState>();
    }

    private void Update() {
        if (!Target)
            return;
        
        Vector2 position = Target.transform.position - camera.transform.position;
        rect.localEulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(position.y, position.x));
        image.enabled = position.magnitude > 10;
        if (cState != null && image.enabled)
            image.enabled = !cState.IsInvisible.Value;
    }
}
