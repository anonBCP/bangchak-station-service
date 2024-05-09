using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotnetAPIApp.Models
{

    [Table("category")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("category_name", TypeName = "varchar")]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [Column("is_active")]
        public bool IsActive {  get; set; }

      

    }
}
