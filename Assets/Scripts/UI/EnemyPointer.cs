using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPointer : MonoBehaviour {

    public GameObject Target;
    
    [SerializeField] RectTransform rect;
    
    [SerializeField] GameObject camera;
    
    [SerializeField] Image image;

    void Start() {
        camera = CameraFollower.singleton.gameObject;
    }

    void Update() {
        if (!Target)
            return;
        
        Vector2 position = Target.transform.position - camera.transform.position;
        rect.localEulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(position.y, position.x));
        image.enabled = position.magnitude > 10;
    }
}
