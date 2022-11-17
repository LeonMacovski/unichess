using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
    public RectTransform promotionKnight;
    public RectTransform promotionQueen;

    [HideInInspector] public PieceType type;
    private List<Cell> legalMoves = new List<Cell>();
    private bool isDragging;
    private Transform newLocation;

    private void Start()
    {
        promotionKnight.sizeDelta = GetComponent<RectTransform>().sizeDelta * .7f;
        promotionQueen.sizeDelta = GetComponent<RectTransform>().sizeDelta * .7f;
    }

    private void Update()
    {
        if (isDragging)
            transform.position = Input.mousePosition;
    }

    public void SetPieceType(PieceType _type) => type = _type;

    public void StartDrag()
    {
        if (!BoardManager.instance.running)
            return;
        isDragging = true;
        legalMoves = BoardManager.instance.GenerateLegalMoves(this);
    }

    public void StopDrag()
    {
        isDragging = false;
        if (newLocation != null && newLocation != transform.parent && legalMoves.Contains(newLocation.GetComponent<Cell>()))
        {
            Cell prevCell = transform.parent.GetComponent<Cell>();
            transform.parent = newLocation;
            prevCell.SetPiece(null);
            transform.parent.GetComponent<Cell>().SetPiece(this);
            legalMoves = new List<Cell>();
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            BoardManager.instance.RegisterMove(this);
        }
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    public void Raise() => StartCoroutine(LerpRoutine(result => GetComponent<RectTransform>().localPosition = result, 
                                                                    new Vector2(GetComponent<RectTransform>().localPosition.x, 0),
                                                                    new Vector2(GetComponent<RectTransform>().localPosition.x, 2000)));

    public void Lower() => StartCoroutine(LerpRoutine(result => GetComponent<RectTransform>().localPosition = result,
                                                                    new Vector2(GetComponent<RectTransform>().localPosition.x, 2000),
                                                                    new Vector2(GetComponent<RectTransform>().localPosition.x, 0)));

    public void Promote(bool knight)
    {
        PieceType promoteTo = knight ? PieceType.KNIGHT : PieceType.QUEEN;
        type = promoteTo;
        GetComponent<Image>().sprite = BoardManager.instance.GetSpriteByType(promoteTo);
        TogglePromotionPanel(false);
        BoardManager.instance.RegisterPromotion(promoteTo);
    }

    public void TogglePromotionPanel(bool show)
    {
        StartCoroutine(LerpRoutine(result => promotionKnight.localScale = result,
                                                                    show ? Vector2.zero : Vector2.one,
                                                                    show ? Vector2.one : Vector2.zero,
                                                                    .14f));
        StartCoroutine(LerpRoutine(result => promotionQueen.localScale = result,
                                                                    show ? Vector2.zero : Vector2.one,
                                                                    show ? Vector2.one : Vector2.zero,
                                                                    .14f));
    }

    private IEnumerator LerpRoutine(Action<Vector3> property, Vector3 relativeStartPoint, Vector3 relativeEndPoint, float duration = .6f)
    {
        float elapsedTime = 0;
        float waitTime = duration;
        while(elapsedTime < waitTime)
        {
            property(Vector3.Lerp(relativeStartPoint, relativeEndPoint, elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        property(relativeEndPoint);
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("cell"))
            newLocation = collision.transform;
    }
}
