namespace Abstracciones.Excepciones;

public class CodigoBarrasDuplicadoException : Exception
{
    public CodigoBarrasDuplicadoException(string codigoBarras)
        : base($"Ya existe un perfume con el código de barras '{codigoBarras}'.")
    {
    }
}
