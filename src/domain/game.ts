export type CreateGameInput = {
  id: string;
  titulo: string;
  genero: string;
  precio: number;
  stock: number;
};

export class DomainError extends Error {
  constructor(
    message: string,
    readonly code: 'JUEGO_INVALIDO' | 'JUEGO_NO_ENCONTRADO' | 'STOCK_INSUFICIENTE' | 'JUEGO_YA_EXISTE',
  ) {
    super(message);
  }
}

export class Game {
  private constructor(
    readonly id: string,
    readonly titulo: string,
    readonly genero: string,
    readonly precio: number,
    readonly stock: number,
    readonly fechaCreacion: Date,
  ) {}

  static create(input: CreateGameInput): Game {
    if (!input.titulo.trim() || !input.genero.trim() || !Number.isFinite(input.precio) || input.precio <= 0 || !Number.isInteger(input.stock) || input.stock < 0) {
      throw new DomainError('Los datos del videojuego no son válidos.', 'JUEGO_INVALIDO');
    }

    return new Game(input.id, input.titulo.trim(), input.genero.trim(), input.precio, input.stock, new Date());
  }

  static restore(data: CreateGameInput & { fechaCreacion: Date }): Game {
    return new Game(data.id, data.titulo, data.genero, data.precio, data.stock, data.fechaCreacion);
  }

  canBePurchased(): boolean {
    return this.stock > 0;
  }
}

export class Sale {
  constructor(
    readonly id: string,
    readonly juegoId: string,
    readonly precio: number,
    readonly fechaCreacion: Date,
  ) {}
}

export interface GameRepository {
  create(game: Game): Promise<Game>;
  findAll(): Promise<Game[]>;
  findById(id: string): Promise<Game | null>;
  purchase(gameId: string): Promise<Sale | null>;
}

export const GAME_REPOSITORY = Symbol('GAME_REPOSITORY');
