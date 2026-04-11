using System.ComponentModel.DataAnnotations;

namespace MellonBank.Areas.Identity.Data
{
    public class Currency
    {
        [Key]
        public int Id { get; set; }
        public decimal AUD { get; set; }
        public decimal CHF { get; set; }
        public decimal GBP { get; set; }
        public decimal USD { get; set; }
    }
}
