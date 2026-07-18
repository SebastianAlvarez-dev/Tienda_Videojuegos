import { Body, Controller, Get, HttpCode, Param, Post } from '@nestjs/common';
import { Type } from 'class-transformer';
import { IsInt, IsNumber, IsString, Min, MinLength } from 'class-validator';
import { CreateGameUseCase, ListGamesUseCase, PurchaseGameUseCase } from '../application/game.use-cases';

class CreateGameDto {
  @IsString()
  @MinLength(1)
  title!: string;

  @IsString()
  @MinLength(1)
  genre!: string;

  @Type(() => Number)
  @IsNumber()
  @Min(0.01)
  price!: number;

  @Type(() => Number)
  @IsInt()
  @Min(0)
  stock!: number;
}

@Controller('games')
export class GamesController {
  constructor(
    private readonly createGame: CreateGameUseCase,
    private readonly listGames: ListGamesUseCase,
    private readonly purchaseGame: PurchaseGameUseCase,
  ) {}

  @Post()
  create(@Body() body: CreateGameDto) {
    return this.createGame.execute(body);
  }

  @Get()
  findAll() {
    return this.listGames.execute();
  }

  @Post(':gameId/purchases')
  @HttpCode(201)
  purchase(@Param('gameId') gameId: string) {
    return this.purchaseGame.execute(gameId);
  }
}
