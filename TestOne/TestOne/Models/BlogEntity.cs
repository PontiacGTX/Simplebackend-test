namespace TestOne.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class BlogEntity
    {
        // Do not change names of these properties
        [Key]
        [Required]
        public int BlogId { get; set; }
        [Required]
        [MaxLength(50), MinLength(10)]
        public string Name { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public List<PostEntity> Articles { get; set; }

    }
}
