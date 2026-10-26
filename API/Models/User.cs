using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

[Index(nameof(Email), IsUnique = true)]
[Table("t_e_user_use")]
public class User
{
    [Key]
    [Column("use_id")]
    public int Id { get; set; }
    
    [Column("use_last_name")]
    [Required]
    [MaxLength(254)]
    public required string LastName { get; set; }
    [Column("use_first_name")]
    [Required]
    [MaxLength(254)]
    public required string FirstName { get; set; }
    [Column("use_email")]
    [Required]
    [MaxLength(254)]
    public required string Email { get; set; }
    [Column("use_password_hash")]
    public required string PasswordHash { get; set; }
    [Column("use_created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("use_updated_at")]
    public DateTime UpdatedAt { get; set; }
}