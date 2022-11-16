using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "UC/New Level")]
public class UC_Level : ScriptableObject
{
    public int dimensions;
    public RowData[] rowData;
    public bool doTopPromotion;
    public bool doBottomPromotion;

    public void ClearData()
    {
        for (int i = 0; i < dimensions; i++)
        {
            rowData[i] = new RowData();
            rowData[i].colData = new PieceType[dimensions];
        }
    }
}

[System.Serializable]
public class RowData
{
    public PieceType[] colData;

    public bool HasPieceInCol(PieceType type)
    {
        return colData.Any(p => p == type);
    }
}

