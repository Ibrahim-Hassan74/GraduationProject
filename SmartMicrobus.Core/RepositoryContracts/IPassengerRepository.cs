using SmartMicrobus.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IPassengerRepository
    {
        Task<Passenger> AddPassengerAsync(Passenger passanger);
    }
}
