using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MonsPieceType
{
    None = 0,
    demon = 1,
    angel = 2,
    drainer = 3,
    spirit = 4,
    mystic = 5,
    mana = 6,
    supermana = 7,
    bomb = 8,
    portion = 9,
    bombOrPortion = 10
}

public class MonsPiece : MonoBehaviour
{
    public int team;
    public int currentX;
    public int currentY;
    public MonsPieceType monsPieceType;

    public Vector3 desiredPos;
    public Vector3 desiredScale;

    public Vector2 resetPos;

    public bool isFainted;
    private int faintedTurnsRemaining;


    public bool isCarryingOppMana = false;
    public bool isCarryingSuperMana = false;
    public bool isCarryingBomb = false;
    public bool isCarryingPortion = false;

    private void Update()
    {
        transform.position = Vector2.Lerp(transform.position,desiredPos, Time.deltaTime*10);
    }

    public virtual List<Vector2Int> GetAvailableMoves(ref MonsPiece[,] board,int tileCount)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        r.Add(new Vector2Int(3, 3));    
        r.Add(new Vector2Int(3, 4));    
        r.Add(new Vector2Int(4, 3));    
        r.Add(new Vector2Int(4, 4));    
        return r;
    }

    public virtual void SetPosition(Vector2 position, bool force = false)
    {
        desiredPos = position;
        if(force)
            transform.position = desiredPos;
    }


    public void FaintForTurns(int numTurns)
    {
        isFainted = true;
        faintedTurnsRemaining = numTurns;
    }

    public void UpdateFaintedTurns()
    {
        if (isFainted)
        {
            faintedTurnsRemaining--;
            if (faintedTurnsRemaining <= 0)
            {
                isFainted = false;
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }


}
