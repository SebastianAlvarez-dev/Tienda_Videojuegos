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
      const created = await this.prisma.juego.create({
        data: { id: game.id, titulo: game.titulo, genero: game.genero, precio: game.precio, stock: game.stock },
      });
      return this.toGame(created);
    } catch (error: unknown) {
      if (typeof error === 'object' && error && 'code' in error && error.code === 'P2002') {
        throw new DomainError('Ya existe un videojuego con ese título.', 'JUEGO_YA_EXISTE');
      }
      throw error;
    }
  }

  async findAll(): Promise<Game[]> {
    const games = await this.prisma.juego.findMany({ orderBy: { fechaCreacion: 'desc' } });
    return games.map((game) => this.toGame(game));
  }

  async findById(id: string): Promise<Game | null> {
    const game = await this.prisma.juego.findUnique({ where: { id } });
    return game ? this.toGame(game) : null;
  }

  async purchase(gameId: string): Promise<Sale | null> {
    return this.prisma.$transaction(async (transaction) => {
      const updated = await transaction.juego.updateMany({
        where: { id: gameId, stock: { gt: 0 } },
        data: { stock: { decrement: 1 } },
      });
      if (!updated.count) return null;

      const game = await transaction.juego.findUniqueOrThrow({ where: { id: gameId } });
      const sale = await transaction.venta.create({ data: { juegoId: gameId, precio: game.precio } });
      return new Sale(sale.id, sale.juegoId, Number(sale.precio), sale.fechaCreacion);
    });
  }

  private toGame(game: { id: string; titulo: string; genero: string; precio: { toString(): string }; stock: number; fechaCreacion: Date }): Game {
    return Game.restore({ ...game, precio: Number(game.precio) });
  }
}
