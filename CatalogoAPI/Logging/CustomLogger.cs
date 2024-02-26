namespace CatalogoAPI.Logging
{
    public class CustomLogger : ILogger
    {
        readonly string loggerName;
        readonly CustomLoggerProviderConfiguration loggerConfig;

        public CustomLogger(string loggerName, CustomLoggerProviderConfiguration loggerConfig)
        {
            this.loggerName = loggerName;
            this.loggerConfig = loggerConfig;
        }
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel.Equals(loggerConfig.LogLevel);
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, 
                                Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var mensagem = $"{logLevel.ToString()}: {eventId.Id} - {formatter(state, exception)}";
            EscreverTextoNoArquivo(mensagem);
        }

        private void EscreverTextoNoArquivo(string mensagem)
        {
            var caminhoDoArquivoDoLog = @"C:********************";

            using (StreamWriter writer = new StreamWriter(caminhoDoArquivoDoLog, true)) 
            {
                try
                {
                    writer.WriteLine(mensagem);
                    writer.Close();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
