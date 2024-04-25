using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon.StructWrapping;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject whiteCellPrefab;
    public GameObject blackCellPrefab;
    public int boardSize = 11;
    public GameObject[,] cells;

    public MonsPiece currentlyDraggingPiece;

    public GameObject[] Whiteprefabs;
    public GameObject[] Blackprefabs;

    public GameObject BombAndPortion;

    public MonsPiece[,] monsPiece;
    public float size;

    private Color originalCellColor;
    private Color hoverCellColor = Color.cyan;

    public GameObject HighlightPrefab;
    public GameObject HighlightHolder;

    public static Board instance;

    public Vector2Int currentHover;

    private List<MonsPiece> deadWhites = new List<MonsPiece>();
    private List<MonsPiece> deadBlacks = new List<MonsPiece>();

    private List<Vector2Int> availableMoves = new List<Vector2Int>();

    private bool isWhiteTurn;
    private int itemChances = 5;
    private bool manaTurn = false;

    public TMP_Text whiteScoreText;
    public TMP_Text blackScoreText;

    private int whiteScore = 0;
    private int blackScore = 0;

    public Color[] boardColors;

    public GameObject BombOrPortionChoicePanel;

    public GameObject[] BPchoices;

    int choice;

    private bool isGameEnd = false;
    public TMP_Text endGameText;


    public GameObject[] Marks;
    public GameObject[] childManaSuperMana;



    public MonsPiece selectedPiece;



    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        isWhiteTurn = true;
        CreateMonsBoard();
        SpawnAllPiece();
        PositionAllPiece();
        CenterBoard();
    }

    void Update()
    {
        if(!isGameEnd)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D info = Physics2D.Raycast(ray.origin, ray.direction, 100, LayerMask.GetMask("Tile", "Hover"));
            if (info)
            {
                Vector2Int hitPosition = LookUpTileIndex(info.transform.gameObject);

                if (currentHover == -Vector2Int.one)
                {
                    currentHover = hitPosition;
                    cells[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                }

                if (currentHover != hitPosition)
                {
                    cells[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                    currentHover = hitPosition;
                    cells[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (monsPiece[hitPosition.x, hitPosition.y] != null && !monsPiece[hitPosition.x, hitPosition.y].isFainted)
                    {
                        if ((monsPiece[hitPosition.x, hitPosition.y].team == 0 && isWhiteTurn) || (monsPiece[hitPosition.x, hitPosition.y].team == 1 && !isWhiteTurn))
                        {
                            currentlyDraggingPiece = monsPiece[hitPosition.x, hitPosition.y];

                            if (itemChances > 0)
                            {
                                if (currentlyDraggingPiece.monsPieceType == MonsPieceType.mana)
                                {
                                    manaTurn = true;
                                    availableMoves = currentlyDraggingPiece.GetAvailableMoves(ref monsPiece, boardSize);
                                    HighLightTiles();
                                }
                                else
                                {
                                    availableMoves = currentlyDraggingPiece.GetAvailableMoves(ref monsPiece, boardSize);
                                    HighLightTiles();
                                }
                            }
                            else
                            {
                                if (manaTurn == false)
                                {
                                    if (currentlyDraggingPiece.monsPieceType == MonsPieceType.mana)
                                    {
                                        availableMoves = currentlyDraggingPiece.GetAvailableMoves(ref monsPiece, boardSize);
                                        HighLightTiles();
                                    }
                                }
                                
                            }
                            //if (currentlyDraggingPiece.monsPieceType == MonsPieceType.spirit)
                            //{
                            //    // Check if there's any piece within 2 spaces
                            //    foreach (MonsPiece piece in monsPiece)
                            //    {
                            //        if (piece != null && piece != currentlyDraggingPiece && Mathf.Abs(piece.currentX - hitPosition.x) <= 2 && Mathf.Abs(piece.currentY - hitPosition.y) <= 2)
                            //        {
                            //            // Show available moves for the piece
                            //            currentlyDraggingPiece = piece;
                            //            availableMoves = piece.GetAvailableMoves(ref monsPiece, boardSize);
                            //            HighLightTiles();
                            //            break;
                            //        }
                            //    }
                            //}
                        }

                    }
                }
                if (currentlyDraggingPiece != null && Input.GetMouseButtonUp(0))
                {
                   
                        Vector2Int previousPosition = new Vector2Int(currentlyDraggingPiece.currentX, currentlyDraggingPiece.currentY);
                        //if (availableMoves.Contains(hitPosition))
                        {
                            bool validMove = MoveTo(currentlyDraggingPiece, hitPosition.x, hitPosition.y);
                            if (!validMove)
                            {
                                if (currentlyDraggingPiece.monsPieceType == MonsPieceType.drainer && currentlyDraggingPiece.isCarryingBomb)
                                {
                                    // Check if the clicked position is within 3 spaces and there's an opponent piece
                                    if (Mathf.Abs(hitPosition.x - currentlyDraggingPiece.currentX) <= 3 &&
                                        Mathf.Abs(hitPosition.y - currentlyDraggingPiece.currentY) <= 3 &&
                                        monsPiece[hitPosition.x, hitPosition.y] != null &&
                                        monsPiece[hitPosition.x, hitPosition.y].team != currentlyDraggingPiece.team &&
                                        monsPiece[hitPosition.x, hitPosition.y].monsPieceType != MonsPieceType.mana)
                                    {
                                        // Faint the opponent piece
                                        FaintOpponentPiece(monsPiece[hitPosition.x, hitPosition.y]);
                                        currentlyDraggingPiece.isCarryingBomb = false;
                                    }
                                }
                                currentlyDraggingPiece.SetPosition(previousPosition);
                            }
                        }
                    
                    currentlyDraggingPiece = null;
                    ClearHighLight();
                }

            }
            else
            {
                if (currentHover != -Vector2Int.one)
                {
                    cells[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                    currentHover = -Vector2Int.one;
                }

                if (currentlyDraggingPiece && Input.GetMouseButtonUp(0))
                {
                    currentlyDraggingPiece.SetPosition(new Vector2(currentlyDraggingPiece.currentX, currentlyDraggingPiece.currentY));
                    currentlyDraggingPiece = null;
                    ClearHighLight();
                }
            }
        }

    }

    public void CreateMonsBoard()
    {
        cells = new GameObject[boardSize,boardSize];

        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                GameObject cellPrefab = (x + y) % 2 == 0 ? whiteCellPrefab : blackCellPrefab;
                GameObject cell = Instantiate(cellPrefab, new Vector3(x , y, 0), Quaternion.identity, transform);
                if(x==0 && y==0 || x==10 && y==0 || x==0 && y==10 || x ==10 && y==10 || x==5 && y==5)
                {
                    cell.GetComponent<SpriteRenderer>().color = boardColors[0];
                }

                if(x == 3 && y == 4 || x == 4 && y == 3 || x == 5 && y == 4 || x == 6 && y == 3 || x == 7 && y == 4 ||
                   x == 3 && y == 6 || x == 4 && y == 7 || x == 5 && y == 6 || x == 6 && y == 7 || x == 7 && y == 6 )
                {
                    cell.GetComponent<SpriteRenderer>().color = boardColors[1];
                }

                if(x==0 && y==5 || x==10 && y==5)
                {
                    cell.GetComponent<SpriteRenderer>().color = boardColors[2];
                }

                cells[x, y] = cell;
                cell.gameObject.name = x + " : " + y;

                int[] pos = { 3, 4, 5, 6, 7 };
                if (pos.Contains(x) && (y==0 || y==10))
                {
                    GameObject go = Instantiate(Marks[x-3], cell.transform);
                    go.transform.SetParent(cell.transform);
                }

                cell.layer = LayerMask.NameToLayer("Tile");
                cell.AddComponent<BoxCollider2D>(); // Add collider for raycast
                cell.AddComponent<CellHover>(); // Add CellHover script for hover effect
            }
        }
    }

    void CenterBoard()
    {
        Vector3 boardOffset = new Vector3(-(boardSize - 1) / 2f, -(boardSize - 1) / 2f, 0);
        //transform.position = new Vector3(-2.76f, -1.7f, 0);
        //transform.position = boardOffset;
    }


    void SpawnAllPiece()
    {
        monsPiece = new MonsPiece[11,11];

        monsPiece[3, 0] = SpawnWhitePiece(MonsPieceType.demon, 0);
        monsPiece[3, 0].resetPos = new Vector2(3, 0);

        monsPiece[4, 0] = SpawnWhitePiece(MonsPieceType.angel, 0);
        monsPiece[4, 0].resetPos = new Vector2(4, 0);

        monsPiece[5, 0] = SpawnWhitePiece(MonsPieceType.drainer, 0);
        monsPiece[5, 0].resetPos = new Vector2(5, 0);

        monsPiece[6, 0] = SpawnWhitePiece(MonsPieceType.spirit, 0);
        monsPiece[6, 0].resetPos = new Vector2(6, 0);

        monsPiece[7, 0] = SpawnWhitePiece(MonsPieceType.mystic, 0);
        monsPiece[7, 0].resetPos = new Vector2(7, 0);

        monsPiece[3, 4] = SpawnWhitePiece(MonsPieceType.mana, 0);
        monsPiece[4, 3] = SpawnWhitePiece(MonsPieceType.mana, 0);
        monsPiece[5, 4] = SpawnWhitePiece(MonsPieceType.mana, 0);
        monsPiece[6, 3] = SpawnWhitePiece(MonsPieceType.mana, 0);
        monsPiece[7, 4] = SpawnWhitePiece(MonsPieceType.mana, 0);

        monsPiece[5, 5] = SpawnWhitePiece(MonsPieceType.supermana, 0);

        monsPiece[0, 5] = SpawnBlackPiece(MonsPieceType.bombOrPortion, -1);
        monsPiece[10, 5] = SpawnBlackPiece(MonsPieceType.bombOrPortion, -1);

        monsPiece[3, 10] = SpawnBlackPiece(MonsPieceType.demon, 1);
        monsPiece[3, 10].resetPos = new Vector2(3, 10);

        monsPiece[4, 10] = SpawnBlackPiece(MonsPieceType.angel, 1);
        monsPiece[4, 10].resetPos = new Vector2(4, 10);

        monsPiece[5, 10] = SpawnBlackPiece(MonsPieceType.drainer, 1);
        monsPiece[5, 10].resetPos = new Vector2(5, 10);

        monsPiece[6, 10] = SpawnBlackPiece(MonsPieceType.spirit, 1);
        monsPiece[6, 10].resetPos = new Vector2(6, 10);

        monsPiece[7, 10] = SpawnBlackPiece(MonsPieceType.mystic, 1);
        monsPiece[7, 10].resetPos = new Vector2(7, 10);

        monsPiece[3, 6] = SpawnBlackPiece(MonsPieceType.mana, 1);
        monsPiece[4, 7] = SpawnBlackPiece(MonsPieceType.mana, 1);
        monsPiece[5, 6] = SpawnBlackPiece(MonsPieceType.mana, 1);
        monsPiece[6, 7] = SpawnBlackPiece(MonsPieceType.mana, 1);
        monsPiece[7, 6] = SpawnBlackPiece(MonsPieceType.mana, 1);
    }

    private MonsPiece SpawnWhitePiece(MonsPieceType type, int team)
    {
        MonsPiece mp = Instantiate(Whiteprefabs[(int)type - 1], transform).GetComponent<MonsPiece>();
        mp.team = team;
        mp.monsPieceType = type;

        // Adjust spawn position for white pieces
        //mp.transform.position = new Vector3(0, 0, 0); // Adjust as per your design
        return mp;
    }

    private MonsPiece SpawnBlackPiece(MonsPieceType type, int team)
    {
        MonsPiece mp = Instantiate(Blackprefabs[(int)type - 1], transform).GetComponent<MonsPiece>();
        mp.team = team;
        mp.monsPieceType = type;

        // Adjust spawn position for black pieces
        //mp.transform.position = new Vector3(10, 10, 0); // Adjust as per your design
        return mp;
    }

 


    public void PositionAllPiece()
    {
        for(int y = 0; y < 11; y++)
            for(int x = 0; x < 11; x++)
                if (monsPiece[x,y] != null)
                    PositionSinglePiece(x,y,true);
    }


    public void PositionSinglePiece(int x,int y,bool force = false)
    {
        monsPiece[x, y].currentX = x;
        monsPiece[x, y].currentY = y;
        monsPiece[x, y].SetPosition(new Vector2(x,y),force);

    }


    private Vector2Int LookUpTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < boardSize; x++)
            for (int y = 0; y < boardSize; y++)
                if (cells[x, y] == hitInfo)
                    return new Vector2Int(x, y);

        return -Vector2Int.one;    // -1 -1

    }

    private void HighLightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            GameObject go = Instantiate(HighlightPrefab, new Vector3(availableMoves[i].x, availableMoves[i].y, 0),Quaternion.identity);
            go.transform.SetParent(HighlightHolder.transform, false);
        }

    }

    private void ClearHighLight()
    {
        for(int i = 0; i <  HighlightHolder.transform.childCount;i++)
        {
            Destroy(HighlightHolder.transform.GetChild(i).gameObject);  
        }
    }
    private bool MoveTo(MonsPiece cp, int x, int y)
    {
        if(!ContainsValidMoves(ref availableMoves , new Vector2(x,y))) return false;

        Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);



        if (monsPiece[x, y] != null)
        {
            Debug.Log(monsPiece[x, y].gameObject.name);
            MonsPiece ocp = monsPiece[x, y];


            if (cp.team == ocp.team)
            {
                if(cp.monsPieceType == MonsPieceType.drainer  && ocp.monsPieceType == MonsPieceType.mana)
                {
                    cp.GetComponent<Drainer>().isCarryingMana = true;
                    //monsPiece[x, y] = cp;
                    //monsPiece[previousPosition.x, previousPosition.y] = null;
                    ocp.gameObject.SetActive(false);
                    //ocp.gameObject.transform.GetComponent<Transform>().localScale = new Vector3(0.3f,0.3f,0.3f);

                    GameObject childMana = Instantiate(childManaSuperMana[0], cp.transform);
                    childMana.transform.SetParent(cp.transform, false);


                }
                else if(cp.monsPieceType == MonsPieceType.drainer && ocp.monsPieceType == MonsPieceType.supermana)                    //confusion
                {
                    cp.isCarryingSuperMana = true;
                    ocp.gameObject.SetActive(false);

                    GameObject childMana = Instantiate(childManaSuperMana[1], cp.transform);
                    childMana.transform.SetParent(cp.transform, false);

                }
                else  return false;
            }
            else
            {
                if ((cp.monsPieceType == MonsPieceType.drainer || cp.monsPieceType == MonsPieceType.mystic || cp.monsPieceType == MonsPieceType.spirit || cp.monsPieceType == MonsPieceType.demon) && ocp.monsPieceType == MonsPieceType.bombOrPortion)
                {
                     monsPiece[x, y] = null;
                     selectedPiece = cp;
                     BombOrPortionChoicePanel.SetActive(true);
                     ocp.gameObject.SetActive(false);
                }
                
                else if(cp.monsPieceType == MonsPieceType.drainer && ocp.monsPieceType == MonsPieceType.mana)
                {
                    cp.isCarryingOppMana = true;
                    ocp.gameObject.SetActive(false);
                    GameObject childMana = Instantiate(childManaSuperMana[0], cp.transform);
                    childMana.transform.SetParent(cp.transform, false);
                }
        
                else
                {


                    if (cp.monsPieceType == MonsPieceType.demon && ocp.currentX == ocp.resetPos.x && ocp.currentY == ocp.resetPos.y)
                    {
                        monsPiece[(int)ocp.resetPos.x, (int)ocp.resetPos.y] = ocp;
                        ocp.gameObject.transform.rotation = Quaternion.Euler(0, 0, -90);
                        return false;
                    }

                    if (ocp.team == 0)
                    {
                        deadWhites.Add(ocp);
                        ocp.FaintForTurns(2);
                        ocp.SetPosition(ocp.resetPos);
                        monsPiece[(int)ocp.resetPos.x, (int)ocp.resetPos.y] = ocp;
                        PositionSinglePiece((int)ocp.resetPos.x, (int)ocp.resetPos.y);
                        ocp.gameObject.transform.rotation = Quaternion.Euler(0, 0, -90);
                    }
                    else
                    {
                        deadBlacks.Add(cp);
                        ocp.FaintForTurns(2);
                        cp.SetPosition(cp.resetPos);
                        monsPiece[(int)ocp.resetPos.x, (int)ocp.resetPos.y] = ocp;
                        PositionSinglePiece((int)ocp.resetPos.x, (int)ocp.resetPos.y);
                        ocp.gameObject.transform.rotation = Quaternion.Euler(0, 0, -90);
                    }
                }


               
            }
               

            
        }

        monsPiece[x, y] = cp;
        
        monsPiece[previousPosition.x, previousPosition.y] = null;

        PositionSinglePiece(x, y);


        if (cp.monsPieceType == MonsPieceType.drainer && cp.GetComponent<Drainer>().isCarryingMana == true)
        {
            if ((x == 0 && y == 0) || (x == 0 && y == boardSize - 1) || (x == boardSize - 1 && y == 0) || (x == boardSize - 1 && y == boardSize - 1))
            {
                cp.GetComponent<Drainer>().isCarryingMana = false;

                Destroy(cp.gameObject.transform.GetChild(0).gameObject);

                if (isWhiteTurn)
                {
                    whiteScore++;
                    whiteScoreText.text = whiteScore.ToString();
                }
                else
                {
                    blackScore++;
                    blackScoreText.text = blackScore.ToString();
                }
            }

        }

        if (cp.monsPieceType == MonsPieceType.drainer && cp.isCarryingOppMana)
        {
            if ((x == 0 && y == 0) || (x == 0 && y == boardSize - 1) || (x == boardSize - 1 && y == 0) || (x == boardSize - 1 && y == boardSize - 1))
            {
                cp.isCarryingOppMana = false;

                Destroy(cp.gameObject.transform.GetChild(0).gameObject);

                if (isWhiteTurn)
                {
                    whiteScore = whiteScore + 2;
                    whiteScoreText.text = whiteScore.ToString();
                }
                else
                {
                    blackScore = blackScore + 2;
                    blackScoreText.text = blackScore.ToString();
                }
            }

        }


        if (cp.monsPieceType == MonsPieceType.drainer && cp.isCarryingSuperMana)
        {
            if ((x == 0 && y == 0) || (x == 0 && y == boardSize - 1) || (x == boardSize - 1 && y == 0) || (x == boardSize - 1 && y == boardSize - 1))
            {
                cp.isCarryingSuperMana = false;
                Destroy(cp.gameObject.transform.GetChild(0).gameObject);
                if (isWhiteTurn)
                {
                    whiteScore = whiteScore +2;
                    whiteScoreText.text = whiteScore.ToString();
                }
                else
                {
                    blackScore = blackScore + 2;
                    blackScoreText.text = blackScore.ToString();
                }
            }

        }
        



        if (cp.monsPieceType == MonsPieceType.mana)
        {
            manaTurn = false;
            isWhiteTurn = !isWhiteTurn;
            foreach (MonsPiece piece in monsPiece)
            {
                if (piece != null)
                {
                    piece.UpdateFaintedTurns();
                }
            }
            itemChances = 5;
        }else
        {
            if ((cp.monsPieceType == MonsPieceType.drainer || cp.monsPieceType == MonsPieceType.demon || cp.monsPieceType == MonsPieceType.mystic || cp.monsPieceType == MonsPieceType.spirit) && cp.isCarryingPortion)
            {
                itemChances++;
                cp.isCarryingPortion = false;
            }
            itemChances--;
        }



        if (whiteScore >= 5 || blackScore >= 5)
        {
            isGameEnd = true;
            if(whiteScore >=5)
                endGameText.text = "White Won !";
            else
                if(blackScore >=5)
                    endGameText.text = "Black Won !";

        }

        return true;

    }






    private bool ContainsValidMoves(ref List<Vector2Int> moves,Vector2 pos)
    {
        for (int i = 0; i < moves.Count; i++)
        {
            if (moves[i].x == pos.x && moves[i].y == pos.y)
            {
                return true;
            }
        }
        return false;
    }

    private void FaintOpponentPiece(MonsPiece target)
    {

        target.FaintForTurns(2);
        target.SetPosition(target.resetPos);
        monsPiece[target.currentX, target.currentY] = null;
        monsPiece[(int)target.resetPos.x, (int)target.resetPos.y] = target;
        PositionSinglePiece((int)target.resetPos.x, (int)target.resetPos.y);
        target.gameObject.transform.rotation = Quaternion.Euler(0, 0, -90);
        Debug.Log("fainted");



        //int range = 3; // Define the range for fainting opponents

        //int startX = Mathf.Max(0, drainer.currentX - range);
        //int endX = Mathf.Min(boardSize - 1, drainer.currentX + range);
        //int startY = Mathf.Max(0, drainer.currentY - range);
        //int endY = Mathf.Min(boardSize - 1, drainer.currentY + range);

        //for (int x = startX; x <= endX; x++)
        //{
        //    for (int y = startY; y <= endY; y++)
        //    {
        //        if (monsPiece[x, y] != null && monsPiece[x, y].team != drainer.team)
        //        {
        //            // Faint the opponent piece
        //            monsPiece[x, y].FaintForTurns(2);
        //            monsPiece[x, y].SetPosition(monsPiece[x, y].resetPos);
        //            monsPiece[(int)monsPiece[x, y].resetPos.x, (int)monsPiece[x, y].resetPos.y] = monsPiece[x, y];
        //            PositionSinglePiece((int)monsPiece[x, y].resetPos.x, (int)monsPiece[x, y].resetPos.y);
        //            monsPiece[x, y].gameObject.transform.rotation = Quaternion.Euler(0, 0, -90);

        //        }
        //    }
        //}
    }



    public void OnButtonBombClick()
    {
        choice = 1;
        selectedPiece.isCarryingBomb = true;
        BombOrPortionChoicePanel.SetActive(false);
    }

    public void OnButtonPortionClick()
    {
        choice = 2;
        selectedPiece.isCarryingPortion = true;
        BombOrPortionChoicePanel.SetActive(false);
    }


}

public class CellHover : MonoBehaviour
{
    private Color originalColor;
    private Color hoverColor = Color.cyan;

    void Start()
    {
        originalColor = GetComponent<SpriteRenderer>().color;
    }

    void OnMouseEnter()
    {
        GetComponent<SpriteRenderer>().color = hoverColor;
    }

    void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().color = originalColor;
    }
}