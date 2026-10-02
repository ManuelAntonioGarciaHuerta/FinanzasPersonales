using System.Text.Json;

namespace FinanzasPersonales;

public class GestorFinanzas
{
    private readonly string _rutaArchivo;
    private List<Transaccion> _transacciones = new();
    private int _siguienteId = 1;

    public GestorFinanzas(string rutaArchivo = "finanzas.json")
    {
        _rutaArchivo = rutaArchivo;
        Cargar();
    }

    public void AgregarTransaccion(DateTime fecha, string descripcion, decimal monto, string categoria, TipoTransaccion tipo)
    {
        var transaccion = new Transaccion
        {
            Id = _siguienteId++,
            Fecha = fecha,
            Descripcion = descripcion,
            Monto = monto,
            Categoria = categoria,
            Tipo = tipo
        };

        _transacciones.Add(transaccion);
        Guardar();
    }

    public bool EliminarTransaccion(int id)
    {
        var transaccion = _transacciones.FirstOrDefault(t => t.Id == id);
        if (transaccion is null) return false;

        _transacciones.Remove(transaccion);
        Guardar();
        return true;
    }

    public List<Transaccion> ObtenerTodas()
    {
        return _transacciones.OrderBy(t => t.Fecha).ToList();
    }

    public decimal ObtenerBalance()
    {
        decimal ingresos = _transacciones.Where(t => t.Tipo == TipoTransaccion.Ingreso).Sum(t => t.Monto);
        decimal gastos = _transacciones.Where(t => t.Tipo == TipoTransaccion.Gasto).Sum(t => t.Monto);
        return ingresos - gastos;
    }

    public decimal ObtenerTotalIngresos() =>
        _transacciones.Where(t => t.Tipo == TipoTransaccion.Ingreso).Sum(t => t.Monto);

    public decimal ObtenerTotalGastos() =>
        _transacciones.Where(t => t.Tipo == TipoTransaccion.Gasto).Sum(t => t.Monto);

    public Dictionary<string, decimal> ObtenerGastosPorCategoria()
    {
        return _transacciones
            .Where(t => t.Tipo == TipoTransaccion.Gasto)
            .GroupBy(t => t.Categoria)
            .OrderByDescending(g => g.Sum(t => t.Monto))
            .ToDictionary(g => g.Key, g => g.Sum(t => t.Monto));
    }

    public Dictionary<string, (decimal Ingresos, decimal Gastos)> ObtenerResumenMensual()
    {
        return _transacciones
            .GroupBy(t => $"{t.Fecha:yyyy-MM}")
            .OrderBy(g => g.Key)
            .ToDictionary(
                g => g.Key,
                g => (
                    Ingresos: g.Where(t => t.Tipo == TipoTransaccion.Ingreso).Sum(t => t.Monto),
                    Gastos: g.Where(t => t.Tipo == TipoTransaccion.Gasto).Sum(t => t.Monto)
                )
            );
    }

    private void Guardar()
    {
        try
        {
            var opciones = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_transacciones, opciones);
            File.WriteAllText(_rutaArchivo, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  No se pudo guardar el archivo: {ex.Message}");
        }
    }

    private void Cargar()
    {
        if (!File.Exists(_rutaArchivo)) return;

        try
        {
            string json = File.ReadAllText(_rutaArchivo);
            var datos = JsonSerializer.Deserialize<List<Transaccion>>(json);
            if (datos is not null)
            {
                _transacciones = datos;
                _siguienteId = _transacciones.Count > 0 ? _transacciones.Max(t => t.Id) + 1 : 1;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  No se pudo cargar el archivo existente: {ex.Message}");
        }
    }
}
