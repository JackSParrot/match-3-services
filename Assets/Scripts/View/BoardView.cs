using System.Collections;
using System.Collections.Generic;
using Game.Services;
using MVC.Controller;
using MVC.Model;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MVC.View
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField]
        private Vector2Int _boardSize = new Vector2Int(9, 9);
        [SerializeField]
        private Camera _inputCamera = null;
        [SerializeField]
        private TMP_Text _movesText = null;
        [SerializeField]
        private TMP_Text _scoreText = null;
        [SerializeField]
        private Button _boosterButton = null;
        [SerializeField]
        private TMP_Text _boosterAmountText = null;
        [SerializeField]
        private GameObject _gameLostPopup = null;
        [SerializeField]
        private GameObject _gameWonPopup = null;
        [SerializeField]
        private Button _adButton = null;

        private Plane                _inputPlane;
        private List<CellView>       _cells = new List<CellView>();
        private BoardController      _controller;
        private List<IViewAnimation> _animations = new List<IViewAnimation>();
        private bool                 _gameOver   = false;

        private AnalyticsGameService   _analytics = null;
        private GameProgressionService _gameProgression;

        private bool IsAnimating => _animations.Count > 0;

        public void AddCellView(CellView cell)
        {
            _cells.Add(cell);
        }

        public CellView GetCellViewAt(Vector2Int position)
        {
            return _cells.Find(cell => cell.Position == position);
        }

        public void RemoveCellView(CellView cell)
        {
            _cells.Remove(cell);
        }

        private void Awake()
        {
            if (_inputCamera == null)
                _inputCamera = Camera.main;

            _inputPlane = new Plane(Vector3.forward, Vector3.zero);
            _controller = new BoardController(10, 40, _boardSize.x, _boardSize.y);
            _controller.OnCellCreated += OnCellCreated;
            _controller.OnCellMoved += OnCellMoved;
            _controller.OnCellDestroyed += OnCellDestroyed;
            _controller.OnWinGame += OnWin;
            _controller.OnLoseGame += OnLose;
            _analytics = ServiceLocator.GetService<AnalyticsGameService>();
            _gameProgression = ServiceLocator.GetService<GameProgressionService>();

            _movesText.text = _controller.Model.MovesRemaining.ToString();
            _scoreText.text = $"{_controller.Model.Score.ToString()}/{_controller.Model.TargetScore.ToString()}";

            _analytics.SendEvent("LevelStarted",
                new Dictionary<string, object> { ["LevelId"] = _gameProgression.CurrentLevel });
            _gameLostPopup.SetActive(false);
            _gameWonPopup.SetActive(false);
            _boosterButton.onClick.AddListener(UseBooster);
        }

        private void Start()
        {
            _controller.ProcessInput(Vector2Int.zero);
            _boosterAmountText.text = _gameProgression.BoosterAmount.ToString();
            _boosterButton.interactable = _gameProgression.BoosterAmount > 0;
        }

        private void OnWin()
        {
            _gameOver = true;
            _analytics.SendEvent("LevelWin");
            _gameWonPopup.SetActive(true);
        }

        private void OnLose()
        {
            _gameOver = true;
            _analytics.SendEvent("LevelLose");
            _gameLostPopup.SetActive(true);
            _adButton.interactable = ServiceLocator.GetService<AdsGameService>().IsAdReady;
        }

        public async void Continue()
        {
            if (await ServiceLocator.GetService<AdsGameService>().ShowAd())
            {
                _controller.UseContinue();
            }
        }

        public void UseBooster()
        {
            _controller.UseBooster();
        }

        private void Update()
        {
            if (IsAnimating || _gameOver)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                var ray = _inputCamera.ScreenPointToRay(Input.mousePosition);
                if (_inputPlane.Raycast(ray, out float hitDistance))
                {
                    Vector3 hitPosition = ray.GetPoint(hitDistance);
                    _controller.ProcessInput(new Vector2Int((int)hitPosition.x, (int)hitPosition.y));
                    _movesText.text = _controller.Model.MovesRemaining.ToString();
                    _scoreText.text =
                        $"{_controller.Model.Score.ToString()}/{_controller.Model.TargetScore.ToString()}";
                    bool destroyedOver5 = _animations.FindAll(a => a is DestroyCellAnimation).Count > 5;
                    if (destroyedOver5)
                    {
                        UnityEngine.Handheld.Vibrate();
                    }
                }
            }
        }

        private void OnCellCreated(CellModel cell)
        {
            _animations.Add(new CreateCellAnimation(cell.Position, cell.Item));
            if (_animations.Count == 1)
            {
                StartCoroutine(ProcessAnimations());
            }
        }

        private void OnCellMoved(CellModel from, CellModel to)
        {
            _animations.Add(new MoveCellAnimation(from.Position, to.Position));
            if (_animations.Count == 1)
            {
                StartCoroutine(ProcessAnimations());
            }
        }

        private void OnCellDestroyed(CellModel cell)
        {
            _animations.Add(new DestroyCellAnimation(GetCellViewAt(cell.Position)));
            if (_animations.Count == 1)
            {
                StartCoroutine(ProcessAnimations());
            }
        }

        private IEnumerator ProcessAnimations()
        {
            while (IsAnimating)
            {
                yield return _animations[0].PlayAnimation(this);
                _animations.RemoveAt(0);
            }
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene(1);
        }
    }
}