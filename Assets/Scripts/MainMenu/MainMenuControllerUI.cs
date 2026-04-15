using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuControllerUI : MonoBehaviour
{
    public static MainMenuControllerUI Instance;


    [SerializeField] private CanvasGroup _playCG;
    [SerializeField] private CanvasGroup _quitCG;
    [SerializeField] private CanvasGroup _optionsCG;


    #region Private fields
    private float _defaultAnimTime = 0.45f;
    #endregion

    private void Awake()
    {
        if (Instance != null)
            Instance = this;
    }



    public void ShowStartCanvas(bool show)
    {
        _playCG.DOFade(show ? 1 : 0, _defaultAnimTime);
        _playCG.interactable = show;
        _playCG.blocksRaycasts = show;
    }

    public void ShowQuitCanvas(bool show)
    {
        _quitCG.DOFade(show ? 1 : 0, _defaultAnimTime);
        _quitCG.interactable = show;
        _quitCG.blocksRaycasts = show;
    }

    public void ShowOptionsCanvas(bool show)
    {
        _optionsCG.DOFade(show ? 1 : 0, _defaultAnimTime);
        _optionsCG.interactable = show;
        _optionsCG.blocksRaycasts = show;
    }
}