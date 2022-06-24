using System.ComponentModel.DataAnnotations;

namespace HotelListing.Models;

public class CreateCountryDTO
{
    [Required]
    [StringLength(maximumLength: 50, ErrorMessage = "Country name must be less than 50 characters")]
    public string? Name { get; set; }
    
    [Required]
    [StringLength(maximumLength: 3, ErrorMessage = "Country short name must be less than 3 characters")]
    public string? ShortName { get; set; }
}

public class UpdateCountryDTO : CreateCountryDTO
{
    public IList<CreateHotelDTO> Hotels { get; set; }
}

public class CountryDTO : UpdateCountryDTO
{
    public int? Id { get; set; }
}

