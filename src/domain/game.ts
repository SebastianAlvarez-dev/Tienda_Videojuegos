export type CreateGameInput = {
  id: string;
  title: string;
  genre: string;
  price: number;
  stock: number;
};

export class DomainError extends Error {
  constructor(
    message: string,
    readonly code: 'INVALID_GAME' | 'GAME_NOT_FOUND' | 'INSUFFICIENT_STOCK' | 'GAME_ALREADY_EXISTS',
  ) {
    super(message);
  }
}

export class Game {
  private constructor(
    readonly id: string,
    readonly title: string,
    readonly genre: string,
    readonly price: number,
    readonly stock: number,
    readonly createdAt: Date,
  ) {}

  static create(input: CreateGameInput): Game {
    if (!input.title.trim() || !input.genre.trim() || !Number.isFinite(input.price) || input.price <= 0 || !Number.isInteger(input.stock) || input.stock < 0) {
      throw new DomainError('Los datos del videojuego no son válidos.', 'INVALID_GAME');
    }

    return new Game(input.id, input.title.trim(), input.genre.trim(), input.price, input.stock, new Date());
  }

  static restore(data: CreateGameInput & { createdAt: Date }): Game {
    return new Game(data.id, data.title, data.genre, data.price, data.stock, data.createdAt);
  }

  canBePurchased(): boolean {
    return this.stock > 0;
  }
}

export class Sale {
  constructor(
    readonly id: string,
    readonly gameId: string,
    readonly price: number,
    readonly createdAt: Date,
  ) {}
}

export interface GameRepository {
  create(game: Game): Promise<Game>;
  findAll(): Promise<Game[]>;
  findById(id: string): Promise<Game | null>;
  purchase(gameId: string): Promise<Sale | null>;
}

export const GAME_REPOSITORY = Symbol('GAME_REPOSITORY');
