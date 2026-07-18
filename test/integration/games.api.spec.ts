import { INestApplication, ValidationPipe } from '@nestjs/common';
import { Test } from '@nestjs/testing';
import request from 'supertest';
import { afterEach, describe, expect, it } from 'vitest';
import { AppModule } from '../../src/app.module';
import { Juego } from '../../src/domain/entities/juego';
import { Venta } from '../../src/domain/entities/venta';
import { REPOSITORIO_JUEGOS, RepositorioJuegos } from '../../src/domain/ports/repositorio-juegos';

describe('Games API', () => {
  let app: INestApplication;

  afterEach(async () => app?.close());

  it('registra, lista y compra un videojuego', async () => {
    const juegos = new Map<string, Juego>();
    const repository: RepositorioJuegos = {
      crear: async (juego) => (juegos.set(juego.id, juego), juego),
      listar: async () => [...juegos.values()],
      buscarPorId: async (id) => juegos.get(id) ?? null,
      comprar: async (id) => {
        const juego = juegos.get(id);
        if (!juego || !juego.puedeComprarse()) return null;
        juegos.set(id, Juego.reconstruir({ ...juego, stock: juego.stock - 1 }));
        return new Venta('venta-1', id, juego.precio, new Date());
      },
    };
    const module = await Test.createTestingModule({ imports: [AppModule] }).overrideProvider(REPOSITORIO_JUEGOS).useValue(repository).compile();
    app = module.createNestApplication();
    app.useGlobalPipes(new ValidationPipe({ whitelist: true, transform: true }));
    await app.init();

    const created = await request(app.getHttpServer()).post('/juegos').send({ titulo: 'Celeste', genero: 'Plataformas', precio: 12.5, stock: 1 }).expect(201);
    await request(app.getHttpServer()).get('/juegos').expect(200).expect(({ body }) => expect(body).toHaveLength(1));
    await request(app.getHttpServer()).post(`/juegos/${created.body.id}/compras`).expect(201);
  });
});
