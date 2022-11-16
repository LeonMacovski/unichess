using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Piece : MonoBehaviour
{
    public RectTransform promotionKnight;
    public RectTransform promotionQueen;

    [HideInInspector] public PieceType type;
    private List<Cell> legalMoves = new List<Cell>();
    private bool isDragging;
    private Transform newLocation;

    private void Update()
    {
        if (isDragging)
            transform.position = Input.mousePosition;

        GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(
            GetComponent<RectTransform>().anchoredPosition,
            new Vector3(GetComponent<RectTransform>().anchoredPosition.x, 0, 0),
            5000 * Time.deltaTime);
    }

    public void SetPieceType(PieceType _type) => type = _type;

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
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            BoardManager.instance.RegisterMove(this);
        }
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    public void Raise()
    {
        GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(
            GetComponent<RectTransform>().anchoredPosition,
            new Vector3(GetComponent<RectTransform>().anchoredPosition.x, 2000, 0),
            5000 * Time.deltaTime);
    }

    public void Promote(bool knight)
    {

    }

    public void ShowPromotionPanel()
    {
     //   promotionKnight.DOScale(1, .3f).SetEase(Ease.Flash);
      //  promotionQueen.DOScale(1, .3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("cell"))
            newLocation = collision.transform;
    }
}
