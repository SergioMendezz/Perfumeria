---
name: codigo-barras
description: Procedimiento para implementar búsqueda de productos por código de barras (completo o por últimos dígitos) y para que el panel admin acepte entrada de una pistola lectora de código de barras tanto al buscar como al registrar productos. Se activa automáticamente en cualquier trabajo sobre el módulo de inventario o CodigoBarras.
---

# Skill · Código de barras (búsqueda + pistola lectora)

## Contexto de negocio
Con un volumen esperado de ~2 millones de unidades de inventario (`docs/vision.md` §4), el personal
necesita encontrar un producto exacto en segundos, ya sea escaneándolo con una pistola lectora o
escribiendo solo los últimos dígitos visibles de una etiqueta parcialmente dañada.

## Cómo se comporta una pistola lectora de código de barras (dato técnico clave)
La gran mayoría de lectores USB se comportan como un **teclado (HID)**: al escanear, "escriben" el
código carácter por carácter en el campo que tenga el foco, y terminan enviando una tecla `Enter`.
**No requieren driver ni librería especial** — cualquier `<input>` de texto enfocado los recibe
correctamente. La única responsabilidad del frontend es:

1. Mantener el campo de búsqueda enfocado por defecto en la pantalla de inventario.
2. Escuchar el evento `Enter` (`onKeyDown` con `e.key === 'Enter'`) y disparar la búsqueda
   automáticamente — sin exigir que el personal haga clic en un botón "Buscar".

```tsx
function useCampoEscaneable(alConfirmar: (codigo: string) => void) {
  const [valor, setValor] = useState('');

  function manejeKeyDown(e: React.KeyboardEvent<HTMLInputElement>) {
    if (e.key === 'Enter' && valor.trim().length > 0) {
      alConfirmar(valor.trim());
    }
  }

  return { valor, setValor, manejeKeyDown };
}
```

## Búsqueda por código completo o por sufijo (últimos dígitos)

- **Código completo:** coincidencia exacta (`WHERE CodigoBarras = @Codigo`), es la búsqueda más
  rápida — usar siempre que la longitud del texto ingresado coincida con la longitud estándar de un
  código de barras (EAN-13 = 13 dígitos, UPC-A = 12 dígitos).
- **Sufijo (últimos dígitos):** cuando el texto ingresado es más corto, buscar por **sufijo**, no por
  subcadena en cualquier posición — evita falsos positivos de coincidencias en el medio del código.

```sql
-- Stored procedure BuscarPorCodigoBarras (ejemplo de forma)
CREATE PROCEDURE BuscarPorCodigoBarras
    @Codigo VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    IF LEN(@Codigo) >= 12
        SELECT * FROM Perfumes WHERE CodigoBarras = @Codigo;
    ELSE
        SELECT * FROM Perfumes WHERE CodigoBarras LIKE '%' + @Codigo;
END
```

**Regla de rendimiento:** un `LIKE '%sufijo'` (comodín al inicio) no puede usar un índice B-tree
estándar de forma eficiente sobre 2 millones de filas. Antes de implementar en producción, evaluar
un índice invertido (columna calculada con el string reverso + índice sobre esa columna) si el
rendimiento de `LIKE '%...'` resulta insuficiente en pruebas de carga — documentarlo como nota de
refinamiento en la US correspondiente si se detecta el problema, no resolverlo de forma implícita.

## Registro de un producto nuevo escaneando su código
Al crear un producto, el campo `CodigoBarras` debe aceptar el mismo patrón de "escanear + Enter" que
la búsqueda — reutilizar el mismo hook `useCampoEscaneable`. Antes de guardar, validar unicidad del
código dentro de su propia tabla (`docs/vision.md` §4) y mostrar un error claro si ya existe, en vez
de fallar silenciosamente o duplicar el registro.

## Errores comunes a evitar
- Exigir que el personal presione un botón "Buscar" además del `Enter` de la pistola (fricción
  innecesaria que el requisito original pide evitar explícitamente).
- Buscar con `LIKE '%sufijo%'` (comodines en ambos extremos) en vez de `LIKE '%sufijo'` — puede
  devolver coincidencias en el medio del código que no corresponden a "los últimos dígitos".
- Olvidar validar la longitud/formato antes de decidir si la búsqueda es exacta o por sufijo.
