using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bazis.Models;

public class CatalogCar
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? PreviewText { get; set; }

    public string? Link { get; set; }

    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; }
    
    public string? Img { get; set; }

    [NotMapped]
    public IFormFile? ImageFile { get; set; }

}
