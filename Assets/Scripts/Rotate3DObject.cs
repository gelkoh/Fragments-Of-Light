using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Rotate3DObject : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_speed;
    [SerializeField] private float m_maxVerticalAngle;
    [SerializeField] private Vector3 m_endPos;

    private bool m_rotateAllowed;
    private InputAction m_leftClickInputAction;
    private InputAction m_mouseLookInputAction;
    
    private float m_currentPitch = 0f;
    private float m_currentYaw = 0f;
    
    private GameStateManager m_gameStateManager;
    private Camera m_mainCamera;

    private void Awake()
    {
        m_mainCamera = Camera.main;
        InputActionAsset inputActions = InputSystem.actions;
        m_leftClickInputAction = inputActions.FindAction("LeftClick");
        m_mouseLookInputAction = inputActions.FindAction("MouseLook");

        if (m_leftClickInputAction != null)
        {
            m_leftClickInputAction.started += OnClickStarted;
            m_leftClickInputAction.canceled += OnClickCanceled;
        }

        m_gameStateManager = ManagersManager.Get<GameStateManager>();
    }

    private void OnEnable()
    {
        GameStateManager.OnStart += HandleStartGame;
        GameStateManager.OnEnd += HandleEndGame;

        m_leftClickInputAction.Enable(); 
        m_mouseLookInputAction.Enable();
    }

    private void OnDisable()
    {
        GameStateManager.OnStart -= HandleStartGame;
        GameStateManager.OnEnd -= HandleEndGame;

        m_leftClickInputAction.Disable(); 
        m_mouseLookInputAction.Disable();
    }

    private void Update()
    {
        if (m_gameStateManager.GetState() != GameState.MainMenu) return;
        if (!m_rotateAllowed) return;

        Vector2 mouseDelta = m_mouseLookInputAction.ReadValue<Vector2>();

        float sensitivity = m_speed * Time.deltaTime * 50f;
        
        // Horizontale Rotation (unbegrenzt) - um die World-Y-Achse
        m_currentYaw -= mouseDelta.x * sensitivity;
        
        // Vertikale Rotation (begrenzt) - relativ zur Kamera
        m_currentPitch += mouseDelta.y * sensitivity;
        m_currentPitch = Mathf.Clamp(m_currentPitch, -m_maxVerticalAngle, m_maxVerticalAngle);
        
        // Rotation anwenden: erst Yaw um World-Y, dann Pitch um Camera-Right
        Quaternion yawRotation = Quaternion.Euler(0f, m_currentYaw, 0f);
        Quaternion pitchRotation = Quaternion.AngleAxis(m_currentPitch, m_mainCamera.transform.right);
        
        transform.rotation = pitchRotation * yawRotation;
    }

    private void SetRotateAllowed(bool allowed)
    {
        m_rotateAllowed = allowed;
        ManagersManager.Get<CursorManager>().SetCursorState(allowed ? CursorState.Rotate : CursorState.Default);
    }

    public void RotateBackAndMoveCloser()
    {
        m_rotateAllowed = false;
        StopAllCoroutines();
        StartCoroutine(ResetPivotRoutineAndMoveBookCloser());
    }

    private IEnumerator ResetPivotRoutineAndMoveBookCloser()
    {
        float duration = 1f;
        float elapsed = 0f;
    
        Quaternion startRot = transform.localRotation;
        Quaternion targetRot = Quaternion.identity; 

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);

            transform.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        transform.localRotation = targetRot;
        m_currentYaw = 0;
        m_currentPitch = 0;
    }

    private void HandleStartGame()
    {
        RotateBackAndMoveCloser();
        
        m_leftClickInputAction.started -= OnClickStarted;
        m_leftClickInputAction.canceled -= OnClickCanceled;

        SetRotateAllowed(false);
    }

    private void HandleEndGame()
    {
        m_leftClickInputAction.started += OnClickStarted;
        m_leftClickInputAction.canceled += OnClickCanceled;

        SetRotateAllowed(true);
    }

    private void OnClickStarted(InputAction.CallbackContext context) => SetRotateAllowed(true);
    private void OnClickCanceled(InputAction.CallbackContext context) => SetRotateAllowed(false);
}