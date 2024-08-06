public class Users
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Password { get; set; }
    public object IdsEstablishments { get; set; } // Puede cambiarse por un tipo de dato más específico según la estructura esperada
    public object Ids { get; set; } // Puede cambiarse por un tipo de dato más específico según la estructura esperada
}