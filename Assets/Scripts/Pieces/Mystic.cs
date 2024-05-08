using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mystic : MonsPiece
{
    public bool CanCollectBomb(ref MonsPiece[,] board, int x, int y)
    {

        if (board[x, y].monsPieceType == MonsPieceType.bombOrPortion)
        {
            return true;
        }

        return false;
    }


    public override List<Vector2Int> GetAvailableMoves(ref MonsPiece[,] board, int tileCount)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        int direction = (team == 0) ? 1 : -1;


        //Calculating the movable position in 1 tile space
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                // Skip the case where both dx and dy are 0 (no movement)
                if (dx == 0 && dy == 0)
                    continue;

                int newX = currentX + dx * direction;
                int newY = currentY + dy * direction;

                if (newX == 5 && newY == 5)//to avoid superMana
                    continue;

                // Check if the new position is within bounds and empty
                if (0 <= newX && newX < tileCount && 0 <= newY && newY < tileCount && (board[newX, newY] == null || CanCollectBomb(ref board, newX, newY)))
                {
                    r.Add(new Vector2Int(newX, newY));
                    print("Available moves for mystic: " + new Vector2Int(newX, newY));

                }
            }
        }


        if (0 <= currentX + direction * 2 && currentX + direction * 2 < tileCount 
            && 0 <= currentY + direction * 2 && currentY + direction * 2 < tileCount
            && board[currentX + direction * 2, currentY + direction * 2] != null 
            && board[currentX + direction * 2, currentY + direction * 2].team != team
            && board[currentX + direction * 2, currentY + direction * 2].monsPieceType!=MonsPieceType.bombOrPortion
            //&& checkAngel(ref board,currentX +  direction * 2, currentY + direction * 2)
            )
        {
            if (board[currentX + direction * 2, currentY + direction * 2].monsPieceType != MonsPieceType.mana)
                r.Add(new Vector2Int(currentX + direction * 2, currentY + direction * 2));
            print("Available moves for mystic: " + new Vector2Int(currentX + direction * 2, currentY + direction * 2));//here right diagonal gap

        }

        //2 tiles gap logic
        if (0 <= currentX + direction * 2 && currentX + direction * 2 < tileCount
            && 0 <= currentY - direction * 2 && currentY - direction * 2 < tileCount
            && board[currentX + direction * 2, currentY - direction * 2] != null
            && board[currentX + direction * 2, currentY - direction * 2].team != team
            && board[currentX + direction * 2, currentY - direction * 2].monsPieceType != MonsPieceType.bombOrPortion
             //&& checkAngel(ref board, currentX + direction * 2, currentY - direction * 2)
             )
        {
            if (board[currentX + direction * 2, currentY - direction * 2].monsPieceType != MonsPieceType.mana)
            r.Add(new Vector2Int(currentX + direction * 2, currentY - direction * 2));
            print("Available moves for mystic: " + new Vector2Int(currentX + direction * 2, currentY - direction * 2));
        }


        if (0 <= currentX - direction * 2 && currentX - direction * 2 < tileCount
            && 0 <= currentY + direction * 2 && currentY + direction * 2 < tileCount
            && board[currentX - direction * 2, currentY + direction * 2] != null
            && board[currentX - direction * 2, currentY + direction * 2].team != team
            && board[currentX - direction * 2, currentY + direction * 2].monsPieceType != MonsPieceType.bombOrPortion
             //&& checkAngel(ref board, currentX - direction * 2, currentY + direction * 2)
             )
        {
            if (board[currentX - direction * 2, currentY + direction * 2].monsPieceType != MonsPieceType.mana)
            r.Add(new Vector2Int(currentX - direction * 2, currentY + direction * 2));
            print("Available moves for mystic: " + new Vector2Int(currentX - direction * 2, currentY + direction * 2));//bottom right diagonal tile gap/ top left diagonal

        }


        if (0 <= currentX -  direction * 2 && currentX - direction * 2 < tileCount
            && 0 <= currentY - direction * 2 && currentY - direction * 2 < tileCount
            && board[currentX - direction * 2, currentY - direction * 2] != null
            && board[currentX - direction * 2, currentY - direction * 2].team != team
            && board[currentX - direction * 2, currentY - direction * 2].monsPieceType != MonsPieceType.bombOrPortion
            && board[currentX - direction * 2, currentY - direction * 2].monsPieceType != MonsPieceType.bombOrPortion
             //&& checkAngel(ref board, currentX - direction * 2, currentY - direction * 2)
             )
        {
            if (board[currentX - direction * 2, currentY - direction * 2].monsPieceType != MonsPieceType.mana)
                r.Add(new Vector2Int(currentX - direction * 2, currentY - direction * 2));
                print("Available moves for mystic: " + new Vector2Int(currentX - direction * 2, currentY - direction * 2));

        }
        return r;
    }



    public bool checkAngel(ref MonsPiece[,] board, int x, int y)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                int newX = x + i;
                int newY = y + j;

                if (0 <= newX && newX < 11 && 0 <= newY && newY < 11 && board[newX, newY] != null && board[newX, newY].monsPieceType == MonsPieceType.angel)
                    return true;
            }
        }

        return false;
    }


    //public override void HighlightValidMovePositions(MonsPiece monsPiece)
    //{
    //    Debug.Log("Mystic highlight called");
    //    base.HighlightValidMovePositions(monsPiece);

    //    for (int i = 1; i <= 2; i++)
    //    {
    //        // Check diagonally
    //        CheckOpponentAtDistance(currentX + i, currentY + i);
    //        CheckOpponentAtDistance(currentX - i, currentY + i);
    //        CheckOpponentAtDistance(currentX + i, currentY - i);
    //        CheckOpponentAtDistance(currentX - i, currentY - i);
    //    }
    //}

    //private void CheckOpponentAtDistance(int targetX, int targetY)
    //{
    //    // Check if there's an opponent piece at the specified position
    //    if (targetX >= 0 && targetX < 11 && targetY >= 0 && targetY < 11)
    //    {
    //        MonsPiece targetPiece = Board.instance.monsPiece[targetX, targetY];
    //        if (targetPiece != null && targetPiece.team != team)
    //        {
    //            HighlightCell(new Vector2(targetX, targetY));
    //        }
    //    }
    //}
}
