using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EnemyPointer : MonoBehaviour {

    public GameObject Target;
    
    [SerializeField]
    private RectTransform rect;
    
    [SerializeField]
    private GameObject camera;
    
    [SerializeField]
    private Image image;
    
    private void Update() {
        if (!Target)
            return;
        
        Vector2 pos = Target.transform.position - camera.transform.position;
        rect.localEulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(pos.y, pos.x));
        image.enabled = pos.magnitude > 10;
    }
}
