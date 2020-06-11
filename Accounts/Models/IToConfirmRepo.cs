using System.Threading.Tasks;

namespace Accounts.Models
{
    /// <summary>
    /// Interface for ToConfirm Repository
    /// For dependency injection
    /// </summary>
    public interface IToConfirmRepo
    {
        Task<ToConfirm> AddAsync(string email);
        Task<ToConfirm> RemoveAsync(int id);
        Task<ToConfirm> GetToConfirmAsync(int token);
        Task DeleteAllMonthOldAsync();
    }
}