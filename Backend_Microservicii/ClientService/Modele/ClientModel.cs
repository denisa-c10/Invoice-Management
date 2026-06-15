using System.ComponentModel.DataAnnotations;

namespace ClientService.Modele;

public class ClientModel
{
    [Key]
    public int IdClient { get; set; }
    public string Nume { get; set; } = string.Empty;
    public string Adresa { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}