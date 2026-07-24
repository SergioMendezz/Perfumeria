namespace Abstracciones.Excepciones;

public class PerfumeNoEncontradoException : Exception
{
    public PerfumeNoEncontradoException(Guid id)
        : base($"No existe un perfume con el Id '{id}'.")
    {
    }
}
