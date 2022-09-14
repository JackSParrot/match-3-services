using System.Threading.Tasks;
using Game.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Load
{
    public class LoadingSceneLogic : MonoBehaviour
    {
        [SerializeField]
        private bool IsDevBuild = true;

        private TaskCompletionSource<bool> _cancellationTaskSource;

        void Start()
        {
            _cancellationTaskSource = new();
            LoadServicesCancellable().ContinueWith(task =>
                    Debug.LogException(task.Exception),
                TaskContinuationOptions.OnlyOnFaulted);
        }

        private void OnDestroy()
        {
            _cancellationTaskSource.SetResult(true);
        }

        private async Task LoadServicesCancellable()
        {
            await Task.WhenAny(LoadServices(), _cancellationTaskSource.Task);
        }

        private async Task LoadServices()
        {
            string environmentId = IsDevBuild ? "development" : "production";

            ServicesInitializer servicesInitializer = new ServicesInitializer(environmentId);

            //create services
            GameConfigService gameConfig = new GameConfigService();
            GameProgressionService gameProgression = new GameProgressionService();

            RemoteConfigGameService remoteConfig = new RemoteConfigGameService();
            LoginGameService loginService = new LoginGameService();
            AnalyticsGameService analyticsService = new AnalyticsGameService();
            AdsGameService adsService = new AdsGameService("#######", "Rewarded_Android");

            //register services
            ServiceLocator.RegisterService(gameConfig);
            ServiceLocator.RegisterService(gameProgression);
            ServiceLocator.RegisterService(remoteConfig);
            ServiceLocator.RegisterService(loginService);
            ServiceLocator.RegisterService(adsService);
            ServiceLocator.RegisterService(analyticsService);

            //initialize services
            await servicesInitializer.Initialize();
            await loginService.Initialize();
            await remoteConfig.Initialize();
            await analyticsService.Initialize();
            bool adsInitialized = await adsService.Initialize(Application.isEditor);
            
            Debug.Log("AdsInitialized: " + adsInitialized);

            gameConfig.Initialize(remoteConfig);
            gameProgression.Initialize(gameConfig);

            SceneManager.LoadScene(1);
        }
    }
}