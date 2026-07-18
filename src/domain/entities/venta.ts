export class Venta {
  constructor(
    readonly id: string,
    readonly juegoId: string,
    readonly precio: number,
    readonly fechaCreacion: Date,
  ) {}
}
