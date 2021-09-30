using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData
{
    [System.Serializable]
    public struct rowData
    {
        public PieceType[] pieces;
    }

    public rowData[] rows = new rowData[8];

    
}
