import { Module } from '@nestjs/common';
import { GAME_REPOSITORY } from '../domain/game';
import { PrismaGameRepository, PrismaService } from './prisma-game.repository';

@Module({
  providers: [PrismaService, { provide: GAME_REPOSITORY, useClass: PrismaGameRepository }],
  exports: [GAME_REPOSITORY],
})
export class InfrastructureModule {}
