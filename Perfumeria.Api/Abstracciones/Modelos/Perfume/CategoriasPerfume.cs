namespace Abstracciones.Modelos.Perfume;

public static class CategoriasPerfume
{
    public static readonly IReadOnlyCollection<string> Validas = new[]
    {
        "Eau de Parfum",
        "Eau de Toilette",
        "Eau de Cologne",
        "Extrait de Parfum",
        "Eau Fraîche"
    };
}
