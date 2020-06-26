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
    [SerializeField] private int maxModules = 5;
    [SerializeField] private Text blocksLeftText;
    [SerializeField] private GameObject readyButton;

    private GameObject currentShip;
    private EditorModule[] modules;
    private float closingTimer;
    private bool timerStarted;
    private int installedModules;

    public void Start() {
        singleton = this;
        if (NetworkManagerCustom.singleton.IsServer)
            SetTimer(timeBeforeClosing);
        else
            new TimerInEditorMessage().SendToServer();

        modules = Resources.LoadAll<EditorModule>("EditorModules/");
        scrollAdapter.SetModules(modules);

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
        Vector3 p = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        p = new Vector3(p.x/Utils.sizeOfOne - 0.5f*(p.x > 0 ? -1 : 1), p.y/Utils.sizeOfOne - 0.5f*(p.y > 0 ? -1 : 1), 0f);
        Vector2Int position = new Vector2Int(Utils.RoundSinged(p.x), Utils.RoundSinged(p.y));
        GameObject shipCell = FindShipCell(position);

        if (scrollAdapter.selectedModule == null || scrollAdapter.selectedModule.Equals("") || (position.x == 0 && position.y == 0) || installedModules > maxModules)
            return;
        
        if (GetNeighbors(position).Any(go => go)) {
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
            cell.transform.localPosition = new Vector3(position.x * Utils.sizeOfOne, position.y * Utils.sizeOfOne, 0);
            GameObject module = Instantiate(Resources.Load<GameObject>("Prefabs/Modules/" + splittedName[0]), cell.transform);
            module.name = modules[int.Parse(splittedName[1])].name;
            module.transform.localPosition = new Vector3(0, 0, -0.1f);
            installedModules++;
            blocksLeftText.text = (maxModules - installedModules) + " blocks left";
            if (installedModules == maxModules)
                OnReadyClick();
        }
    }

    private GameObject[] GetNeighbors(Vector2Int pos) {
        GameObject[] neighbors = new GameObject[4];
        neighbors[0] = FindShipCell(new Vector2Int(pos.x - 1, pos.y));
        neighbors[1] = FindShipCell(new Vector2Int(pos.x + 1, pos.y));
        neighbors[2] = FindShipCell(new Vector2Int(pos.x, pos.y - 1));
        neighbors[3] = FindShipCell(new Vector2Int(pos.x, pos.y + 1));
        return neighbors;
    }

    private GameObject FindShipCell(Vector2Int pos) {
        return currentShip.transform.Find("ShipCell " + pos.x + " " + pos.y)?.gameObject;
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
