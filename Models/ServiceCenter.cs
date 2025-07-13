namespace Bazis.Models;

public class ServiceCenter
{
    public int  ServiceCenterId { get; set; } // PK
    public string  Name { get; set; } // например, "Центр на ул. Ленина"
    public string?  Address { get; set; } // например, "Центр на ул. Ленина"
    public string?  Phone { get; set; } // например, "Центр на ул. Ленина"
    public string?  OpeningHours { get; set; } // например, "Центр на ул. Ленина"
    public string?  MapLink { get; set; } // например, "Центр на ул. Ленина"
    public string?  Rating { get; set; } // например, "Центр на ул. Ленина"
    public ICollection<Lift> Lifts { get; set; }
}