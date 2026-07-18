import { Module } from '@nestjs/common';
import { REPOSITORIO_JUEGOS } from '../domain/ports/repositorio-juegos';
import { PrismaGameRepository, PrismaService } from './prisma-game.repository';

@Module({
  providers: [PrismaService, { provide: REPOSITORIO_JUEGOS, useClass: PrismaGameRepository }],
  exports: [REPOSITORIO_JUEGOS],
})
export class InfrastructureModule {}
