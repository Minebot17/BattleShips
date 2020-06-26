using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModulesScrollAdapter : MonoBehaviour {
    public RectTransform content;
    public GameObject modulePrefab;
    private readonly List<Image> moduleBackgrounds = new List<Image>();
    public string selectedModule;

    public void SetModules(EditorModule[] contentContainer) {
        foreach (Transform child in content) {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < contentContainer.Length; i++) {
            GameObject editorModule = AddElement(contentContainer[i]);
            editorModule.name = contentContainer[i].prefab.name + " " + i;
            editorModule.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -120 * i - 20);
            editorModule.transform.GetChild(0).GetComponent<Image>().sprite =
                contentContainer[i].prefab.GetComponent<SpriteRenderer>().sprite;
        }
    }

    public GameObject AddElement(EditorModule element) {
        GameObject editorModule = Instantiate(modulePrefab, content);
        moduleBackgrounds.Add(editorModule.GetComponent<Image>());
        editorModule.GetComponent<Button>().onClick.AddListener(() => { OnSelectModuleClick(editorModule); });
        return editorModule;
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
}