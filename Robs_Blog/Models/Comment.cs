using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Robs_Blog.Models
{
    //defining class
    public class Comment
    {
        //Unique value to reference
        public int Id { get; set; }        
        //Holds Primary Key
        public int BlogPostId { get; set; }
        //who wrote the comment
        public string AuthorId { get;set; }
        //Time it was created
        public DateTime Created { get; set; }
        //Holds the comment
        
        public string CommentBody { get; set; }
        //When its updated can be nullable
        public DateTime? Updated { get; set; }
        //Update reason
        public string UpdateReason { get; set; }
        
        //Virtual Navigation Section
        public virtual BlogPost BlogPost { get; set; }
        public virtual ApplicationUser Author { get; set; }



    }
}