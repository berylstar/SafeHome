using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BuildData", menuName = "Scriptable/Build Data")]
public class BuildDataSO : GridScriptableObject
{
    public GameObject StructurePrefab;
    public override string GetDataInfo()
    {
        return StructurePrefab.GetComponent<TurretAIBase>().GetTurretInfo();
    }

    public override string GetName()
    {
        return StructurePrefab.GetComponent<TurretAIBase>().GetTurretName();
    }
}
