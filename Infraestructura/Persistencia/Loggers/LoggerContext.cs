using Aplicacion.Interfaces;
using Dominio.Entidades.Models;
using Dominio.Interfaces;
using Infraestructura.Persistencia.Loggers;

public class LoggerContext
{
    private static LoggerContext _instancia;
    private ILoggerStrategy _strategy;
    private static IRepositorio<Log> _repo;

    public static LoggerContext GetLogger(IRepositorio<Log> repo = null)
    {
        if (_instancia == null)
        {
            if (repo == null) throw new ArgumentNullException(nameof(repo));
            _repo = repo;
            _instancia = new LoggerContext();
        }
        return _instancia;
    }

    private LoggerContext() { }

    public void SetDatabaseLogger()
    {
        _strategy = new DatabaseLogger(_repo);
    }

    public void SetFileLogger()
    {
        _strategy = new FileLogger();
    }

    public async Task Registrar(Log log)
    {
        if (_strategy == null) throw new InvalidOperationException("Debe establecerse una estrategia.");
        await _strategy.Registrar(log);
    }
}
