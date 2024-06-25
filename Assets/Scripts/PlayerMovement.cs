using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Photon.Pun;
using Cinemachine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    public float speed = 5f;
    public Rigidbody rb;
    public Animator animator;
    private Vector2 targetPosition;
    private bool isMovingToClick = false;
    Vector3 movement;
    private GameObject currentRock;
    private bool canSwing = false;
    private bool coolDown = false;
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    public GameObject canvas;
    public GameObject CoinPrefab;
    public int coinCount;
    public TMP_InputField chatInputField;
    private bool inputFieldSelected = false;
    bool movingTowardsrock = false;
    bool closeTorock = false;
    public float groundDist;
    private PhotonView photonView;
    public CinemachineVirtualCamera virtualCamera;
    public GameObject MonsCanvas;
    [SerializeField]private GameObject otherCollided;

    public LayerMask groundLayer;
    // private PlayFabManager playFabManager;



    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        canvas = PhotonManager.instance.canvas;

        if (photonView.IsMine)
        {
            GetComponent<PlayerMovement>().enabled = true;
            virtualCamera.enabled = true;
        }
        else
        {
            GetComponent<PlayerMovement>().enabled = false;
            virtualCamera.enabled = false;
        }
        //playFabManager = FindObjectOfType<PlayFabManager>();
        //  playFabManager.RetrieveCoins();
    }
    bool rayClickedToMove = false;
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        RaycastFromMouse();
        //if (PhotonChatManager.Instance.chatField.GetComponent<TMP_InputField>().isFocused)
        //{
        //    return;
        //}

        Ray mousePosition = Camera.main.ScreenPointToRay(Input.mousePosition);
        //mousePosition.z = 0;
        RaycastHit hit;
        bool hit1 = Physics.Raycast(mousePosition.origin, Vector3.forward,out hit, 1000f);

        if(hit.collider!=null)
        print("RayCastHit: " + hit.collider.gameObject.name);

        Vector3 castpos = transform.position;
        castpos.y += 1;
        //if (Physics.Raycast(castpos, -transform.up, out hit2, Mathf.Infinity, groundLayer))
        //{
        //    if (hit2.collider != null)
        //    {
        //        Vector3 movePos = transform.position;
        //        movePos.y = hit.point.y + groundDist;
        //        transform.position = movePos;
        //    }
        //}
        if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider != null && hit.collider.CompareTag("Ground"))
            {
                print("ground hit");
                movingTowardsrock = true;
                currentRock = hit.collider.gameObject; // Set currentRock to the clicked rock
                targetPosition = currentRock.transform.position;

                if (Vector2.Distance(transform.position, currentRock.transform.position) > 1f)
                {
                    isMovingToClick = true; // Move towards the clicked rock
                }
            }
            else
            {
                targetPosition = hit.point;
                isMovingToClick = true;
            }
        }

        if (movingTowardsrock)
        {
            if (Vector2.Distance(transform.position, currentRock.transform.position) < 1f)
            {
                closeTorock = true;
            }

            if (closeTorock)
            {
                // RockCollision();
                movingTowardsrock = false;
                closeTorock = false;
            }
        }

        // Check for keyboard input to override click-to-move
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            isMovingToClick = false; // Cancel click-to-move if there's keyboard input
        }

        if (isMovingToClick)
        {
            movement = (targetPosition - (Vector2)transform.position).normalized;
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMovingToClick = false;
                movement = Vector2.zero; // Stop movement when the target is reached
                                         //if(hit.collider.CompareTag("Rock")) RockCollision();
            }
        }
        else
        {
            if (!PhotonChatManager.Instance.chatField.GetComponent<TMP_InputField>().isFocused)
            {
                movement.x = Input.GetAxisRaw("Horizontal");
                movement.y = Input.GetAxisRaw("Vertical");
            }
           
        }


        if (rayClickedToMove)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, rayclickPosition, Time.deltaTime * 2f);
        }
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);


        if (otherCollided != null && otherCollided.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                MonsCanvas.SetActive(photonView.IsMine);
                //photonView.RPC(nameof(ShowChallengeUI), RpcTarget.Others, otherCollided.name);
            }
        }
        //if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) && canSwing && !coolDown)
        //{
        //    RockCollision();
        //}  
    }
    Vector3 rayclickPosition;
    void RaycastFromMouse()
    {
        // Create a ray from the camera to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Variable to store hit information
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            rayclickPosition = hit.point;
            rayClickedToMove = true;
        }
    }

    void FixedUpdate()
    {
        if (!PhotonChatManager.Instance.chatField.GetComponent<TMP_InputField>().isFocused)
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            Vector3 moveDir = new Vector3(x, 0, y);
            rb.velocity = moveDir * speed;
        }
            
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Rock"))
        {
            Debug.Log("Rock Collision");
            currentRock = collision.gameObject;
            canSwing = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            otherCollided = gameObject;
            collision.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            //otherCollided = null;
            //collision.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            audioSource.PlayOneShot(audioClips[3]);
            Destroy(collision.gameObject);
            //  coinCount = playFabManager.getCoins();
            coinCount++;
            //  playFabManager.SaveCoins(coinCount);
            Debug.Log(coinCount);
            //PlayerPrefs.SetInt("coins", coinCount);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject == currentRock)
        {
            canSwing = false;
        }
    }

    public void RockCollision()
    {
        if (currentRock != null)
        {
            Debug.Log("Space Bar Pressed");
            coolDown = true;
            audioSource.PlayOneShot(audioClips[0]);
            Vector3 relativePosition = transform.InverseTransformPoint(currentRock.transform.position);
            if (relativePosition.x < 0)
            {
                animator.SetBool("Left Swing", true);
                Invoke("StopLeftSwing", .4f);
            }
            else
            {
                animator.SetBool("Right Swing", true);
                Invoke("StopRightSwing", .4f);
            }

            CheckDestroyRockCondition(currentRock);
        }
    }

    public void CheckDestroyRockCondition(GameObject rock)
    {
        Rock rockComponent = rock.GetComponent<Rock>();
        audioSource.PlayOneShot(audioClips[1]);
        int rockViewID = rock.GetComponent<PhotonView>().ViewID;
        int collisionCount = rockComponent.collisionCount;
        photonView.RPC("PlayCollisionParticleEffect", RpcTarget.All, rockViewID, collisionCount > 0);
        rockComponent.collisionCount--;

        if (rockComponent.collisionCount <= 0)
        {
            //rock.transform.GetChild(2).GetComponent<ParticleSystem>().Play();
            photonView.RPC("PlayCollisionParticleEffect", RpcTarget.All, rockViewID, collisionCount > 0);
            audioSource.PlayOneShot(audioClips[2]);
            Destroy(rock.transform.GetChild(0).gameObject);
            rock.GetComponent<CircleCollider2D>().enabled = false;

            // Instantiate the coin prefab
            GameObject coin = Instantiate(CoinPrefab, Vector3.zero, Quaternion.identity);
            coin.transform.SetParent(canvas.transform, false);

            // Convert the world position of the rock to a position within the canvas
            Vector2 viewportPosition = Camera.main.WorldToViewportPoint(rock.transform.position);
            Vector2 canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
            Vector2 coinPosition = new Vector2(viewportPosition.x * canvasSize.x - canvasSize.x * 0.5f, viewportPosition.y * canvasSize.y - canvasSize.y * 0.5f);

            // Set the position of the coin
            RectTransform coinRectTransform = coin.GetComponent<RectTransform>();
            coinRectTransform.anchoredPosition = coinPosition;

            StartCoroutine(DestroyRockAfterDelay(rock, .3f));
        }
    }

    [PunRPC]
    private void PlayCollisionParticleEffect(int rockViewID, bool play)
    {
        // Find the rock GameObject using its network ID
        GameObject rock = PhotonView.Find(rockViewID)?.gameObject;

        // Check if the rock GameObject is valid
        if (rock != null)
        {
            // Play or stop particle effect based on the 'play' parameter
            ParticleSystem particleSystem = rock.transform.GetChild(1).GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                if (play)
                {
                    particleSystem.Play();
                }
                else
                {
                    particleSystem.Stop();
                }
            }
        }
    }

    IEnumerator DestroyRockAfterDelay(GameObject rock, float delay)
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(rock);
    }

    public void StopLeftSwing()
    {
        animator.SetBool("Left Swing", false);
        coolDown = false;
    }

    public void StopRightSwing()
    {
        animator.SetBool("Right Swing", false);
        coolDown = false;
    }
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
  
    }
    public void LoadScene()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.LoadLevel(2);
            //PhotonManager.instance.DisconnectFromCurrentRoom();
        }
      
        //SceneManager.LoadScene(2);
    }
    [PunRPC]
    public void ShowChallengeUI(string playerName)
    {
        print("Show Challenge ui is getting called " + playerName + gameObject.name);
        //if(playerName == gameObject.name)
       // MonsCanvas.SetActive(true);
    }
}