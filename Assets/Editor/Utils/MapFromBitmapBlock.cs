using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MapFromBitmapBlock : IUtilBlock {
	
	Dictionary<int, Tuple<string, GameObject>> colorToPrefabName = new Dictionary<int, Tuple<string, GameObject>>() {
		{ 0xFFFFFF, new Tuple<string, GameObject>("MapWall", null) }, 
		{ 0x636363, new Tuple<string, GameObject>("MapBrokenWall", null) },
		{ 0xC60003, new Tuple<string, GameObject>("MapTurret", null) }
	};

	Sprite mapSprite;
	
	
	public void Draw() {
		mapSprite = (Sprite) EditorGUILayout.ObjectField("Map bitmap as sprite", mapSprite, typeof(Sprite), false);
		if (GUILayout.Button("Generate map")) {
			int[] keys = colorToPrefabName.Keys.ToArray();
			for (int i = 0; i < keys.Length; i++)
				colorToPrefabName[keys[i]] = new Tuple<string, GameObject>(colorToPrefabName[keys[i]].Item1,
					Resources.Load<GameObject>("MapElements/" + colorToPrefabName[keys[i]].Item1)
				);
			
			Transform parent = new GameObject("Generated map").transform;
			Map map = parent.gameObject.AddComponent<Map>();
			map.size = new Vector2(mapSprite.rect.width * 0.64f, mapSprite.rect.height * 0.64f);
			for(int x = 0; x < mapSprite.rect.width; x++)
				for (int y = 0; y < mapSprite.rect.height; y++) {
					int color = mapSprite.texture.GetPixel(x, y).ToHex();
					if (color != 0) {
						GameObject element = (GameObject) PrefabUtility.InstantiatePrefab(colorToPrefabName[color].Item2);
						element.transform.parent = parent;
						element.transform.localPosition = new Vector3(-map.size.x/2f + 0.32f + x * 0.64f, -map.size.y/2f + 0.32f + y * 0.64f);
					}
				}
		}
	}

	public string GetName() {
		return "MapFromBitmapBlock";
	}
}
