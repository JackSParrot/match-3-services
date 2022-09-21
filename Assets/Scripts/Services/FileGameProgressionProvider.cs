using System.Threading.Tasks;
using UnityEngine;

namespace Game.Services
{
    public class FileGameProgressionProvider : IGameProgressionProvider
    {
        private static string kSavePath = "/gameProgression.json";

        public async Task<bool> Initialize()
        {
            await Task.Yield();
            return true;
        }

        public void Save(string data)
        {
            System.IO.File.WriteAllText(Application.persistentDataPath + kSavePath, data);
        }

        public string Load()
        {
            if (System.IO.File.Exists(Application.persistentDataPath + kSavePath))
            {
                return System.IO.File.ReadAllText(Application.persistentDataPath + kSavePath);
            }

            return string.Empty;
        }
    }
}