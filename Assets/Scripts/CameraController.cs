using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target; // Usually the Core
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Zoom Settings")]
    [SerializeField] private float defaultZoom = 10f;
    [SerializeField] private float zoomDamping = 2f;
    [SerializeField] private float beatZoomStrength = 1f; // How much to zoom in on beat

    [Header("Shake Settings")]
    [SerializeField] private float shakeRecoverySpeed = 5f;
    
    private float _currentShakeIntensity;
    private float _targetZoom;
    private Camera _cam;
    private Vector3 _initialPosition;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        if (_cam == null) _cam = Camera.main;
        
        _targetZoom = defaultZoom;
    }

    private void Start()
    {
        if (target != null)
        {
            // Center the camera on the target initially, keeping Z
            Vector3 pos = target.position;
            pos.z = transform.position.z;
            transform.position = pos;
        }
        _initialPosition = transform.position;
    }

    private void LateUpdate()
    {
        HandleZoom();
        HandleShake();
    }

    private void HandleZoom()
    {
        if (_cam == null) return;

        // Always try to return to default zoom
        _targetZoom = Mathf.Lerp(_targetZoom, defaultZoom, Time.deltaTime * zoomDamping);
        
        // Apply zoom
        _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, _targetZoom, Time.deltaTime * smoothSpeed);
    }

    private void HandleShake()
    {
        if (_currentShakeIntensity > 0)
        {
            Vector3 randomOffset = Random.insideUnitSphere * _currentShakeIntensity;
            randomOffset.z = 0; // Keep 2D
            
            // Apply shake offset relative to the tracked position
            // Note: If following a target, add randomOffset to the target position logic.
            // Here assuming static camera or separate logic, applying local offset essentially.
            transform.position = _initialPosition + randomOffset;

            _currentShakeIntensity = Mathf.Lerp(_currentShakeIntensity, 0f, Time.deltaTime * shakeRecoverySpeed);
        }
        else
        {
            // Return to exact center if not shaking
            // (If we had target following logic, it would go here)
            transform.position = Vector3.Lerp(transform.position, _initialPosition, Time.deltaTime * smoothSpeed);
        }
    }

    // Call this method when a Beat occurs
    public void OnBeat(float intensity = 1f)
    {
        // Zoom in slightly (subtract size)
        _targetZoom = defaultZoom - (beatZoomStrength * intensity);
    }

    // Call this method when Player gets hit or switches orbit violently
    public void TriggerShake(float intensity)
    {
        _currentShakeIntensity = intensity;
    }
}
