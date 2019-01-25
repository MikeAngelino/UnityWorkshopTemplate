using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class SimpleCharacterControl : MonoBehaviour {

    private enum ControlMode
    {
        Tank,
        Direct
    }

    private bool m_canJump = false;

    private bool m_canBeInvisible = true;
    private bool m_isInvisible = false;
    public GameObject jumpSphere;

    public SimpleCharacterControl player;

    public GameObject spawnPoint;

    [FormerlySerializedAs("m_moveSpeed")] [SerializeField] private float moveSpeed = 2;
    [FormerlySerializedAs("m_turnSpeed")] [SerializeField] private float turnSpeed = 200;
    [FormerlySerializedAs("m_jumpForce")] [SerializeField] private float jumpForce = 4;
    [FormerlySerializedAs("m_animator")] [SerializeField] private Animator animator;
    [FormerlySerializedAs("m_rigidBody")] [SerializeField] private Rigidbody rigidBody;

    [FormerlySerializedAs("m_controlMode")] [SerializeField] private ControlMode controlMode = ControlMode.Direct;

    private float m_currentV = 0;
    private float m_currentH = 0;

    private readonly float m_interpolation = 10;
    private readonly float m_walkScale = 0.33f;
    private readonly float m_backwardsWalkScale = 0.16f;
    private readonly float m_backwardRunScale = 0.66f;

    private bool m_wasGrounded;
    private Vector3 m_currentDirection = Vector3.zero;

    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;

    private bool m_isGrounded;
    private List<Collider> m_collisions = new List<Collider>();

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for(int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider)) {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if(validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        } else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }


    void Update ()
    {

        //Debug.Log("Update Works");
        
        animator.SetBool("Grounded", m_isGrounded);

        switch(controlMode)
        {
            case ControlMode.Direct:
                DirectUpdate();
                break;

            case ControlMode.Tank:
                TankUpdate();
                break;

            default:
                Debug.LogError("Unsupported state");
                break;
        }

        m_wasGrounded = m_isGrounded;

        

        //calls invisibility function
        /*if (Input.GetKey(KeyCode.F) & m_canBeInvisible & !m_isInvisible)
        {
            gameObject.transform.localScale = new Vector3(0, 0, 0);
        }
        
        if (Input.GetKey(KeyCode.F) & m_canBeInvisible & !m_isInvisible)
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }*/
    }
    
    private void TankUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        bool walk = Input.GetKey(KeyCode.LeftShift);

        if (v < 0) {
            if (walk) { v *= m_backwardsWalkScale; }
            else { v *= m_backwardRunScale; }
        } else if(walk)
        {
            v *= m_walkScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        transform.position += transform.forward * m_currentV * moveSpeed * Time.deltaTime;
        transform.Rotate(0, m_currentH * turnSpeed * Time.deltaTime, 0);

        animator.SetFloat("MoveSpeed", m_currentV);
        //Debug.Log("Tank Update");
        JumpingAndLanding();
    }

    private void DirectUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        Transform camera = Camera.main.transform;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            v *= m_walkScale;
            h *= m_walkScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        Vector3 direction = camera.forward * m_currentV + camera.right * m_currentH;

        float directionLength = direction.magnitude;
        direction.y = 0;
        direction = direction.normalized * directionLength;

        if(direction != Vector3.zero)
        {
            m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);

            transform.rotation = Quaternion.LookRotation(m_currentDirection);
            transform.position += m_currentDirection * moveSpeed * Time.deltaTime;

            animator.SetFloat("MoveSpeed", direction.magnitude);
        }

        //Debug.Log("Call Jump and Land");
        JumpingAndLanding();
    }

    private void JumpingAndLanding()
    {
        //Debug.Log("MethodJumpCall");
        if (m_canJump == true)
        {
            bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

            if (jumpCooldownOver && m_isGrounded && Input.GetKey(KeyCode.Space))
            {
                //Debug.Log("Jump");
                m_jumpTimeStamp = Time.time;
                rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }

            if (!m_wasGrounded && m_isGrounded)
            {
                //Debug.Log("Land");
                animator.SetTrigger("Land");
            }

            if (!m_isGrounded && m_wasGrounded)
            {
                //Debug.Log("Jumping");
                animator.SetTrigger("Jump");
            }
        }
    }
    
    IEnumerator TurnAround()
    {
        yield return new WaitForSeconds(3f);
        
        gameObject.transform.RotateAround (transform.position, transform.up, 180f);
    }
    
    private void RotateMove(int dir)
    {
        gameObject.transform.Rotate(Vector3.up);
    }
    
    

}
