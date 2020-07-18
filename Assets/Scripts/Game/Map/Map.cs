using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

public class Map : MonoBehaviour {

    public static Map singleton;
    public static Dictionary<string, GameObject> mapElements;
    public static GameObject lootBoxPrefab;
    public static GameObject lootItemPrefab;
    public Vector2 size;
    private const BindingFlags flag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    public void Start() {
        singleton = this;
        GameObject parent = new GameObject("Borders");
        
        GameObject left = new GameObject("Left");
        left.transform.parent = parent.transform;
        left.transform.localPosition = new Vector3(-size.x/2f, 0, 0);
        left.AddComponent<BoxCollider2D>().size = new Vector2(0.01f, size.y);
        left.tag = "TransparentForBullets";
        CameraFollower.singleton.leftBorder = left.GetComponent<BoxCollider2D>();
        
        GameObject right = new GameObject("Right");
        right.transform.parent = parent.transform;
        right.transform.localPosition = new Vector3(size.x/2f, 0, 0);
        right.AddComponent<BoxCollider2D>().size = new Vector2(0.01f, size.y);
        right.tag = "TransparentForBullets";
        CameraFollower.singleton.rightBorder = right.GetComponent<BoxCollider2D>();
        
        GameObject top = new GameObject("Top");
        top.transform.parent = parent.transform;
        top.transform.localPosition = new Vector3(0, size.x/2f, 0);
        top.AddComponent<BoxCollider2D>().size = new Vector2(size.x, 0.01f);
        top.tag = "TransparentForBullets";
        CameraFollower.singleton.topBorder = top.GetComponent<BoxCollider2D>();
        
        GameObject bottom = new GameObject("Bottom");
        bottom.transform.parent = parent.transform;
        bottom.transform.localPosition = new Vector3(0, -size.x/2f, 0);
        bottom.AddComponent<BoxCollider2D>().size = new Vector2(size.x, 0.01f);
        bottom.tag = "TransparentForBullets";
        CameraFollower.singleton.bottomBorder = bottom.GetComponent<BoxCollider2D>();
    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position + new Vector3(-size.x/2f, -size.y/2f, 0), transform.position + new Vector3(size.x/2f, -size.y/2f, 0));
        Gizmos.DrawLine(transform.position + new Vector3(size.x/2f, -size.y/2f, 0), transform.position + new Vector3(size.x/2f, size.y/2f, 0));
        Gizmos.DrawLine(transform.position + new Vector3(size.x/2f, size.y/2f, 0), transform.position + new Vector3(-size.x/2f, size.y/2f, 0));
        Gizmos.DrawLine(transform.position + new Vector3(-size.x/2f, size.y/2f, 0), transform.position + new Vector3(-size.x/2f, -size.y/2f, 0));
    }

    public static void Initialize() {
        if (mapElements != null) 
            return;
        
        GameObject[] elements = Resources.LoadAll<GameObject>("MapElements/");
        mapElements = elements.ToDictionary(e => e.name, e => e);
        lootBoxPrefab = elements.First(p => p.name.Equals("LootBox"));
        lootItemPrefab = elements.First(p => p.name.Equals("LootItem"));
    }
    
    public static void SpawnLootBox(Vector2 position, string moduleName) {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;

        GameObject lootBox = Instantiate(lootBoxPrefab);
        lootBox.name = "LootBox_" + moduleName;
        lootBox.transform.position = position.ToVector3(-0.05f);
        NetworkServer.Spawn(lootBox);
    }

    /// <summary>
    /// Спавнит карту и синхронизирует ее. Вызывать только на сервере
    /// </summary>
    /// <param name="mapName">Название префаба в папке Maps в ресурсах</param>
    public static void SpawnMap(string mapName) {
        GameObject mapPrefab = Resources.Load<GameObject>("Maps/" + mapName);
        Instantiate(mapPrefab.transform.Find("SpawnPoints").gameObject).name = "SpawnPoints";
        GameObject mapObj = new GameObject(mapPrefab.name);
        Map map = mapObj.AddComponent<Map>();
        map.size = mapPrefab.GetComponent<Map>().size;
        
        if (Players.GetGlobal().WithLootItems.Value)
            Instantiate(mapPrefab.transform.Find("LootBoxes").gameObject).name = "LootBoxes";

        // element name, position, localEulerAngles
        List<(string, Vector3, Vector3)> objectsWithoutNI = new List<(string, Vector3, Vector3)>();
        for (int i = 0; i < mapPrefab.transform.childCount; i++) {
            GameObject child = mapPrefab.transform.GetChild(i).gameObject;
            if (!mapElements.ContainsKey(child.name))
                continue;
            
            GameObject element = Instantiate(mapElements[child.name]);
            element.transform.position = child.transform.localPosition;
            element.transform.localEulerAngles = child.transform.localEulerAngles;
            TranslateVariables(child, element);
            if (element.TryGetComponent(out NetworkIdentity identity) && identity != null)
                NetworkServer.Spawn(element);
            else
                objectsWithoutNI.Add((child.name, element.transform.position, element.transform.localEulerAngles));
        }
        new CreateMapClientMessage(mapName, map.size, objectsWithoutNI).SendToAllClientExceptHost();
    }

    private static void TranslateVariables(GameObject from, GameObject to) {
        MonoBehaviour[] components = from.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour component in components) {
            Type componentType = component.GetType();
            if (!componentType.IsDefined(typeof(MapElementAttribute), false))
                continue;

            FieldInfo[] fieldsInfo = componentType.GetFields(flag).Where(f => f.IsDefined(typeof(MapElementAttribute), false)).ToArray();
            if (fieldsInfo.Length == 0)
                continue;

            Component toComponent = to.GetComponent(component.GetType());
            Type toComponentType = toComponent.GetType();
            foreach (FieldInfo info in fieldsInfo)
                toComponentType.GetField(info.Name, flag).SetValue(toComponent, info.GetValue(component));
        }
    }
}
