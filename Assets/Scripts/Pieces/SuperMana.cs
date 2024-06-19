using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperMana : MonsPiece
{
    private Vector3 cachedDesiredPos;

    private void Start()
    {
        cachedDesiredPos = monsPieceDataType.desiredPos;
    }
    public override List<Vector2Int> GetAvailableMoves(ref MonsPiece[,] board, int tileCount)
    {
        return null;
    }
    private void OnDisable()
    {
        monsPieceDataType.desiredPos = cachedDesiredPos;
    }

}
