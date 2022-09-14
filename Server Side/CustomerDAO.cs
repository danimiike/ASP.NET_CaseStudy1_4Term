using Casestudy.DAL.DomainClasses;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Casestudy.DAL.DAO
{
    public class CustomerDAO
    {
        private AppDbContext _db;
        public CustomerDAO(AppDbContext ctx)
        {
            _db = ctx;
        }
        public async Task<Customer> Register(Customer customer)
        {
            await _db.Customers.AddAsync(customer);
            await _db.SaveChangesAsync();
            return customer;
        }
        public async Task<Customer> GetByEmail(string email)
        {
            Customer customer = await _db.Customers.FirstOrDefaultAsync(u => u.Email == email);
            return customer;
        }
    }
}
