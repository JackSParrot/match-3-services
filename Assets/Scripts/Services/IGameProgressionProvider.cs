using System.Threading.Tasks;

namespace Game.Services
{
    public interface IGameProgressionProvider
    {
        Task<bool> Initialize();
        string Load();
        void Save(string text);
    }
}