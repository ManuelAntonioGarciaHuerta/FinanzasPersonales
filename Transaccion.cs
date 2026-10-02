namespace FinanzasPersonales;

public enum TipoTransaccion
{
    Ingreso,
    Gasto
}

public class Transaccion
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public TipoTransaccion Tipo { get; set; }

    public override string ToString()
    {
        string signo = Tipo == TipoTransaccion.Ingreso ? "+" : "-";
        string tipoTexto = Tipo == TipoTransaccion.Ingreso ? "Ingreso" : "Gasto  ";
        return $"#{Id,-4} {Fecha:dd/MM/yyyy}  {tipoTexto}  {Categoria,-15} {signo}{Monto,10:C}  {Descripcion}";
    }
}
