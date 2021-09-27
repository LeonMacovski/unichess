using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public Cell cell;
    public RectTransform parent;
    public int dimensions;
    public Color pieceColor;
    public Color ligher;
    public Color darker;

    private Cell[,] cells;

    void Start()
    {
        cells = new Cell[dimensions, dimensions];
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        float cellSize = parent.sizeDelta.x / dimensions;
        Vector2 offset = new Vector2(-650, 650);

        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                cells[i, j] = Instantiate(cell, parent);
                cells[i, j].GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize, cellSize);
                cells[i, j].GetComponent<RectTransform>().anchoredPosition = offset + new Vector2(i * cellSize, -j * cellSize);
                cells[i, j].SetCoordinates(new Vector2(i, j));;

                cells[i, j].GetComponent<Image>().color = (j + i) % 2 == 0 ? ligher : darker;
            }
        }
    }
}
