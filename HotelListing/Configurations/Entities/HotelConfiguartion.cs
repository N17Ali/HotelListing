using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Configurations.Entities;

public class HotelConfiguartion : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.HasData(
            new Hotel
            {
                Id = 1,
                Name = "Mitsui Garden Hotel Shinmachi Bettei",
                Address = "361 Rokkakucho, Nakagyo Ward, Kyoto, 604-8212, Japan",
                Rating = 4.3,
                CountryId = 1
            },
            new Hotel
            {
                Id = 2,
                Name = "Parsian Azadi Hotel",
                Address = "Tehran Province, Tehran, Chamran Highway، Q9QQ+WWW, Iran",
                Rating = 3,
                CountryId = 2
            },
            new Hotel
            {
                Id = 3,
                Name = "The Chanler at Cliff Walk",
                Address = "117 Memorial Blvd, Newport, RI 02840",
                Rating = 5,
                CountryId = 3
            }
        );
    }
}
