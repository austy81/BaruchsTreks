using System.ComponentModel.DataAnnotations;

namespace BaruchsTreks.Data
{
    public class DbEntity
    {
        [Key]
        public Guid id { get; set; }
        public DateTime Created{ get; set; }
        public DateTime Modified { get; set; }
    }
}