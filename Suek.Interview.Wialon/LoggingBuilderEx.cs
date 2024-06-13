namespace Suek.Interview.Wialon;

public static class LoggingBuilderEx {
    public static void AddFileDefault(this ILoggingBuilder builder) {
        builder.AddFile<BasicFormatter>(ConfigureLogFile, ConfigureFormatter);

        return;

        static void ConfigureLogFile(FileLoggerOptions configuration) {
            configuration.Directory = @".\";
            configuration.UseRollingFiles = true;
            configuration.RollingFileTimestampFormat = "yyyy-MM-dd";
            configuration.FileExtension = "log";
            configuration.FileNamePrefix = "suek-wialon";
            configuration.MinimumLogLevel = LogLevel.Debug;
        }

        static void ConfigureFormatter(FileLoggerFormatterOptions formatter) {
            formatter.CaptureScopes = true;
            formatter.UseUtcTimestamp = true;
            formatter.IncludePID = true;
            formatter.IncludeUser = true;
        }
    }
}
