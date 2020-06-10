using System.Threading.Tasks;

namespace Accounts.Models
{
    public interface IToConfirmRepo
    {
        Task<ToConfirm> AddAsync(string email);
        Task<ToConfirm> RemoveAsync(int id);
        Task<ToConfirm> GetToConfirmAsync(int token);
        Task DeleteAllMonthOldAsync();
    }
}