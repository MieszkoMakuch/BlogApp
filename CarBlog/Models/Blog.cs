using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BlogApp.Models
{
    public class Blog
    {
        //
        // Primary Key of the Table "Blogs" in database
        [Key]
        [StringLength(50)]
        public string Url { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Publication> Publications { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }

    }
}