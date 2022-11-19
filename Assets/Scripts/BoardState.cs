using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BoardState
{
    public RowData[] state;
    public PieceType movedPiced;
    public List<(int, int)> checkedKings;

    public BoardState(RowData[] s, PieceType mP)
    {
        state = s;
        movedPiced = mP;
        checkedKings = CalculateChecks();
    }

    private List<(int, int)> CalculateChecks()
    {
        if (!state.Any(r => r.HasPieceInCol(PieceType.KING)))
            return new List<(int, int)>();

        List<(int, int)> kingList = new List<(int, int)>();
        List<(int, int)> checkedKingList = new List<(int, int)>();

        for (int i = 0; i < state.Length; i++)
            for (int j = 0; j < state[i].colData.Length; j++)
                if (state[i].colData[j] == PieceType.KING)
                    kingList.Add((i, j));

        for (int k = 0; k < kingList.Count; k++)
        {
            for (int i = 0; i < state.Length; i++)
            {
                bool shouldAdd = false;
                for (int j = 0; j < state[i].colData.Length; j++)
                {
                    PieceType curr = state[i].colData[j];

                    if (curr == PieceType.NONE)
                        continue;

                    if (curr == PieceType.PAWN)
                        if (kingList[k].Item1 == i - 1 && (kingList[k].Item2 == j - 1 || kingList[k].Item2 == j + 1))
                        {
                            shouldAdd = true;
                            break;
                        }


                }

                if(shouldAdd)
                {
                    Debug.Log(kingList[k]);
                    //checkedKings.Add(kingList[k]);
                    break;
                }
            }
        }

       // checkedKings.ToList().ForEach(c => Debug.Log(c));

        return new List<(int, int)>();
    }
}
