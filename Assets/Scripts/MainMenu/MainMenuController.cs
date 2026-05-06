using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _aboutButton;
    [SerializeField] private Button _quitButton;

    [Header("About Panel")]
    [SerializeField] private CanvasGroup _aboutPanel;
    [SerializeField] private Button _aboutBackButton;

    [Header("Audio")]
    [SerializeField] private Button _audioToggleButton;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private GameObject _muteIcon;
    [SerializeField] private GameObject _unmuteIcon;

    [Header("Scene Settings")]
    [SerializeField] private int _gameSceneIndex = 1;

    [Header("Animation Settings")]
    [SerializeField] private float _fadeDuration = 0.3f;

    private bool _isMusicPlaying = true;

    private void Awake()
    {
        // Main menu buttons
        _playButton.onClick.AddListener(OnPlayClicked);
        _aboutButton.onClick.AddListener(OnAboutClicked);
        _quitButton.onClick.AddListener(OnQuitClicked);

        // About panel back button
        if (_aboutBackButton != null)
            _aboutBackButton.onClick.AddListener(CloseAboutPanel);

        // Audio toggle (optional - only if assigned)
        if (_audioToggleButton != null)
            _audioToggleButton.onClick.AddListener(ToggleMusic);

        // Ensure About panel starts hidden
        if (_aboutPanel != null)
        {
            _aboutPanel.alpha = 0f;
            _aboutPanel.interactable = false;
            _aboutPanel.blocksRaycasts = false;
            _aboutPanel.gameObject.SetActive(false);
        }
    }

    private void OnPlayClicked()
    {
        Debug.Log("Loading game scene...");

        // Destroy the DontDestroyOnLoad music object so it stops when game starts
        GameObject menuMusic = GameObject.Find("MainMusicAudioSource");
        if (menuMusic != null)
        {
            Destroy(menuMusic);
        }

        SceneManager.LoadScene(_gameSceneIndex);
    }

    private void OnAboutClicked()
    {
        Debug.Log("About button clicked - showing panel");
        ShowAboutPanel();
    }

    private void ShowAboutPanel()
    {
        if (_aboutPanel == null)
        {
            Debug.LogWarning("About panel not assigned!");
            return;
        }

        // Disable main menu buttons
        SetMainMenuButtonsInteractable(false);

        // Show and fade in the About panel
        _aboutPanel.gameObject.SetActive(true);
        _aboutPanel.DOFade(1f, _fadeDuration).OnComplete(() =>
        {
            _aboutPanel.interactable = true;
            _aboutPanel.blocksRaycasts = true;
        });
    }

    public void CloseAboutPanel()
    {
        Debug.Log("Closing About panel");

        if (_aboutPanel == null) return;

        _aboutPanel.interactable = false;
        _aboutPanel.blocksRaycasts = false;

        // Fade out and hide
        _aboutPanel.DOFade(0f, _fadeDuration).OnComplete(() =>
        {
            _aboutPanel.gameObject.SetActive(false);
            // Re-enable main menu buttons
            SetMainMenuButtonsInteractable(true);
        });
    }

    private void SetMainMenuButtonsInteractable(bool interactable)
    {
        if (_playButton != null) _playButton.interactable = interactable;
        if (_aboutButton != null) _aboutButton.interactable = interactable;
        if (_quitButton != null) _quitButton.interactable = interactable;
    }

    private void OnQuitClicked()
    {
        Debug.Log("QUIT BUTTON CLICKED!");

        #if UNITY_EDITOR
            Debug.Log("Stopping Editor play mode...");
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Debug.Log("Calling Application.Quit()...");
            Application.Quit();
        #endif
    }

    private void ToggleMusic()
    {
        if (_audioSource == null) return;

        _isMusicPlaying = !_isMusicPlaying;
        _audioSource.mute = !_isMusicPlaying;

        if (_muteIcon != null) _muteIcon.SetActive(!_isMusicPlaying);
        if (_unmuteIcon != null) _unmuteIcon.SetActive(_isMusicPlaying);
    }

    public void PlayMusic(bool play)
    {
        if (_audioSource == null) return;

        if (play)
            _audioSource.Play();
        else
            _audioSource.Stop();
    }
}
