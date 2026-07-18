import { Module } from '@nestjs/common';
import { ActualizarJuegoCasoUso } from './application/use-cases/actualizar-juego.caso-uso';
import { ComprarJuegoCasoUso } from './application/use-cases/comprar-juego.caso-uso';
import { CrearJuegoCasoUso } from './application/use-cases/crear-juego.caso-uso';
import { ListarJuegosCasoUso } from './application/use-cases/listar-juegos.caso-uso';
import { InfrastructureModule } from './infrastructure/infrastructure.module';
import { GamesController } from './presentation/games.controller';

@Module({
  imports: [InfrastructureModule],
  controllers: [GamesController],
  providers: [CrearJuegoCasoUso, ListarJuegosCasoUso, ActualizarJuegoCasoUso, ComprarJuegoCasoUso],
})
export class AppModule {}
