#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// Keeps track of how the race is going, how many laps have been completed
/// how many checkpoints have been passed
/// Also in charge of reseting the race when pressing R, placing the player at the starting point
/// and showing the countdown
public class RacingController : Singleton<RacingController>
{
    private enum EState
    {
        Title,
        Delay,
        Countdown,
        Racing,
        Finished,
    }

    [SerializeField]
    private List<Portal> m_portals = null;
    [SerializeField]
    private int m_totalLaps = 3;
    [SerializeField]
    private float m_delay = 2;
    [SerializeField]
    private float m_countdown = 3;
    [SerializeField]
    private ShipController m_ship = null;
    [SerializeField]
    private CameraController m_camera = null;
    [SerializeField]
    private GameObject m_titleWrapper = null;
    [SerializeField]
    private GameObject m_titleText = null;
    [SerializeField]
    private float m_titleAmpl = 1;
    [SerializeField]
    private float m_titleFreq = 1;

    //UI
    [SerializeField]
    private TextMeshProUGUI m_countdownTxt = null;
    [SerializeField]
    private TextMeshProUGUI m_timerTxt = null;
    [SerializeField]
    private TextMeshProUGUI m_lapsTxt = null;

    [ShowInInspector, ReadOnly]
    private List<Portal> PassedPortals { get; set; } = new List<Portal>();
    [ShowInInspector, ReadOnly]
    private int RemainingPortals => m_portals.Count - PassedPortals.Count;
    [ShowInInspector, ReadOnly]
    public int Lap { get; private set; } = 0;
    public int TotalLaps => m_totalLaps;
    private EState State { get; set; } = EState.Title;
    private float StartTime { get; set; } = 0;
    public float TotalTime { get; set; } = 0;
    public bool CanMove => State == EState.Racing;
    public float Countdown { get; private set; } = 0;
    private Coroutine ResetRoutine { get; set; } = null;

    private Vector3 TitleOriginalPosition { get; set; } = default;
    private Vector3 OriginalPosition { get; set; } = default;
    private Quaternion OriginalRotation { get; set; } = default;

    void Start()
    {
        OriginalPosition = m_ship.transform.position;
        OriginalRotation = m_ship.transform.rotation;

        TitleOriginalPosition = m_titleText.transform.position;

        foreach (var item in m_portals)
        {
            item.Parent = this;
        }

        ResetRace();
    }

    void Update()
    {
        if (State == EState.Racing)
        {
            TotalTime = Time.time - StartTime;
            m_timerTxt.text = TotalTime.ToString("F3", CultureInfo.InvariantCulture);
        }

        if(State == EState.Title && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            State = EState.Delay;
        }

        m_titleText.transform.position = TitleOriginalPosition + m_titleAmpl * Mathf.Sin(Time.time * m_titleFreq) * Vector3.up;
    }

    public void OnEnter(Portal portal)
    {
        if (State != EState.Racing) return;
        if (!PassedPortals.Contains(portal))
        {
            PassedPortals.Add(portal);
        }

        if (portal.IsEnd && RemainingPortals == 0)
        {
            NextLap();
        }
    }

    private void NextLap()
    {
        Lap++;
        UpdateLapText();
        PassedPortals?.Clear();

        if(Lap == TotalLaps)
        {
            Win();
        }
    }

    public void ResetRace()
    {
        if (ResetRoutine != null) return;

        m_ship.transform.SetPositionAndRotation(OriginalPosition, OriginalRotation);
        m_ship.ResetRace();
        m_camera.ResetRace();

        Lap = 0;
        PassedPortals = new List<Portal>();
        TotalTime = 0;

        m_countdownTxt.gameObject.SetActive(false);
        m_timerTxt.gameObject.SetActive(false);
        m_lapsTxt.gameObject.SetActive(false);

        UpdateLapText();

        ResetRoutine = StartCoroutine(StartRace());
    }

    private IEnumerator StartRace()
    {
        while (State == EState.Title)
        {
            yield return null;
        }
        m_titleWrapper.SetActive(false);
        State = EState.Delay;
        yield return new WaitForSeconds(m_delay);
        State = EState.Countdown;
        Countdown = m_countdown;
        m_countdownTxt.gameObject.SetActive(true);

        while (Countdown > 0)
        {
            Countdown -= Time.deltaTime;
            m_countdownTxt.text = Mathf.CeilToInt(Countdown).ToString();
            yield return null;
        }

        m_countdownTxt.gameObject.SetActive(false);
        State = EState.Racing;
        StartTime = Time.time;
        m_timerTxt.gameObject.SetActive(true);
        m_lapsTxt.gameObject.SetActive(true);
        ResetRoutine = null;
        yield break;
    }

    private void Win()
    {
        State = EState.Finished;
        TotalTime = Time.time - StartTime;
        m_lapsTxt.gameObject.SetActive(false);
    }

    private void UpdateLapText()
    {
        m_lapsTxt.text = $"<size=130%>{Lap+1}</size> / {TotalLaps}";
    }
}
