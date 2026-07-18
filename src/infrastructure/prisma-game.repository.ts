import { Injectable } from '@nestjs/common';
import { PrismaClient } from '@prisma/client';
import { Juego } from '../domain/entities/juego';
import { Venta } from '../domain/entities/venta';
import { ErrorDominio } from '../domain/errors/error-dominio';
import { RepositorioJuegos } from '../domain/ports/repositorio-juegos';

@Injectable()
export class PrismaService extends PrismaClient {}

@Injectable()
export class PrismaGameRepository implements RepositorioJuegos {
  constructor(private readonly prisma: PrismaService) {}

  async crear(juego: Juego): Promise<Juego> {
    try {
      const created = await this.prisma.juego.create({
        data: { id: juego.id, titulo: juego.titulo, genero: juego.genero, precio: juego.precio, stock: juego.stock },
      });
      return this.toJuego(created);
    } catch (error: unknown) {
      if (typeof error === 'object' && error && 'code' in error && error.code === 'P2002') {
        throw new ErrorDominio('Ya existe un videojuego con ese título.', 'JUEGO_YA_EXISTE');
      }
      throw error;
    }
  }

  async listar(): Promise<Juego[]> {
    const juegos = await this.prisma.juego.findMany({ orderBy: { fechaCreacion: 'desc' } });
    return juegos.map((juego) => this.toJuego(juego));
  }

  async buscarPorId(id: string): Promise<Juego | null> {
    const juego = await this.prisma.juego.findUnique({ where: { id } });
    return juego ? this.toJuego(juego) : null;
  }

  async comprar(juegoId: string): Promise<Venta | null> {
    return this.prisma.$transaction(async (transaction) => {
      const updated = await transaction.juego.updateMany({
        where: { id: juegoId, stock: { gt: 0 } },
        data: { stock: { decrement: 1 } },
      });
      if (!updated.count) return null;

      const juego = await transaction.juego.findUniqueOrThrow({ where: { id: juegoId } });
      const venta = await transaction.venta.create({ data: { juegoId, precio: juego.precio } });
      return new Venta(venta.id, venta.juegoId, Number(venta.precio), venta.fechaCreacion);
    });
  }

  private toJuego(juego: { id: string; titulo: string; genero: string; precio: { toString(): string }; stock: number; fechaCreacion: Date }): Juego {
    return Juego.reconstruir({ ...juego, precio: Number(juego.precio) });
  }
}
