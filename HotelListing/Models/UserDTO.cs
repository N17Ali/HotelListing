using System.ComponentModel.DataAnnotations;

namespace HotelListing.Models;

public class LoginUserDTO
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required]
    [StringLength(15, MinimumLength = 5, ErrorMessage = "Password must be between 5 and 15 characters")]
    public string Password { get; set; }
}

public class UserDTO : LoginUserDTO
{
    public string FirstName {get; set; }
    public string LastName { get; set; }

    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; }

    public ICollection<string> Roles { get; set; }
}
