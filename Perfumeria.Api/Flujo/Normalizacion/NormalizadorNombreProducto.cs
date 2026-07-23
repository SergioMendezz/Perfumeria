namespace Flujo.Normalizacion;

public static class NormalizadorNombreProducto
{
    public static string Normalizar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return valor;

        var palabras = valor.Split(' ');
        for (int i = 0; i < palabras.Length; i++)
        {
            if (palabras[i].Length > 0)
            {
                palabras[i] = char.ToUpper(palabras[i][0]) + palabras[i].Substring(1);
            }
        }
        return string.Join(' ', palabras);
    }
}
