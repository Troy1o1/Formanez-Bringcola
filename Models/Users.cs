using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Formanez_Bringcola.Models
{
    [Table("Users")]
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        [Column(TypeName = "nvarchar(100)")]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string PasswordHash { get; set; }

        // Add these two properties
        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string? FirstName { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string? LastName { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column(TypeName = "datetime2")]
        public DateTime? LastLoginAt { get; set; }

        [Column(TypeName = "bit")]
        public bool IsActive { get; set; } = true;

        
    }
}