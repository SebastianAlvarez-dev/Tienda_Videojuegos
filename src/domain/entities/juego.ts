import { ErrorDominio } from '../errors/error-dominio';

export type DatosJuego = {
  id: string;
  titulo: string;
  genero: string;
  precio: number;
  stock: number;
};

export class Juego {
  private constructor(
    readonly id: string,
    readonly titulo: string,
    readonly genero: string,
    readonly precio: number,
    readonly stock: number,
    readonly fechaCreacion: Date,
  ) {}

  static crear(datos: DatosJuego): Juego {
    if (!datos.titulo.trim() || !datos.genero.trim() || !Number.isFinite(datos.precio) || datos.precio <= 0 || !Number.isInteger(datos.stock) || datos.stock < 0) {
      throw new ErrorDominio('Los datos del videojuego no son válidos.', 'JUEGO_INVALIDO');
    }

    return new Juego(datos.id, datos.titulo.trim(), datos.genero.trim(), datos.precio, datos.stock, new Date());
  }

  static reconstruir(datos: DatosJuego & { fechaCreacion: Date }): Juego {
    return new Juego(datos.id, datos.titulo, datos.genero, datos.precio, datos.stock, datos.fechaCreacion);
  }

  puedeComprarse(): boolean {
    return this.stock > 0;
  }
}
