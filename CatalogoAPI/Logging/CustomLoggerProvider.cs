using System.Collections.Concurrent;

namespace CatalogoAPI.Logging
{
    public class CustomLoggerProvider : ILoggerProvider
    {
        readonly CustomLoggerProviderConfiguration loggerConfig;
        readonly ConcurrentDictionary<string, CustomLogger> loggers = 
                                      new ConcurrentDictionary<string, CustomLogger>();
        public CustomLoggerProvider(CustomLoggerProviderConfiguration loggerConfig)
        {
            this.loggerConfig = loggerConfig;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, name => new CustomLogger(name, loggerConfig));
        }

        public void Dispose()
        {
            loggers.Clear();
        }
    }
}
