using System.ComponentModel.DataAnnotations;

namespace ClientAPI.Models
{
    public class Client
    {
        [Required]
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string MainAddress { get; set; }
    }
}
