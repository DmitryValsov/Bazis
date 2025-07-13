namespace Bazis.Models
{
   public class Slot
{
    public int     SlotId           { get; set; }
    public int     ServiceCenterId  { get; set; } // FK → ServiceCenter
    public int     LiftId           { get; set; } // FK → Lift
    public string  Date             { get; set; } // "yyyy-MM-dd"
    public string  Time             { get; set; } // "HH:mm"
    public bool    IsBooked         { get; set; }

    public ServiceCenter ServiceCenter { get; set; }
    public Lift          Lift          { get; set; }
}

}