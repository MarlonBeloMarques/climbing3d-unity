using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    float horizontal;
    float vertical;
    Vector3 moveDirection;
    float moveAmount;
    Vector3 camYforward;

    Transform camHolder;

    Rigidbody rigid;
    Collider col;
    Animator anim;

    public float moveSpeed = 4;
    public float rotSpeed = 9;
    public float jumpSpeed = 10;

    public bool onGround;
    bool keepOffGround;

    float savedTime;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.angularDrag = 999;
        rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        col = GetComponent<Collider>();

        camHolder = CameraHolder.singleton.transform;
        anim = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        onGround = OnGround();
        Movement();  
    }

    void Movement()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        camYforward = camHolder.forward;
        Vector3 v = vertical * camHolder.forward;
        Vector3 h = horizontal * camHolder.right;

        moveDirection = (v + h).normalized;
        moveAmount = Mathf.Clamp01((Mathf.Abs(horizontal) + Mathf.Abs(vertical)));

        Vector3 targetDir = moveDirection;
        targetDir.y = 0;

        if (targetDir == Vector3.zero)
            targetDir = transform.forward;

        Quaternion lookDir = Quaternion.LookRotation(targetDir);
        Quaternion targetRot = Quaternion.Slerp(transform.rotation, lookDir, Time.deltaTime * rotSpeed);
        transform.rotation = targetRot;

        Vector3 dir = transform.forward * (moveSpeed * moveAmount);
        dir.y = rigid.velocity.y;
        rigid.velocity = dir;
    }

    private void Update()
    {
        onGround = OnGround();

        if (keepOffGround)
        {
            if(Time.realtimeSinceStartup - savedTime > 0.5f)
            {
                keepOffGround = false;
            }
        }

        Jump();

        anim.SetFloat("move", moveAmount);
        anim.SetBool("onAir", !onGround);

    }

    void Jump()
    {

        if (onGround)
        {
            bool jump = Input.GetButtonUp("Jump");

            if (jump)
            {
                Vector3 v = rigid.velocity;
                v.y = jumpSpeed;
                rigid.velocity = v;
                savedTime = Time.realtimeSinceStartup;
                keepOffGround = true;
            }
        }

    }

    bool OnGround()
    {
        if (keepOffGround)
            return false;

        Vector3 origin = transform.position;
        origin.y += 0.4f;
        Vector3 direction = -transform.up;
        RaycastHit hit;
        Debug.Log(direction);
        Debug.DrawRay(origin, direction, Color.black);
        if (Physics.Raycast(origin, direction, out hit, 0.41f))
        {
            return true;
        }

        return false;
    }
}
