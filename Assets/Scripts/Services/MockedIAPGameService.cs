using System.Threading.Tasks;

namespace Game.Services
{
    public class MockedIAPGameService : IIAPGameService
    {
        public bool IsReady()
        {
            return true;
        }

        public async Task<bool> StartPurchase(string product)
        {
            await Task.Delay(2000);
            return true;
        }

        public string GetLocalizedPrice(string product)
        {
            return "0.99$";
        }

        public void Clear()
        {
        }
    }
}