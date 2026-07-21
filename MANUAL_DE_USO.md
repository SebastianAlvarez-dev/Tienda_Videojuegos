# Manual de uso — GameStore API

Esta guía explica cómo preparar, ejecutar y probar la API aunque estés empezando con Node.js, Visual Studio Code y Postman.

## 1. Qué necesitas instalar

Instala estas herramientas antes de comenzar:

1. **Node.js LTS** (incluye `npm`). Esta API necesita una versión reciente de Node.js porque usa `--env-file`.
2. **Visual Studio Code** para abrir el proyecto y usar su terminal integrada.
3. **Postman** para enviar solicitudes HTTP a la API.
4. Una cuenta de **Supabase** con la base de datos ya configurada.

Para comprobar Node.js y npm, abre una terminal y ejecuta:

```powershell
node --version
npm --version
```

Si ambos comandos muestran una versión, la instalación está lista.

## 2. Abrir el proyecto en Visual Studio Code

1. Abre Visual Studio Code.
2. Selecciona **File > Open Folder**.
3. Elige la carpeta del proyecto `Examen Servidores`.
4. Abre la terminal integrada con **Terminal > New Terminal** o el atajo `Ctrl` + `` ` ``.
5. Confirma que la terminal muestra la carpeta del proyecto antes de ejecutar comandos.

## 3. Instalar las dependencias

En la terminal integrada, ejecuta una sola vez:

```powershell
npm install
```

Este comando descarga NestJS, Prisma, Vitest y las demás librerías definidas en `package.json`. Al terminar aparecerá la carpeta `node_modules`.

## 4. Configurar la conexión a Supabase

La API usa el archivo privado `.env` para conectarse a PostgreSQL. Este archivo contiene secretos y no se sube al repositorio.

1. Si todavía no existe `.env`, créalo copiando el ejemplo:

   ```powershell
   Copy-Item .env.example .env
   ```

2. Abre `.env` en Visual Studio Code.
3. Completa las URLs que entrega Supabase en **Connect**:

   ```env
   DATABASE_URL="URL_DEL_TRANSACTION_POOLER"
   DIRECT_URL="URL_DEL_SESSION_POOLER"
   PORT=3000
   ```

4. Sustituye `PASSWORD` por la contraseña real de tu base de datos, sin compartirla ni enviarla por chat.
5. Guarda el archivo.

Para esta base ya existente no hace falta crear las tablas otra vez. Si conectas una base nueva y vacía, ejecuta antes de iniciar la API:

```powershell
npm run prisma:generate
npx prisma migrate deploy
```

## 5. Iniciar la API

Ejecuta:

```powershell
npm run start:dev
```

Cuando veas `Nest application successfully started`, la API está disponible en:

```text
http://localhost:3000
```

**Importante:** deja esta terminal abierta mientras pruebas con Postman. El comando actual compila e inicia la aplicación, pero no vigila automáticamente los cambios de código. Si modificas archivos en `src`, detén el proceso con `Ctrl + C` y ejecuta `npm run start:dev` de nuevo.

## 6. Probar la API con Postman

### Importar la colección

1. Abre Postman.
2. En la pantalla principal, busca **Import**. Si no aparece, arrastra el archivo `postman/GameStore.postman_collection.json` directamente a la ventana de Postman.
3. Selecciona el archivo y confirma la importación.
4. En la barra lateral aparecerá la colección **GameStore API**.

La colección usa estas variables:

| Variable | Valor inicial | Para qué sirve |
| --- | --- | --- |
| `baseUrl` | `http://localhost:3000` | Dirección local de la API. |
| `juegoId` | Vacío | Identificador del último juego creado. |

No necesitas crear un environment: las variables están guardadas dentro de la colección.

### Orden recomendado de pruebas

#### A. Registrar videojuego — `POST /juegos`

1. Abre **Registrar videojuego**.
2. Confirma que el método sea `POST` y la URL sea `{{baseUrl}}/juegos`.
3. En **Body > raw > JSON**, usa un cuerpo como este:

   ```json
   {
     "titulo": "Celeste",
     "genero": "Plataformas",
     "precio": 12.5,
     "stock": 5
   }
   ```

