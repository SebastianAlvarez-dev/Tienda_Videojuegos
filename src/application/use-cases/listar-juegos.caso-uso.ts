import { Inject, Injectable } from '@nestjs/common';
import { Juego } from '../../domain/entities/juego';
import { REPOSITORIO_JUEGOS, RepositorioJuegos } from '../../domain/ports/repositorio-juegos';

@Injectable()
export class ListarJuegosCasoUso {
  constructor(@Inject(REPOSITORIO_JUEGOS) private readonly juegos: RepositorioJuegos) {}

  ejecutar(): Promise<Juego[]> {
    return this.juegos.listar();
  }
}
