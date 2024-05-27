using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BangchakStationService.Models
{

    [Table("users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("user_id", TypeName = "varchar")]
        [StringLength(450)]
        public string UserId { get; set; } = null!;

        [Column("fullname")]
        public string Fullname { get; set; } = null!;

    }
}
