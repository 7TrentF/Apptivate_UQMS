using System.ComponentModel.DataAnnotations;

namespace Apptivate_UQMS_WebApp.Models
{
    public class KnowledgeBaseFAQ
    {

        public class KnowledgeBase
        {
            [Key]
            public int ArticleID { get; set; }


            [Required]
            public string? Title { get; set; }

            
            public string? Content { get; set; }



            [Required]
            [Url]
            public string GuideUrl { get; set; }  // URL to the GitHub Pages guide

            public string? Category { get; set; }

            public DateTime? CreatedDate { get; set; }

            public DateTime LastUpdatedDate { get; set; }

            [Display(Name = "GitHub Repository URL")]
            [Url]
            public string GitHubRepoUrl { get; set; }  // Optional: Link to the source repository
        }

        public class FAQ
        {
            [Key]
            public int FaqId { get; set; }
            public string? Question { get; set; }
            public string? Answer { get; set; }
            public string? Category { get; set; }
            public DateTime? CreatedDate { get; set; }
            public DateTime LastUpdatedDate { get; set; }
        }





    }
}
