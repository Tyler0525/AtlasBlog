using System.ComponentModel.DataAnnotations;

namespace AtlasBlog.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Blog Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at most {1} and atleast {2} charcters long", MinimumLength = 2)]
        public string BlogName { get; set; } = "";
       
        [Required]
        [StringLength(300, ErrorMessage = "The {0} must be at most {1} and atleast {2} charcters long", MinimumLength = 10)]
        public string Description { get; set; } = "";
        
        [DataType(DataType.Date)]
        public DateTime Created{ get; set; }
        public DateTime? Updated { get; set; }

        //I want to store an image for this Blog 
        [Display(Name ="Choose Image")]
        public byte[] ImageData { get; set; } = Array.Empty<byte>();
        public string ImageType { get; set; } = "";

        //This model should have a list of Posts as Children 
        public ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();


    }
}
