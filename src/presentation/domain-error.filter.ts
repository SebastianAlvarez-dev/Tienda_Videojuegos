import { ArgumentsHost, Catch, ExceptionFilter, HttpStatus } from '@nestjs/common';
import { Response } from 'express';
import { DomainError } from '../domain/game';

const statusByCode = {
  JUEGO_INVALIDO: HttpStatus.BAD_REQUEST,
  JUEGO_NO_ENCONTRADO: HttpStatus.NOT_FOUND,
  STOCK_INSUFICIENTE: HttpStatus.CONFLICT,
  JUEGO_YA_EXISTE: HttpStatus.CONFLICT,
};

@Catch(DomainError)
export class DomainErrorFilter implements ExceptionFilter {
  catch(error: DomainError, host: ArgumentsHost) {
    host.switchToHttp().getResponse<Response>().status(statusByCode[error.code]).json({ message: error.message, code: error.code });
  }
}
