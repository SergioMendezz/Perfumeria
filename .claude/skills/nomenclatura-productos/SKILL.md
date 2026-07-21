---
name: nomenclatura-productos
description: Procedimiento para normalizar los campos Nombre y Marca de cualquier producto (perfume, desodorante, body spray, shower gel, crema corporal, body o estuche) a mayúscula inicial tras cada espacio, tanto en el formulario de creación/edición del panel admin como en el backend antes de persistir. Se activa automáticamente al crear, editar o validar estos dos campos.
---

# Skill · Nomenclatura de productos (mayúscula tras cada espacio)

## Cuándo aplica
Cualquier flujo de creación o edición de: `Perfume`, `Desodorante`, `BodySpray`, `ShowerGel`,
`CremaCorporal`, `Body`, `Estuche` o `Marca` — específicamente sus campos `Nombre` y `Marca`
(nombre de la marca). Ningún otro campo se normaliza con esta regla (no aplica a `Descripcion`,
`CodigoBarras`, etc.).

## Regla exacta (tal como la definió la propietaria del negocio)
> "Que al colocar la primera letra luego de cada espacio sea en mayúscula, solo en el nombre y en
> la marca de los productos."

Es decir: la primera letra de la cadena, y la primera letra después de cada espacio, se convierten a
mayúscula. El resto de cada palabra se deja tal cual el usuario lo escribió (esta skill **no**
fuerza minúsculas en el resto — evita "corregir" acrónimos de marca que el usuario haya escrito
intencionalmente en mayúsculas, p. ej. "YSL").

## Algoritmo de referencia (TypeScript — frontend, `perfumeria-admin`)

```ts
export function normalizarNombreProducto(valor: string): string {
  return valor
    .split(' ')
    .map((palabra) =>
      palabra.length === 0
        ? palabra
        : palabra.charAt(0).toUpperCase() + palabra.slice(1)
    )
    .join(' ');
}
```

Aplicar en el `onBlur` del campo (no en cada `onChange`, para no interrumpir al usuario mientras
escribe) y también justo antes de enviar el formulario.

## Algoritmo de referencia (C# — backend, defensa en profundidad)

```csharp
public static string NormalizarNombreProducto(string valor)
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
```

Invocar en `Flujo` (no en `API` ni en `DA`) justo antes de pasar el `Request` a `DA.Agregar`/`Editar`
— así el backend nunca confía ciegamente en que el frontend ya lo normalizó.

## Casos borde a considerar
- Espacios múltiples consecutivos (`"Chanel  N°5"`) — el algoritmo de referencia no colapsa espacios
  dobles; si `docs/vision.md` no pide colapsarlos, no agregar esa lógica sin confirmarlo primero
  (regla de "no inventar reglas").
- Cadena vacía o solo espacios — devolver tal cual, no lanzar excepción.
- Nombres que ya vienen en mayúsculas de marca (`"YSL"`, `"D&G"`) — el algoritmo no los altera más
  allá de la primera letra de cada "palabra" separada por espacio, así que quedan intactos.

## Dónde probarlo (TDD)
Escribir la prueba unitaria del método `NormalizarNombreProducto`/`normalizarNombreProducto` como
parte del ciclo RED→GREEN normal de la historia que agregue el campo — no es una historia aparte,
es parte de la validación de `PerfumeRequest`/`MarcaRequest`, etc.
