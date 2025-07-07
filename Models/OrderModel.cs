// Models/Order.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bazis.Data;  // чтобы видеть ApplicationUser
using Microsoft.AspNetCore.Identity;

namespace Bazis.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        // Когда создан заказ
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }

        // Пример дополнительного поля
        [Required]
        [StringLength(200)]
        public string? Name { get; set; }
        [Required]
        [StringLength(200)]
        public string? Phone { get; set; }


        //[Required]
        //[StringLength(200)]
        //public string? Auto { get; set; }

        [Required]
        [StringLength(200)]
        public string? ServiceAddress { get; set; }
        [Required]
        [StringLength(200)]
        public string? Usluga { get; set; }
        [Required]
        [StringLength(200)]
        public string? Comment { get; set; }
        [Required]
        [StringLength(200)]
        public string? Status { get; set; }
        


        public int? CarId { get; set; } // Optional foreign key property
        public Car? Car { get; set; }
    
    }
}
