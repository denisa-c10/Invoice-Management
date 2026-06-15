namespace Facturi.Services;

public interface IFactura
{
    List<FacturaModel2> GetFacturi();
    FacturaModel2 GetFacturaByNr(int nrFactura);
    List<FacturaModel2> GetFacturiByClient(int idClient);
    void AddFactura(FacturaModel2 factura);
    void UpdateFactura(int nrFactura, FacturaModel2 factura);
    void DeleteFactura(int nrFactura);
    void DeleteProdusDinFactura(int nrFactura, int idProdus);
}
