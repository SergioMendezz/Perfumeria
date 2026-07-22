namespace Abstracciones.Modelos.Compartido;

public class ResultadoPaginado<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int Total { get; set; }
}
