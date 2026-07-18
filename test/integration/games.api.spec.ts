import { INestApplication, ValidationPipe } from '@nestjs/common';
import { Test } from '@nestjs/testing';
import request from 'supertest';
import { afterEach, describe, expect, it } from 'vitest';
import { AppModule } from '../../src/app.module';
import { Game, GameRepository, Sale, GAME_REPOSITORY } from '../../src/domain/game';

describe('Games API', () => {
  let app: INestApplication;

  afterEach(async () => app?.close());

  it('registra, lista y compra un videojuego', async () => {
    const games = new Map<string, Game>();
    const repository: GameRepository = {
      create: async (game) => (games.set(game.id, game), game),
      findAll: async () => [...games.values()],
      findById: async (id) => games.get(id) ?? null,
      purchase: async (id) => {
        const game = games.get(id);
        if (!game || !game.canBePurchased()) return null;
        games.set(id, Game.restore({ ...game, stock: game.stock - 1 }));
        return new Sale('venta-1', id, game.precio, new Date());
      },
    };
    const module = await Test.createTestingModule({ imports: [AppModule] }).overrideProvider(GAME_REPOSITORY).useValue(repository).compile();
    app = module.createNestApplication();
    app.useGlobalPipes(new ValidationPipe({ whitelist: true, transform: true }));
    await app.init();

    const created = await request(app.getHttpServer()).post('/juegos').send({ titulo: 'Celeste', genero: 'Plataformas', precio: 12.5, stock: 1 }).expect(201);
    await request(app.getHttpServer()).get('/juegos').expect(200).expect(({ body }) => expect(body).toHaveLength(1));
    await request(app.getHttpServer()).post(`/juegos/${created.body.id}/compras`).expect(201);
  });
});
