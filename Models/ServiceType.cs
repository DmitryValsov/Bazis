namespace Bazis.Models;


public class ServiceType
{
    public int  ServiceTypeId { get; set; } // PK
    public string  Name { get; set; } // например, "Шиномонтаж"
    public int DurationHours  { get; set; } // 1, 3, 6 или любое другое
}