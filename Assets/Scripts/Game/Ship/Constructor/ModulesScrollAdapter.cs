using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModulesScrollAdapter : MonoBehaviour {
    public RectTransform content;
    public GameObject modulePrefab;
    public Action onModuleUpdate;
    private GameObject selectedModule;
    private readonly List<Image> moduleBackgrounds = new List<Image>();
    private bool endlessMode;
    private Dictionary<EditorModule, GameObject> disabledModules = new Dictionary<EditorModule, GameObject>();

    public string SelectedModule => selectedModule ? selectedModule.name : null;

    public void SetModules(InventoryState iState, EditorModule[] contentContainer) {
        endlessMode = !Players.GetGlobal().WithLootItems.Value;
        foreach (Transform child in content) {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < contentContainer.Length; i++) {
            int moduleCount = iState.modulesCount[i].Value;
            if (!endlessMode && moduleCount == 0)
                continue;
            
            GameObject editorModule = AddElement();
            editorModule.name = contentContainer[i].prefab.name + " " + i;
            editorModule.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -120 * i - 20);
            editorModule.transform.GetChild(2).GetComponent<Image>().sprite =
                contentContainer[i].prefab.GetComponent<SpriteRenderer>().sprite;
            editorModule.transform.GetChild(3).GetComponent<Text>().text = endlessMode || moduleCount == -1 ? "∞" : moduleCount+"";
            editorModule.transform.GetChild(1).GetComponent<Image>().sprite = 
                contentContainer[i].prefab.transform.Find("Bloom").GetComponent<SpriteRenderer>().sprite;
        }
    }

    public GameObject AddElement() {
        GameObject editorModule = Instantiate(modulePrefab, content);
        moduleBackgrounds.Add(editorModule.transform.GetChild(0).GetComponent<Image>());
        editorModule.GetComponent<Button>().onClick.AddListener(() => { OnSelectModuleClick(editorModule); });
        return editorModule;
    }

    public void OnSelectModuleClick(GameObject buttonObject) {
        selectedModule = buttonObject;
        foreach (Image img in moduleBackgrounds) {
            if (!img)
                continue;

            img.enabled = false;
        }

        buttonObject.transform.GetChild(0).GetComponent<Image>().enabled = true;
        onModuleUpdate.Invoke();
    }

    public void OnModulePlaced(EditorModule module, int newCount) {
        if (endlessMode || module.endlessModule)
            return;
        
        GameObject updateModule = null;
        for (int i = 0; i < content.childCount; i++)
            if (content.GetChild(i).name.StartsWith(module.prefab.name))
                updateModule = content.GetChild(i).gameObject;

        if (newCount == 0) {
            disabledModules.Add(module, updateModule);
            updateModule.SetActive(false);
            onModuleUpdate.Invoke();
            return;
        }

        updateModule.transform.GetChild(3).GetComponent<Text>().text = newCount + "";
    }

    public void OnModuleReturned(EditorModule module, int newCount) {
        if (endlessMode || module.endlessModule)
            return;

        if (newCount == 1) {
            disabledModules[module].SetActive(true);
            disabledModules.Remove(module);
            onModuleUpdate.Invoke();
            return;
        }

        GameObject updateModule = null;
        for (int i = 0; i < content.childCount; i++)
            if (content.GetChild(i).name.StartsWith(module.prefab.name))
                updateModule = content.GetChild(i).gameObject;
        
        updateModule.transform.GetChild(3).GetComponent<Text>().text = newCount + "";
    }
}