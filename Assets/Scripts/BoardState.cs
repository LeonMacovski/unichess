using System;
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
        checkedKings.ForEach(c => Debug.Log(c));
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
                    int kI = kingList[k].Item1;
                    int kJ = kingList[k].Item2;

                    if (curr == PieceType.NONE)
                        continue;

                    else if (curr == PieceType.PAWN)
                    {
                        if (kI == i - 1 && (kingList[k].Item2 == j - 1 || kJ == j + 1))
                        {
                            shouldAdd = true;
                            break;
                        }
                    }

                    else if (curr == PieceType.KNIGHT)
                    { 
                        if (kJ == j + 2 && kI == i + 1)
                        {
                            shouldAdd = true;
                            break;
                        }
                        if (kJ == j + 2 && kI == i - 1)
                        {
                            shouldAdd = true;
                            break;
                        }
                        if (kJ == j - 2 && kI == i + 1)
                        {
                            shouldAdd = true;
                            break;
                        }
                        if (kJ == j - 2 && kI == i - 1)
                        {
                            shouldAdd = true;
                            break;
                        }
                        if (kJ == j + 1 && kI == i + 2)
                        {
                            shouldAdd = true;
                            break;
                        }
                        if (kJ == j - 1 && kI == i + 2)
                        {
                            shouldAdd = true;
                            break;
                        }
                        if (kJ == j + 1 && kI == i - 2)
                        {
                            shouldAdd = true;
                            break;
                        }
                        if (kJ == j - 1 && kI == i - 2)
                        {
                            shouldAdd = true;
                            break;
                        }
                    }

                    else if (curr == PieceType.BISHOP)
                    {
                        int tempI = i;
                        int tempKi = kI;
                        int tempJ = j;
                        int tempKj = kJ;

                        if (Math.Abs(i - kI) != Math.Abs(j - kJ))
                            continue;
                        if (tempI > tempKi)
                        {
                            (tempI, tempKi) = (tempKi, tempI);
                            (tempJ, tempKj) = (tempKj, tempJ);
                        }

                        if(tempKj > tempJ)
                            for (int i1 = tempI + 1, j1 = tempJ + 1; i1 <= tempKi && j1 <= tempKj; i1++, j1++)
                            {
                                if (state[i1].colData[j1] == PieceType.KING || (i1, j1) == (i, j))
                                {
                                    shouldAdd = true;
                                    break;
                                }
                                else if (state[i1].colData[j1] != PieceType.NONE)
                                    break;
                            }
                                    
                        else
                            for (int i2 = tempI + 1, j2 = tempJ - 1; i2 <= tempKi && j2 >= tempKj; i2++, j2--)
                            {
                                if (state[i2].colData[j2] == PieceType.KING || (i2, j2) == (i, j))
                                {
                                    shouldAdd = true;
                                    break;
                                }
                                else if (state[i2].colData[j2] != PieceType.NONE)
                                    break;
                            }
                    }

                    else if (curr == PieceType.ROOK)
                    {
                        int tempI = i;
                        int tempKi = kI;
                        int tempJ = j;
                        int tempKj = kJ;

                        if (i != kI && j != kJ)
                            continue;

                        if (i == kI)
                        {
                            if (tempJ > tempKj)
                                (tempJ, tempKj) = (tempKj, tempJ);

                            for (int j1 = tempJ + 1; j1 <= tempKj; j1++)
                            {
                                if (state[tempI].colData[j1] == PieceType.KING || (tempI, j1) == (i, j))
                                {
                                    shouldAdd = true;
                                    break;
                                }
                                else if (state[tempI].colData[j1] != PieceType.NONE)
                                    break;
                            }
                        }

                        else if (j == kJ)
                        {
                            if (tempI > tempKi)
                                (tempI, tempKi) = (tempKi, tempI);

                            for (int i1 = tempI + 1; i1 <= tempKi; i1++)
                            {
                                if (state[i1].colData[tempJ] == PieceType.KING || (i1, tempJ) == (i, j))
                                {
                                    shouldAdd = true;
                                    break;
                                }
                                else if (state[i1].colData[tempJ] != PieceType.NONE)
                                    break;
                            }
                        }
                    }

                    else if(curr == PieceType.QUEEN)
                    {
                        int tempI = i;
                        int tempKi = kI;
                        int tempJ = j;
                        int tempKj = kJ;

                        if (Math.Abs(i - kI) != Math.Abs(j - kJ) && kJ != j && kI != i)
                            continue;

                        if (i == kI)
                        {
                            if (tempJ > tempKj)
                                (tempJ, tempKj) = (tempKj, tempJ);

                            for (int j1 = tempJ + 1; j1 <= tempKj; j1++)
                            {
                                if (state[tempI].colData[j1] == PieceType.KING || (tempI, j1) == (i, j))
                                {
                                    shouldAdd = true;
                                    break;
                                }
                                else if (state[tempI].colData[j1] != PieceType.NONE)
                                    break;
                            }
                        }

                        else if (j == kJ)
                        {
                            if (tempI > tempKi)
                                (tempI, tempKi) = (tempKi, tempI);

                            for (int i1 = tempI + 1; i1 <= tempKi; i1++)
                            {
                                if (state[i1].colData[tempJ] == PieceType.KING || (i1, tempJ) == (i, j))
                                {
                                    shouldAdd = true;
                                    break;
                                }
                                else if (state[i1].colData[tempJ] != PieceType.NONE)
                                    break;
                            }
                        }

                        else
                        {
                            if (tempI > tempKi)
                            {
                                (tempI, tempKi) = (tempKi, tempI);
                                (tempJ, tempKj) = (tempKj, tempJ);
                            }

                            if (tempKj > tempJ)
                                for (int i1 = tempI + 1, j1 = tempJ + 1; i1 <= tempKi && j1 <= tempKj; i1++, j1++)
                                {
                                    if (state[i1].colData[j1] == PieceType.KING || (i1, j1) == (i, j))
                                    {
                                        shouldAdd = true;
                                        break;
                                    }
                                    else if (state[i1].colData[j1] != PieceType.NONE)
                                        break;
                                }

                            else
                                for (int i2 = tempI + 1, j2 = tempJ - 1; i2 <= tempKi && j2 >= tempKj; i2++, j2--)
                                {
                                    if (state[i2].colData[j2] == PieceType.KING || (i2, j2) == (i, j))
                                    {
                                        shouldAdd = true;
                                        break;
                                    }
                                    else if (state[i2].colData[j2] != PieceType.NONE)
                                        break;
                                }
                        }
                    }

                    else if (curr == PieceType.KING)
                    {
                        if (kI == i + 1 && kJ == j)
                        {
                            shouldAdd = true;
                            break;
                        }
                        if (kI == i - 1 && kJ == j)
                        {
                            shouldAdd = true;
                            break;
                        }
                        if (kJ == j + 1 && kI == i)
                        {
                            shouldAdd = true;
                            break;
                        }
                        if (kJ == j - 1 && kI == i)
                        {
                            shouldAdd = true;
                            break;
                        }
                        if (kJ == j - 1 && kI == i - 1)
                        {
                            shouldAdd = true;
                            break;
                        }
                        if (kJ == j - 1 && kI == i + 1)
                        {
                            shouldAdd = true;
                            break;
                        }
                        if (kJ == j + 1 && kI == i - 1)
                        {
                            shouldAdd = true;
                            break;
                        }
                        if (kJ == j + 1 && kI == i + 1)
                        {
                            shouldAdd = true;
                            break;
                        }
                    }
                }

                if (shouldAdd)
                {
                    checkedKingList.Add(kingList[k]);
                    break;
                }
            }
        }


        return checkedKingList;
    }
}
