using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UC_LevelCreator : MonoBehaviour
{
    public TextMeshProUGUI levelText;

    private PieceType pieceToPlace;
    private Cell hoveredCell;

    public static UC_LevelCreator instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        pieceToPlace = PieceType.NONE;
        levelText.text = "Current level: " + BoardManager.instance.boardData.name;
    }

    public void FinalizeBoard(Cell[,] cells)
    {
        for (int i = 0; i < BoardManager.instance.boardData.dimensions; i++)
        {
            for (int j = 0; j < BoardManager.instance.boardData.dimensions; j++)
            {
                Button btn = cells[i, j].gameObject.AddComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    Cell attachedCell = btn.GetComponent<Cell>();
                    if (pieceToPlace == attachedCell.piece.type)
                        attachedCell.SetPiece(null);

                    else
                    {
                        Piece tempPiece = Instantiate(BoardManager.instance.piece, attachedCell.transform);
                        attachedCell.SetPiece(tempPiece);
                        tempPiece.SetPieceType(pieceToPlace);
                        tempPiece.GetComponent<Image>().sprite = BoardManager.instance.GetSpriteByType(pieceToPlace);
                        tempPiece.GetComponent<RectTransform>().sizeDelta = new Vector2(BoardManager.instance.cellSize, BoardManager.instance.cellSize);
                        tempPiece.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                        Destroy(tempPiece.GetComponent<BoxCollider2D>());
                        Destroy(tempPiece.GetComponent<EventTrigger>());
                    }
                });
            }
        }
    }

    public void UpdateLevel()
    {
        UC_Level tempLevel = BoardManager.instance.boardData;
        tempLevel.rowData = new RowData[tempLevel.dimensions];
        tempLevel.ClearData();
            

        for (int i = 0; i < tempLevel.dimensions; i++)
        {
            for (int j = 0; j < tempLevel.dimensions; j++)
            {
                tempLevel.rowData[i].colData[j] = BoardManager.instance.cells[j, i].piece.type;
            }
        }
    }

    public void SelectPieceToPlace(Transform obj)
    {
        PieceType piece = PieceType.NONE;
        switch (obj.name)
        {
            case "pawn":
                piece = PieceType.PAWN;
                break;
            case "knight":
                piece = PieceType.KNIGHT;
                break;
            case "bishop":
                piece = PieceType.BISHOP;
                break;
            case "rook":
                piece = PieceType.ROOK;
                break;
            case "queen":
                piece = PieceType.QUEEN;
                break;
            case "king":
                piece = PieceType.KING;
                break;
        }

        pieceToPlace = piece;
    }
}
