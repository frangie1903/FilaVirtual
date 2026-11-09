using System.Globalization;

namespace FilaVirtual.Models;

// Un tramo = un día de la semana abierto con su hora de apertura y cierre.
public record TramoDia(DayOfWeek Dia, TimeSpan Apertura, TimeSpan Cierre);

// Guarda/lee el horario semanal como texto: "1|08:00|17:00;6|09:00|13:00"
// (día de la semana | apertura | cierre). Un día que no aparece = cerrado.
public static class HorarioHelper
{
    private const string FormatoHora = @"hh\:mm";

    public static readonly string[] Abreviaturas =
        { "Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb" };

    // Lunes = 0 ... Domingo = 6 (para ordenar la semana empezando en lunes).
    public static int Orden(DayOfWeek dia) => ((int)dia + 6) % 7;

    public static string Serializar(IEnumerable<TramoDia> tramos) =>
        string.Join(";", tramos.Select(t =>
            $"{(int)t.Dia}|{t.Apertura.ToString(FormatoHora)}|{t.Cierre.ToString(FormatoHora)}"));

    public static List<TramoDia> Leer(string? texto)
    {
        var resultado = new List<TramoDia>();

        if (string.IsNullOrWhiteSpace(texto))
            return resultado;

        foreach (var parte in texto.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var p = parte.Split('|');

            if (p.Length == 3 &&
                int.TryParse(p[0], out int dia) && dia is >= 0 and <= 6 &&
                TimeSpan.TryParseExact(p[1], FormatoHora, CultureInfo.InvariantCulture, out var apertura) &&
                TimeSpan.TryParseExact(p[2], FormatoHora, CultureInfo.InvariantCulture, out var cierre))
            {
                resultado.Add(new TramoDia((DayOfWeek)dia, apertura, cierre));
            }
        }

        return resultado;
    }

    // "Lun-Vie 08:00-17:00, Sáb 09:00-13:00"
    public static string Describir(IEnumerable<TramoDia> tramos)
    {
        var lista = tramos.OrderBy(t => Orden(t.Dia)).ToList();

        if (lista.Count == 0)
            return "Sin horario";

        var grupos = new List<string>();
        int inicio = 0;

        for (int i = 1; i <= lista.Count; i++)
        {
            bool continua = i < lista.Count
                && Orden(lista[i].Dia) == Orden(lista[i - 1].Dia) + 1
                && lista[i].Apertura == lista[inicio].Apertura
                && lista[i].Cierre == lista[inicio].Cierre;

            if (continua)
                continue;

            var primero = lista[inicio];
            var ultimo = lista[i - 1];

            string dias = inicio == i - 1
                ? Abreviaturas[(int)primero.Dia]
                : $"{Abreviaturas[(int)primero.Dia]}-{Abreviaturas[(int)ultimo.Dia]}";

            grupos.Add($"{dias} {primero.Apertura.ToString(FormatoHora)}-{primero.Cierre.ToString(FormatoHora)}");
            inicio = i;
        }

        return string.Join(", ", grupos);
    }
}
