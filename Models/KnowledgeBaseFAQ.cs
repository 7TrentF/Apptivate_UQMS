using System.ComponentModel.DataAnnotations;

namespace Apptivate_UQMS_WebApp.Models
{
    public class KnowledgeBaseFAQ
    {

        public class KnowledgeBase
        {

            [Key]
            int ArticleID { get; set; }

            string? Tilte { get; set; }

            string? Content { get; set; }

            string? Category { get; set; }

            DateTime? CreatedDate { get; set; }

            DateTime? LastUpdatedDate { get; set; }

        }

        public class FAQ
        {

            [Key]
            int FaqId { get; set; }

            int ArticleID { get; set; }

            string? Question { get; set; }

            string? Answer { get; set; }

            string? Category { get; set; }

            DateTime? CreatedDate { get; set; }

            DateTime? LastUpdatedDate { get; set; }

        }





        }
}
