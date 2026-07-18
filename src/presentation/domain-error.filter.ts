import { ArgumentsHost, Catch, ExceptionFilter, HttpStatus } from '@nestjs/common';
import { Response } from 'express';
import { DomainError } from '../domain/game';

const statusByCode = {
  INVALID_GAME: HttpStatus.BAD_REQUEST,
  GAME_NOT_FOUND: HttpStatus.NOT_FOUND,
  INSUFFICIENT_STOCK: HttpStatus.CONFLICT,
  GAME_ALREADY_EXISTS: HttpStatus.CONFLICT,
};

@Catch(DomainError)
export class DomainErrorFilter implements ExceptionFilter {
  catch(error: DomainError, host: ArgumentsHost) {
    host.switchToHttp().getResponse<Response>().status(statusByCode[error.code]).json({ message: error.message, code: error.code });
  }
}
