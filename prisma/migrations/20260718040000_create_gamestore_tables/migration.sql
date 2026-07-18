CREATE TABLE "Juego" (
  "id" TEXT NOT NULL,
  "titulo" TEXT NOT NULL,
  "genero" TEXT NOT NULL,
  "precio" DECIMAL(10,2) NOT NULL,
  "stock" INTEGER NOT NULL,
  "fechaCreacion" TIMESTAMPTZ(3) NOT NULL DEFAULT CURRENT_TIMESTAMP,

  CONSTRAINT "Juego_pkey" PRIMARY KEY ("id"),
  CONSTRAINT "Juego_precio_positivo" CHECK ("precio" > 0),
  CONSTRAINT "Juego_stock_no_negativo" CHECK ("stock" >= 0)
);

CREATE TABLE "Venta" (
  "id" TEXT NOT NULL,
  "juegoId" TEXT NOT NULL,
  "precio" DECIMAL(10,2) NOT NULL,
  "fechaCreacion" TIMESTAMPTZ(3) NOT NULL DEFAULT CURRENT_TIMESTAMP,

  CONSTRAINT "Venta_pkey" PRIMARY KEY ("id")
);

CREATE UNIQUE INDEX "Juego_titulo_key" ON "Juego"("titulo");
CREATE INDEX "Venta_juegoId_idx" ON "Venta"("juegoId");

ALTER TABLE "Venta" ADD CONSTRAINT "Venta_juegoId_fkey" FOREIGN KEY ("juegoId") REFERENCES "Juego"("id") ON DELETE RESTRICT ON UPDATE CASCADE;
ALTER TABLE "Juego" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Venta" ENABLE ROW LEVEL SECURITY;
