using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Tools
{
    //[MenuItem("Tools/Assign Tile Material")]
    public static void AssignTileMaterial()
    {
        DoMaterialAssignment("EnemyTile");
        DoMaterialAssignment("WallTile");
        DoMaterialAssignment("ResourceTile");
        DoMaterialAssignment("RepairTile");
        DoMaterialAssignment("BaseTile");
        DoMaterialAssignment("Wall");
    }

    private static void DoMaterialAssignment(string tileName)
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag(tileName);
        Material material = Resources.Load<Material>("Tile/" + tileName.Replace("Tile", "Material"));

        foreach (var t in tiles)
        {
            t.GetComponent<Renderer>().material = material;
        }
    }
}
