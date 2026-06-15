using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Facturi;

public class ClientModel
{
    [Key]
    public int IdClient { get; set; }
    public string NumeClient { get; set; }
    public string Adresa { get; set; }

    public string Telefon { get; set; }

    // Relația: un client poate avea mai multe facturi
    [JsonIgnore]
    public List<FacturaModel2>? Facturi { get; set; }

    public ClientModel(int id, string nume, string adresa, string telefon)
    {
        this.IdClient = id;
        this.NumeClient = nume;
        this.Adresa = adresa;
        this.Telefon = telefon;
    }

    public ClientModel()
    {
    }
}