using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.Domain.Entities
{
    public class BaseEntity<T>
    {
        [Key]
        public T Id { get; set; }
    }
}
