﻿using System.ComponentModel.DataAnnotations;

namespace AtlasBlog.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be atleast {2} and at most {1} charcters long", MinimumLength = 2)]
        public string Title { get; set; } = "";

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be atleast {2} and at most {1} charcters long", MinimumLength = 2)]
        public string Abstract { get; set; } = "";
      
        [Required]
        //[StringLength(100, ErrorMessage = "The {0} must be atleast {2} and at most {1} charcters long", MinimumLength = 2)]
        public string Body { get; set; } = "";

        public DateTime Created{ get; set; }
        public DateTime? Updated { get; set; }




        //Navigation Properties
        public Blog Blog { get; set; } = default!;
    }
}