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
            editorModule.transform.GetChild(0).GetComponent<Image>().sprite =
                contentContainer[i].prefab.GetComponent<SpriteRenderer>().sprite;
            editorModule.transform.GetChild(1).GetComponent<Text>().text = endlessMode || moduleCount == -1 ? "∞" : moduleCount+"";
        }
    }

    public GameObject AddElement() {
        GameObject editorModule = Instantiate(modulePrefab, content);
        moduleBackgrounds.Add(editorModule.GetComponent<Image>());
        editorModule.GetComponent<Button>().onClick.AddListener(() => { OnSelectModuleClick(editorModule); });
        return editorModule;
    }

    public void OnSelectModuleClick(GameObject buttonObject) {
        selectedModule = buttonObject;
        foreach (Image img in moduleBackgrounds) {
            if (!img)
                continue;
            
            Color color = img.color;
            color.a = 0f;
            img.color = color;
        }

        Color currentColor = buttonObject.GetComponent<Image>().color;
        currentColor.a = 1f;
        buttonObject.GetComponent<Image>().color = currentColor;
        onModuleUpdate.Invoke();
    }

    public void OnModulePlaced(EditorModule module, int newCount) {
        if (endlessMode || module.endlessModule)
            return;

        if (newCount == 0) {
            DestroyImmediate(selectedModule);
            onModuleUpdate.Invoke();
            return;
        }

        selectedModule.transform.GetChild(1).GetComponent<Text>().text = newCount + "";
    }
}