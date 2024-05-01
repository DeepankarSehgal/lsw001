using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPlayer : MonoBehaviour
{

    public float speed;
    public float groundDist;

    public LayerMask groundLayer;
    public Rigidbody rb;
    public SpriteRenderer sr;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

    }
    private void Update()
    {
        RaycastHit hit;
        Vector3 castpos = transform.position;
        castpos.y += 1;
        if (Physics.Raycast(castpos, -transform.up, out hit, Mathf.Infinity, groundLayer))
        {
            if (hit.collider != null)
            {
                Vector3 movePos = transform.position;
                movePos.y = hit.point.y + groundDist;
                transform.position = movePos;
            }
        }
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 moveDir = new Vector3(x, 0, y);
        rb.velocity = moveDir * speed;

        if (x != 0 && x < 0)
        {
            sr.flipX = true;
        }
        else if (x != 0 && x > 0)
        {
            sr.flipX = false;
        }
    }
}
