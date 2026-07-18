import { Module } from '@nestjs/common';
import { CreateGameUseCase, ListGamesUseCase, PurchaseGameUseCase } from './application/game.use-cases';
import { InfrastructureModule } from './infrastructure/infrastructure.module';
import { GamesController } from './presentation/games.controller';

@Module({
  imports: [InfrastructureModule],
  controllers: [GamesController],
  providers: [CreateGameUseCase, ListGamesUseCase, PurchaseGameUseCase],
})
export class AppModule {}
