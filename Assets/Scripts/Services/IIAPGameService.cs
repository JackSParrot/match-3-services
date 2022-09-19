using System.Threading.Tasks;

namespace Game.Services
{
    public interface IIAPGameService : IService
    {
        public bool IsReady();
        public Task<bool> StartPurchase(string product);
        
        public string GetLocalizedPrice(string product);
    }
}