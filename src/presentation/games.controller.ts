import { Body, Controller, Get, HttpCode, Param, Post } from '@nestjs/common';
import { Type } from 'class-transformer';
import { IsInt, IsNumber, IsString, Min, MinLength } from 'class-validator';
import { ComprarJuegoCasoUso } from '../application/use-cases/comprar-juego.caso-uso';
import { CrearJuegoCasoUso } from '../application/use-cases/crear-juego.caso-uso';
import { ListarJuegosCasoUso } from '../application/use-cases/listar-juegos.caso-uso';

class CreateGameDto {
  @IsString()
  @MinLength(1)
  titulo!: string;

  @IsString()
  @MinLength(1)
  genero!: string;

  @Type(() => Number)
  @IsNumber()
  @Min(0.01)
  precio!: number;

  @Type(() => Number)
  @IsInt()
  @Min(0)
  stock!: number;
}

@Controller('juegos')
export class GamesController {
  constructor(
    private readonly crearJuego: CrearJuegoCasoUso,
    private readonly listarJuegos: ListarJuegosCasoUso,
    private readonly comprarJuego: ComprarJuegoCasoUso,
  ) {}

  @Post()
  create(@Body() body: CreateGameDto) {
    return this.crearJuego.ejecutar(body);
  }

  @Get()
  findAll() {
    return this.listarJuegos.ejecutar();
  }

  @Post(':juegoId/compras')
  @HttpCode(201)
  purchase(@Param('juegoId') juegoId: string) {
    return this.comprarJuego.ejecutar(juegoId);
  }
}
