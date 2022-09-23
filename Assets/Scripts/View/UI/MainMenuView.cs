using System;
using Game.Services;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _levelText = null;

    private GameProgressionService _gameProgression;

    private void Awake()
    {
        _gameProgression = ServiceLocator.GetService<GameProgressionService>();
    }

    void Start()
    {
        _levelText.text = _gameProgression.CurrentLevel.ToString();
    }

    public void GoToGameplay()
    {
        SceneManager.LoadScene(2);
    }
}