using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Piece : MonoBehaviour
{
    [HideInInspector] public PieceType type;
    private BoxCollider2D collider;
    private List<Cell> legalMoves = new List<Cell>();
    private bool isDragging;
    Transform newLocation;

    public void SetPieceType(PieceType _type) => type = _type;

    public void SetCollider()
    {
        collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector3(4, 4, 0);
    }

    private void Update()
    {
        if (isDragging)
            transform.position = Input.mousePosition;
    }

    public void StartDrag()
    {
        isDragging = true;
        legalMoves = BoardManager.instance.GenerateLegalMoves(this);
    }

    public void StopDrag()
    {
        isDragging = false;
        if (newLocation != null && newLocation != transform.parent && legalMoves.Contains(newLocation.GetComponent<Cell>()))
        {
            transform.parent.GetComponent<Cell>().SetPiece(PieceType.NONE);
            BoardManager.instance.ClearCell(newLocation.GetComponent<Cell>(), type);
            transform.parent = newLocation;
            legalMoves = new List<Cell>();
        }
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("cell"))
            newLocation = collision.transform;
    }
}
