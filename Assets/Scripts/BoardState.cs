using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BoardState
{
    public RowData[] state;
    public PieceType movedPiced;
    public List<Vector2> checkedKings;

    public BoardState(RowData[] s, PieceType mP)
    {
        state = s;
        movedPiced = mP;
        checkedKings = CalculateChecks();
    }

    private List<Vector2> CalculateChecks()
    {
        if(!state.Any(r => r.HasPieceInCol(PieceType.KING)))
            return new List<Vector2>();

        return new List<Vector2>();
    }
}
