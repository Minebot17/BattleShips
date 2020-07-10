using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class ShipEditor : MonoBehaviour {
    public static ShipEditor singleton;
    public Text timerText;
    public int timeBeforeClosing = 30;
    public ModulesScrollAdapter scrollAdapter;
    
    [SerializeField] private Camera mainCamera;
    [SerializeField] private int maxModules = 4;
    [SerializeField] private Vector2Int gridSize = new Vector2Int(9, 9);
    [SerializeField] private Text blocksLeftText;
    [SerializeField] private GameObject readyButton;
    [SerializeField] private GameObject gridBorders;
    [SerializeField] private GameObject modulePlaces;
    [SerializeField] private GameObject modulePlaceFramePrefab;
    
    private GameObject currentShip;
    private EditorModule[] modules;
    private float closingTimer;
    private bool timerStarted;
    private int installedModules;
    private HashSet<Vector2Int> freePlaces = new HashSet<Vector2Int>();
    private readonly Vector2Int[] neighborBypassOrder = { 
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1)
    };

    public void Start() {
        singleton = this;
        if (NetworkManagerCustom.singleton.IsServer)
            SetTimer(timeBeforeClosing);
        else
            new TimerInEditorMessage().SendToServer();

        gridBorders.GetComponent<LineRenderer>().SetPositions(new [] {
            new Vector3(gridSize.x/2 + 0.5f, gridSize.y/2 + 0.5f) * Utils.sizeOfOne,
            new Vector3(-gridSize.x/2 - 0.5f, gridSize.y/2 + 0.5f) * Utils.sizeOfOne,
            new Vector3(-gridSize.x/2 - 0.5f, -gridSize.y/2 - 0.5f) * Utils.sizeOfOne,
            new Vector3(gridSize.x/2 + 0.5f, -gridSize.y/2 - 0.5f) * Utils.sizeOfOne
        });
        
        modules = Resources.LoadAll<EditorModule>("EditorModules/");
        scrollAdapter.SetModules(modules);
        scrollAdapter.onModuleSelect = UpdateFreePlaces;
        OpenShip(Players.GetClient().GetState<CommonState>().ShipJson.Value);
    }

    public void SetTimer(int seconds) {
        closingTimer = seconds;
        timerText.text = (int) Math.Ceiling(closingTimer) + ""; 
        timerText.enabled = true;
        timerStarted = true;
    }

    public void Update() {
        if (!timerStarted)
            return;
        
        if (closingTimer > 0)
            closingTimer -= Time.deltaTime;
        else {
            new SendShipServerMessage(Utils.SerializeShip(currentShip)).SendToServer();
            timerStarted = false;
        }

        timerText.text = (int) Math.Ceiling(closingTimer) + ""; 
    }

    public void OpenShip(string json) {
        currentShip = Utils.DeserializeShipFromJson(json);
    }

    public void OnConstructorClick() {
        Vector2Int position = GetClickPosition();
        GameObject shipCell = FindShipCell(position);

        if (scrollAdapter.selectedModule == null 
            || scrollAdapter.selectedModule.Equals("") 
            || installedModules > maxModules
            || position.x == 0 && position.y == 0
            || position.x < -gridSize.x/2 || position.x > gridSize.x/2
            || position.y < -gridSize.y/2 || position.y > gridSize.y/2
            || !freePlaces.Contains(position))
            return;
            
        string[] splittedName = scrollAdapter.selectedModule.Split(' ');
        if (shipCell) {
            GameObject oldModule = shipCell.transform.GetChild(0).gameObject;
            if (!oldModule.name.Equals(splittedName[0]))
                DestroyImmediate(shipCell);
            else 
                return;
        }
        
        GameObject cell = Instantiate(Resources.Load<GameObject>("Prefabs/ShipCell"), currentShip.transform);
        cell.name = "ShipCell " + position.x + " " + position.y;
        cell.transform.localPosition = GridPosToWorldPos(position);
        GameObject module = Instantiate(Resources.Load<GameObject>("Prefabs/Modules/" + splittedName[0]), cell.transform);
        module.name = modules[int.Parse(splittedName[1])].name;
        module.transform.localPosition = new Vector3(0, 0, -0.1f);
        installedModules++;
        blocksLeftText.text = (maxModules - installedModules) + " blocks left";
        if (installedModules == maxModules)
            OnReadyClick();
        
        UpdateFreePlaces();
    }

    private void UpdateFreePlaces() {
        foreach (Transform child in modulePlaces.transform)
            Destroy(child.gameObject);

        if (installedModules == maxModules)
            return;
        
        freePlaces = FindFreePlaces();
        foreach (Vector2Int freePlace in freePlaces) {
            // TODO пересоздание сетки изрядно нагружает CPU, надо реализовать через буффер объектов
            GameObject modulePlaceFrame = Instantiate(modulePlaceFramePrefab, modulePlaces.transform); 
            modulePlaceFrame.name = "ModulePlaceFrame";
            modulePlaceFrame.transform.localPosition = GridPosToWorldPos(freePlace);
        }
    }

    private Vector2Int GetClickPosition() {
        Vector3 p = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        p = new Vector3(p.x/Utils.sizeOfOne - 0.5f*(p.x > 0 ? -1 : 1), p.y/Utils.sizeOfOne - 0.5f*(p.y > 0 ? -1 : 1), 0f);
        Vector2Int position = new Vector2Int(Utils.RoundSinged(p.x), Utils.RoundSinged(p.y));
        return position;
    }

    private HashSet<Vector2Int> FindFreePlaces() {
        HashSet<Vector2Int> freePlaces = new HashSet<Vector2Int>();
        for (int i = 0; i < currentShip.transform.childCount; i++) {
            GameObject cell = currentShip.transform.GetChild(i).gameObject;
            if (!cell.name.StartsWith("ShipCell"))
                continue;
            
            Vector2Int cellCoords = GetCellCoords(cell);
            GameObject[] neighbors = FindNeighbors(cellCoords);

            for (int j = 0; j < neighborBypassOrder.Length; j++) {
                Vector2Int coords = neighborBypassOrder[j] + cellCoords;
                if (!neighbors[j] 
                    && coords.x >= -gridSize.x/2 
                    && coords.x <= gridSize.x/2 
                    && coords.y >= -gridSize.y/2 
                    && coords.y <= gridSize.y/2)
                    freePlaces.Add(coords);
            }
        }

        // check for engines
        freePlaces.RemoveWhere(freePlace => {
            GameObject upperNeighbor = FindShipCell(freePlace + Vector2Int.up);
            return upperNeighbor && upperNeighbor.transform.GetChild(0).name.StartsWith("EngineModule");
        });

        return freePlaces;
    }

    private GameObject[] FindNeighbors(Vector2Int pos) {
        GameObject[] neighbors = new GameObject[neighborBypassOrder.Length];
        for (int i = 0; i < neighborBypassOrder.Length; i++)
            neighbors[i] = FindShipCell(pos + neighborBypassOrder[i]);

        return neighbors;
    }

    private GameObject FindShipCell(Vector2Int pos) {
        return currentShip.transform.Find("ShipCell " + pos.x + " " + pos.y)?.gameObject;
    }

    private Vector2Int GetCellCoords(GameObject cell) {
        string[] splittedCoords = cell.name.Substring(9).Split(' ');
        return new Vector2Int(int.Parse(splittedCoords[0]), int.Parse(splittedCoords[1]));
    }

    private Vector3 GridPosToWorldPos(Vector2Int gridPos) {
        return new Vector3(gridPos.x * Utils.sizeOfOne, gridPos.y * Utils.sizeOfOne, 0);
    }

    public void OnReadyClick() {
        new SendShipServerMessage(Utils.SerializeShip(currentShip)).SendToServer();
        Destroy(readyButton.GetComponent<Button>());
        readyButton.transform.GetChild(0).GetComponent<Text>().text = "Waiting...";
        blocksLeftText.enabled = false;
        timerText.enabled = false;
        timerStarted = false;
    }
}
