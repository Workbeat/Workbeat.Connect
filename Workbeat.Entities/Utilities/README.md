## Object Mapper
Esta librería es utilizada para hacer el mapeo entre identificadores creados en Workbeat y los creados en el cliente externo.

Si se crea un empleado en Workbeat, se le asigna un identificador. Cuando se quiere crear en el cliente externo, este tambien 
puede que le asigne un identificador diferente. Este objeto ayuda a hacer el link entre los dos entidades.

## Setup
La librería de object mapper guarda las relaciones entre objetos en una base de datos de SQLite. https://www.sqlite.org .
Es necesario incluir las librerías de SQLite.

1. Descargue los binarios necesarios, segun su versión de .NET de [la pagina de SQLite](http://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki)
(en nuestro caso se descargaron los [binarios precompilados para .NET 4.0 64bits](http://system.data.sqlite.org/downloads/1.0.98.0/sqlite-netFx40-binary-bundle-x64-2010-1.0.98.0.zip).
Puede cambiar, segun su ambiente de trabajo)
2. Extraer los archivos del zip a un directorio.
3. Copiar System.Data.SQLite.dll al directorio "Referencias" de la aplicacion inicial.
4. Incluir en el proyecto este dll como referencia.

Nota: [Este es un artículo que explica como utilizar SQLite en .NET](http://blog.tigrangasparian.com/2012/02/09/getting-started-with-sqlite-in-c-part-one/)

### Objeto Map

#### Propiedades
|**Propiedad**|**Tipo**|**Descripcion**|
|entityType|Enum EntityType|Tipo de entidad a mapear (Empleado, posicion)
|workbeatId|string|Identificador del lado de Workbeat|

#### Métodos



