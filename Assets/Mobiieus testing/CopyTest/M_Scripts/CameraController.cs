#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Simple camera controller that follows a target
/// with added effects such as delays, speed to fov, speed to pitch,
/// speed to distance
/// 
/// Also in charge of toggling the colors when the camera enters a portal
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform m_target = null;

    [SerializeField, Range(0,1)]
    private float m_lerp = 0.1f;
    [SerializeField, Range(0, 1)]
    private float m_rotationLerp = 0.1f;
    [SerializeField, Range(0, 1)]
    private float m_fovLerp = 0.3f;
    [SerializeField, Range(0, 1)]
    private float m_rollLerp = 0.3f;
    [SerializeField]
    private Vector3 m_offset = Vector3.up;

    [SerializeField]
    private Vector2 m_pitchRange = new Vector2(22, 12); // y value is reached at peak velocity
    [SerializeField]
    private Vector2 m_distanceRange = new Vector2(3, 4); // y value is reached at peak velocity
    [SerializeField]
    private Vector2 m_fovRange = new Vector2(35, 50); // y value is reached at peak velocity

    [SerializeField]
    private float m_maxRoll = 3;

    [SerializeField]
    private float m_referenceMaxSpeed = 60;
    [SerializeField]
    private float m_referenceAngularMaxSpeed = 2.0f;

    [SerializeField]
    private List<Camera> m_additionalCameras = null;

    private float rollGoal = 0.0f;

    [ShowInInspector, ShowIf("@UnityEngine.Application.isPlaying")]
    private float VelocityRatio => !UnityEngine.Application.isPlaying ? 0 : Mathf.Clamp(TargetRigidbody.velocity.magnitude / m_referenceMaxSpeed, 0.0f, 1.0f);
    [ShowInInspector, ShowIf("@UnityEngine.Application.isPlaying")]
    private float SmoothedRatio => !UnityEngine.Application.isPlaying ? 0 : QuadraticEaseInOut(VelocityRatio);
    [ShowInInspector, ShowIf("@UnityEngine.Application.isPlaying")]
    private float AngularVelocityRatio => !UnityEngine.Application.isPlaying ? 0 : Mathf.Clamp(TargetRigidbody.angularVelocity.y / m_referenceAngularMaxSpeed, -1.0f, 1.0f);

    [ShowInInspector, ReadOnly]
    private bool Colors { get; set; } = false;


    private Camera m_camera = null;
    public Camera Camera => m_camera ?? (m_camera = GetComponent<Camera>());


    private Rigidbody m_targetRigidbody = null;
    public Rigidbody TargetRigidbody => m_targetRigidbody ?? (m_targetRigidbody = m_target.GetComponent<Rigidbody>());
    private Vector3 OriginalPosition { get; set; } = default;


    void Start()
    {
        OriginalPosition = transform.position;
        Colors = false;
        Shader.DisableKeyword("_COLORS");
    }

    void FixedUpdate()
    {
        Vector3 dir = m_target.forward;
        dir = Quaternion.AngleAxis(((m_pitchRange.y - m_pitchRange.x) * SmoothedRatio + m_pitchRange.x), m_target.right) * dir;
        Vector3 goal = m_target.position - ((m_distanceRange.y - m_distanceRange.x) * SmoothedRatio + m_distanceRange.x) * dir;

        transform.position = m_offset + Vector3.Slerp(transform.position, goal, m_lerp * 60 * Time.fixedDeltaTime);
        transform.forward = Vector3.Slerp(transform.forward, dir, m_rotationLerp * 60 * Time.fixedDeltaTime);


        rollGoal = Mathf.Lerp(rollGoal, m_maxRoll * AngularVelocityRatio, m_rollLerp * 60 * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(rollGoal * transform.forward) * transform.localRotation;

        float fovGoal = m_fovRange.x;
        Vector3 planeForward = transform.forward;
        planeForward.y = 0.0f;
        planeForward.Normalize();
        if (Vector3.Dot(TargetRigidbody.velocity, planeForward) >= 0.0f)
        {
            fovGoal = (m_fovRange.y - m_fovRange.x) * SmoothedRatio + m_fovRange.x;
        }
        float newFov = Mathf.Lerp(Camera.fieldOfView, fovGoal, m_fovLerp * 60 * Time.fixedDeltaTime);
        Camera.fieldOfView = newFov;
        foreach (var camera in m_additionalCameras)
        {
            camera.fieldOfView = newFov;
        }
    }

    public void ResetRace()
    {
        Colors = false;
        Shader.DisableKeyword("_COLORS");
        transform.position = OriginalPosition;
    }

    [Button]
    public void ToggleColors()
    {
        Colors = !Colors;
        //Debug.Log($"New colors {Colors}");
        if (Colors)
        {
            Shader.EnableKeyword("_COLORS");
        }
        else
        {
            Shader.DisableKeyword("_COLORS");
        }
    }

    private float QuadraticEaseInOut(float t)
    {
        if (t < 0.5f)
        {
            return 2 * t * t;
        }
        else
        {
            return (-2 * t * t) + (4 * t) - 1;
        }
    }
}
