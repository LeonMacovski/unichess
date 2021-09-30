using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public PieceType piece;
    public Vector2 position;
    [HideInInspector] public BoxCollider2D collider;

    public void SetPiece(PieceType _piece) => piece = _piece;
    public void SetCoordinates(Vector2 _position) => position = _position;
    public void SetCollider()
    {
        collider = GetComponent<BoxCollider2D>();
        float size = GetComponent<RectTransform>().sizeDelta.x;
        collider.offset = Vector3.zero;
        collider.size = new Vector3(size, size, 0);
    }

    public void MovePiece()
    {
        if(BoardManager.instance.selectedPiece != null)
        {
            BoardManager.instance.selectedPiece.transform.parent = transform;
            BoardManager.instance.selectedPiece.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

        }
    }
}
