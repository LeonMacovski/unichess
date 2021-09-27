using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private PieceType piece;
    private Vector2 position;

    public void SetPiece(PieceType _piece) => piece = _piece;
    public void SetCoordinates(Vector2 _position) => position = _position;


    
}
