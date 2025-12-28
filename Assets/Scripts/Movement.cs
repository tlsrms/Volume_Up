using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Orbit Settings")]
    [SerializeField] private Transform orbitCenter;
    [SerializeField] private float innerRadius = 3f;
    [SerializeField] private float outerRadius = 5f;
    [SerializeField] private float orbitSpeed = 100f;
    [SerializeField] private float radiusSmoothTime = 0.1f;

    [Header("Input Settings")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private float inputCooldown = 0.15f;

    private float _currentAngle;
    private float _currentRadius;
    private float _targetRadius;
    private float _radiusVelocity;
    private int _orbitDirection = 1; // 1: CW, -1: CCW
    
    // Input State
    private float _lastInputTimeX;
    private float _lastInputTimeY;

    private void Awake()
    {
        // Initialize State
        _currentRadius = innerRadius;
        _targetRadius = innerRadius;
        _currentAngle = 90f; // Start at top
    }

    private void OnEnable()
    {
        if (moveAction != null)
            moveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null)
            moveAction.action.Disable();
    }

    private void Start()
    {
        if (orbitCenter == null)
        {
            // Create a temporary center if none assigned, to prevent errors
            GameObject centerObj = new GameObject("Core_Center");
            centerObj.transform.position = Vector3.zero;
            orbitCenter = centerObj.transform;
        }

        if (moveAction == null)
        {
            Debug.LogWarning("Move Action is not assigned in Movement script. Please assign 'Move' action from InputSystem_Actions in the Inspector.");
        }
    }

    private void Update()
    {
        HandleInput();
        UpdateOrbit();
        UpdateVisuals();
    }

    private void HandleInput()
    {
        if (moveAction == null) return;

        Vector2 input = moveAction.action.ReadValue<Vector2>();

        // Horizontal: Absolute Direction Setting
        if (input.x > 0.5f)
        {
            // Right Arrow -> Clockwise (In standard math, decreasing angle is CW)
            _orbitDirection = -1;
        }
        else if (input.x < -0.5f)
        {
            // Left Arrow -> Counter-Clockwise (Increasing angle is CCW)
            _orbitDirection = 1;
        }

        // Vertical: Absolute Radius Setting
        if (input.y > 0.5f)
        {
            // Up Arrow -> Outer Orbit
            _targetRadius = outerRadius;
        }
        else if (input.y < -0.5f)
        {
            // Down Arrow -> Inner Orbit
            _targetRadius = innerRadius;
        }
    }

    private void UpdateOrbit()
    {
        // Update Angle
        _currentAngle += _orbitDirection * orbitSpeed * Time.deltaTime;
        
        // Wrap angle
        _currentAngle %= 360f;

        // Smooth Radius
        _currentRadius = Mathf.SmoothDamp(_currentRadius, _targetRadius, ref _radiusVelocity, radiusSmoothTime);

        // Convert Polar to Cartesian
        float rad = _currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * _currentRadius;
        transform.position = orbitCenter.position + offset;
    }

    private void UpdateVisuals()
    {
        // Rotate sprite to face movement direction (Tangent)
        float tangentAngle = _currentAngle + (_orbitDirection > 0 ? 90f : -90f);
        transform.rotation = Quaternion.Euler(0, 0, tangentAngle);
    }
}
