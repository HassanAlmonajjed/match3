using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PausePopupController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button sfxButton;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button quitButton;

    [Header("Toggle Handles")]
    [SerializeField] private RectTransform musicToggleHandle;
    [SerializeField] private RectTransform sfxToggleHandle;

    [Header("Animation Settings")]
    [SerializeField] private Vector2 offPosition;
    [SerializeField] private Vector2 onPosition;
    [SerializeField] private float animationDuration = 0.3f;

    private const string MusicKey = "MusicEnabled";
    private const string SfxKey = "SfxEnabled";

    private void Start()
    {
        // Add listeners to buttons
        closeButton.onClick.AddListener(OnCloseClicked);
        
        musicButton.onClick.AddListener(() => ToggleSetting(MusicKey, musicToggleHandle));
        sfxButton.onClick.AddListener(() => ToggleSetting(SfxKey, sfxToggleHandle));

        replayButton.onClick.AddListener(() => Debug.Log("Replay Button Clicked"));
        quitButton.onClick.AddListener(() => Debug.Log("Quit Button Clicked"));

        // Initialize handle positions based on saved player preferences
        InitializeHandle(MusicKey, musicToggleHandle);
        InitializeHandle(SfxKey, sfxToggleHandle);
    }

    private void OnCloseClicked()
    {
        // Set game state to playing and hide the popup
        GameManager.Instance.SetState(GameState.Playing);
        gameObject.SetActive(false);
    }

    private void InitializeHandle(string key, RectTransform handle)
    {
        if (handle == null) return;
        
        // Default to on (1) if no preference is saved
        bool isOn = PlayerPrefs.GetInt(key, 1) == 1;
        handle.anchoredPosition = isOn ? onPosition : offPosition;
    }

    private void ToggleSetting(string key, RectTransform handle)
    {
        if (handle == null) return;

        // Toggle the value
        bool currentlyOn = PlayerPrefs.GetInt(key, 1) == 1;
        bool newState = !currentlyOn;
        
        // Save choice
        PlayerPrefs.SetInt(key, newState ? 1 : 0);
        PlayerPrefs.Save();

        // Animate to new position
        Vector2 targetPos = newState ? onPosition : offPosition;
        handle.DOMove(targetPos, animationDuration).SetEase(Ease.OutBack);
        
        Debug.Log($"{key} toggled to {(newState ? "ON" : "OFF")}");
    }
}
