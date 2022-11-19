using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public RectTransform topPromotionIndicator;
    public RectTransform bottomPromotionIndicator;

    public Piece piece { get; private set; }
    public Vector2 position { get; private set; }
    [HideInInspector] public BoxCollider2D coll2D;

    public void SetPiece(Piece _piece)
    {
        if (piece != null && _piece != null)
            Destroy(piece.gameObject);

        piece = _piece;
    }

    public void SetCoordinates(Vector2 _position)
    {
        position = _position;
        topPromotionIndicator.gameObject.SetActive(position.y == 0 && BoardManager.instance.boardData.doTopPromotion);
        bottomPromotionIndicator.gameObject.SetActive(position.y == BoardManager.instance.boardData.dimensions - 1 && BoardManager.instance.boardData.doBottomPromotion);
    }

    public void SetCollider()
    {
        coll2D = GetComponent<BoxCollider2D>();
        float size = GetComponent<RectTransform>().sizeDelta.x;
        coll2D.offset = Vector3.zero;
        coll2D.size = new Vector3(size, size, 0);
    }
}
