namespace GameStore.Application.Abstracciones;

public interface IComando<TResult>;

public interface IConsulta<TResult>;

public interface IManejadorComando<in TComando, TResult> where TComando : IComando<TResult>
{
    Task<TResult> ManejarAsync(TComando comando, CancellationToken cancellationToken);
}

public interface IManejadorConsulta<in TConsulta, TResult> where TConsulta : IConsulta<TResult>
{
    Task<TResult> ManejarAsync(TConsulta consulta, CancellationToken cancellationToken);
}
