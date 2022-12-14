using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

public class BoardManager : PopupObjectResult
{
    [Header("Setup")]
    public Cell cell;
    public Piece piece;
    public Sprite[] pieceSprites;
    public RectTransform parent;
    public Color pieceColor;
    public Color ligher;
    public Color darker;
    public Image checkIndicator;

    [Header("Board")]
    public UC_Level boardData;


    private List<BoardState> history;
    private List<Piece> currentPieces;
    private Vector2 offset;
    private bool boardSet;
    private List<BoardState> stateChecks;

    public float cellSize { get; private set; }
    public Cell[,] cells { get; private set; }

    public bool running { get; private set; }

    public static BoardManager instance;

    private void Awake()
    {
        instance = this;
    }

    public override void Execute()
    {
        currentPieces = new List<Piece>();
        history = new List<BoardState>();
        stateChecks = new List<BoardState>();
        boardSet = false;
        running = true;
        Init(boardData);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            Execute();

        if (boardSet)
        {
            checkIndicator.gameObject.SetActive(history[^1].hasCheck);
            currentPieces.RemoveAll(p => p == null);
            if (currentPieces.Count == 1)
            {
                HelperScript.instance.DelayedExecution(.4f, () =>
                {
                    currentPieces[0].GetComponent<Piece>().enabled = false;
                    currentPieces[0].Raise();
                });
                boardSet = false;
            }
        }
    }

    public void Init(UC_Level currLevel)
    {
        boardData = currLevel;
        SetValues();
        GenerateBoard();
        StartCoroutine(SetBoard());
    }

    public void ClearCell(Cell cell, Piece piece)
    {
        Destroy(cell.piece.gameObject);
        cell.SetPiece(piece);
    }

    private void SetValues()
    {
        cells = new Cell[boardData.dimensions, boardData.dimensions];
        cellSize = parent.sizeDelta.x / boardData.dimensions;
        offset = new Vector2((-parent.sizeDelta.x + cellSize) / 2, (parent.sizeDelta.x - cellSize) / 2);
    }

