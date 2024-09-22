using System.ComponentModel.DataAnnotations;

namespace Apptivate_UQMS_WebApp.Models
{
    public class AppUsers
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }

        // New LastSeen property
        public DateTime? LastSeen { get; set; } // Nullable to allow for uninitialized values



    }




}