4. Presiona **Send**.
5. Debes recibir `201 Created`. La respuesta incluye el campo `id`.

La colección guarda ese `id` automáticamente en `juegoId`. Si creas varias veces el mismo título, cambia el título porque no se permiten duplicados.

#### B. Consultar catálogo — `GET /juegos`

1. Abre **Consultar catálogo**.
2. Presiona **Send**.
3. Debes recibir `200 OK` y una lista de juegos.

#### C. Actualizar videojuego — `PATCH /juegos/:juegoId`

1. Abre **Actualizar videojuego**.
2. Verifica que la URL muestre un valor real, por ejemplo `http://localhost:3000/juegos/abc...`, y no `{{juegoId}}` vacío.
3. Envía únicamente los campos que deseas cambiar:

   ```json
   {
     "precio": 10,
     "stock": 8
   }
   ```

4. Presiona **Send**.
5. Debes recibir `200 OK` con el juego actualizado.

Puedes actualizar `titulo`, `genero`, `precio` o `stock`. Los campos no enviados conservan su valor anterior.

#### D. Comprar videojuego — `POST /juegos/:juegoId/compras`

1. Abre **Comprar videojuego**.
2. Confirma que la URL contiene el mismo `juegoId`.
3. Presiona **Send**. No necesita cuerpo.
4. Debes recibir `201 Created` con los datos de la venta.
5. Ejecuta nuevamente **Consultar catálogo**: el `stock` del juego se reduce en uno.

## 7. Códigos de respuesta y problemas comunes

| Código | Significado | Qué revisar |
| --- | --- | --- |
| `200 OK` | Consulta o actualización correcta. | No debes hacer nada más. |
| `201 Created` | Juego o compra creados correctamente. | Guarda o reutiliza el `id` recibido. |
| `400 Bad Request` | Datos inválidos. | Revisa que título y género no estén vacíos, precio sea mayor que 0 y stock sea entero mayor o igual que 0. |
| `404 Not Found` | La ruta o el juego no existen. | Comprueba que la API esté reiniciada y que `juegoId` corresponda a un juego real. |
| `409 Conflict` | Título repetido o no hay stock. | Usa otro título o aumenta el stock antes de comprar. |

### Si aparece `Cannot PATCH /juegos/...`

El servidor se inició antes de que existiera la ruta `PATCH`. En la terminal donde está la API presiona `Ctrl + C` y vuelve a ejecutar:

```powershell
npm run start:dev
```

### Si Postman muestra `{{juegoId}}` vacío

Ejecuta primero **Registrar videojuego** y verifica que haya respondido `201`. La prueba de esa solicitud guarda automáticamente el identificador. También puedes copiar el campo `id` de la respuesta y pegarlo en la variable `juegoId` de la colección.

### Si la API no inicia

1. Revisa que `.env` exista y tenga las dos URLs de Supabase.
2. Ejecuta `npm run prisma:generate`.
3. Comprueba que nadie más use el puerto `3000`.
4. Lee el mensaje de error completo en la terminal; no cierres la ventana antes de revisarlo.

## 8. Ejecutar las pruebas automáticas

Detén la API con `Ctrl + C` o abre otra terminal en el proyecto. Luego ejecuta:

```powershell
npm test
```

El resultado esperado es que Vitest informe las pruebas aprobadas. También puedes ejecutar grupos concretos:

```powershell
npm run test:unit
npm run test:integration
```

- Las pruebas unitarias verifican las reglas de negocio sin usar Supabase.
- Las pruebas de integración verifican las rutas HTTP de NestJS.

En Visual Studio Code puedes instalar la extensión **Vitest** y abrir el panel **Testing** para ejecutar las pruebas visualmente.

## 9. Secuencia rápida para cada día

Cuando vuelvas a trabajar en el proyecto, normalmente solo necesitas:

```powershell
npm run start:dev
```

Después abre Postman, ejecuta **Registrar videojuego**, **Consultar catálogo**, **Actualizar videojuego** y **Comprar videojuego**, en ese orden. Al terminar, detén la API con `Ctrl + C`.
