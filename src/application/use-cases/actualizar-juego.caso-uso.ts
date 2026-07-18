import { Inject, Injectable } from '@nestjs/common';
import { CambiosJuego, Juego } from '../../domain/entities/juego';
import { ErrorDominio } from '../../domain/errors/error-dominio';
import { REPOSITORIO_JUEGOS, RepositorioJuegos } from '../../domain/ports/repositorio-juegos';

@Injectable()
export class ActualizarJuegoCasoUso {
  constructor(@Inject(REPOSITORIO_JUEGOS) private readonly juegos: RepositorioJuegos) {}

  async ejecutar(juegoId: string, cambios: CambiosJuego): Promise<Juego> {
    const juego = await this.juegos.buscarPorId(juegoId);
    if (!juego) throw new ErrorDominio('El videojuego no existe.', 'JUEGO_NO_ENCONTRADO');

    return this.juegos.actualizar(juego.actualizar(cambios));
  }
}
