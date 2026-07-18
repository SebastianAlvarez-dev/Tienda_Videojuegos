export class ErrorDominio extends Error {
  constructor(
    message: string,
    readonly codigo: 'JUEGO_INVALIDO' | 'JUEGO_NO_ENCONTRADO' | 'STOCK_INSUFICIENTE' | 'JUEGO_YA_EXISTE',
  ) {
    super(message);
  }
}
