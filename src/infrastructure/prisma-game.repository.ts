import { Injectable } from '@nestjs/common';
import { PrismaClient } from '@prisma/client';
import { DomainError, Game, GameRepository, Sale } from '../domain/game';

@Injectable()
export class PrismaService extends PrismaClient {}

@Injectable()
export class PrismaGameRepository implements GameRepository {
  constructor(private readonly prisma: PrismaService) {}

  async create(game: Game): Promise<Game> {
    try {
      const created = await this.prisma.game.create({
        data: { id: game.id, title: game.title, genre: game.genre, price: game.price, stock: game.stock },
      });
      return this.toGame(created);
    } catch (error: unknown) {
      if (typeof error === 'object' && error && 'code' in error && error.code === 'P2002') {
        throw new DomainError('Ya existe un videojuego con ese título.', 'GAME_ALREADY_EXISTS');
      }
      throw error;
    }
  }

  async findAll(): Promise<Game[]> {
    const games = await this.prisma.game.findMany({ orderBy: { createdAt: 'desc' } });
    return games.map((game) => this.toGame(game));
  }

  async findById(id: string): Promise<Game | null> {
    const game = await this.prisma.game.findUnique({ where: { id } });
    return game ? this.toGame(game) : null;
  }

  async purchase(gameId: string): Promise<Sale | null> {
    return this.prisma.$transaction(async (transaction) => {
      const updated = await transaction.game.updateMany({
        where: { id: gameId, stock: { gt: 0 } },
        data: { stock: { decrement: 1 } },
      });
      if (!updated.count) return null;

      const game = await transaction.game.findUniqueOrThrow({ where: { id: gameId } });
      const sale = await transaction.sale.create({ data: { gameId, price: game.price } });
      return new Sale(sale.id, sale.gameId, Number(sale.price), sale.createdAt);
    });
  }

  private toGame(game: { id: string; title: string; genre: string; price: { toString(): string }; stock: number; createdAt: Date }): Game {
    return Game.restore({ ...game, price: Number(game.price) });
  }
}
