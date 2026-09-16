namespace Practical_Test_Xtramile.Application.Features.Countries.DTOs;

public record CountryDto(int Id, string Code, string Name);
public record CityDto(int Id, string Name, string CountryCode);
