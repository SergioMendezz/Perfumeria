namespace Abstracciones.Excepciones;

public class CredencialesInvalidasException : Exception
{
    public CredencialesInvalidasException()
        : base("Correo o contraseña incorrectos.")
    {
    }
}
