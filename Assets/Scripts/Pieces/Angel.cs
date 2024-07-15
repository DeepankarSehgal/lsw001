using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angel : MonsPiece
{
    int tileDistance = 1;
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


        //Normal movement
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                // Skip the case where both dx and dy are 0 (no movement)
                if (dx == 0 && dy == 0)
                    continue;

                int newX = monsPieceDataType.currentX + dx * direction;
                int newY = monsPieceDataType.currentY + dy * direction;

                if (newX == 5 && newY == 5)
                    continue;

                // Check if the new position is within bounds and empty
                if (0 <= newX && newX < tileCount && 0 <= newY && newY < tileCount && (board[newX, newY] == null || board[newX, newY] != null && !monsPieceDataType.isCarryingBomb && !monsPieceDataType.isCarryingPortion && board[newX, newY].monsPieceDataType.monsPieceType == MonsPieceType.bombOrPortion))
                {
                    r.Add(new Vector2Int(newX, newY));
                }
            }
        }

        if (monsPieceDataType.isCarryingBomb)
        {


            tileDistance = 2;
            for (int i = 0; i < 2; i++)
            {
                // movement with carrying bomb
                for (int dx = -tileDistance; dx <= tileDistance; dx++)
                {
                    for (int dy = -tileDistance; dy <= tileDistance; dy++)
                    {
                        // Skip the case where both dx and dy are 0 (no movement)
                        if (dx == 0 && dy == 0)
                            continue;

                        int newX = monsPieceDataType.currentX + dx * direction;
                        int newY = monsPieceDataType.currentY + dy * direction;

                        if (newX == 5 && newY == 5)
                            continue;

                        // Check if the new position is within bounds and empty
                        if (0 <= newX && newX < tileCount && 0 <= newY && newY < tileCount && (/*!isCarryingBomb && board[newX, newY] == null ||*/ /*board[newX,newY]!=null && board[newX, newY].monsPieceType==MonsPieceType.bombOrPortion || */(monsPieceDataType.isCarryingBomb && board[newX, newY] != null && board[newX, newY].monsPieceDataType.team != monsPieceDataType.team && board[newX, newY].monsPieceDataType.monsPieceType != MonsPieceType.mana && board[newX, newY].monsPieceDataType.monsPieceType != MonsPieceType.supermana)))
                        {
                            r.Add(new Vector2Int(newX, newY));
                        }
                    }
                }
                tileDistance++;
            }

     
        }
        else
        {
            // movement with carrying bomb
            for (int dx = -tileDistance; dx <= tileDistance; dx++)
            {
                for (int dy = -tileDistance; dy <= tileDistance; dy++)
                {
                    // Skip the case where both dx and dy are 0 (no movement)
                    if (dx == 0 && dy == 0)
                        continue;

                    int newX = monsPieceDataType.currentX + dx * direction;
                    int newY = monsPieceDataType.currentY + dy * direction;

                    if (newX == 5 && newY == 5)
                        continue;

                    // Check if the new position is within bounds and empty
                    if (0 <= newX && newX < tileCount && 0 <= newY && newY < tileCount && (/*!isCarryingBomb && board[newX, newY] == null ||*/ /*board[newX,newY]!=null && board[newX, newY].monsPieceType==MonsPieceType.bombOrPortion || */(monsPieceDataType.isCarryingBomb && board[newX, newY] != null && board[newX, newY].monsPieceDataType.team != monsPieceDataType.team && board[newX, newY].monsPieceDataType.monsPieceType != MonsPieceType.mana && board[newX, newY].monsPieceDataType.monsPieceType != MonsPieceType.supermana)))
                    {
                        r.Add(new Vector2Int(newX, newY));
                    }
                }
            }
        }



        return r;
    }


    //private Board board; // Reference to the board script
    //private bool isSelected = false; // Flag to check if the piece is selected

    //// Start is called before the first frame update
    //void Start()
    //{
    //    board = GameObject.FindObjectOfType<Board>(); // Get the reference to the board script
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    // Check if the piece is selected
    //    if (isSelected)
    //    {
    //        // Handle movement when the piece is selected
    //        MovePiece();
    //    }
    //}

    //// Method to handle movement of the piece
    //private void MovePiece()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        // Get the position of the mouse click
    //        Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        clickPosition.z = 0; // Ensure z-coordinate is 0

    //        // Move the piece to the clicked position
    //        transform.position = clickPosition;
    //        isSelected = false; // Deselect the piece after moving
    //    }
    //}

    //// Method to select the piece
    //public void SelectPiece()
    //{
    //    isSelected = true;
    //}
}
