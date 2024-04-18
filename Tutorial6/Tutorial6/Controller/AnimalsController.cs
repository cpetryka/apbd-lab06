using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Tutorial6.Model;
using Tutorial6.Model.Dto;

namespace Tutorial6.Controller;

[ApiController]
[Route(("api/[controller]"))]
public class AnimalsController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult GetAnimals([FromQuery] string orderBy = "name")
    {
        // Lista dozwolonych kolumn do sortowania
        var allowedOrderByValues = new HashSet<string>{"name", "description", "category", "area"};

        // Ustawienie domyślnego sortowania, jeśli przekazany parametr jest nieprawidłowy
        if (!allowedOrderByValues.Contains(orderBy))
        {
            orderBy = "name";
        }

        // Otwieramy połączenie
        using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        // Definiujemy command
        using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = $"SELECT * FROM Animal ORDER BY {orderBy} ASC";

        // Wykonujemy zapytanie
        var reader = command.ExecuteReader();

        // Przetwarzamy wynik zapytania
        var animals = new List<Animal>();

        int idAnimalOrdinal = reader.GetOrdinal("IdAnimal");
        int nameOrdinal = reader.GetOrdinal("Name");
        int descriptionOrdinal = reader.GetOrdinal("Description");
        int categoryOrdinal = reader.GetOrdinal("Category");
        int areaOrdinal = reader.GetOrdinal("Area");

        while (reader.Read())
        {
            animals.Add(new Animal()
            {
                IdAnimal = reader.GetInt32(idAnimalOrdinal),
                Name = reader.GetString(nameOrdinal),
                Description = reader.IsDBNull(descriptionOrdinal) ? null : reader.GetString(descriptionOrdinal),
                Category = reader.GetString(categoryOrdinal),
                Area = reader.GetString(areaOrdinal)
            });
        }

        return Ok(animals);
    }

    [HttpPost]
    public IActionResult AddAnimal(CreateAnimalDto createAnimalDto)
    {
        // Otwieramy połączenie
        using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        // Definiujemy command
        using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = $"INSERT INTO Animal VALUES (@animalName, @animalDescription, @animalCategory, @animalArea)";
        command.Parameters.AddWithValue("@animalName", createAnimalDto.Name);
        command.Parameters.AddWithValue("@animalDescription",
            createAnimalDto.Description.IsNullOrEmpty() ? "" : createAnimalDto.Description);
        command.Parameters.AddWithValue("@animalCategory", createAnimalDto.Category);
        command.Parameters.AddWithValue("@animalArea", createAnimalDto.Area);

        // Wykonujemy zapytanie
        command.ExecuteReader();

        return Created("api/animals", null);
    }

    [HttpPut("{idAnimal}")]
    public IActionResult UpdateAnimal(int idAnimal, UpdateAnimalDto updateAnimalDto)
    {
        // Otwieramy połączenie
        using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        // Definiujemy command
        using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "UPDATE Animal SET Name = @animalName, Description = @animalDescription, Category = @animalCategory, Area = @animalArea WHERE IdAnimal = @animalId";
        command.Parameters.AddWithValue("@animalName", updateAnimalDto.Name);
        command.Parameters.AddWithValue("@animalDescription",
            updateAnimalDto.Description.IsNullOrEmpty() ? "" : updateAnimalDto.Description);
        command.Parameters.AddWithValue("@animalCategory", updateAnimalDto.Category);
        command.Parameters.AddWithValue("@animalArea", updateAnimalDto.Area);
        command.Parameters.AddWithValue("@animalId", idAnimal);

        // Wykonujemy zapytanie
        command.ExecuteReader();

        return Ok();
    }

    [HttpDelete("{idAnimal}")]
    public IActionResult DeleteAnimal(int idAnimal)
    {
        // Otwieramy połączenie
        using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        // Definiujemy command
        using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "DELETE FROM Animal WHERE IdAnimal = @animalId";
        command.Parameters.AddWithValue("@animalId", idAnimal);

        // Wykonujemy zapytanie
        command.ExecuteReader();

        return NoContent();
    }
}