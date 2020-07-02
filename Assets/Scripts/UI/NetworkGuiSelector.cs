using UnityEngine;

public class NetworkGuiSelector : MonoBehaviour {
    private void OnGUI() {
        if (GUILayout.Button("Math services")) {
            Destroy(this);
            gameObject.AddComponent<NetworkGuiMatch>();
        }

        if (GUILayout.Button("Direct")) {
            Destroy(this);
            gameObject.AddComponent<NetworkGuiDirect>();
        }
    }
}
