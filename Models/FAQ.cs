using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Apptivate_UQMS_WebApp.Models
{
    public class FAQ
    {

        [Key]
        int ArticleID { get; set; }

        string? Tilte { get; set; }

         string?   Content { get; set; }

        string?    Category { get; set; }

       DateTime CreatedDate { get; set; }

        DateTime LastUpdatedDate   { get; set; }



    }
}
