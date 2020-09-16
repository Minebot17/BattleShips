using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviour {

    public static Map singleton;
    public static Dictionary<string, GameObject> mapElements;
    public static GameObject lootBoxPrefab;
    public static GameObject lootItemPrefab;
    private const BindingFlags flag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    [SerializeField] private List<Vector2> spawnPoints = new List<Vector2>();

    public Vector2 Size;
    public List<Vector2> SpawnPoints => spawnPoints;

    public void Start() {
        singleton = this;
        //DontDestroyOnLoad(gameObject);
        GameObject parent = new GameObject("Borders");
        
        GameObject left = new GameObject("Left");
        left.transform.parent = parent.transform;
        left.transform.localPosition = new Vector3(-Size.x/2f, 0, 0);
        left.AddComponent<BoxCollider2D>().size = new Vector2(0.01f, Size.y);
        left.tag = "TransparentForBullets";
        CameraFollower.singleton.leftBorder = left.GetComponent<BoxCollider2D>();
        
        GameObject right = new GameObject("Right");
        right.transform.parent = parent.transform;
        right.transform.localPosition = new Vector3(Size.x/2f, 0, 0);
        right.AddComponent<BoxCollider2D>().size = new Vector2(0.01f, Size.y);
        right.tag = "TransparentForBullets";
        CameraFollower.singleton.rightBorder = right.GetComponent<BoxCollider2D>();
        
        GameObject top = new GameObject("Top");
        top.transform.parent = parent.transform;
        top.transform.localPosition = new Vector3(0, Size.x/2f, 0);
        top.AddComponent<BoxCollider2D>().size = new Vector2(Size.x, 0.01f);
        top.tag = "TransparentForBullets";
        CameraFollower.singleton.topBorder = top.GetComponent<BoxCollider2D>();
        
        GameObject bottom = new GameObject("Bottom");
        bottom.transform.parent = parent.transform;
        bottom.transform.localPosition = new Vector3(0, -Size.x/2f, 0);
        bottom.AddComponent<BoxCollider2D>().size = new Vector2(Size.x, 0.01f);
        bottom.tag = "TransparentForBullets";
        CameraFollower.singleton.bottomBorder = bottom.GetComponent<BoxCollider2D>();
    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position + new Vector3(-Size.x/2f, -Size.y/2f, 0), transform.position + new Vector3(Size.x/2f, -Size.y/2f, 0));
        Gizmos.DrawLine(transform.position + new Vector3(Size.x/2f, -Size.y/2f, 0), transform.position + new Vector3(Size.x/2f, Size.y/2f, 0));
        Gizmos.DrawLine(transform.position + new Vector3(Size.x/2f, Size.y/2f, 0), transform.position + new Vector3(-Size.x/2f, Size.y/2f, 0));
        Gizmos.DrawLine(transform.position + new Vector3(-Size.x/2f, Size.y/2f, 0), transform.position + new Vector3(-Size.x/2f, -Size.y/2f, 0));
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
    public static void SpawnMap(string mapName, bool spawnInEditor = false) {
        GameObject mapPrefab = Resources.Load<GameObject>("Maps/" + mapName);
        Instantiate(mapPrefab.transform.Find("SpawnPoints").gameObject).name = "SpawnPoints";
        GameObject mapObj = new GameObject(mapPrefab.name);
        Map map = mapObj.AddComponent<Map>();
        map.Size = mapPrefab.GetComponent<Map>().Size;
        
        if (Players.GetGlobal().WithLootItems.Value && !spawnInEditor)
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
        
        if (SceneManager.GetActiveScene().name.Equals("ShipEditor"))
            ShipEditor.singleton.MoveShipTo(Players.GetHost().GetState<CommonState>().SpawnPoint.Value);
        
        new CreateMapClientMessage(mapName, map.Size, objectsWithoutNI).SendToAllClientExceptHost();
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
