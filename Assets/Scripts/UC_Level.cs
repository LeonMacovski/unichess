using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "UC/New Level")]
public class UC_Level : ScriptableObject
{
    public int dimension;
    public RowData[] rowData;

    public void ClearData()
    {
        for (int i = 0; i < dimension; i++)
            rowData[i].colData = new PieceType[dimension];
    }
}

[System.Serializable]
public struct RowData
{
    public PieceType[] colData;
}

