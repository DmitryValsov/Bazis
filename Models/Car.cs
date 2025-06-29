// Models/Item.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Bazis.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }

        public string? Year { get; set; }

        public string? Color { get; set; }

        public string? VIN { get; set; }

        public string? GosNomer { get; set; }

        public string? Img { get; set; }




        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }


        [NotMapped]
        public IFormFile? ImageFile { get; set; }


        //Связь с пользователем 
       public string? UserId { get; set; }
       public IdentityUser? User { get; set; }

    
    }
}
