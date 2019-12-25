using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace ElmaWebApi.App.Core
{
    /// <summary>
    /// Stores FileSystemWatcher instanses. Threadsafe.
    /// </summary>
    internal static class WatchersStorage
    {
        private static ConcurrentBag<FileSystemWatcher> bag = new ConcurrentBag<FileSystemWatcher>();

        public static void AddWatcher(string path, FileSystemEventHandler handler, params WatcherChangeTypes[] types)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Необходимо заполнить значение пути.");
            }

            if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out var uri))
            {
                throw new FormatException($"Неверный формат пути: {path}.");
            }

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Не найдена часть пути {path}.");
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var fileSystemWatcher = new FileSystemWatcher(path);
            fileSystemWatcher.EnableRaisingEvents = true;

            fileSystemWatcher.Subscribe(types, handler);
            bag.Add(fileSystemWatcher);
        }

        private static void Subscribe(this FileSystemWatcher watcher, WatcherChangeTypes[] types, FileSystemEventHandler handler)
        {
            if (types.Contains(WatcherChangeTypes.Renamed))
            {
                throw new NotImplementedException();
            }

            if (types.Contains(WatcherChangeTypes.All))
            {
                watcher.Changed += handler;
                watcher.Created += handler;
                watcher.Deleted += handler;
                return;
            }
            if (types.Contains(WatcherChangeTypes.Changed))
            {
                watcher.Changed += handler;
            }
            if (types.Contains(WatcherChangeTypes.Created))
            {
                watcher.Created += handler;
            }
            if (types.Contains(WatcherChangeTypes.Deleted))
            {
                watcher.Deleted += handler;
            }
        }
    }
}
