using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Movement playerMovement;

    [Header("Rhythm Test")]
    [SerializeField] private float bpm = 120f;
    [SerializeField] private bool autoBeat = true;

    private float _beatInterval;
    private float _timer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        _beatInterval = 60f / bpm;
        
        if (cameraController == null)
            cameraController = FindAnyObjectByType<CameraController>();

        if (playerMovement == null)
            playerMovement = FindAnyObjectByType<Movement>();
    }

    private void Update()
    {
        if (autoBeat)
        {
            _timer += Time.deltaTime;
            if (_timer >= _beatInterval)
            {
                OnBeat();
                _timer -= _beatInterval;
            }
        }
    }

    private void OnBeat()
    {
        // Visual feedback on beat
        if (cameraController != null)
        {
            cameraController.OnBeat();
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
        // Stop time or show UI
        Time.timeScale = 0f;
    }
}
