using System;
using System.Collections.Generic;
using UnityEngine;

public class LootBoxPoint : MonoBehaviour {
    
    [Range(0f, 1f)]
    public float chance = 1f;
    public LootSpawnRules rule = LootSpawnRules.ALL;
    public EditorModule[] specificList = new EditorModule[0];

    private void Start() {
        if (Utils.rnd.NextDouble() < chance) {
            List<EditorModule> modulesInBox = rule.FindTheseModules(specificList);
            Map.SpawnLootBox(transform.position, modulesInBox[Utils.rnd.Next(modulesInBox.Count)].name);
        }
    }
}
