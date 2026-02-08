using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMicrobus.Infrastructure.Repository
{
    public class PassangerRepository(ApplicationDbContext context) : IPassengerRepository
    {
        public async Task<Passenger> AddPassengerAsync(Passenger passanger)
        {
            context.Passangers.Add(passanger);
            await context.SaveChangesAsync();
            return passanger;
        }
    }
}
