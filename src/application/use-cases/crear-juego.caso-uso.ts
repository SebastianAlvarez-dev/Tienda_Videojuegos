import { randomUUID } from 'node:crypto';
import { Inject, Injectable } from '@nestjs/common';
import { DatosJuego, Juego } from '../../domain/entities/juego';
import { REPOSITORIO_JUEGOS, RepositorioJuegos } from '../../domain/ports/repositorio-juegos';

@Injectable()
export class CrearJuegoCasoUso {
  constructor(@Inject(REPOSITORIO_JUEGOS) private readonly juegos: RepositorioJuegos) {}

  ejecutar(datos: Omit<DatosJuego, 'id'>): Promise<Juego> {
    return this.juegos.crear(Juego.crear({ ...datos, id: randomUUID() }));
  }
}
