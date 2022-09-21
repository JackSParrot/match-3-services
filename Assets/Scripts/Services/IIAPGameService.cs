using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.Services
{
    public interface IIAPGameService : IService
    {
        Task Initialize(Dictionary<string, string> products);
        bool IsReady();
        Task<bool> StartPurchase(string product);
        
        string GetLocalizedPrice(string product);
    }
}