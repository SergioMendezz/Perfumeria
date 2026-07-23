namespace Abstracciones.Excepciones;

public class CategoriaInvalidaException : Exception
{
    public CategoriaInvalidaException(string categoria)
        : base($"La categoría '{categoria}' no es válida. Debe usarse el nombre completo (ver docs/vision.md §3.1).")
    {
    }
}
