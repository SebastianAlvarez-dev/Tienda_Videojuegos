import { describe, expect, it, vi } from 'vitest';
import { CreateGameUseCase, PurchaseGameUseCase } from '../../src/application/game.use-cases';
import { DomainError, Game, GameRepository, Sale } from '../../src/domain/game';

function repository(game: Game | null = null): GameRepository {
  return {
    create: vi.fn(async (newGame) => newGame),
    findAll: vi.fn(async () => (game ? [game] : [])),
    findById: vi.fn(async () => game),
    purchase: vi.fn(async (gameId) => new Sale('sale-1', gameId, 10, new Date())),
  };
}

describe('casos de uso de juegos', () => {
  it('crea un videojuego válido', async () => {
    const games = repository();
    const game = await new CreateGameUseCase(games).execute({ titulo: 'Hades', genero: 'Roguelike', precio: 15, stock: 3 });

    expect(game.titulo).toBe('Hades');
    expect(games.create).toHaveBeenCalledOnce();
  });

  it('rechaza una compra sin stock', async () => {
    const games = repository(Game.restore({ id: 'juego-1', titulo: 'Hades', genero: 'Roguelike', precio: 15, stock: 0, fechaCreacion: new Date() }));

    await expect(new PurchaseGameUseCase(games).execute('juego-1')).rejects.toMatchObject<Partial<DomainError>>({ code: 'STOCK_INSUFICIENTE' });
    expect(games.purchase).not.toHaveBeenCalled();
  });
});
