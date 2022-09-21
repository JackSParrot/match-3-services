using System.Threading.Tasks;

namespace Game.Services
{
    public class GameProgressionProvider : IGameProgressionProvider
    {
        private FileGameProgressionProvider   _local  = new FileGameProgressionProvider();
        private RemoteGameProgressionProvider _remote = new RemoteGameProgressionProvider();

        public async Task<bool> Initialize()
        {
            await Task.WhenAll(_local.Initialize(), _remote.Initialize());
            return true;
        }

        public void Save(string data)
        {
            _local.Save(data);
            _remote.Save(data);
        }

        public string Load()
        {
            string localData = _local.Load();
            string remoteData = _remote.Load();

            if (string.IsNullOrEmpty(localData) && !string.IsNullOrEmpty(remoteData))
            {
                return remoteData;
            }

            if (!string.IsNullOrEmpty(localData) && string.IsNullOrEmpty(remoteData))
            {
                return localData;
            }

            //decide which one to keep

            return localData;
        }
    }
}