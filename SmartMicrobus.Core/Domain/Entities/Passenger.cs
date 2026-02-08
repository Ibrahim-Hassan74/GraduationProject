using SmartMicrobus.Core.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMicrobus.Core.Domain.Entities
{
    public class Passenger : BaseEntity<Guid>
    {
        public ApplicationUser ApplicationUser { get; set; }
    }
}
