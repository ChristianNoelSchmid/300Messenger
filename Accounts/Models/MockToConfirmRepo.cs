using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounts.Models
{
    public class MockToConfirmRepo : IToConfirmRepo
    {
        private List<ToConfirm> _toConfirms = new List<ToConfirm>();

        public MockToConfirmRepo()
        {
            _toConfirms.AddRange(
                new ToConfirm[] {
                    new ToConfirm
                    {
                        Id = 1,
                        EmailToConfirm = "pete@fakemail.com",
                        TimeStamp = DateTime.Now,
                        Token = "pete@fakemail.com".GetHashCode()
                    },
                    new ToConfirm
                    {
                        Id = 2,
                        EmailToConfirm = "roger@fakemail.com",
                        TimeStamp = DateTime.Now,
                        Token = "roger@fakemail.com".GetHashCode()
                    },
                    new ToConfirm
                    {
                        Id = 2,
                        EmailToConfirm = "robert@fakemail.com",
                        TimeStamp = DateTime.Now,
                        Token = "robert@fakemail.com".GetHashCode()
                    }
                }
            );
        }

        public async Task<ToConfirm> AddAsync(string email)
        {
            ToConfirm toConfirm;
            _toConfirms.Add(
                toConfirm = new ToConfirm
                {
                    Id = _toConfirms.Max(tc => tc.Id) + 1,
                    EmailToConfirm = email,
                    TimeStamp = DateTime.Now,
                    Token = email.GetHashCode()
                }
            );

            return toConfirm;
        }

        public Task DeleteAllMonthOldAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ToConfirm> GetToConfirmAsync(int token)
        {
            var toConfirm = _toConfirms.FirstOrDefault(tc => tc.Token == token);
            return await Task.Run(() => toConfirm);
        }

        public async Task<ToConfirm> RemoveAsync(int id)
        {
            var toConfirm = _toConfirms.FirstOrDefault(tc => tc.Id == id);
            if (toConfirm != null)
            {
                _toConfirms.Remove(toConfirm);
            }

            return await Task.Run(() => toConfirm);
        }
    }
}
