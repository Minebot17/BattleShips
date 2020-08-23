using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FixSortTexturesBlock : IUtilBlock {

    private DefaultAsset folder;
    
    public void Draw() {
        folder = (DefaultAsset) EditorGUILayout.ObjectField("Folder", folder, typeof(DefaultAsset), false);

        if (!GUILayout.Button("Fix"))
            return;
        
        string sAssetFolderPath = AssetDatabase.GetAssetPath(folder);
        string sFolderPath = Application.dataPath.Substring(0 ,Application.dataPath.Length-6)+sAssetFolderPath;
        string[] filesPath = Directory.GetFiles(sFolderPath);
        
        List<string> sortGroups = new List<string>();
        foreach (string path in filesPath) {
            if (path.Substring(path.Length - 5, 5).Equals(".meta")
                || path.Split('\\').Last().Any(char.IsDigit))
                continue;
            
            sortGroups.Add(path);
        }

        foreach (string sortGroup in sortGroups) {
            string[] sortSplitted = sortGroup.Split('.');
            File.Move(sortGroup, sortSplitted[0] + "-0." + sortSplitted[1]);
            File.Move(sortGroup+".meta", sortSplitted[0] + "-0." + sortSplitted[1] + ".meta");
            
            List<string> filesToSort = new List<string>();
            foreach (string path in filesPath) {
                if (path.Substring(path.Length - 5, 5).Equals(".meta")
                    || !path.StartsWith(sortSplitted[0])
                    || path.Equals(sortGroup))
                    continue;
            
                filesToSort.Add(path);
            }

            filesToSort = filesToSort.OrderBy(p => int.Parse(p.Split('\\').Last().Split('-')[1].Split('.')[0])).ToList();
            for (int i = 0; i < filesToSort.Count; i++) {
                string dest = sortSplitted[0] + "-" + (i + 1) + "." + sortSplitted[1];
                if (filesToSort[i].Equals(dest))
                    continue;
                
                File.Move(filesToSort[i], dest);
                File.Move(filesToSort[i]+".meta", dest + ".meta");
            }
        }
    }

    public string GetName() {
        return "FixSortTexturesBlock";
    }
}