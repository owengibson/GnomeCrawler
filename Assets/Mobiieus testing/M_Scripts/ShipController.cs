using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// Basic Rigidbody-based character controller for a floating spaceship/car
/// will probe the terrain underneath to know at what altitude it should hover
public class ShipController : MonoBehaviour, ShipMap.IShipActions
{
    [SerializeField]
    private float m_movementSpeed = 10;
    [SerializeField]
    private float m_rotationSpeed = 0.1f;
    [SerializeField]
    private float m_forceFactor = 400;
    [SerializeField]
    private float m_hoverHeight = 1f;
    [SerializeField, Range(0,1)]
    private float m_hoverLerp = 0.1f;
    [SerializeField]
    private LayerMask m_terrain = default;
    [SerializeField]
    private Transform m_view = null;
    [SerializeField]
    private FullScreenPassRendererFeature m_fullScreenPass = null;
    [SerializeField]
    private float m_boostTime = 2;
    [SerializeField]
    private float m_boostDelay = 5;
    [SerializeField]
    private float m_boostMultiplier = 2;

    private Vector2 m_lastMove = default;

    private Rigidbody m_rigidbody = null;
    public Rigidbody RigidBody => m_rigidbody ?? (m_rigidbody = GetComponent<Rigidbody>());

    private float BoostStart { get; set; } = 0;

    private ShipMap m_playerInputs = null;

    void Start()
    {
        m_playerInputs = new ShipMap();
        m_playerInputs.Ship.SetCallbacks(this);
        m_playerInputs.Ship.Enable();
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position+3* Vector3.up, -Vector3.up, out hit, 10, m_terrain))
        {

            var pos = transform.position;
            //pos.y = Mathf.Lerp(pos.y, hit.point.y + m_hoverHeight, m_hoverLerp);
            pos.y = hit.point.y;
            pos.y = Mathf.Max(0, pos.y);
            //transform.position = pos;
            //RigidBody.Move(pos, transform.rotation);
            //transform.up = Vector3.Slerp(transform.up, hit.normal, m_hoverLerp);

            float currentHover = m_view.localPosition.y;
            transform.position = hit.point;
            currentHover = Mathf.Lerp(currentHover, m_hoverHeight, m_hoverLerp);
            m_view.position = transform.position + currentHover * Vector3.up;
            //m_view.rotation = Quaternion.LookRotation(m_view.forward, hit.normal);
        }

        m_fullScreenPass.passMaterial.SetFloat("_ShadowScroll", m_fullScreenPass.passMaterial.GetFloat("_ShadowScroll") + RigidBody.velocity.magnitude);
    }

    private void FixedUpdate()
    {
        if (!RacingController.Instance.CanMove) return;
        Vector3 currentPos = RigidBody.position;
        Vector2 inputMove = m_lastMove;
        bool boosting = Time.time - BoostStart < m_boostTime;
        float speed = m_movementSpeed;
        if (boosting)
        {
            inputMove.y = 1;
            speed *= m_boostMultiplier;
        }

        if (Mathf.Abs(inputMove.y) > 0.1f)
        {
            RigidBody.AddForce(inputMove.y * transform.forward * speed * Time.fixedDeltaTime * m_forceFactor);
        }
        if (Mathf.Abs(inputMove.x) > 0.1f)
        {
            //transform.forward = Vector3.Slerp(transform.forward, inputMove.x * transform.right, m_rotationSpeed * Time.fixedDeltaTime);
            RigidBody.AddRelativeTorque(transform.up * inputMove.x * m_rotationSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        m_lastMove = context.ReadValue<Vector2>();
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            RacingController.Instance.ResetRace();
        }
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        if (!RacingController.Instance.CanMove) return;
        if (Time.time - BoostStart < m_boostDelay) return;
        BoostStart = Time.time;
    }

    public void ResetRace()
    {
        BoostStart = 0;
        RigidBody.velocity = default;
        RigidBody.angularVelocity = default;
    }
}
