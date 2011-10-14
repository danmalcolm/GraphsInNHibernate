using System;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace Network.NHibernate
{
    public class QuerySpy : IDisposable
    {
        private readonly int expectedNumber;
        private readonly NHibernateSqlLogSpy spy;

        public QuerySpy(int expectedNumber)
        {
            this.expectedNumber = expectedNumber;
            spy = new NHibernateSqlLogSpy();
        }

        public void Dispose()
        {
            //Assert.AreEqual(expectedNumber, spy.Appender.GetEvents().Count());
        }
    }

    public static class NumberOfQueriesUsedExtension
    {
        public static QuerySpy Queries(this int expectedNumber)
        {
            return new QuerySpy(expectedNumber);
        }
    }

    public class NHibernateSqlLogSpy : LogSpy
    {
        public NHibernateSqlLogSpy() : base("NHibernate.SQL") { }
    }

    public class LogSpy : IDisposable
    {
        static LogSpy()
        {
            
        }

        private readonly MemoryAppender appender;
        private readonly Logger logger;
        private readonly Level prevLogLevel;

        public LogSpy(string loggerName)
        {
            logger = (Logger)LogManager.GetLogger(loggerName).Logger;
            if (logger == null)
                throw new NullReferenceException();
            prevLogLevel = logger.Level;
            logger.Level = Level.Debug;
            appender = new MemoryAppender();
            logger.AddAppender(appender);
        }

        public MemoryAppender Appender { get { return appender; } }

        public virtual void Dispose()
        {
            logger.Level = prevLogLevel;
            logger.RemoveAppender(appender);
        }
    }

}