import { describe, expect, it, vi } from 'vitest';
import { ActualizarJuegoCasoUso } from '../../src/application/use-cases/actualizar-juego.caso-uso';
import { ComprarJuegoCasoUso } from '../../src/application/use-cases/comprar-juego.caso-uso';
import { CrearJuegoCasoUso } from '../../src/application/use-cases/crear-juego.caso-uso';
import { Juego } from '../../src/domain/entities/juego';
import { Venta } from '../../src/domain/entities/venta';
import { ErrorDominio } from '../../src/domain/errors/error-dominio';
import { RepositorioJuegos } from '../../src/domain/ports/repositorio-juegos';

function repository(juego: Juego | null = null): RepositorioJuegos {
  return {
    crear: vi.fn(async (nuevoJuego) => nuevoJuego),
    listar: vi.fn(async () => (juego ? [juego] : [])),
    buscarPorId: vi.fn(async () => juego),
    actualizar: vi.fn(async (juegoActualizado) => juegoActualizado),
    comprar: vi.fn(async (juegoId) => new Venta('venta-1', juegoId, 10, new Date())),
  };
}

describe('casos de uso de juegos', () => {
  it('crea un videojuego válido', async () => {
    const games = repository();
    const game = await new CrearJuegoCasoUso(games).ejecutar({ titulo: 'Hades', genero: 'Roguelike', precio: 15, stock: 3 });

    expect(game.titulo).toBe('Hades');
    expect(games.crear).toHaveBeenCalledOnce();
  });

  it('rechaza una compra sin stock', async () => {
    const games = repository(Juego.reconstruir({ id: 'juego-1', titulo: 'Hades', genero: 'Roguelike', precio: 15, stock: 0, fechaCreacion: new Date() }));

    await expect(new ComprarJuegoCasoUso(games).ejecutar('juego-1')).rejects.toMatchObject<Partial<ErrorDominio>>({ codigo: 'STOCK_INSUFICIENTE' });
    expect(games.comprar).not.toHaveBeenCalled();
  });

  it('actualiza sólo los campos recibidos', async () => {
    const games = repository(Juego.reconstruir({ id: 'juego-1', titulo: 'Hades', genero: 'Roguelike', precio: 15, stock: 3, fechaCreacion: new Date() }));

    const juego = await new ActualizarJuegoCasoUso(games).ejecutar('juego-1', { precio: 12, stock: 5 });

    expect(juego.precio).toBe(12);
    expect(juego.stock).toBe(5);
    expect(juego.titulo).toBe('Hades');
  });
});
