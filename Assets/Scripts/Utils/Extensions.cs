using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public static class Extensions {
    public static Vector2 ToVector2(this Vector3 vec) {
        return new Vector2(vec.x, vec.y);
    }
    
    public static Vector2 ToVector2(this Vector2Int vec) {
        return new Vector2(vec.x, vec.y);
    }
    
    public static Vector3 ToVector3(this Vector2 vec) {
        return new Vector3(vec.x, vec.y, 0);
    }
    
    public static Vector3 ToVector3(this Vector2 vec, float z) {
        return new Vector3(vec.x, vec.y, z);
    }
    
    public static int RoundSinged(this float number) {
        return (int)(number > 0 ? Mathf.Floor(number) : Mathf.Ceil(number));
    }
    
    public static Color ToColor(this int HexVal) {
        byte R = (byte)((HexVal >> 16) & 0xFF);
        byte G = (byte)((HexVal >> 8) & 0xFF);
        byte B = (byte)((HexVal) & 0xFF);
        return new Color(R, G, B, 255);
    }
    
    public static int ToHex(this Color color) {
        int hex = 0;
        hex += (int)(color.b * 255);
        hex += (int)(color.g * 255) << 8;
        hex += (int)(color.r * 255) << 16;
        return hex;
    }
    
    public static U Get<T, U>(this Dictionary<T, U> dict, T key) where U : class {
        U val;
        dict.TryGetValue(key, out val);
        return val;
    }
    
    public static void MarkServerChange(this Rigidbody2D rigidbody) {
        NetworkSyncVelocity syncVelocity = rigidbody.gameObject.GetComponent<NetworkSyncVelocity>();
        if (!syncVelocity) 
            return;
        
        syncVelocity.LastVelocity = rigidbody.velocity;
        syncVelocity.TargetMarkChangeVelocity(rigidbody.GetComponent<NetworkIdentity>().clientAuthorityOwner, rigidbody.velocity);
    }
    
    public static List<EditorModule> FindTheseModules(this LootSpawnRules rule, EditorModule[] specificList) {
        IEnumerable<EditorModule> result = null;
        
        switch (rule) {
            case LootSpawnRules.ALL:
                result = ShipEditor.modules.ToList();
                break;
            
            case LootSpawnRules.WEAPONS:
                result = ShipEditor.modules.Where(m => m.isWeapon).ToList();
                break;
            
            case LootSpawnRules.SUBSIDIARY:
                result = ShipEditor.modules.Where(m => !m.isWeapon).ToList();
                break;
            
            case LootSpawnRules.USABLE:
                result = ShipEditor.modules.Where(m => m.isUsable).ToList();
                break;
            
            case LootSpawnRules.LIST:
                result = specificList.ToList();
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(rule), rule, null);
        }

        return result.Where(m => !m.availableInitially || !m.endlessModule).ToList();
    }

    public static T GetRandom<T>(this IList<T> collection) {
        int count = collection.Count;
        return collection[Utils.rnd.Next(count)];
    }

    public static void Write(this NetworkWriter writer, Player player) {
        writer.Write(player.Id);
    }
    
    public static Player ReadPlayer(this NetworkReader reader) {
        return Players.GetPlayer(reader.ReadInt32());
    }
}