    private void GenerateBoard()
    {
        for (int i = 0; i < boardData.dimensions; i++)
        {
            for (int j = 0; j < boardData.dimensions; j++)
            {
                cells[i, j] = Instantiate(cell, parent);
                cells[i, j].GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize, cellSize);
                cells[i, j].GetComponent<RectTransform>().anchoredPosition = offset + new Vector2(i * cellSize, -j * cellSize);
                cells[i, j].SetCollider();
                cells[i, j].SetCoordinates(new Vector2(i, j));
                cells[i, j].SetPiece(null);
                cells[i, j].GetComponent<Image>().color = (j + i) % 2 == 0 ? ligher : darker;
            }
        }
    }

    private IEnumerator SetBoard()
    {
        if (GameObject.FindObjectOfType<UC_LevelCreator>())
        {
            Debug.Log("Detected level creator");
            UC_LevelCreator.instance.FinalizeBoard(cells);
            yield break;
        }
        yield return new WaitForSeconds(.4f);
        ClearBoard();
        StartCoroutine(SetPieces(boardData.rowData, .3f, true));
        history.Add(new BoardState(boardData.rowData, PieceType.NONE));
    }

    private void ClearBoard()
    {
        for (int i = 0; i < boardData.dimensions; i++)
            for (int j = 0; j < boardData.dimensions; j++)
                cells[i, j].SetPiece(null);

        currentPieces.ForEach(p => Destroy(p.gameObject));
        currentPieces = new List<Piece>();
    }

    private IEnumerator SetPieces(RowData[] data, float delay = 0, bool doAnim = false)
    {
        boardSet = false;
        for (int i = 0; i < boardData.dimensions; i++)
            for (int j = 0; j < boardData.dimensions; j++)
                if (data[i].colData[j] != PieceType.NONE)
                {
                    currentPieces.Add(Instantiate(piece, cells[j, i].transform));
                    currentPieces[^1].SetPieceType(data[i].colData[j]);
                    currentPieces[^1].GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize, cellSize);
                    cells[j, i].SetPiece(currentPieces[^1]);
                    if (doAnim)
                        currentPieces[^1].Lower();

                    int spriteIndex = 0;
                    if (data[i].colData[j] == PieceType.PAWN)
                        spriteIndex = 2;
                    if (data[i].colData[j] == PieceType.ROOK)
                        spriteIndex = 0;
                    if (data[i].colData[j] == PieceType.QUEEN)
                        spriteIndex = 1;
                    if (data[i].colData[j] == PieceType.KNIGHT)
                        spriteIndex = 3;
                    if (data[i].colData[j] == PieceType.KING)
                        spriteIndex = 4;
                    if (data[i].colData[j] == PieceType.BISHOP)
                        spriteIndex = 5;

                    currentPieces[^1].GetComponent<Image>().sprite = pieceSprites[spriteIndex];
                    currentPieces[^1].GetComponent<Image>().color = pieceColor;
                    yield return new WaitForSeconds(delay);
                }

        boardSet = true;
    }

    public Sprite GetSpriteByType(PieceType type)
    {
        if (type == PieceType.PAWN)
            return pieceSprites[2];
        if (type == PieceType.ROOK)
            return pieceSprites[0];
        if (type == PieceType.QUEEN)
            return pieceSprites[1];
        if (type == PieceType.KNIGHT)
            return pieceSprites[3];
        if (type == PieceType.KING)
            return pieceSprites[4];
        if (type == PieceType.BISHOP)
            return pieceSprites[5];

        return null;
    }

    public List<Cell> GenerateLegalMoves(Piece piece)
    {
        List<Cell> legalMoves = new List<Cell>();

        for (int i = 0; i < boardData.dimensions; i++)
        {
            for (int j = 0; j < boardData.dimensions; j++)
            {
                float cI = piece.transform.parent.GetComponent<Cell>().position.x;
                float cJ = piece.transform.parent.GetComponent<Cell>().position.y;

                if (i == cI && j == cJ)
                    continue;

                if (cells[i, j].piece == null)
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
                                if (cells[i, j1].piece != null)
                                {
                                    shouldAdd = false;
                                    break;
                                }
                        }

                        else if (cJ < j)
                        {
                            for (int j1 = j - 1; j1 > cJ; j1--)
                                if (cells[i, j1].piece != null)
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
                                if (cells[i1, j].piece != null)
                                {
                                    shouldAdd = false;
                                    break;
                                }
                        }

                        else if (cI < i)
                        {
                            for (int i1 = i - 1; i1 > cI; i1--)
                                if (cells[i1, j].piece != null)
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
                        if (cells[i1, j1].piece != null)
                        {
                            shouldMove = false;
                            break;
                        }

                    for (int i1 = i + 1, j1 = j + 1; i1 < cI && j1 < cJ; i1++, j1++)
                        if (cells[i1, j1].piece != null)
                        {
                            shouldMove = false;
                            break;
                        }

                    for (int i1 = i + 1, j1 = j - 1; i1 < cI && j1 > cJ; i1++, j1--)
                        if (cells[i1, j1].piece != null)
                        {
                            shouldMove = false;
                            break;
                        }

                    for (int i1 = i - 1, j1 = j + 1; i1 > cI && j1 < cJ; i1--, j1++)
                        if (cells[i1, j1].piece != null)
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

                    if (cI == i + 1 && cJ == j && !BordersPiece(i, j, PieceType.KING, ((int)cI, (int)cJ)))
                        shouldMove = true;
                    else if (cI == i - 1 && cJ == j && !BordersPiece(i, j, PieceType.KING, ((int)cI, (int)cJ)))
                        shouldMove = true;
                    else if (cJ == j + 1 && cI == i && !BordersPiece(i, j, PieceType.KING, ((int)cI, (int)cJ)))
                        shouldMove = true;
                    else if (cJ == j - 1 && cI == i && !BordersPiece(i, j, PieceType.KING, ((int)cI, (int)cJ)))
                        shouldMove = true;
                    else if (cJ == j - 1 && cI == i - 1 && !BordersPiece(i, j, PieceType.KING, ((int)cI, (int)cJ)))
                        shouldMove = true;
                    else if (cJ == j - 1 && cI == i + 1 && !BordersPiece(i, j, PieceType.KING, ((int)cI, (int)cJ)))
                        shouldMove = true;
                    else if (cJ == j + 1 && cI == i - 1 && !BordersPiece(i, j, PieceType.KING, ((int)cI, (int)cJ)))
                        shouldMove = true;
                    else if (cJ == j + 1 && cI == i + 1 && !BordersPiece(i, j, PieceType.KING, ((int)cI, (int)cJ)))
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
                                if (cells[i, j1].piece != null)
                                {
                                    shouldAdd = false;
                                    break;
                                }
                        }

                        else if (cJ < j)
                        {
                            for (int j1 = j - 1; j1 > cJ; j1--)
                                if (cells[i, j1].piece != null)
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
                                if (cells[i1, j].piece != null)
                                {
                                    shouldAdd = false;
                                    break;
                                }
                        }

                        else if (cI < i)
                        {
                            for (int i1 = i - 1; i1 > cI; i1--)
                                if (cells[i1, j].piece != null)
                                {
                                    shouldAdd = false;
                                    break;
                                }
                        }
                    }

                    else
                    {
                        for (int i1 = i - 1, j1 = j - 1; i1 > cI && j1 > cJ; i1--, j1--)
                            if (cells[i1, j1].piece != null)
                            {
                                shouldAdd = false;
                                break;
                            }

                        for (int i1 = i + 1, j1 = j + 1; i1 < cI && j1 < cJ; i1++, j1++)
                            if (cells[i1, j1].piece != null)
                            {
                                shouldAdd = false;
                                break;
                            }

                        for (int i1 = i + 1, j1 = j - 1; i1 < cI && j1 > cJ; i1++, j1--)
                            if (cells[i1, j1].piece != null)
                            {
                                shouldAdd = false;
                                break;
                            }

                        for (int i1 = i - 1, j1 = j + 1; i1 > cI && j1 < cJ; i1--, j1++)
                            if (cells[i1, j1].piece != null)
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

                if (history[^1].hasCheck && StateCheck(piece, i, j))
                    continue;

                legalMoves.Add(cells[i, j]);
            }
        }

        return legalMoves;
    }

    private bool StateCheck(Piece piece, int x, int y)
    {
        int cI = (int)piece.transform.parent.GetComponent<Cell>().position.x;
        int cJ = (int)piece.transform.parent.GetComponent<Cell>().position.y;
        RowData[] tempData = new RowData[boardData.dimensions];

        for (int i = 0; i < boardData.dimensions; i++)
        {
            tempData[i] = new RowData();
            tempData[i].colData = new PieceType[boardData.dimensions];
            for (int j = 0; j < boardData.dimensions; j++)
                tempData[i].colData[j] = cells[j, i].piece == null ? PieceType.NONE : cells[j, i].piece.type;
        }

        PieceType targetType = tempData[y].colData[x];

        tempData[cJ].colData[cI] = PieceType.NONE;
        tempData[y].colData[x] = piece.type;

        BoardState tempState = new BoardState(tempData, piece.type, $"{targetType} - {y}:{x}");
        stateChecks.Add(tempState);

        return tempState.hasCheck;
    }

    private bool BordersPiece(int x, int y, PieceType piece, (int, int) ignore)
    {
        bool borders = false;

        for (int i = x - 1; i < x + 2; i++)
        {
            for (int j = y - 1; j < y + 2; j++)
            {
                if ((i, j) == ignore)
                    continue;

                bool hEdge = i < 0 || i >= boardData.dimensions;
                bool vEdge = j < 0 || j >= boardData.dimensions;

                if (hEdge || vEdge)
                    continue;
                if (cells[i, j].piece == null)
                    continue;
                if (cells[i, j].piece.type == piece)
                    borders = true;
            }
        }
        return borders;
    }

    public void RegisterMove(Piece movedPiece)
    {
        RowData[] tempData = new RowData[boardData.dimensions];

        for (int i = 0; i < boardData.dimensions; i++)
        {
            tempData[i] = new RowData();
            tempData[i].colData = new PieceType[boardData.dimensions];
            for (int j = 0; j < boardData.dimensions; j++)
                tempData[i].colData[j] = cells[j, i].piece == null ? PieceType.NONE : cells[j, i].piece.type;
        }

        if (movedPiece.type == PieceType.PAWN && (boardData.doTopPromotion || boardData.doBottomPromotion))
        {
            Cell parentCell = movedPiece.GetComponentInParent<Cell>();
            if ((parentCell.position.y == 0 && boardData.doTopPromotion) ||
               (parentCell.position.y == boardData.dimensions - 1 && boardData.doBottomPromotion))
            {
                movedPiece.TogglePromotionPanel(true);
                running = false;
                return;
            }
        }

        history.Add(new BoardState(tempData, movedPiece.type));
    }

    public void RegisterPromotion(PieceType type)
    {
        RowData[] tempData = new RowData[boardData.dimensions];

        for (int i = 0; i < boardData.dimensions; i++)
        {
            tempData[i] = new RowData();
            tempData[i].colData = new PieceType[boardData.dimensions];
            for (int j = 0; j < boardData.dimensions; j++)
                tempData[i].colData[j] = cells[j, i].piece == null ? PieceType.NONE : cells[j, i].piece.type;
        }

        history.Add(new BoardState(tempData, type));
        running = true;
    }

    public void UndoMove()
    {
        if (history.Count < 2 || !running)
            return;
        history.RemoveAt(history.Count - 1);
        ClearBoard();
        StartCoroutine(SetPieces(history[^1].state));
    }
}
