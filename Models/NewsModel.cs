using System.ComponentModel.DataAnnotations;

namespace Bazis.Models;

public class News
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? PreviewText { get; set; }
    public string? DetailText { get; set; }
   public string? NamePicture { get; set; }
    public string? Path { get; set; }

    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; }

}
