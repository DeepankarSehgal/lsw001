using ExitGames.Client.Photon;
using Org.BouncyCastle.Asn1.BC;
using Photon.Pun;
using Photon.Realtime;
using Scripts.Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviourPunCallbacks
{
    public GameObject whiteCellPrefab;
    public GameObject blackCellPrefab;
    public int boardSize = 11;
    public GameObject[,] cells;

    public MonsPiece currentlyDraggingPiece;
    public MonsPiece previousDraggingPiece;

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

    public bool isWhiteTurn;
    [SerializeField] private int itemChances = 5;
    public bool manaTurn = false;

    public TMP_Text whiteScoreText;
    public TMP_Text blackScoreText;

    private int whiteScore = 0;
    private int blackScore = 0;

    public Color[] boardColors;

    public GameObject BombOrPortionChoicePanel;

    public GameObject[] BPchoices;
    public GameObject GameoverPanel;

    int choice;

    private bool isGameEnd = false;
    public TMP_Text endGameText;


    public GameObject[] Marks;
    public GameObject[] childManaSuperMana;



    public MonsPiece selectedPiece;

    [SerializeField] public GameObject[] BombOrPortionObj;
    [SerializeField] private GameObject[] RemainingMovesHolder;
    [SerializeField] public Image player1Icon;
    [SerializeField] public GameObject player1IconPrefab;
    [SerializeField] public Image player2Icon;
    [SerializeField] public GameObject player2IconPrefab;
    public Transform PieceHolder;
    public bool startGameWhenAllReady = true;
    bool startGame = false;

    public MonsPiece superManaRef;
    public MonsPiece currentManaPickedByWhiteRef;
    public MonsPiece currentManaPickedByBlackRef;
    public Sprite player1IconRef, player2IconRef;

    [SerializeField] private GameObject localMoveHolder, remoteMoveHolder, localScoreHolder, remoteScoreHolder;
    private void Awake()
    {
        instance = this;
        startGameWhenAllReady = true;
        MonsPieceDataType.Register();
    }


    private void OnDestroy()
    {
        instance = null;
    }
    void Start()
    {
        isWhiteTurn = true;
        //CreateMonsBoard();

        //#if UNITY_EDITOR
        //        OnJoinedRoom();
        //#endif
        //SpawnAllPiece();
        // PositionAllPiece();
        //CenterBoard();
    }
    private new void OnEnable()
    {
        GameplayManager.onJoinedRoom -= OnJoinedRoom;
        GameplayManager.onJoinedRoom += OnJoinedRoom;

        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public new void OnJoinedRoom()
    {

        if (PhotonNetwork.IsConnectedAndReady)
        {
            CreateMonsBoard();
            SpawnAllPiece();
            //Invoke(nameof(PositionAllPiece), 10f);
            PositionAllPiece();
            CenterBoard();
            Invoke(nameof(DelayCall), 3f);
            startGame = true;
            onUpdatePlayerState?.Invoke(false);

            object pieceType;
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("PieceType", out pieceType);
            print("On Joined room of board get called!" + (string)pieceType);

            if ((string)pieceType == "Black")
            {
                RemainingMovesHolder[1].transform.parent = localMoveHolder.transform;
                RemainingMovesHolder[0].transform.parent = remoteMoveHolder.transform;

                RemainingMovesHolder[1].GetComponent<RectTransform>().offsetMin = Vector2.zero;
                RemainingMovesHolder[1].GetComponent<RectTransform>().offsetMax = Vector2.zero;
                RemainingMovesHolder[0].GetComponent<RectTransform>().offsetMin = Vector2.zero;
                RemainingMovesHolder[0].GetComponent<RectTransform>().offsetMax = Vector2.zero;


                whiteScoreText.transform.parent = remoteScoreHolder.transform;
                blackScoreText.transform.parent = localScoreHolder.transform;

                whiteScoreText.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                whiteScoreText.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                blackScoreText.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                blackScoreText.GetComponent<RectTransform>().offsetMax = Vector2.zero;

            }
            //onUpdatePlayerVisuals?.Invoke(true);

            //GameplayManager.startSynch?.Invoke();
        }

    }

    private void DelayCall()
    {

    }
    int tapCount = 0;
    void Update()
    {
        if (!isGameEnd && startGame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D info = Physics2D.Raycast(ray.origin, ray.direction, 100, LayerMask.GetMask("Tile", "Hover"));
            if (info)
            {
                //print("Rayhit info object: " + info.transform.name); 
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
                
                if (Input.GetMouseButton(0) && currentlyDraggingPiece == null)
                {
                    print("Hit position : " + monsPiece[hitPosition.x, hitPosition.y]);

                    if (monsPiece[hitPosition.x, hitPosition.y] != null && !monsPiece[hitPosition.x, hitPosition.y].monsPieceDataType.isFainted)
                    {

                        if ((monsPiece[hitPosition.x, hitPosition.y].monsPieceDataType.team == 0 && (isWhiteTurn || previousDraggingPiece != null && previousDraggingPiece.monsPieceDataType.monsPieceType == MonsPieceType.spirit)) || (monsPiece[hitPosition.x, hitPosition.y].monsPieceDataType.team == 1 && (!isWhiteTurn || previousDraggingPiece != null && previousDraggingPiece.monsPieceDataType.monsPieceType == MonsPieceType.spirit)))
                        {
                            print("Previous piece " + previousDraggingPiece);
                            tapCount += 1;
                            currentlyDraggingPiece = monsPiece[hitPosition.x, hitPosition.y];
                            if (currentlyDraggingPiece != null && !currentlyDraggingPiece.GetComponent<SynchronizationPlayers>().photonView.IsMine) return;
                            //print("Previous piece 1" + currentlyDraggingPiece);
                            if (itemChances > 0)
                            {
                                if (currentlyDraggingPiece.monsPieceDataType.monsPieceType == MonsPieceType.mana)
                                {
                                    //if(!currentlyDraggingPiece.monsPieceDataType.isHitBySpirit)
                                    //manaTurn = true;
                                    availableMoves = currentlyDraggingPiece.GetAvailableMoves(ref monsPiece, boardSize);
                                    HighLightTiles();
                                    //spawnMovesHiglighter = false;

                                }
                                else
                                {
                                    //FYI: here are the moves for players type..
                                    availableMoves = currentlyDraggingPiece.GetAvailableMoves(ref monsPiece, boardSize);

                                    HighLightTiles();
                                    // spawnMovesHiglighter = false;

                                }
                            }
                            else
                            {
                                if (manaTurn == false)
                                {
                                    if (currentlyDraggingPiece.monsPieceDataType.monsPieceType == MonsPieceType.mana)
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
                if (currentlyDraggingPiece != null && Input.GetMouseButtonDown(0))
                {

                    Vector2Int previousPosition = new Vector2Int(currentlyDraggingPiece.monsPieceDataType.currentX, currentlyDraggingPiece.monsPieceDataType.currentY);
                    currentlyDraggingPiece.monsPieceDataType.previousPosition = previousPosition;
                    if (availableMoves != null && availableMoves.Contains(hitPosition))
                    {
                        if (currentlyDraggingPiece.monsPieceDataType.monsPieceType != MonsPieceType.mana && itemChances <= 0)//To avoid glitch movess
                        {
                            return;
                        }
                        bool validMove = MoveTo(currentlyDraggingPiece, hitPosition.x, hitPosition.y);
                        if (!validMove)
                        {
                            if (currentlyDraggingPiece.monsPieceDataType.monsPieceType == MonsPieceType.drainer && currentlyDraggingPiece.monsPieceDataType.isCarryingBomb)
                            {
                                // Check if the clicked position is within 3 spaces and there's an opponent piece
                                if (Mathf.Abs(hitPosition.x - currentlyDraggingPiece.monsPieceDataType.currentX) <= 3 &&
                                    Mathf.Abs(hitPosition.y - currentlyDraggingPiece.monsPieceDataType.currentY) <= 3 &&
                                    monsPiece[hitPosition.x, hitPosition.y] != null &&
                                    monsPiece[hitPosition.x, hitPosition.y].monsPieceDataType.team != currentlyDraggingPiece.monsPieceDataType.team &&
                                    monsPiece[hitPosition.x, hitPosition.y].monsPieceDataType.monsPieceType != MonsPieceType.mana)
                                {
                                    // Faint the opponent piece
                                    print("Fainting the opponent piece by " + currentlyDraggingPiece);

                                    currentlyDraggingPiece.monsPieceDataType.isCarryingBomb = false;
                                    if(currentlyDraggingPiece.transform.childCount > 0)
                                    Destroy(transform.GetChild(0).gameObject);

                                    FaintOpponentPiece(monsPiece[hitPosition.x, hitPosition.y]);
                                    DrainerGotKilledWhileCarryingLogic(monsPiece[hitPosition.x, hitPosition.y]);

                                }
                            }
                            currentlyDraggingPiece.SetPosition(previousPosition);

                        }

                        tapCount = 0;
                        //ClearHighLight();

                    }


                    ClearHighLight();
                    //spawnMovesHiglighter = false;

                    //if(currentlyDraggingPiece != previousDraggingPiece /*&& tapCount <= 1*/)
                    //{
                    //    previousDraggingPiece = currentlyDraggingPiece;
                    //    tapCount = 0;
                    //}

                    currentlyDraggingPiece = null;


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
                    currentlyDraggingPiece.SetPosition(new Vector2(currentlyDraggingPiece.monsPieceDataType.currentX, currentlyDraggingPiece.monsPieceDataType.currentY));
                    currentlyDraggingPiece = null;
                    ClearHighLight();
                }
            }
        }

    }

    public void CreateMonsBoard()
    {
        cells = new GameObject[boardSize, boardSize];

        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                GameObject cellPrefab = (x + y) % 2 == 0 ? whiteCellPrefab : blackCellPrefab;
                GameObject cell = Instantiate(cellPrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                if (x == 0 && y == 0 || x == 10 && y == 0 || x == 0 && y == 10 || x == 10 && y == 10 || x == 5 && y == 5)
                {
                    cell.GetComponent<SpriteRenderer>().color = boardColors[0];
                }

                if (x == 3 && y == 4 || x == 4 && y == 3 || x == 5 && y == 4 || x == 6 && y == 3 || x == 7 && y == 4 ||
                   x == 3 && y == 6 || x == 4 && y == 7 || x == 5 && y == 6 || x == 6 && y == 7 || x == 7 && y == 6)
                {
                    cell.GetComponent<SpriteRenderer>().color = boardColors[1];
                }

                if (x == 0 && y == 5 || x == 10 && y == 5)
                {
                    cell.GetComponent<SpriteRenderer>().color = boardColors[2];
                }

                cells[x, y] = cell;
                cell.gameObject.name = x + " : " + y;

                int[] pos = { 3, 4, 5, 6, 7 };
                if (pos.Contains(x) && (y == 0 || y == 10))
                {
                    GameObject go = Instantiate(Marks[x - 3], cell.transform);
                    go.transform.SetParent(cell.transform);
                }

                cell.layer = LayerMask.NameToLayer("Tile");
                cell.AddComponent<BoxCollider2D>(); // Add collider for raycast
                cell.AddComponent<CellHover>(); // Add CellHover script for hover effect
            }
        }
    }

    public void CenterBoard()
    {
        Vector3 boardOffset = new Vector3(-(boardSize - 1) / 2f, -(boardSize - 1) / 2f, 0);
        //transform.position = new Vector3(-2.76f, -1.7f, 0);
        //transform.position = boardOffset;
    }


    public void SpawnAllPiece()
    {
        monsPiece = new MonsPiece[11, 11];

        //White Pieces
        monsPiece[3, 0] = SpawnWhitePiece(MonsPieceType.demon, 0);
        if (monsPiece[3, 0] != null)
            monsPiece[3, 0].monsPieceDataType.resetPos = new Vector2(3, 0);

        monsPiece[4, 0] = SpawnWhitePiece(MonsPieceType.angel, 0);
        if (monsPiece[4, 0] != null)
            monsPiece[4, 0].monsPieceDataType.resetPos = new Vector2(4, 0);

        monsPiece[5, 0] = SpawnWhitePiece(MonsPieceType.drainer, 0);
        if (monsPiece[5, 0] != null)
            monsPiece[5, 0].monsPieceDataType.resetPos = new Vector2(5, 0);

        monsPiece[6, 0] = SpawnWhitePiece(MonsPieceType.spirit, 0);
        if (monsPiece[6, 0] != null)
            monsPiece[6, 0].monsPieceDataType.resetPos = new Vector2(6, 0);

        monsPiece[7, 0] = SpawnWhitePiece(MonsPieceType.mystic, 0);
        if (monsPiece[7, 0] != null)
            monsPiece[7, 0].monsPieceDataType.resetPos = new Vector2(7, 0);

        monsPiece[3, 4] = SpawnWhitePiece(MonsPieceType.mana, 0);
        monsPiece[4, 3] = SpawnWhitePiece(MonsPieceType.mana, 0);
        monsPiece[5, 4] = SpawnWhitePiece(MonsPieceType.mana, 0);
        monsPiece[6, 3] = SpawnWhitePiece(MonsPieceType.mana, 0);
        monsPiece[7, 4] = SpawnWhitePiece(MonsPieceType.mana, 0);
        monsPiece[5, 5] = SpawnWhitePiece(MonsPieceType.supermana, 0);

        //Black Pieces
        monsPiece[0, 5] = SpawnBlackPiece(MonsPieceType.bombOrPortion, -1);
        monsPiece[10, 5] = SpawnBlackPiece(MonsPieceType.bombOrPortion, -1);

        monsPiece[3, 10] = SpawnBlackPiece(MonsPieceType.demon, 1);
        if (monsPiece[3, 10] != null)
            monsPiece[3, 10].monsPieceDataType.resetPos = new Vector2(3, 10);

        monsPiece[4, 10] = SpawnBlackPiece(MonsPieceType.angel, 1);
        if (monsPiece[4, 10] != null)
            monsPiece[4, 10].monsPieceDataType.resetPos = new Vector2(4, 10);

        monsPiece[5, 10] = SpawnBlackPiece(MonsPieceType.drainer, 1);
        if (monsPiece[5, 10] != null)
            monsPiece[5, 10].monsPieceDataType.resetPos = new Vector2(5, 10);

        monsPiece[6, 10] = SpawnBlackPiece(MonsPieceType.spirit, 1);
        if (monsPiece[6, 10] != null)
            monsPiece[6, 10].monsPieceDataType.resetPos = new Vector2(6, 10);

        monsPiece[7, 10] = SpawnBlackPiece(MonsPieceType.mystic, 1);
        if (monsPiece[7, 10] != null)
            monsPiece[7, 10].monsPieceDataType.resetPos = new Vector2(7, 10);

        monsPiece[3, 6] = SpawnBlackPiece(MonsPieceType.mana, 1);
        monsPiece[4, 7] = SpawnBlackPiece(MonsPieceType.mana, 1);
        monsPiece[5, 6] = SpawnBlackPiece(MonsPieceType.mana, 1);
        monsPiece[6, 7] = SpawnBlackPiece(MonsPieceType.mana, 1);
        monsPiece[7, 6] = SpawnBlackPiece(MonsPieceType.mana, 1);

        object pieceType;
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("PieceType", out pieceType);
        print("PieceType from photon received: " + (string)pieceType);
        if ((string)pieceType == "Black")
        {
            Camera.main.transform.localEulerAngles = new Vector3(0f, 0f, -180f);
            PieceHolder.transform.localEulerAngles = new Vector3(0f, 0f, 180f);

        }


    }

    private MonsPiece SpawnWhitePiece(MonsPieceType type, int team)
    {
        MonsPiece mp = null;
        object pieceType;
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("PieceType", out pieceType);
        print("PieceType from photon received: " + (string)pieceType);
        if ((string)pieceType == "White")
        {
            mp = PhotonNetwork.Instantiate(Whiteprefabs[(int)type - 1].name, transform.localPosition, Quaternion.identity).GetComponent<MonsPiece>();
            mp.tag = "White";
            mp.transform.parent = PieceHolder;
            mp.monsPieceDataType.team = team;
            mp.monsPieceDataType.monsPieceType = type;
        }
        else
        {
            //Local testing..
            //#if UNITY_EDITOR
            //            mp = Instantiate(Whiteprefabs[(int)type - 1], transform).GetComponent<MonsPiece>();
            //            mp.team = team;
            //            mp.monsPieceType = type;
            //#endif

        }
        // Adjust spawn position for white pieces
        //mp.transform.position = new Vector3(0, 0, 0); // Adjust as per your design
        return mp;
    }

    private MonsPiece SpawnBlackPiece(MonsPieceType type, int team)
    {
        MonsPiece mp = null;
        object pieceType;
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("PieceType", out pieceType);
        print("PieceType from photon received: " + (string)pieceType);
        if ((string)pieceType == "Black")
        {
            mp = PhotonNetwork.Instantiate(Blackprefabs[(int)type - 1].name, transform.localPosition, Quaternion.identity).GetComponent<MonsPiece>();
            mp.transform.parent = PieceHolder;

            mp.monsPieceDataType.team = team;
            mp.monsPieceDataType.monsPieceType = type;
        }
        else
        {
            //#if UNITY_EDITOR
            //            mp = Instantiate(Blackprefabs[(int)type - 1], transform).GetComponent<MonsPiece>();
            //            mp.team = team;
            //            mp.monsPieceType = type;
            //#endif
        }


        // Adjust spawn position for black pieces
        //mp.transform.position = new Vector3(10, 10, 0); // Adjust as per your design
        return mp;
    }




    public void PositionAllPiece()
    {
        for (int y = 0; y < 11; y++)
            for (int x = 0; x < 11; x++)
                if (monsPiece[x, y] != null)
                    PositionSinglePiece(x, y, true);
    }


    public void PositionSinglePiece(int x, int y, bool force = false)
    {

        monsPiece[x, y].monsPieceDataType.currentX = x;
        monsPiece[x, y].monsPieceDataType.currentY = y;
        monsPiece[x, y].SetPosition(new Vector2(x, y), force);
        print("MONS PIECE Position: " + monsPiece[x, y].monsPieceDataType.team + ": x: " + x + " y: " + y);

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
        if (availableMoves == null) return;
        Color highlightColor = Color.green;
        for (int i = 0; i < availableMoves.Count; i++)
        {
            if (currentlyDraggingPiece.monsPieceDataType.isHitBySpirit)
            {
                highlightColor = Color.red;
            }
            GameObject go = Instantiate(HighlightPrefab, new Vector3(availableMoves[i].x, availableMoves[i].y, 0), Quaternion.identity);
            go.GetComponent<SpriteRenderer>().color = highlightColor;
            go.transform.SetParent(HighlightHolder.transform, false);
        }

    }

    private void ClearHighLight()
    {
        print("clear highlight!");
        for (int i = 0; i < HighlightHolder.transform.childCount; i++)
        {
            Destroy(HighlightHolder.transform.GetChild(i).gameObject);
        }
    }
    private bool isMysticAttackingTheOpponentPlayer = false;
    private bool MoveTo(MonsPiece cp, int x, int y)
    {
        if (!ContainsValidMoves(ref availableMoves, new Vector2(x, y))) return false;

        Vector2Int previousPosition = new Vector2Int(cp.monsPieceDataType.currentX, cp.monsPieceDataType.currentY);


        MonsPiece ocp = null;
        if (monsPiece[x, y] != null)
        {
            Debug.Log(monsPiece[x, y].gameObject.name + "Hitted by " + cp.name);//hit pieces player name

            ocp = monsPiece[x, y];
            previousDraggingPiece = cp;
            Vector2Int previousPosition1 = new Vector2Int(ocp.monsPieceDataType.currentX, ocp.monsPieceDataType.currentY);
            ocp.monsPieceDataType.previousPosition = previousPosition1;
            bool isHitBySpirit = SpiritPushOtherPlayersLogic(ref cp, ref ocp);
            print("Opponent piece: " + cp.name + isHitBySpirit);
            if (isHitBySpirit)
            {
                isHitBySpirit = false;
                manaTurn = false;
                return true;
            }
            cp.monsPieceDataType.mySpecialAbilityUsed = true;

            if (cp.monsPieceDataType.team == ocp.monsPieceDataType.team)
            {
                if (cp.monsPieceDataType.monsPieceType == MonsPieceType.drainer && (!cp.monsPieceDataType.isCarryingSuperMana && !cp.monsPieceDataType.isCarryingOppMana && !cp.monsPieceDataType.isCarryingMana) && ocp.monsPieceDataType.monsPieceType == MonsPieceType.mana)
                {
                    cp.monsPieceDataType.isCarryingMana = true;
                    //monsPiece[x, y] = cp;
                    //monsPiece[previousPosition.x, previousPosition.y] = null;
                    ocp.monsPieceDataType.isCarriedByDrainer = true;
                    ocp.GetComponent<SynchronizationPlayers>().OnUpdatePlayerState(false);
                    ocp.gameObject.SetActive(false);
                    if(cp.monsPieceDataType.team == 0)
                    currentManaPickedByWhiteRef = ocp;
                    else
                    currentManaPickedByBlackRef = ocp;
                    //ocp.gameObject.transform.GetComponent<Transform>().localScale = new Vector3(0.3f,0.3f,0.3f);
                    if (cp.monsPieceDataType.team == 0) //white
                    {
                        GameObject childMana = Instantiate(childManaSuperMana[0], cp.transform);
                        childMana.transform.SetParent(cp.transform, false);
                    }
                    else
                    {
                        GameObject childMana = Instantiate(childManaSuperMana[1], cp.transform);
                        childMana.transform.SetParent(cp.transform, false);
                    }
                }

                else if (cp.monsPieceDataType.monsPieceType == MonsPieceType.drainer && (!cp.monsPieceDataType.isCarryingSuperMana && !cp.monsPieceDataType.isCarryingOppMana && !cp.monsPieceDataType.isCarryingMana) && ocp.monsPieceDataType.monsPieceType == MonsPieceType.supermana)//confusion
                {
                    cp.monsPieceDataType.isCarryingSuperMana = true;
                    ocp.gameObject.SetActive(false);
                    ocp.monsPieceDataType.isCarriedByDrainer = true;
                    superManaRef = ocp;
                    ocp.GetComponent<SynchronizationPlayers>().OnUpdatePlayerState(false);
                    GameObject childMana = Instantiate(childManaSuperMana[2], cp.transform);
                    childMana.transform.SetParent(cp.transform, false);
                    print("Supermana picked!");

                }
                else return false;
            }
            else
            {
                //when both are not in the same team
                if (cp.monsPieceDataType.monsPieceType == MonsPieceType.drainer && (!cp.monsPieceDataType.isCarryingSuperMana && !cp.monsPieceDataType.isCarryingOppMana && !cp.monsPieceDataType.isCarryingMana) && ocp.monsPieceDataType.monsPieceType == MonsPieceType.supermana)//confusion
                {
                    cp.monsPieceDataType.isCarryingSuperMana = true;
                  
                    ocp.monsPieceDataType.isCarriedByDrainer = true;
                    superManaRef = ocp;
                    ocp.GetComponent<SynchronizationPlayers>().OnUpdatePlayerState(false);
                    ocp.gameObject.SetActive(false);
                    GameObject childMana = Instantiate(childManaSuperMana[2], cp.transform);
                    childMana.transform.SetParent(cp.transform, false);
                    print("Supermana picked!");

                }



                if ((cp.monsPieceDataType.monsPieceType == MonsPieceType.drainer || cp.monsPieceDataType.monsPieceType == MonsPieceType.mystic || cp.monsPieceDataType.monsPieceType == MonsPieceType.spirit || cp.monsPieceDataType.monsPieceType == MonsPieceType.demon || cp.monsPieceDataType.monsPieceType == MonsPieceType.angel) && !cp.monsPieceDataType.isCarryingBomb && !cp.monsPieceDataType.isCarryingPortion && ocp.monsPieceDataType.monsPieceType == MonsPieceType.bombOrPortion)
                {
                    monsPiece[x, y] = null;
                    selectedPiece = cp;
                    BombOrPortionChoicePanel.SetActive(true);
                    ocp.monsPieceDataType.isFainted = true;
                    ocp.gameObject.GetComponent<SynchronizationPlayers>().OnUpdatePlayerState(false);
                    
                    ocp.gameObject.SetActive(false);
                    
                    //cp.monsPieceDataType.mySpecialAbilityUsed = false;
                }

                else if (cp.monsPieceDataType.monsPieceType == MonsPieceType.drainer && (!cp.monsPieceDataType.isCarryingSuperMana && !cp.monsPieceDataType.isCarryingOppMana && !cp.monsPieceDataType.isCarryingMana) && ocp.monsPieceDataType.monsPieceType == MonsPieceType.mana)//carrying mana when both are not in the same team
                {
                    cp.monsPieceDataType.isCarryingOppMana = true;
                    ocp.gameObject.SetActive(false);
                    ocp.monsPieceDataType.isCarriedByDrainer = true;
                    if (cp.monsPieceDataType.team == 0)
                        currentManaPickedByWhiteRef = ocp;
                    else
                        currentManaPickedByBlackRef = ocp;
                    ocp.GetComponent<SynchronizationPlayers>().OnUpdatePlayerState(false);
                    if (ocp.monsPieceDataType.team == 0)
                    {
                        GameObject childMana = Instantiate(childManaSuperMana[0], cp.transform);
                        childMana.transform.SetParent(cp.transform, false);
                    }
                    else
                    {
                        GameObject childMana = Instantiate(childManaSuperMana[1], cp.transform);
                        childMana.transform.SetParent(cp.transform, false);
                    }

                }

                else
                {
                    //demon kill 
                    if (cp.monsPieceDataType.monsPieceType == MonsPieceType.demon && ocp.monsPieceDataType.currentX != ocp.monsPieceDataType.resetPos.x && ocp.monsPieceDataType.currentY != ocp.monsPieceDataType.resetPos.y)
                    {
                        monsPiece[(int)ocp.monsPieceDataType.resetPos.x, (int)ocp.monsPieceDataType.resetPos.y] = ocp;
                        ocp.monsPieceDataType.isFainted = true;
                        ocp.gameObject.transform.rotation = Quaternion.Euler(0, 0, -90);
                        ocp.SetPosition(ocp.monsPieceDataType.resetPos);
                        faintPlayers.Add(ocp);
                        ocp.GetComponent<SynchronizationPlayers>().OnUpdatePlayerState(true);
                        if (cp.monsPieceDataType.isCarryingBomb)
                        {
                            if (currentlyDraggingPiece.transform.childCount > 0)
                                Destroy(transform.GetChild(0).gameObject);
                        }
                        cp.monsPieceDataType.isCarryingBomb = false;
                       
                        DrainerGotKilledWhileCarryingLogic(ocp);



                        return false;
                    }

                    if (ocp.monsPieceDataType.team == 0) //white team
                    {
                        if ((cp.monsPieceDataType.monsPieceType == MonsPieceType.mystic || cp.monsPieceDataType.isCarryingBomb) && ocp.monsPieceDataType.monsPieceType != MonsPieceType.bombOrPortion)
                        {
                            isMysticAttackingTheOpponentPlayer = true;

                        }
                        deadWhites.Add(ocp);
                        ocp.FaintForTurns(2);
                        if (!isMysticAttackingTheOpponentPlayer)
                        {
                            cp.SetPosition(ocp.monsPieceDataType.resetPos);

                        }
                        monsPiece[(int)ocp.monsPieceDataType.desiredPos.x, (int)ocp.monsPieceDataType.desiredPos.y] = null;
                        //PositionSinglePiece((int)ocp.monsPieceDataType.resetPos.x, (int)ocp.monsPieceDataType.resetPos.y);
                        ocp.SetPosition(ocp.monsPieceDataType.resetPos);

                        ocp.gameObject.transform.rotation = Quaternion.Euler(0, 0, -90);
                        faintPlayers.Add(ocp);
                        //remove bomb if carrying
                        if (cp.monsPieceDataType.isCarryingBomb)
                        {
                            cp.monsPieceDataType.isCarryingBomb = false;
                            if (currentlyDraggingPiece.transform.childCount > 0)
                                Destroy(transform.GetChild(0).gameObject);

                        }
                        ocp.GetComponent<SynchronizationPlayers>().OnUpdatePlayerState(true);
                        DrainerGotKilledWhileCarryingLogic(ocp);

                    }
                    else
                    {
                        //black team
                        if ((cp.monsPieceDataType.monsPieceType == MonsPieceType.mystic || cp.monsPieceDataType.isCarryingBomb) && ocp.monsPieceDataType.monsPieceType != MonsPieceType.bombOrPortion)
                        {
                            isMysticAttackingTheOpponentPlayer = true;

                        }
                        deadBlacks.Add(cp);
                        ocp.FaintForTurns(2);

                        if (!isMysticAttackingTheOpponentPlayer)
                        {
                            cp.SetPosition(cp.monsPieceDataType.resetPos);

                        }
                        //monsPiece[(int)ocp.monsPieceDataType.resetPos.x, (int)ocp.monsPieceDataType.resetPos.y] = ocp;
                        monsPiece[(int)ocp.monsPieceDataType.desiredPos.x, (int)ocp.monsPieceDataType.desiredPos.y] = null;
                        ocp.SetPosition(ocp.monsPieceDataType.resetPos);

                        //PositionSinglePiece((int)ocp.monsPieceDataType.resetPos.x, (int)ocp.monsPieceDataType.resetPos.y);
                        ocp.gameObject.transform.rotation = Quaternion.Euler(0, 0, -90);
                        faintPlayers.Add(ocp);
                        //remove bomb if carrying
                        if (cp.monsPieceDataType.isCarryingBomb)
                        {
                            cp.monsPieceDataType.isCarryingBomb = false;
                            if (currentlyDraggingPiece.transform.childCount > 0)
                                Destroy(transform.GetChild(0).gameObject);
                        }
                        ocp.GetComponent<SynchronizationPlayers>().OnUpdatePlayerState(true);
                        DrainerGotKilledWhileCarryingLogic(ocp);

                    }
                }



            }



        }


        //onUpdatePlayerState?.Invoke();
        //resetting here at last


        if (!isMysticAttackingTheOpponentPlayer)
        {
            monsPiece[x, y] = cp;
            monsPiece[previousPosition.x, previousPosition.y] = null;
        }



        if (!isMysticAttackingTheOpponentPlayer)
        {
            PositionSinglePiece(x, y);
            currentlyDraggingPiece.GetComponent<SynchronizationPlayers>().OnUpdatePlayerState(true);
        }
        else
        {
            isMysticAttackingTheOpponentPlayer = false;
            currentlyDraggingPiece.GetComponent<SynchronizationPlayers>().OnUpdatePlayerState(false);

        }


        //scoring logic..

        if (cp.monsPieceDataType.monsPieceType == MonsPieceType.drainer && cp.monsPieceDataType.isCarryingMana == true)
        {
            if ((x == 0 && y == 0) || (x == 0 && y == boardSize - 1) || (x == boardSize - 1 && y == 0) || (x == boardSize - 1 && y == boardSize - 1))
            {
                cp.monsPieceDataType.isCarryingMana = false;

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
                cp.GetComponent<SynchronizationPlayers>().UpdateScore(whiteScore, blackScore);
            }

        }

        if (cp.monsPieceDataType.monsPieceType == MonsPieceType.drainer && cp.monsPieceDataType.isCarryingOppMana)
        {
            if ((x == 0 && y == 0) || (x == 0 && y == boardSize - 1) || (x == boardSize - 1 && y == 0) || (x == boardSize - 1 && y == boardSize - 1))
            {
                cp.monsPieceDataType.isCarryingOppMana = false;

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
                cp.GetComponent<SynchronizationPlayers>().UpdateScore(whiteScore, blackScore);

            }

        }


        if (cp.monsPieceDataType.monsPieceType == MonsPieceType.drainer && cp.monsPieceDataType.isCarryingSuperMana)
        {
            if ((x == 0 && y == 0) || (x == 0 && y == boardSize - 1) || (x == boardSize - 1 && y == 0) || (x == boardSize - 1 && y == boardSize - 1))
            {
                cp.monsPieceDataType.isCarryingSuperMana = false;
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
                cp.GetComponent<SynchronizationPlayers>().UpdateScore(whiteScore, blackScore);
                cp.GetComponent<SynchronizationPlayers>().OnUpdatePlayerState(true);

            }

        }




        if (cp.monsPieceDataType.monsPieceType == MonsPieceType.mana)
        {
            print("Mana move logic: " + cp.monsPieceDataType.monsPieceType + cp.monsPieceDataType.isHitBySpirit + manaTurn);
            if (cp.monsPieceDataType.isHitBySpirit)//spirit move logic
            {

                previousDraggingPiece.monsPieceDataType.mySpecialAbilityUsed = true;
                previousDraggingPiece.monsPieceDataType.onceAbilityUsed = false;
                if (cp.monsPieceDataType.team == 1 && isWhiteTurn) //teams is black but white chance is going on and not finished 
                {
                    previousDraggingPiece = cp;
                }
                manaTurn = false;
                cp.monsPieceDataType.isHitBySpirit = false;
                print("Mana move logic 1: " + cp.monsPieceDataType.monsPieceType + cp.monsPieceDataType.isHitBySpirit + manaTurn);

                return true;
            }
            //Reset special ability on turn end.
            if(previousDraggingPiece!=null)
            {
                previousDraggingPiece.monsPieceDataType.mySpecialAbilityUsed=false;
            }
            manaTurn = false;
            //if (previousDraggingPiece != null)
            //    previousDraggingPiece.monsPieceDataType.mySpecialAbilityUsed = false;

            //if (ocp.isFainted)
            //{
            //    cp.isFainted = false;
            //}
            //reset faint players
            if (faintPlayers != null)
            {
                foreach (MonsPiece monsPiece in faintPlayers)
                {
                    if (isWhiteTurn && monsPiece.monsPieceDataType.team == 1)
                    {
                        monsPiece.monsPieceDataType.blackFaintGaps++;
                        monsPiece.GetComponent<SynchronizationPlayers>().OnUpdatePlayerState(false);

                    }
                    else
                    {
                        monsPiece.monsPieceDataType.whiteFaintGaps++;
                        monsPiece.GetComponent<SynchronizationPlayers>().OnUpdatePlayerState(false);

                    }
                    if (!isWhiteTurn && monsPiece.monsPieceDataType.team == 0 && monsPiece.monsPieceDataType.isFainted && monsPiece.monsPieceDataType.whiteFaintGaps > 1)//for white faint players
                    {
                        monsPiece.transform.localEulerAngles = Vector3.zero;
                        monsPiece.monsPieceDataType.isFainted = false;
                    }
                    else if (isWhiteTurn && monsPiece.monsPieceDataType.team == 1 && monsPiece.monsPieceDataType.isFainted && monsPiece.monsPieceDataType.blackFaintGaps > 1)//for black faint players
                    {
                        monsPiece.transform.localEulerAngles = Vector3.zero;
                        monsPiece.monsPieceDataType.isFainted = false;
                    }



                }
            }
            UpdatePlayerTurns(!isWhiteTurn);
            //isWhiteTurn = !isWhiteTurn;

            //#if !UNITY_EDITOR
            //            UpdatePlayerTurns(!isWhiteTurn);
            //#endif

            //Faint reset logic 
            foreach (MonsPiece piece in monsPiece)
            {
                if (piece != null)
                {
                    // piece.UpdateFaintedTurns();
                }
            }
            //Mana score logic 
            ManaScoreLogic(cp);
            //UpdateRemainingMove(cp.monsPieceDataType);

         
            previousDraggingPiece = cp;
            itemChances = 5;
            currentlyDraggingPiece.monsPieceDataType.itemChances = itemChances;
            SendCustomType(cp.monsPieceDataType);
        }
        else
        {
            if ((cp.monsPieceDataType.monsPieceType == MonsPieceType.drainer || cp.monsPieceDataType.monsPieceType == MonsPieceType.demon || cp.monsPieceDataType.monsPieceType == MonsPieceType.mystic || cp.monsPieceDataType.monsPieceType == MonsPieceType.spirit) && cp.monsPieceDataType.isCarryingPortion && ocp != null)
            {
                itemChances++;
                cp.monsPieceDataType.isCarryingPortion = false;
            }
            if (cp.monsPieceDataType.isHitBySpirit)//spirit move logic
            {
                itemChances++;
                cp.monsPieceDataType.isHitBySpirit = false;
                if (cp.monsPieceDataType.team == 1 && isWhiteTurn) //teams is black but white chance is going on and not finished 
                {
                    previousDraggingPiece = cp;
                }

            }
            previousDraggingPiece = cp;
            //UpdateRemainingMove(cp.monsPieceDataType);
        
            itemChances--;
            cp.monsPieceDataType.itemChances = itemChances;
            SendCustomType(cp.monsPieceDataType);
            print("else part of mana item chances coming" + itemChances);

    
        }

        if (itemChances <= 0)
        {
            cp.monsPieceDataType.isCarryingPortion = false;
        }

        return true;

    }
    
    private void GameWinLogic()
    {

        if (whiteScore >= 5 || blackScore >= 5) // for testing turning it to 1 by default is 5
        {
            isGameEnd = true;
            if (whiteScore >= 5)
            {
                endGameText.text = "White Won !";
            }

            else if (blackScore >= 5)
            {
                endGameText.text = "Black Won !";
            }
            TurnOnGameObject(endGameText.text);
        }
    }



    private void DrainerGotKilledWhileCarryingLogic(MonsPiece ocp)
    {
        if (ocp.monsPieceDataType.monsPieceType == MonsPieceType.drainer)
        {
            if (ocp.monsPieceDataType.isCarryingSuperMana)
            {
                superManaRef.monsPieceDataType.isFainted = false;
                superManaRef.monsPieceDataType.isCarriedByDrainer = false;
                superManaRef.monsPieceDataType.desiredPos = new Vector3(5f, 5f, 5f);
                superManaRef.gameObject.transform.localEulerAngles = Vector3.zero;
                monsPiece[5, 5] = superManaRef;
                superManaRef.gameObject.SetActive(true);
                ocp.GetComponent<SynchronizationPlayers>().OnUpdatePlayerState(false);
            }

            if (ocp.monsPieceDataType.isCarryingMana || ocp.monsPieceDataType.isCarryingOppMana)
            {
                if(ocp.monsPieceDataType.team == 0)
                {
                    currentManaPickedByWhiteRef.monsPieceDataType.isFainted = false;
                    currentManaPickedByWhiteRef.monsPieceDataType.isCarriedByDrainer = false;
                    currentManaPickedByWhiteRef.monsPieceDataType.desiredPos = new Vector3(ocp.monsPieceDataType.currentX, ocp.monsPieceDataType.currentY, 0f);
                    monsPiece[(int)currentManaPickedByWhiteRef.monsPieceDataType.desiredPos.x, (int)currentManaPickedByWhiteRef.monsPieceDataType.desiredPos.y] = currentManaPickedByWhiteRef;
                    currentManaPickedByWhiteRef.monsPieceDataType.currentX = (int)currentManaPickedByWhiteRef.monsPieceDataType.desiredPos.x;
                    currentManaPickedByWhiteRef.monsPieceDataType.currentY = (int)currentManaPickedByWhiteRef.monsPieceDataType.desiredPos.y;
                    currentManaPickedByWhiteRef.gameObject.SetActive(true);
                }
                else
                {
                    currentManaPickedByBlackRef.monsPieceDataType.isFainted = false;
                    currentManaPickedByBlackRef.monsPieceDataType.isCarriedByDrainer = false;
                    currentManaPickedByBlackRef.monsPieceDataType.desiredPos = new Vector3(ocp.monsPieceDataType.currentX, ocp.monsPieceDataType.currentY, 0f);
                    monsPiece[(int)currentManaPickedByBlackRef.monsPieceDataType.desiredPos.x, (int)currentManaPickedByBlackRef.monsPieceDataType.desiredPos.y] = currentManaPickedByBlackRef;
                    currentManaPickedByBlackRef.monsPieceDataType.currentX = (int)currentManaPickedByBlackRef.monsPieceDataType.desiredPos.x;
                    currentManaPickedByBlackRef.monsPieceDataType.currentY = (int)currentManaPickedByBlackRef.monsPieceDataType.desiredPos.y;
                    currentManaPickedByBlackRef.gameObject.SetActive(true);
                }
        
                ocp.GetComponent<SynchronizationPlayers>().OnUpdatePlayerState(false);
                print("Current Mana");
            }
        }
    }
    private void ManaScoreLogic(MonsPiece cp)
    {
        if (cp == null) return;
        Vector3 desiredPos = cp.monsPieceDataType.desiredPos;
        if (desiredPos == new Vector3(0f, 0f, 0f) || desiredPos == new Vector3(0f, 10f, 0f) || desiredPos == new Vector3(10f, 10f, 0f) || desiredPos == new Vector3(10f, 0f, 0f))
        {
            if (cp.monsPieceDataType.team == 0)//white Mana
            {
                whiteScore++;
                whiteScoreText.text = whiteScore.ToString();
            }
            else
            {
                blackScore++;
                blackScoreText.text = blackScore.ToString();
            }
            cp.gameObject.SetActive(false);

        }
    }

    private bool SpiritPushOtherPlayersLogic(ref MonsPiece cp, ref MonsPiece ocp)
    {
        if (cp.monsPieceDataType.monsPieceType == MonsPieceType.spirit && !cp.monsPieceDataType.mySpecialAbilityUsed && ocp.monsPieceDataType.monsPieceType != MonsPieceType.bombOrPortion && !cp.monsPieceDataType.isCarryingBomb)
        {
            //spirit logic 
            ocp.monsPieceDataType.isHitBySpirit = true;
            cp.monsPieceDataType.onceAbilityUsed = true;
            //cp.monsPieceDataType.mySpecialAbilityUsed = true;
            return true;
        }

        return false;
    }

    public Action<bool> onUpdatePlayerTurn;
    public Action<bool> onUpdatePlayerVisuals;
    public Action<bool> onUpdatePlayerState;
    public void UpdatePlayerTurns(bool swapTurn = false)
    {
        isWhiteTurn = swapTurn;
        onUpdatePlayerTurn?.Invoke(isWhiteTurn);
    }


    private bool ContainsValidMoves(ref List<Vector2Int> moves, Vector2 pos)
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

    public void SynchScore(int whiteScore, int blackScore)
    {
        if (isWhiteTurn)
        {
            this.whiteScore = whiteScore;
            whiteScoreText.text = whiteScore.ToString();
        }
        else
        {
            this.blackScore = blackScore;
            blackScoreText.text = blackScore.ToString();
        }

        GameWinLogic();
    }
    [SerializeField] private List<MonsPiece> faintPlayers = new List<MonsPiece>();

    private void FaintOpponentPiece(MonsPiece target)
    {

        target.FaintForTurns(2);
        target.SetPosition(target.monsPieceDataType.resetPos);
        monsPiece[target.monsPieceDataType.currentX, target.monsPieceDataType.currentY] = null;
        monsPiece[(int)target.monsPieceDataType.resetPos.x, (int)target.monsPieceDataType.resetPos.y] = target;
        PositionSinglePiece((int)target.monsPieceDataType.resetPos.x, (int)target.monsPieceDataType.resetPos.y);
        target.gameObject.transform.rotation = Quaternion.Euler(0, 0, -90);
        Debug.Log("fainted");
        faintPlayers.Add(target);


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
        //if (selectedPiece != null)
        //{
        //    bool isCarryingBomb = selectedPiece.monsPieceDataType.isCarryingBomb;
        //    if (isCarryingBomb)
        //    {
        //        return;
        //    }
        //}
        selectedPiece.monsPieceDataType.isCarryingBomb = true;
        GameObject Bomb = Instantiate(BombOrPortionObj[0]);
        Bomb.transform.SetParent(selectedPiece.transform);
        Bomb.transform.localPosition = new Vector3(0.1f, -0.22f);
        Bomb.transform.localScale = new Vector3(0.6f, 0.6f);
        Bomb.SetActive(true);
        selectedPiece.GetComponent<SynchronizationPlayers>().OnUpdatePlayerState(false);
        BombOrPortionChoicePanel.SetActive(false);
    }

    public void OnButtonPortionClick()
    {
        choice = 2;
        selectedPiece.monsPieceDataType.isCarryingPortion = true;
        GameObject Bomb = Instantiate(BombOrPortionObj[1]);
        Bomb.transform.SetParent(selectedPiece.transform);
        Bomb.transform.localPosition = new Vector3(0.1f, -0.22f);
        Bomb.transform.localScale = new Vector3(0.6f, 0.6f);
        Bomb.SetActive(true);
        selectedPiece.GetComponent<SynchronizationPlayers>().OnUpdatePlayerState(false);
        BombOrPortionChoicePanel.SetActive(false);
    }


    private void ResetRemainingMovesHolder()
    {
        for (int i = 0; i < RemainingMovesHolder[0].transform.childCount; i++)
        {
            RemainingMovesHolder[0].transform.GetChild(i).gameObject.SetActive(true);
            RemainingMovesHolder[1].transform.GetChild(i).gameObject.SetActive(true);
        }
    }
    [PunRPC]
    public void UpdatePlayerIcon()
    {
        //if (PhotonNetwork.IsMasterClient)

        object pieceType;
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("PieceType", out pieceType);
        print("PieceType from photon received: " + (string)pieceType);
        if ((string)pieceType == "Black")
        {

            //UpdateBlackIcon();

        }
        else
        {
           // UpdateWhiteIcon();
        }

    }
    public void UpdateRemainingMove(MonsPieceDataType cp)
    {
        if (cp.monsPieceType == MonsPieceType.mana)
        {
            //All remaining moves over for that current Player
            if (cp.team == 0)
            {
                RemainingMovesHolder[0].SetActive(false);
                RemainingMovesHolder[1].SetActive(true);
            }
            else
            {
                RemainingMovesHolder[1].SetActive(false);
                RemainingMovesHolder[0].SetActive(true);

            }
            ResetRemainingMovesHolder();

        }
        else
        {
            print("Update remaing moves else part" + cp.mySpecialAbilityUsed + cp.onceAbilityUsed);
            if (cp.onceAbilityUsed || !cp.mySpecialAbilityUsed)
            {
                if (cp.itemChances > 5 || cp.itemChances < 0) return;
                if (cp.team == 0)
                {
                    print("Item Chances index" + cp.itemChances);
                    RemainingMovesHolder[0].transform.GetChild(cp.itemChances).gameObject.SetActive(false);
                }
                else
                {
                    RemainingMovesHolder[1].transform.GetChild(cp.itemChances).gameObject.SetActive(false);
                }

            }
            else
            {
                if (cp.team == 0)
                {
                    RemainingMovesHolder[0].transform.GetChild(5).gameObject.SetActive(false);
                }
                else
                {
                    RemainingMovesHolder[1].transform.GetChild(5).gameObject.SetActive(false);
                }
                itemChances++;
                cp.itemChances = itemChances;
                cp.onceAbilityUsed = true;
                if(previousDraggingPiece!=null)
                previousDraggingPiece.monsPieceDataType.onceAbilityUsed = true;
                SendCustomType(cp);
                //cp.GetComponent<SynchronizationPlayers>().OnUpdatePlayerState(false);
                print("Update remaing moves else part" + cp.mySpecialAbilityUsed + " " + cp.onceAbilityUsed);


            }
        }

        // onUpdatePlayerState?.Invoke(cp);


    }


    //PUN2
    bool sendCustomData = true;
    public void SendCustomType(MonsPieceDataType customData)
    {
        PhotonNetwork.RaiseEvent(0, customData, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {

        if (photonEvent.Code == 0)//For updating on movement and remaining moving within the room
        {
            MonsPieceDataType receivedData = (MonsPieceDataType)photonEvent.CustomData;
            Debug.Log("Received custom data: " + receivedData.team + " PieceType: " + receivedData.monsPieceType);
        

            UpdateRemainingMove(receivedData);
        }
        //if (photonEvent.Code == 1)//For updating Board respective pieces
        //{
        //    MonsPieceDataType receivedData = (MonsPieceDataType)photonEvent.CustomData;
        //    Debug.Log("Received custom data: " + receivedData.team +  " PieceType: " +  receivedData.monsPieceType);
        //    UpdatePiecePositionRelativeToBoard(receivedData);
        //}
    }

    private void UpdatePiecePositionRelativeToBoard(MonsPiece piece)
    {
        Vector2 currentPos = piece.monsPieceDataType.desiredPos;
        monsPiece[(int)currentPos.x, (int)currentPos.y] = piece;
    }

    [SerializeField] private TextMeshProUGUI playerWonText;
    [PunRPC]
    void TurnOnGameObject(string whoWonText)
    {
        //WON TEXT
        playerWonText.text = whoWonText;
        GameoverPanel.SetActive(true);
    }

    new void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        GameplayManager.onJoinedRoom -= OnJoinedRoom;
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