import { Inject, Injectable } from '@nestjs/common';
import { Venta } from '../../domain/entities/venta';
import { ErrorDominio } from '../../domain/errors/error-dominio';
import { REPOSITORIO_JUEGOS, RepositorioJuegos } from '../../domain/ports/repositorio-juegos';

@Injectable()
export class ComprarJuegoCasoUso {
  constructor(@Inject(REPOSITORIO_JUEGOS) private readonly juegos: RepositorioJuegos) {}

  async ejecutar(juegoId: string): Promise<Venta> {
    const juego = await this.juegos.buscarPorId(juegoId);
    if (!juego) throw new ErrorDominio('El videojuego no existe.', 'JUEGO_NO_ENCONTRADO');
    if (!juego.puedeComprarse()) throw new ErrorDominio('El videojuego no tiene stock.', 'STOCK_INSUFICIENTE');

    const venta = await this.juegos.comprar(juegoId);
    if (!venta) throw new ErrorDominio('El videojuego no tiene stock.', 'STOCK_INSUFICIENTE');
    return venta;
  }
}
