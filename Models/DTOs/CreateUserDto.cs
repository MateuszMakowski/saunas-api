using System.ComponentModel.DataAnnotations;

public class CreateUserDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; }

    // Dodaj inne adnotacje walidacyjne zgodnie z potrzebami
}
