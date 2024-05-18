using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PlayerContext : MonoBehaviour
{
    [SerializeField] private GameObject _playerGO;
    public static PlayerContext Instance { get; private set; }

    public PlayerInteractions Interactions { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;
        Interactions = _playerGO.GetComponent<PlayerInteractions>();
    }
}