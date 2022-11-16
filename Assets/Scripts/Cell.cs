using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public PieceType piece;
    public Vector2 position;
    [HideInInspector] public BoxCollider2D coll2D;

    public void SetPiece(PieceType _piece) => piece = _piece;
    public void SetCoordinates(Vector2 _position) => position = _position;
    public void SetCollider()
    {
        coll2D = GetComponent<BoxCollider2D>();
        float size = GetComponent<RectTransform>().sizeDelta.x;
        coll2D.offset = Vector3.zero;
        coll2D.size = new Vector3(size, size, 0);
    }
}
