using System.Collections.Concurrent;
using System.Linq;
using ElmaWebApi.App.Models;

namespace ElmaWebApi.App.Core
{
    /// <summary>
    /// Stores directory change reports
    /// </summary>
    internal static class ChangesQueue
    {
        private static ConcurrentQueue<DirectoryChangeReport> reports = new ConcurrentQueue<DirectoryChangeReport>();
        // Amendment. If the change event of the same file occurs too often, ignore (file system feature)
        private const int Deflection = 50;

        public static int Count => reports.Count;

        /// <summary>
        /// Enqueue new change report
        /// </summary>
        public static void AddReport(DirectoryChangeReport report)
        {
            var existingReportTime = reports
                .Where(r => r.FullFilePath == report.FullFilePath)
                .OrderByDescending(r => r.ChangeTime)
                .FirstOrDefault();

            if (existingReportTime == null || existingReportTime.ChangeTime.Millisecond - report.ChangeTime.Millisecond > Deflection)
            {
                reports.Enqueue(report);
            }
        }

        /// <summary>
        /// Dequeue last report
        /// </summary>
        public static DirectoryChangeReport GetReport()
        {
            if (reports.TryDequeue(out var report))
            {
                return report;
            }

            return null;
        }
    }
}
