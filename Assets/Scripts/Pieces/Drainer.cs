using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drainer : MonsPiece
{

    
    int tileDistance = 1;
    public bool CanCollectBomb(ref MonsPiece[,] board, int x, int y)
    {

        if (!monsPieceDataType.isCarryingBomb && !monsPieceDataType.isCarryingPortion &&  board[x, y].monsPieceDataType.monsPieceType == MonsPieceType.bombOrPortion)
        {
            return true;
        }

        return false;
    }
    public override List<Vector2Int> GetAvailableMoves(ref MonsPiece[,] board, int tileCount)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        int direction = (monsPieceDataType.team == 0) ? 1 : -1;

        if (monsPieceDataType.isCarryingBomb)
        {
            tileDistance = 3;
        }
        else
        {
            tileDistance = 1;
        }



        if (monsPieceDataType.isCarryingBomb)
        {
       

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        // Skip the case where both dx and dy are 0 (no movement)
                        if (dx == 0 && dy == 0)
                            continue;

                        int newX = monsPieceDataType.currentX + dx * direction;
                        int newY = monsPieceDataType.currentY + dy * direction;


                        bool isNotCarryingAnything = (!monsPieceDataType.isCarryingMana && !monsPieceDataType.isCarryingSuperMana && !monsPieceDataType.isCarryingOppMana && !monsPieceDataType.isCarryingBomb);
                        // Check if the new position is within bounds and empty
                        if (0 <= newX && newX < tileCount && 0 <= newY && newY < tileCount && (board[newX, newY] == null
                                                                                                || isNotCarryingAnything && board[newX, newY].monsPieceDataType.monsPieceType == MonsPieceType.mana
                                                                                                || isNotCarryingAnything && board[newX, newY].monsPieceDataType.monsPieceType == MonsPieceType.supermana
                                                                                                || isNotCarryingAnything && CanCollectBomb(ref board, newX, newY)))
                        {

                            r.Add(new Vector2Int(newX, newY));
                        }
                    }
                }
                //atack moves 4 way direction(x and y)
                if (0 <= monsPieceDataType.currentY + direction * tileDistance && monsPieceDataType.currentY + direction * tileDistance < tileCount && board[monsPieceDataType.currentX, monsPieceDataType.currentY + direction * tileDistance] != null && board[monsPieceDataType.currentX, monsPieceDataType.currentY + direction * tileDistance].monsPieceDataType.team != monsPieceDataType.team && board[monsPieceDataType.currentX, monsPieceDataType.currentY + direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.mana && !checkAngel(ref board, monsPieceDataType.currentX, monsPieceDataType.currentY + direction * tileDistance))
                {
                    r.Add(new Vector2Int(monsPieceDataType.currentX, monsPieceDataType.currentY + direction * tileDistance));
                }

                if (0 <= monsPieceDataType.currentX + direction * tileDistance && monsPieceDataType.currentX + direction * tileDistance < tileCount && board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY] != null && board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY].monsPieceDataType.team != monsPieceDataType.team && board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY].monsPieceDataType.monsPieceType != MonsPieceType.mana && !checkAngel(ref board, monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY))
                {
                    r.Add(new Vector2Int(monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY));
                }

                if (0 <= monsPieceDataType.currentX - direction * tileDistance && monsPieceDataType.currentX - direction * tileDistance < tileCount && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY] != null && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY].monsPieceDataType.team != monsPieceDataType.team && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY].monsPieceDataType.monsPieceType != MonsPieceType.mana && !checkAngel(ref board, monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY))
                {
                    r.Add(new Vector2Int(monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY));
                }

                if (0 <= monsPieceDataType.currentY - direction * tileDistance && monsPieceDataType.currentY - direction * tileDistance < tileCount && board[monsPieceDataType.currentX, monsPieceDataType.currentY - direction * tileDistance] != null && board[monsPieceDataType.currentX, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.team != monsPieceDataType.team && board[monsPieceDataType.currentX, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.mana && !checkAngel(ref board, monsPieceDataType.currentX, monsPieceDataType.currentY - direction * tileDistance))
                {
                    r.Add(new Vector2Int(monsPieceDataType.currentX, monsPieceDataType.currentY - direction * tileDistance));
                }

            //atack moves 4 way direction(x+y and y+x) diagonal moves
            if (0 <= monsPieceDataType.currentX + direction * tileDistance && monsPieceDataType.currentX + direction * tileDistance < tileCount
&& 0 <= monsPieceDataType.currentY + direction * tileDistance && monsPieceDataType.currentY + direction * tileDistance < tileCount
&& board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance] != null
&& board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance].monsPieceDataType.team != monsPieceDataType.team
&& board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.bombOrPortion
&& !checkAngel(ref board, monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance)
)
            {
                if (board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.mana)
                    r.Add(new Vector2Int(monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance));
                print("Available moves for mystic: " + new Vector2Int(monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance));//here right diagonal gap

            }

            //2 tiles gap logic
            if (0 <= monsPieceDataType.currentX + direction * tileDistance && monsPieceDataType.currentX + direction * tileDistance < tileCount
                && 0 <= monsPieceDataType.currentY - direction * tileDistance && monsPieceDataType.currentY - direction * tileDistance < tileCount
                && board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance] != null
                && board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.team != monsPieceDataType.team
                && board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.bombOrPortion
                 && !checkAngel(ref board, monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance)
                 )
            {
                if (board[monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.mana)
                    r.Add(new Vector2Int(monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance));
                print("Available moves for mystic: " + new Vector2Int(monsPieceDataType.currentX + direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance));
            }


            if (0 <= monsPieceDataType.currentX - direction * tileDistance && monsPieceDataType.currentX - direction * tileDistance < tileCount
                && 0 <= monsPieceDataType.currentY + direction * tileDistance && monsPieceDataType.currentY + direction * tileDistance < tileCount
                && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance] != null
                && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance].monsPieceDataType.team != monsPieceDataType.team
                && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.bombOrPortion
                 && !checkAngel(ref board, monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance)
                 )
            {
                if (board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.mana)
                    r.Add(new Vector2Int(monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance));
                print("Available moves for mystic: " + new Vector2Int(monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY + direction * tileDistance));//bottom right diagonal tile gap/ top left diagonal

            }


            if (0 <= monsPieceDataType.currentX - direction * tileDistance && monsPieceDataType.currentX - direction * tileDistance < tileCount
                && 0 <= monsPieceDataType.currentY - direction * tileDistance && monsPieceDataType.currentY - direction * tileDistance < tileCount
                && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance] != null
                && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.team != monsPieceDataType.team
                && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.bombOrPortion
                && board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.bombOrPortion
                 && !checkAngel(ref board, monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance)
                 )
            {
                if (board[monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance].monsPieceDataType.monsPieceType != MonsPieceType.mana)
                    r.Add(new Vector2Int(monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance));
                print("Available moves for mystic: " + new Vector2Int(monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance) + "check angel " + checkAngel(ref board, monsPieceDataType.currentX - direction * tileDistance, monsPieceDataType.currentY - direction * tileDistance));

            }
        }
            
        else
        {
            for (int dx = -tileDistance; dx <= tileDistance; dx++)
            {
                for (int dy = -tileDistance; dy <= tileDistance; dy++)
                {
                    // Skip the case where both dx and dy are 0 (no movement)
                    if (dx == 0 && dy == 0)
                        continue;

                    int newX = monsPieceDataType.currentX + dx * direction;
                    int newY = monsPieceDataType.currentY + dy * direction;


                    bool isNotCarryingAnything = (!monsPieceDataType.isCarryingMana && !monsPieceDataType.isCarryingSuperMana && !monsPieceDataType.isCarryingOppMana);
                    // Check if the new position is within bounds and empty
                    if (0 <= newX && newX < tileCount && 0 <= newY && newY < tileCount && (board[newX, newY] == null
                                                                                            || isNotCarryingAnything && board[newX, newY].monsPieceDataType.monsPieceType == MonsPieceType.mana
                                                                                            || isNotCarryingAnything && board[newX, newY].monsPieceDataType.monsPieceType == MonsPieceType.supermana
                                                                                            || CanCollectBomb(ref board, newX, newY)))
                    {

                        r.Add(new Vector2Int(newX, newY));
                    }
                }
            }
        }


       


        //if (0 <= currentX + direction && currentX + direction <= tileCount && 0 <= currentY + direction && currentY + direction <= tileCount && (board[currentX + direction, currentY + direction] == null || board[currentX + direction, currentY + direction].monsPieceType == MonsPieceType.mana || CanCollectBomb(ref board,currentX+direction,currentY+direction)))
        //{
        //    r.Add(new Vector2Int(currentX + direction, currentY + direction)); // Diagonal up-right
        //}
        //if (0 <= currentX + direction && currentX + direction <= tileCount && 0 <= currentY - direction && currentY - direction <= tileCount)
        //{   if(board[currentX + direction, currentY - direction] == null || board[currentX + direction, currentY - direction].monsPieceType == MonsPieceType.mana || CanCollectBomb(ref board, currentX + direction, currentY - direction))
        //    {
        //        r.Add(new Vector2Int(currentX + direction, currentY - direction));
        //    } // Diagonal down-right
        //}
        //if (0 <= currentX - direction && currentX - direction <= tileCount && 0 <= currentY + direction && currentY + direction <= tileCount && (board[currentX - direction, currentY + direction] == null || board[currentX - direction, currentY + direction].monsPieceType == MonsPieceType.mana || CanCollectBomb(ref board, currentX - direction, currentY + direction)))
        //{
        //    r.Add(new Vector2Int(currentX - direction, currentY + direction)); // Diagonal up-left
        //}
        //if (0 <= currentX - direction && currentX - direction <= tileCount && 0 <= currentY - direction && currentY - direction <= tileCount && (board[currentX - direction, currentY - direction] == null || board[currentX - direction, currentY - direction].monsPieceType == MonsPieceType.mana || CanCollectBomb(ref board, currentX - direction, currentY - direction)))
        //{
        //    r.Add(new Vector2Int(currentX - direction, currentY - direction)); // Diagonal down-left
        //}

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

                if (0 <= newX && newX < 11 && 0 <= newY && newY < 11 && board[newX, newY] != null && board[newX, newY].monsPieceDataType.monsPieceType == MonsPieceType.angel && board[newX, newY].monsPieceDataType.team != monsPieceDataType.team)
                    return true;
            }
        }

        return false;
    }



}
