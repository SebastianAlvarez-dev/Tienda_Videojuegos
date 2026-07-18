import { ArgumentsHost, Catch, ExceptionFilter, HttpStatus } from '@nestjs/common';
import { Response } from 'express';
import { ErrorDominio } from '../domain/errors/error-dominio';

const statusByCode = {
  JUEGO_INVALIDO: HttpStatus.BAD_REQUEST,
  JUEGO_NO_ENCONTRADO: HttpStatus.NOT_FOUND,
  STOCK_INSUFICIENTE: HttpStatus.CONFLICT,
  JUEGO_YA_EXISTE: HttpStatus.CONFLICT,
};

@Catch(ErrorDominio)
export class DomainErrorFilter implements ExceptionFilter {
  catch(error: ErrorDominio, host: ArgumentsHost) {
    host.switchToHttp().getResponse<Response>().status(statusByCode[error.codigo]).json({ message: error.message, code: error.codigo });
  }
}
