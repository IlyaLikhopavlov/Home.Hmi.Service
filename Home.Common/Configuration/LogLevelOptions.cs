using Microsoft.Extensions.Configuration;
using Serilog.Events;

namespace Home.Common.Configuration
{
    public class LogLevelOptions
    {
        public const string LogLevel = "LogLevel";

        public const string DefaultName = nameof(Default);

        public const string MicrosoftName = nameof(Microsoft);

        public const string MicrosoftAspNetCoreName = @"Microsoft.AspNetCore";

        public string Default { get; set; }

        public string Microsoft { get; set; }

        [ConfigurationKeyName(MicrosoftAspNetCoreName)]
        public string MicrosoftAspNetCore { get; set; }

        private static LogEventLevel Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return LogEventLevel.Debug;
            }

            return
                Enum.TryParse(value, out LogEventLevel logEventLevel)
                    ? logEventLevel
                    : LogEventLevel.Debug;
        }

        public LogEventLevel GetDefault() => Parse(Default);

        public LogEventLevel GetMicrosoft() => Parse(Microsoft);

        public LogEventLevel GetMicrosoftAspNetCore() => Parse(MicrosoftAspNetCore);

    }
}
