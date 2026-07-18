import { Juego } from '../entities/juego';
import { Venta } from '../entities/venta';

export interface RepositorioJuegos {
  crear(juego: Juego): Promise<Juego>;
  listar(): Promise<Juego[]>;
  buscarPorId(id: string): Promise<Juego | null>;
  comprar(juegoId: string): Promise<Venta | null>;
}

export const REPOSITORIO_JUEGOS = Symbol('REPOSITORIO_JUEGOS');
