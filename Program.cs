using System.Globalization;
using FinanzasPersonales;

var cultura = new CultureInfo("es-MX");
Thread.CurrentThread.CurrentCulture = cultura;

var gestor = new GestorFinanzas();
bool salir = false;

while (!salir)
{
    MostrarMenu();
    string opcion = Console.ReadLine()?.Trim() ?? "";

    switch (opcion)
    {
        case "1":
            RegistrarTransaccion(TipoTransaccion.Ingreso);
            break;
        case "2":
            RegistrarTransaccion(TipoTransaccion.Gasto);
            break;
        case "3":
            ListarTransacciones();
            break;
        case "4":
            EliminarTransaccion();
            break;
        case "5":
            MostrarBalance();
            break;
        case "6":
            MostrarReportePorCategoria();
            break;
        case "7":
            MostrarResumenMensual();
            break;
        case "0":
            salir = true;
            Console.WriteLine("¡Hasta luego! 👋");
            break;
        default:
            Console.WriteLine("Opción no válida. Intenta de nuevo.\n");
            break;
    }
}

void MostrarMenu()
{
    Console.WriteLine("=========================================");
    Console.WriteLine("     💰 CALCULADORA DE FINANZAS PERSONALES");
    Console.WriteLine("=========================================");
    Console.WriteLine("1. Registrar ingreso");
    Console.WriteLine("2. Registrar gasto");
    Console.WriteLine("3. Ver todas las transacciones");
    Console.WriteLine("4. Eliminar una transacción");
    Console.WriteLine("5. Ver balance total");
    Console.WriteLine("6. Reporte por categoría");
    Console.WriteLine("7. Resumen mensual");
    Console.WriteLine("0. Salir");
    Console.Write("Elige una opción: ");
}

void RegistrarTransaccion(TipoTransaccion tipo)
{
    Console.WriteLine();
    string etiqueta = tipo == TipoTransaccion.Ingreso ? "ingreso" : "gasto";

    Console.Write($"Descripción del {etiqueta}: ");
    string descripcion = Console.ReadLine() ?? "";

    decimal monto = LeerDecimal("Monto: $");

    Console.Write("Categoría (ej. Comida, Transporte, Sueldo, Renta): ");
    string categoria = Console.ReadLine() ?? "Sin categoría";
    if (string.IsNullOrWhiteSpace(categoria)) categoria = "Sin categoría";

    DateTime fecha = LeerFechaOpcional();

    gestor.AgregarTransaccion(fecha, descripcion, monto, categoria, tipo);
    Console.WriteLine($"✅ {etiqueta} registrado correctamente.\n");
}

decimal LeerDecimal(string mensaje)
{
    decimal valor;
    while (true)
    {
        Console.Write(mensaje);
        string entrada = Console.ReadLine() ?? "";
        if (decimal.TryParse(entrada, NumberStyles.Any, CultureInfo.InvariantCulture, out valor) && valor > 0)
        {
            return valor;
        }
        if (decimal.TryParse(entrada, out valor) && valor > 0)
        {
            return valor;
        }
        Console.WriteLine("⚠️  Ingresa un número válido mayor a 0.");
    }
}

DateTime LeerFechaOpcional()
{
    Console.Write("Fecha (dd/mm/aaaa) o Enter para hoy: ");
    string entrada = Console.ReadLine() ?? "";
    if (string.IsNullOrWhiteSpace(entrada)) return DateTime.Today;

    if (DateTime.TryParse(entrada, cultura, DateTimeStyles.None, out DateTime fecha))
    {
        return fecha;
    }

    Console.WriteLine("⚠️  Fecha no válida, se usará la fecha de hoy.");
    return DateTime.Today;
}

void ListarTransacciones()
{
    Console.WriteLine();
    var transacciones = gestor.ObtenerTodas();

    if (transacciones.Count == 0)
    {
        Console.WriteLine("No hay transacciones registradas todavía.\n");
        return;
    }

    Console.WriteLine("--- TRANSACCIONES ---");
    foreach (var t in transacciones)
    {
        Console.WriteLine(t);
    }
    Console.WriteLine();
}

void EliminarTransaccion()
{
    ListarTransacciones();
    Console.Write("ID de la transacción a eliminar: ");
    if (int.TryParse(Console.ReadLine(), out int id))
    {
        bool exito = gestor.EliminarTransaccion(id);
        Console.WriteLine(exito ? "✅ Transacción eliminada.\n" : "⚠️  No se encontró esa transacción.\n");
    }
    else
    {
        Console.WriteLine("⚠️  ID no válido.\n");
    }
}

void MostrarBalance()
{
    Console.WriteLine();
    decimal ingresos = gestor.ObtenerTotalIngresos();
    decimal gastos = gestor.ObtenerTotalGastos();
    decimal balance = gestor.ObtenerBalance();

    Console.WriteLine("--- BALANCE GENERAL ---");
    Console.WriteLine($"Ingresos totales: {ingresos:C}");
    Console.WriteLine($"Gastos totales:   {gastos:C}");
    Console.WriteLine($"Balance:          {balance:C}");
    Console.WriteLine();
}

void MostrarReportePorCategoria()
{
    Console.WriteLine();
    var porCategoria = gestor.ObtenerGastosPorCategoria();

    if (porCategoria.Count == 0)
    {
        Console.WriteLine("No hay gastos registrados todavía.\n");
        return;
    }

    Console.WriteLine("--- GASTOS POR CATEGORÍA ---");
    foreach (var (categoria, total) in porCategoria)
    {
        Console.WriteLine($"{categoria,-20} {total:C}");
    }
    Console.WriteLine();
}

void MostrarResumenMensual()
{
    Console.WriteLine();
    var resumen = gestor.ObtenerResumenMensual();

    if (resumen.Count == 0)
    {
        Console.WriteLine("No hay transacciones registradas todavía.\n");
        return;
    }

    Console.WriteLine("--- RESUMEN MENSUAL ---");
    foreach (var (mes, datos) in resumen)
    {
        decimal balanceMes = datos.Ingresos - datos.Gastos;
        Console.WriteLine($"{mes}  |  Ingresos: {datos.Ingresos,10:C}  |  Gastos: {datos.Gastos,10:C}  |  Balance: {balanceMes,10:C}");
    }
    Console.WriteLine();
}
