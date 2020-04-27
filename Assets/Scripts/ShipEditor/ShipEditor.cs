using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class ShipEditor : MonoBehaviour {
    public static ShipEditor singleton;
    public Transform editorModules;
    public GameObject editorModulePrefab;
    public Text timerText;
    public int timeBeforeClosing = 30;

    [SerializeField]
    private Camera mainCamera;

    private string selectedModule;
    private GameObject currentShip;
    private EditorModule[] modules;
    private List<Image> moduleBackgrounds = new List<Image>();
    private float closingTimer;
    private bool timerStarted;
    private int installedModules = 0;
    private int maxModules = 5;

    public void Start() {
        singleton = this;
        if (NetworkManagerCustom.singleton.IsServer)
            SetTimer(timeBeforeClosing);
        else
            MessageManager.RequestTimerInEditorServerMessage.SendToServer(new EmptyMessage());

        modules = Resources.LoadAll<EditorModule>("EditorModules/");
        moduleBackgrounds.Add(editorModules.transform.GetChild(0).GetComponent<Image>());
        for (int i = 0; i < modules.Length; i++) {
            GameObject editorModule = Instantiate(editorModulePrefab, editorModules);
            editorModule.name = modules[i].prefab.name + " " + i;
            moduleBackgrounds.Add(editorModule.GetComponent<Image>());
            editorModule.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -120 * i - 140);
            editorModule.transform.GetChild(0).GetComponent<Image>().sprite = modules[i].prefab.GetComponent<SpriteRenderer>().sprite;
            editorModule.GetComponent<Button>().onClick.AddListener(() => { OnSelectModuleClick(editorModule); });
        }

        MessageManager.RequestShipEditorServerMessage.SendToServer(new EmptyMessage());
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
            MessageManager.SendShipServerMessage.SendToServer(new StringMessage(Utils.SerializeShip(currentShip)));
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

        if (selectedModule == null || (position.x == 0 && position.y == 0) || installedModules > maxModules)
            return;

        if (shipCell)
        {
            Destroy(shipCell);
        }
        else if (!shipCell && GetNeighbors(position).Any(go => go)) {
            string[] splittedName = selectedModule.Split(' ');
            GameObject cell = Instantiate(Resources.Load<GameObject>("Prefabs/ShipCell"), currentShip.transform);
            cell.name = "ShipCell " + position.x + " " + position.y;
            cell.transform.localPosition = new Vector3(position.x * Utils.sizeOfOne, position.y * Utils.sizeOfOne, 0);
            GameObject module = Instantiate(Resources.Load<GameObject>("Prefabs/Modules/" + splittedName[0]), cell.transform);
            module.name = modules[int.Parse(splittedName[1])].name;
            module.transform.localPosition = new Vector3(0, 0, -0.1f);
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

    public void OnSelectModuleClick(GameObject buttonObject) {
        selectedModule = buttonObject.name;
        foreach (Image img in moduleBackgrounds) {
            Color color = img.color;
            color.a = 0f;
            img.color = color;
        }

        Color currentColor = buttonObject.GetComponent<Image>().color;
        currentColor.a = 1f;
        buttonObject.GetComponent<Image>().color = currentColor;
    }

    public void OnReadyClick(GameObject buttonObject) {
        MessageManager.SendShipServerMessage.SendToServer(new StringMessage(Utils.SerializeShip(currentShip)));
        Destroy(buttonObject.GetComponent<Button>());
        buttonObject.transform.GetChild(0).GetComponent<Text>().text = "Waiting...";
        timerText.enabled = false;
        timerStarted = false;
    }
}
