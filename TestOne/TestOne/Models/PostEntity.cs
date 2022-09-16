namespace TestOne.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class PostEntity
    {
        [Key]
        [Required]
        public int PostId { get; set; }
        [MaxLength(50), MinLength(10)]
        [Required]
        public string Name { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        [Required]
        public int ParentId { get; set; }
        public BlogEntity Blog { get; set; }
    }

}
