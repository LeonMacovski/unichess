using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[System.Serializable]
public enum PieceType
{
    NONE,
    PAWN,
    ROOK,
    KNIGHT,
    BISHOP,
    KING,
    QUEEN
}

public class BoardManager : MonoBehaviour
{
    public Cell cell;
    public Piece piece;
    public Sprite[] pieceSprites;
    public RectTransform parent;
    public int dimensions;
    public Color pieceColor;
    public Color ligher;
    public Color darker;
    public TileData boardData;

    private Cell[,] cells;
    private List<Piece> currentPieces;
    private float cellSize;
    private Vector2 offset;

    [HideInInspector] public Piece selectedPiece;

    public static BoardManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        cells = new Cell[dimensions, dimensions];
        currentPieces = new List<Piece>();
        cellSize = parent.sizeDelta.x / dimensions;
        offset = new Vector2(-650 + (cellSize / 2), 650 - (cellSize / 2));
        GenerateBoard();
        SetBoard();
    }

    private void Update()
    {
        currentPieces.RemoveAll(p => p == null);
        if (currentPieces.Count == 1)
            Debug.Log("You Win");
    }

    public void ClearCell(Cell cell, PieceType type)
    {
        if (cell.transform.childCount > 0)
        {
            Destroy(cell.transform.GetChild(0).gameObject);
        }
        cell.SetPiece(type);
    }

    private void GenerateBoard()
    {
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                cells[i, j] = Instantiate(cell, parent);
                cells[i, j].GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize, cellSize);
                cells[i, j].GetComponent<RectTransform>().anchoredPosition = offset + new Vector2(i * cellSize, -j * cellSize);
                cells[i, j].SetCollider();
                cells[i, j].SetCoordinates(new Vector2(i, j));
                cells[i, j].SetPiece(PieceType.NONE);
                cells[i, j].GetComponent<Image>().color = (j + i) % 2 == 0 ? ligher : darker;
            }
        }
    }

    private void SetBoard()
    {
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                if (boardData.rows[j].pieces[i] != PieceType.NONE)
                {
                    cells[i, j].SetPiece(boardData.rows[j].pieces[i]);
                    currentPieces.Add(Instantiate(piece, cells[i, j].transform));
                    currentPieces[currentPieces.Count - 1].SetPieceType(boardData.rows[j].pieces[i]);
                    currentPieces[currentPieces.Count - 1].GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize, cellSize);
                    currentPieces[currentPieces.Count - 1].GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

                    //currentPieces[currentPieces.Count - 1].SetStartingPos();
                    currentPieces[currentPieces.Count - 1].SetCollider();

                    int spriteIndex = 0;
                    if (boardData.rows[j].pieces[i] == PieceType.PAWN)
                        spriteIndex = 2;
                    if (boardData.rows[j].pieces[i] == PieceType.ROOK)
                        spriteIndex = 0;
                    if (boardData.rows[j].pieces[i] == PieceType.QUEEN)
                        spriteIndex = 1;
                    if (boardData.rows[j].pieces[i] == PieceType.KNIGHT)
                        spriteIndex = 3;
                    if (boardData.rows[j].pieces[i] == PieceType.KING)
                        spriteIndex = 4;
                    if (boardData.rows[j].pieces[i] == PieceType.BISHOP)
                        spriteIndex = 5;

                    currentPieces[currentPieces.Count - 1].GetComponent<Image>().sprite = pieceSprites[spriteIndex];
                    currentPieces[currentPieces.Count - 1].GetComponent<Image>().color = pieceColor;
                }
            }
        }
    }

    public List<Cell> GenerateLegalMoves(Piece piece)
    {
        List<Cell> legalMoves = new List<Cell>();

        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                float cI = piece.transform.parent.GetComponent<Cell>().position.x;
                float cJ = piece.transform.parent.GetComponent<Cell>().position.y;

                if (cells[i, j].piece == PieceType.NONE)
                    continue;

                if (piece.type == PieceType.PAWN)
                {
                    if (cJ != j + 1)
                        continue;
                    if (cI != i + 1 && cI != i - 1)
                        continue;
                }

                else if (piece.type == PieceType.ROOK)
                {
                    if (cJ != j && cI != i)
                        continue;

                    bool shouldAdd = true;

                    if (cI == i)
                    {
                        if (cJ > j)
                        {
                            for (int j1 = j + 1; j1 < cJ; j1++)
                                if (cells[i, j1].piece != PieceType.NONE)
                                {
                                    shouldAdd = false;
                                    break;
                                }
                        }

                        else if (cJ < j)
                        {
                            for (int j1 = j - 1; j1 > cJ; j1--)
                                if (cells[i, j1].piece != PieceType.NONE)
                                {
                                    shouldAdd = false;
                                    break;
                                }
                        }
                    }

                    else if (cJ == j)
                    {
                        if (cI > i)
                        {
                            for (int i1 = i + 1; i1 < cI; i1++)
                                if (cells[i1, j].piece != PieceType.NONE)
                                {
                                    shouldAdd = false;
                                    break;
                                }
                        }

                        else if (cI < i)
                        {
                            for (int i1 = i - 1; i1 > cI; i1--)
                                if (cells[i1, j].piece != PieceType.NONE)
                                {
                                    shouldAdd = false;
                                    break;
                                }
                        }
                    }


                    if (!shouldAdd)
                        continue;

                }

                else if (piece.type == PieceType.BISHOP)
                {
                    bool shouldMove = true;

                    if (Math.Abs(i - cI) != Math.Abs(j - cJ))
                        continue;

                    for (int i1 = i - 1, j1 = j - 1; i1 > cI && j1 > cJ; i1--, j1--)
                        if (cells[i1, j1].piece != PieceType.NONE)
                        {
                            shouldMove = false;
                            break;
                        }

                    for (int i1 = i + 1, j1 = j + 1; i1 < cI && j1 < cJ; i1++, j1++)
                        if (cells[i1, j1].piece != PieceType.NONE)
                        {
                            shouldMove = false;
                            break;
                        }

                    for (int i1 = i + 1, j1 = j - 1; i1 < cI && j1 > cJ; i1++, j1--)
                        if (cells[i1, j1].piece != PieceType.NONE)
                        {
                            shouldMove = false;
                            break;
                        }

                    for (int i1 = i - 1, j1 = j + 1; i1 > cI && j1 < cJ; i1--, j1++)
                        if (cells[i1, j1].piece != PieceType.NONE)
                        {
                            shouldMove = false;
                            break;
                        }

                    if (!shouldMove)
                        continue;

                }

                else if (piece.type == PieceType.KING)
                {
                    bool shouldMove = false;

                    if (cI == i + 1 && cJ == j)
                        shouldMove = true;
                    else if (cI == i - 1 && cJ == j)
                        shouldMove = true;
                    else if (cJ == j + 1 && cI == i)
                        shouldMove = true;
                    else if (cJ == j - 1 && cI == i)
                        shouldMove = true;
                    else if (cJ == j - 1 && cI == i - 1)
                        shouldMove = true;
                    else if (cJ == j - 1 && cI == i + 1)
                        shouldMove = true;
                    else if (cJ == j + 1 && cI == i - 1)
                        shouldMove = true;
                    else if (cJ == j + 1 && cI == i + 1)
                        shouldMove = true;

                    if (!shouldMove)
                        continue;
                }

                else if (piece.type == PieceType.QUEEN)
                {
                    if (Math.Abs(i - cI) != Math.Abs(j - cJ) && cJ != j && cI != i)
                        continue;

                    bool shouldAdd = true;

                    if (cI == i)
                    {
                        if (cJ > j)
                        {
                            for (int j1 = j + 1; j1 < cJ; j1++)
                                if (cells[i, j1].piece != PieceType.NONE)
                                {
                                    shouldAdd = false;
                                    break;
                                }
                        }

                        else if (cJ < j)
                        {
                            for (int j1 = j - 1; j1 > cJ; j1--)
                                if (cells[i, j1].piece != PieceType.NONE)
                                {
                                    shouldAdd = false;
                                    break;
                                }
                        }
                    }

                    else if (cJ == j)
                    {
                        if (cI > i)
                        {
                            for (int i1 = i + 1; i1 < cI; i1++)
                                if (cells[i1, j].piece != PieceType.NONE)
                                {
                                    shouldAdd = false;
                                    break;
                                }
                        }

                        else if (cI < i)
                        {
                            for (int i1 = i - 1; i1 > cI; i1--)
                                if (cells[i1, j].piece != PieceType.NONE)
                                {
                                    shouldAdd = false;
                                    break;
                                }
                        }
                    }

                    else
                    {
                        for (int i1 = i - 1, j1 = j - 1; i1 > cI && j1 > cJ; i1--, j1--)
                            if (cells[i1, j1].piece != PieceType.NONE)
                            {
                                shouldAdd = false;
                                break;
                            }

                        for (int i1 = i + 1, j1 = j + 1; i1 < cI && j1 < cJ; i1++, j1++)
                            if (cells[i1, j1].piece != PieceType.NONE)
                            {
                                shouldAdd = false;
                                break;
                            }

                        for (int i1 = i + 1, j1 = j - 1; i1 < cI && j1 > cJ; i1++, j1--)
                            if (cells[i1, j1].piece != PieceType.NONE)
                            {
                                shouldAdd = false;
                                break;
                            }

                        for (int i1 = i - 1, j1 = j + 1; i1 > cI && j1 < cJ; i1--, j1++)
                            if (cells[i1, j1].piece != PieceType.NONE)
                            {
                                shouldAdd = false;
                                break;
                            }
                    }

                    if (!shouldAdd)
                        continue;
                }

                else if (piece.type == PieceType.KNIGHT)
                {
                    bool shouldMove = false;
                    if (cJ == j + 2 && cI == i + 1)
                        shouldMove = true;
                    else if (cJ == j + 2 && cI == i - 1)
                        shouldMove = true;
                    else if (cJ == j - 2 && cI == i + 1)
                        shouldMove = true;
                    else if (cJ == j - 2 && cI == i - 1)
                        shouldMove = true;
                    else if (cJ == j + 1 && cI == i + 2)
                        shouldMove = true;
                    else if (cJ == j - 1 && cI == i + 2)
                        shouldMove = true;
                    else if (cJ == j + 1 && cI == i - 2)
                        shouldMove = true;
                    else if (cJ == j - 1 && cI == i - 2)
                        shouldMove = true;

                    if (!shouldMove)
                        continue;
                }

                legalMoves.Add(cells[i, j]);
            }

        }

        return legalMoves;
    }
}
