﻿using System.ComponentModel.DataAnnotations;

namespace HotelListing.Models;

public class CreateHotelDTO
{
    [Required]
    [StringLength(maximumLength: 150, ErrorMessage = "Hotel name must be less than 150 characters")]
    public string? Name { get; set; }
    
    [Required]
    [StringLength(maximumLength: 250, ErrorMessage = "Hotel address must be less than 250 characters")]
    public string? Address { get; set; }
    
    [Required]
    [Range(1,5)]
    public double? Rating { get; set; }

    [Required(ErrorMessage = "Required")]
    public int? CountryId { get; set; }
}

public class UpdateHotelDTO : CreateHotelDTO
{ }

public class HotelDTO : CreateHotelDTO
{
    public int? Id { get; set; }
    public CountryDTO Country { get; set; }
}