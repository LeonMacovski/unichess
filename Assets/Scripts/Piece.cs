using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Piece : MonoBehaviour
{
    [HideInInspector] public PieceType type;
    private BoxCollider2D coll2D;
    private List<Cell> legalMoves = new List<Cell>();
    private bool isDragging;
    private Transform newLocation;
    private bool isSet = false;

    public void SetPieceType(PieceType _type) => type = _type;

    public void SetCollider()
    {
        coll2D = GetComponent<BoxCollider2D>();
        coll2D.size = new Vector3(4, 4, 0);
    }

    private void Update()
    {
        if (isDragging)
            transform.position = Input.mousePosition;

        float step = 5000 * Time.deltaTime;

        if (!isSet)
        {
            GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(
                GetComponent<RectTransform>().anchoredPosition,
                new Vector3(GetComponent<RectTransform>().anchoredPosition.x, 0, 0),
                step);
        }
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

    public void Raise()
    {
        isSet = true;
        float step = 5000 * Time.deltaTime;

        GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(
            GetComponent<RectTransform>().anchoredPosition,
            new Vector3(GetComponent<RectTransform>().anchoredPosition.x, 2000, 0),
            step);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("cell"))
            newLocation = collision.transform;
    }
}
