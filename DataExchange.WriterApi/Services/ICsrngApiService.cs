using DataExchange.Shared.Models;

namespace DataExchange.WriterApi.Services
{
    public interface ICsrngApiService
    {
        Task<List<CsrngResponse>> GetRandomNumbersAsync(int count, int min = 1, int max = 1000);
    }
}