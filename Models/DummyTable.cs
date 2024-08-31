using System.ComponentModel.DataAnnotations;

namespace Apptivate_UQMS_WebApp.Models
{
    public class DummyTable
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
