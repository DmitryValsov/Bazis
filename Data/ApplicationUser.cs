// Data/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using Bazis.Models;

namespace Bazis.Data
{
    public class ApplicationUser : IdentityUser
    {
        // Навигационное свойство: у одного пользователя много заказов
        public ICollection<Order> Orders { get; set; } = new List<Order>();

         public ICollection<Car> Car { get; set; } = new List<Car>();
    }
}
