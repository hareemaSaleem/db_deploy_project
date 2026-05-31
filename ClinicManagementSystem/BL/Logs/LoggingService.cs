namespace ClinicManagementSystem.BL.Logs
{
        public interface ILoggingService
        {
            void LogInfo(string message);
            void LogWarning(string message);
            void LogError(string message, Exception ex = null);
            void LogDebug(string message);
            void LogAudit(string action, string performedBy, string details);
        }
        public class LoggingService : ILoggingService
        {
            private readonly string _logFilePath;
            private readonly string _auditFilePath;
            private readonly string _errorFilePath;

            public LoggingService()
            {
                string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

                if (!Directory.Exists(logDirectory))
                    Directory.CreateDirectory(logDirectory);

                _logFilePath = Path.Combine(logDirectory, "clinic_log.txt");
                _auditFilePath = Path.Combine(logDirectory, "audit_log.txt");
                _errorFilePath = Path.Combine(logDirectory, "error_log.txt");
            }

            public void LogInfo(string message)
            {
                WriteLog(_logFilePath, "INFO", message);
            }

            public void LogWarning(string message)
            {
                WriteLog(_logFilePath, "WARNING", message);
            }

            public void LogError(string message, Exception ex = null)
            {
                string errorMessage = message;
                if (ex != null)
                    errorMessage += $" | Exception: {ex.Message} | StackTrace: {ex.StackTrace}";

                WriteLog(_errorFilePath, "ERROR", errorMessage);
                WriteLog(_logFilePath, "ERROR", errorMessage);
            }

            public void LogDebug(string message)
            {
                WriteLog(_logFilePath, "DEBUG", message);
            }

            public void LogAudit(string action, string performedBy, string details)
            {
                string auditMessage = $"Action: {action} | Performed By: {performedBy} | Details: {details}";
                WriteLog(_auditFilePath, "AUDIT", auditMessage);
                WriteLog(_logFilePath, "AUDIT", auditMessage);
            }

            private void WriteLog(string filePath, string level, string message)
            {
                try
                {
                    string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
                    File.AppendAllText(filePath, logEntry + Environment.NewLine);
                }
                catch { }
            }
        }
}