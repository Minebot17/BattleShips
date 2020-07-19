using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class ShipEditor : MonoBehaviour {
    public static ShipEditor singleton;
    public static EditorModule[] modules;
    public Text timerText;
    public int timeBeforeClosing = 30;
    public ModulesScrollAdapter scrollAdapter;
    
    [SerializeField] private Camera mainCamera;
    [SerializeField] private int maxModules;
    [SerializeField] private Vector2Int gridSize = new Vector2Int(9, 9);
    [SerializeField] private Text blocksLeftText;
    [SerializeField] private GameObject readyButton;
    [SerializeField] private GameObject gridBorders;
    [SerializeField] private GameObject moduleInfoPanel;
    [SerializeField] private GameObject modulePlaces;
    [SerializeField] private GameObject modulePlaceFramePrefab;
    [SerializeField] private GameObject shipCellPrefab;
    
    private GameObject currentShip;
    private float closingTimer;
    private bool timerStarted;
    private int installedModules;
    private HashSet<Vector2Int> freePlaces = new HashSet<Vector2Int>();
    private InventoryState iState;
    private Stack<int> placedModules = new Stack<int>();
    private GlobalState global;

    public static void Initialize() {
        modules = Resources.LoadAll<EditorModule>("EditorModules/").Select(m => {
            EditorModule em = Instantiate(m);
            em.name = em.name.Substring(0, em.name.Length - 7);
            return em;
        }).ToArray();
    }

    public void Start() {
        singleton = this;

        global = Players.GetGlobal();
        iState = Players.GetClient().GetState<InventoryState>();
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
        
        scrollAdapter.SetModules(iState, modules);
        scrollAdapter.onModuleUpdate = () => {
            UpdateFreePlaces();
            
            moduleInfoPanel.SetActive(scrollAdapter.SelectedModule != null);
            if (scrollAdapter.SelectedModule == null)
                return;

            string[] splitted = scrollAdapter.SelectedModule.Split(' ');
            moduleInfoPanel.transform.GetChild(0).gameObject.GetComponent<Text>().text = LanguageManager.GetValue( splitted[0] + ".name");
            moduleInfoPanel.transform.GetChild(1).gameObject.GetComponent<Text>().text = LanguageManager.GetValue(splitted[0] + ".description");

            GameObject modulePrefab = modules[int.Parse(splitted[1])].prefab;
            List<string> parameters = new List<string>();

            if (modulePrefab.TryGetComponent(out AbstractGunModule gun)) {
                parameters.Add(LanguageManager.GetValue("moduleParams.coolDown") + ": " 
                               + gun.CoolDown.ToString("#0.##") + " " + LanguageManager.GetValue("moduleParams.seconds"));
                parameters.Add(LanguageManager.GetValue("moduleParams.recoilForce") + ": " 
                               + gun.RecoilForce.ToString("#0.##"));
                parameters.Add(LanguageManager.GetValue("moduleParams.damage") + ": " 
                               + LanguageManager.GetValue("damageRating." + (int) gun.DamageRating));
            }

            if (modulePrefab.TryGetComponent(out ModuleHp hp))
                parameters.Add(LanguageManager.GetValue("moduleParams.health") + ": " + hp.MaxHealth);

            moduleInfoPanel.transform.GetChild(2).gameObject.GetComponent<Text>().text = string.Join("\n", parameters);
        };
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
            new SendShipServerMessage(Utils.SerializeShip(currentShip), placedModules.ToList()).SendToServer();
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

        if (scrollAdapter.SelectedModule == null 
            || scrollAdapter.SelectedModule.Equals("") 
            || installedModules > maxModules
            || position.x == 0 && position.y == 0
            || position.x < -gridSize.x/2 || position.x > gridSize.x/2
            || position.y < -gridSize.y/2 || position.y > gridSize.y/2
            || !shipCell && !freePlaces.Contains(position))
            return;
            
        string[] splittedName = scrollAdapter.SelectedModule.Split(' ');
        int moduleIndex = int.Parse(splittedName[1]);
        if (global.WithLootItems.Value && !modules[moduleIndex].endlessModule && iState.modulesCount[moduleIndex].Value == 0)
            return;
        
        if (shipCell) {
            GameObject oldModule = shipCell.transform.GetChild(0).gameObject;
            if (!oldModule.name.Equals(splittedName[0]))
                DestroyImmediate(shipCell);
            else 
                return;
        }
        
        placedModules.Push(moduleIndex);
        if (global.WithLootItems.Value && iState.modulesCount[moduleIndex].Value != -1)
            iState.modulesCount[moduleIndex].Value--;
        
        EditorModule editorModule = modules[moduleIndex];
        GameObject cell = Instantiate(shipCellPrefab, currentShip.transform);
        cell.name = "ShipCell " + position.x + " " + position.y;
        cell.transform.localPosition = GridPosToWorldPos(position);
        GameObject module = Instantiate(editorModule.prefab, cell.transform);
        module.name = editorModule.name;
        module.transform.localPosition = new Vector3(0, 0, -0.1f);
        installedModules++;
        blocksLeftText.text = (maxModules - installedModules) + " blocks left";
        scrollAdapter.OnModulePlaced(editorModule, iState.modulesCount[moduleIndex].Value);
        if (installedModules == maxModules)
            OnReadyClick();
        
        UpdateFreePlaces();
    }

    private void UpdateFreePlaces() {
        foreach (Transform child in modulePlaces.transform)
            Destroy(child.gameObject);

        if (installedModules == maxModules || scrollAdapter.SelectedModule == null)
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

            for (int j = 0; j < Utils.neighborBypassOrder.Length; j++) {
                Vector2Int coords = Utils.neighborBypassOrder[j] + cellCoords;
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

        if (scrollAdapter.SelectedModule.StartsWith("EngineModule"))
            freePlaces.RemoveWhere(freePlace => {
                GameObject bottomNeighbor = FindShipCell(freePlace + Vector2Int.down);
                return bottomNeighbor;
            });

        return freePlaces;
    }

    private GameObject[] FindNeighbors(Vector2Int pos) {
        GameObject[] neighbors = new GameObject[Utils.neighborBypassOrder.Length];
        for (int i = 0; i < Utils.neighborBypassOrder.Length; i++)
            neighbors[i] = FindShipCell(pos + Utils.neighborBypassOrder[i]);

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
        new SendShipServerMessage(Utils.SerializeShip(currentShip), placedModules.ToList()).SendToServer();
        Destroy(readyButton.GetComponent<Button>());
        readyButton.transform.GetChild(0).GetComponent<Text>().text = "Waiting...";
        blocksLeftText.enabled = false;
        timerText.enabled = false;
        timerStarted = false;
    }
}
