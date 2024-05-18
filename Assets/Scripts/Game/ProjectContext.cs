using UnityEngine;

public class ProjectContext : MonoBehaviour
{
    public static ProjectContext Instance { get; private set; }

    [SerializeField] private SceneLoader _sceneLoader;
    [SerializeField] private SoundManager _soundManager;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private GUIManager _guiManager;

    public SoundManager SoundManager => _soundManager;
    public SceneLoader SceneLoader => _sceneLoader;
    public InputManager InputManager => _inputManager;
    public GUIManager GUIManager => _guiManager;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    public void Initialize()
    {
        SoundManager.Initialize(SceneLoader);
    }

    private void OnEnable()
    {
        SoundManager.Enable();
    }

    private void OnDisable()
    {
        SoundManager.Disable();
    }

    
}