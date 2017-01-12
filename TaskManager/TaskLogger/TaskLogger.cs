using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;

using System.Diagnostics;
using DbModels.DomainModels.DbTasks;
using DbModels.DataContext;

namespace TaskManager
{
    public class TaskLogger
    {
        #region LOG
        public Logger logger = LogManager.GetCurrentClassLogger();
        public Context context { get; set; }
        public TaskLog TaskLog { get; set; }
        public string TaskName { get; set; }
        public TaskLogger(TaskLog taskLog, Context context)
        {
            this.context = context;
            this.TaskLog = taskLog;
        }

        private string GetFormattedMessage(string message)
        {
            return string.Format("{0}: {1}", TaskName, message);
        }

        public void LogError(string message)
        {

            message = GetFormattedMessage(message);
            LogEventInfo error = new LogEventInfo(LogLevel.Error, "logger", message);
            try
            {
                Log log = new Log() {  Message= message, Status = "Error", TaskLog = TaskLog };
                context.Logs.Add(log);
                context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                logger.Error("Ошибка при логировании"+ex.Message);
            }
            logger.Log(error);
            Debug.WriteLine(error);

        }
        public void LogDebug(string message)
        {
            message = GetFormattedMessage(message);
            LogEventInfo debug = new LogEventInfo(LogLevel.Debug, "logger", message);
            try
            {

                Log log = new Log() { Message= message, Status = "Debug", TaskLog = TaskLog };
            context.Logs.Add(log);
            context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                logger.Error("Ошибка при логировании"+ex.Message);
            }
            logger.Log(debug);

            Debug.WriteLine(message);
        }
        public void LogWarn(string message)
        {

            message = GetFormattedMessage(message);
            LogEventInfo warn = new LogEventInfo(LogLevel.Warn, "logger", message);
            try
            {

                Log log = new Log() { Message = message, Status = "Warn", TaskLog = TaskLog };
                context.Logs.Add(log);
                context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                 logger.Error("Ошибка при логировании"+ex.Message);
             
            }
            logger.Log(warn);
            Debug.WriteLine(warn);

        }
        public void LogInfo(string message)
        {
            message = GetFormattedMessage(message);
            LogEventInfo info = new LogEventInfo(LogLevel.Info, "logger", message);
            try
            {

                Log log = new Log() { Message = message, Status = "Info", TaskLog = TaskLog };
                context.Logs.Add(log);
                context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                logger.Error("Ошибка при логировании" + ex.Message);
            }
            logger.Log(info);
            Debug.WriteLine(info);

        }
        public void LogError(string message, byte[] file)
        {
            message = GetFormattedMessage(message);
            LogEventInfo error = new LogEventInfo(LogLevel.Error, "logger", message);
            try
            {

                Log log = new Log() { Message = message, Status = "Error", TaskLog = TaskLog};//, File = file };
                context.Logs.Add(log);
                context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                logger.Error("Ошибка при логировании" + ex.Message);
            }
            logger.Log(error);
            Debug.WriteLine(error);

        }
        public void LogDebug(string message, byte[] file)
        {
            message = GetFormattedMessage(message);
            LogEventInfo debug = new LogEventInfo(LogLevel.Debug, "logger", message);
            try
            {

                Log log = new Log() { Message = message, Status = "Debug", TaskLog = TaskLog};//, File = file };
                context.Logs.Add(log);
                context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                logger.Error("Ошибка при логировании" + ex.Message);
            }
            logger.Log(debug);

            Debug.WriteLine(message);
        }
        public void LogWarn(string message, byte[] file)
        {
            message = GetFormattedMessage(message);
            LogEventInfo warn = new LogEventInfo(LogLevel.Warn, "logger", message);
            try
            {

                Log log = new Log() { Message = message, Status = "Warn", TaskLog = TaskLog};//, File = file };
                context.Logs.Add(log);
                context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                logger.Error("Ошибка при логировании" + ex.Message);
            }
            logger.Log(warn);
            Debug.WriteLine(warn);

        }
        #endregion

    }
}