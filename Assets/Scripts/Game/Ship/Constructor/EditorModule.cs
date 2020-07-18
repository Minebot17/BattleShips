using UnityEngine;

[CreateAssetMenu(fileName = "EditorModule", menuName = "ScriptableObjects/EditorModule", order = 1)]
public class EditorModule : ScriptableObject {
    public GameObject prefab;
    public bool isWeapon;
    public bool availableInitially;
    public bool endlessModule = true;
    public int startAmount = 1;
}
