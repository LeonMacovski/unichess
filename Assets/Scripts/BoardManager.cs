using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Cell cell;
    public int dimensions;
    public Color pieceColor;
    public Color colorOne;
    public Color colorTwo;

    private Cell[,] cells;

    void Start()
    {
        cells = new Cell[dimensions, dimensions];
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        cell.transform.localScale = new Vector3(5.0f / dimensions, 5.0f / dimensions, 0);
        Vector3 offset = new Vector3(-2.5f, 3f, 0);
        float size = cell.transform.localScale.x;

        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                cells[i, j] = Instantiate(cell, offset + new Vector3(i * size, -j * size, 0), Quaternion.identity);
                if ((j + i) % 2 == 0)
                    cells[i, j].GetComponent<SpriteRenderer>().color = colorOne;
                else
                    cells[i, j].GetComponent<SpriteRenderer>().color = colorTwo;

            }
        }
    }
}
