import { randomUUID } from 'node:crypto';
import { Inject, Injectable } from '@nestjs/common';
import { CreateGameInput, DomainError, Game, GameRepository, Sale, GAME_REPOSITORY } from '../domain/game';

@Injectable()
export class CreateGameUseCase {
  constructor(@Inject(GAME_REPOSITORY) private readonly games: GameRepository) {}

  execute(input: Omit<CreateGameInput, 'id'>): Promise<Game> {
    return this.games.create(Game.create({ ...input, id: randomUUID() }));
  }
}

@Injectable()
export class ListGamesUseCase {
  constructor(@Inject(GAME_REPOSITORY) private readonly games: GameRepository) {}

  execute(): Promise<Game[]> {
    return this.games.findAll();
  }
}

@Injectable()
export class PurchaseGameUseCase {
  constructor(@Inject(GAME_REPOSITORY) private readonly games: GameRepository) {}

  async execute(gameId: string): Promise<Sale> {
    const game = await this.games.findById(gameId);
    if (!game) throw new DomainError('El videojuego no existe.', 'GAME_NOT_FOUND');
    if (!game.canBePurchased()) throw new DomainError('El videojuego no tiene stock.', 'INSUFFICIENT_STOCK');

    const sale = await this.games.purchase(gameId);
    if (!sale) throw new DomainError('El videojuego no tiene stock.', 'INSUFFICIENT_STOCK');
    return sale;
  }
}
