using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private MainMenuControllerUI _mainMenuUI;
    [Header("Buttons on main canvas")]
    [SerializeField] private Button _playButton;    
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _closeStartCanvasBtn;
    [Header("Audio properties")]
    [SerializeField] private Button _audioBtnPlayPause;
    [SerializeField] private AudioSource _audioSource;
    [Header("Buttons on PlayWindow")]
    [SerializeField] private Button _playGameBtn;
    [Header("AudioImages")]
    [SerializeField] private GameObject _muteIcon;
    [SerializeField] private GameObject _unmuteIcon;

    private int _playSceneIndex = 1;

    private bool _isActive = true;

    private void Awake()
    {
        _playButton.onClick.AddListener(OnPlayBtnClick);
        _optionsButton.onClick.AddListener(OnOptionsBtnClick);
        _quitButton.onClick.AddListener(OnQuitBtnclick);
        _playGameBtn.onClick.AddListener(LoadPlayScene);
        _audioBtnPlayPause.onClick.AddListener(MuteMusic);

        _closeStartCanvasBtn.onClick.AddListener(() => { _mainMenuUI.ShowStartCanvas(false); });

    }

    public void PlayaAudioSong(bool play)
    {
        if (play)
            _audioSource.Play();
        else
            _audioSource.Stop();
    }

    private void MuteMusic()
    {
        _audioSource.mute = _isActive;
        _isActive = !_isActive;

        _muteIcon.SetActive(!_isActive);
        _unmuteIcon.SetActive(_isActive);
    }

    private void OnDisable()
    {
        //_mainMenuUI.ShowStartCanvas(false);
        //_mainMenuUI.ShowStartCanvas(false);
    }

    private void LoadPlayScene()
    {
        SceneManager.LoadScene(_playSceneIndex);
    }

    private void OnPlayBtnClick()
    {
        Debug.Log("Play btn clicked!");
        _mainMenuUI.ShowStartCanvas(true);
    }


    private void OnOptionsBtnClick()
    {
        Debug.Log("Options btn clicked!");
    }

    private void OnQuitBtnclick()
    {
        _mainMenuUI.ShowQuitCanvas(true);
        Debug.Log("Quit btn clicked");
    }
}
