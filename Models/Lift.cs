namespace Bazis.Models;

public class Lift
{
    public int     LiftId          { get; set; }
    public int     ServiceCenterId { get; set; } // FK → ServiceCenter
    public string  Name            { get; set; } // "Подъёмник 1"
    public ServiceCenter  ServiceCenter { get; set; }
}