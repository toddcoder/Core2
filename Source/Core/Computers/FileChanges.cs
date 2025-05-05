using System;
using System.IO;
using Core.Applications.Messaging;
using Core.Numbers;

namespace Core.Computers;

public class FileChanges : IDisposable
{
   protected FileSystemWatcher watcher = new();

   public readonly MessageEvent<FileChange> Changed = new();

   public FileChanges(FolderName folderToWatch, bool includeSubdirectories)
   {
      watcher.Path = folderToWatch.FullPath;
      watcher.IncludeSubdirectories = includeSubdirectories;
      watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
      watcher.Changed += (_, e) => Changed.Invoke(new FileChange.Changed(e.FullPath));
      watcher.Created += (_, e) => Changed.Invoke(new FileChange.Created(e.FullPath));
      watcher.Deleted += (_, e) => Changed.Invoke(new FileChange.Deleted(e.FullPath));
      watcher.Renamed += (_, e) => Changed.Invoke(new FileChange.Renamed(e.OldFullPath, e.FullPath));
      watcher.Error += (_, e) => Changed.Invoke(new FileChange.Error(e.GetException()));
   }

   public FolderName FolderToWatch
   {
      get => watcher.Path;
      set => watcher.Path = value.FullPath;
   }

   public bool IncludeSubdirectories
   {
      get => watcher.IncludeSubdirectories;
      set => watcher.IncludeSubdirectories = value;
   }

   public NotifyFilters NotifyFilter
   {
      get => watcher.NotifyFilter;
      set => watcher.NotifyFilter = value;
   }

   public bool Enabled
   {
      get => watcher.EnableRaisingEvents;
      set => watcher.EnableRaisingEvents = value;
   }

   public int BufferSize
   {
      get => watcher.InternalBufferSize;
      set => watcher.InternalBufferSize = value;
   }

   protected Bits32<NotifyFilters> getNotifyFilter() => watcher.NotifyFilter;

   public bool FileName
   {
      get => getNotifyFilter()[NotifyFilters.FileName];
      set
      {
         var filter = getNotifyFilter();
         filter[NotifyFilters.FileName] = value;
         watcher.NotifyFilter = filter;
      }
   }

   public bool FolderName
   {
      get => getNotifyFilter()[NotifyFilters.DirectoryName];
      set
      {
         var filter = getNotifyFilter();
         filter[NotifyFilters.DirectoryName] = value;
         watcher.NotifyFilter = filter;
      }
   }

   public bool Attributes
   {
      get => getNotifyFilter()[NotifyFilters.Attributes];
      set
      {
         var filter = getNotifyFilter();
         filter[NotifyFilters.Attributes] = value;
         watcher.NotifyFilter = filter;
      }
   }

   public bool Size
   {
      get => getNotifyFilter()[NotifyFilters.Size];
      set
      {
         var filter = getNotifyFilter();
         filter[NotifyFilters.Size] = value;
         watcher.NotifyFilter = filter;
      }
   }

   public bool LastWrite
   {
      get => getNotifyFilter()[NotifyFilters.LastWrite];
      set
      {
         var filter = getNotifyFilter();
         filter[NotifyFilters.LastWrite] = value;
         watcher.NotifyFilter = filter;
      }
   }

   public bool LastAccess
   {
      get => getNotifyFilter()[NotifyFilters.LastAccess];
      set
      {
         var filter = getNotifyFilter();
         filter[NotifyFilters.LastAccess] = value;
         watcher.NotifyFilter = filter;
      }
   }

   public bool Security
   {
      get => getNotifyFilter()[NotifyFilters.Security];
      set
      {
         var filter = getNotifyFilter();
         filter[NotifyFilters.Security] = value;
         watcher.NotifyFilter = filter;
      }
   }

   public bool CreationTime
   {
      get => getNotifyFilter()[NotifyFilters.CreationTime];
      set
      {
         var filter = getNotifyFilter();
         filter[NotifyFilters.CreationTime] = value;
         watcher.NotifyFilter = filter;
      }
   }

   public void Dispose()
   {
      watcher.Dispose();
      GC.SuppressFinalize(this);
   }

   ~FileChanges() => Dispose();
}