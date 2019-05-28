using Never.IO;
using Never.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Never.Configuration.ConfigCenter
{
    /// <summary>
    /// 配置文件监听
    /// </summary>
    public sealed class ConfigurationWatcher : ThreadCircler
    {
        #region netsted

        private struct ChangeQueue : IEquatable<ChangeQueue>
        {
            /// <summary>
            /// 文件全路径
            /// </summary>
            public string FullName { get; set; }

            /// <summary>
            /// 重命名前的文件全路径
            /// </summary>
            public string OldFullName { get; set; }

            /// <summary>
            /// create=0,change=1,rename=2,delete= 3
            /// </summary>
            public int Action { get; set; }

            /// <summary>
            /// 共享文件的修改 = 1，应用文件 = 2
            /// </summary>
            public bool Shared { get; set; }

            /// <summary>
            /// 是否为路径事件
            /// </summary>
            public bool PathChanged { get; set; }

            public bool Equals(ChangeQueue other)
            {
                if (this.FullName == null)
                {
                    return this.OldFullName.IsEquals(other.OldFullName, StringComparison.OrdinalIgnoreCase);
                }

                return this.FullName.IsEquals(other.FullName, StringComparison.OrdinalIgnoreCase);
            }

            public string ToAction()
            {
                switch (this.Action)
                {
                    case 0:
                        return "create";
                    case 1:
                        return "changed";
                    case 2:
                        return "renamed";
                    case 3:
                        return "deleted";
                }

                return this.Action.ToString();
            }
        }

        #endregion

        #region field and ctor

        private readonly List<ShareConfigurationBuilder> shareConfiguration = null;
        private readonly List<IShareFileReference> appConfiguration = null;
        private readonly ICustomKeyValueFinder keyValueFinder = null;
        private readonly Dictionary<string, FileSystemWatcher> fileWather = null;
        private readonly Dictionary<string, DateTime> moreTimeLimit = null;
        private readonly TimeSpan sleepTimeSpan = TimeSpan.Zero;
        private readonly System.Collections.Concurrent.ConcurrentQueue<ChangeQueue> changeQueue = null;

        /// <summary>
        /// 共享文件重命名时
        /// </summary>
        public event EventHandler<ConfigurationWatcherEventArgs> OnShareFileRenamed;

        /// <summary>
        /// 共享文件删除时
        /// </summary>
        public event EventHandler<ConfigurationWatcherEventArgs> OnShareFileDeleted;

        /// <summary>
        /// 共享文件修改时
        /// </summary>
        public event EventHandler<ConfigurationWatcherEventArgs> OnShareFileChanged;

        /// <summary>
        /// 应用文件重命名时
        /// </summary>
        public event EventHandler<ConfigurationWatcherEventArgs> OnAppFileRenamed;

        /// <summary>
        /// 应用文件删除时
        /// </summary>
        public event EventHandler<ConfigurationWatcherEventArgs> OnAppFileDeleted;

        /// <summary>
        /// 应用文件修改时
        /// </summary>
        public event EventHandler<ConfigurationWatcherEventArgs> OnAppFileChanged;

        /// <summary>
        /// 日志构建者
        /// </summary>
        public Func<Logging.ILoggerBuilder> LoggerBuilder { get; set; }

        /// <summary>
        /// 构建者
        /// </summary>
        internal EventHandler<ShareFileEventArgs> ShareFileEventHandler { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ConfigurationWatcher(IEnumerable<ShareConfigurationBuilder> shareConfiguration, IEnumerable<IShareFileReference> appConfiguration, ICustomKeyValueFinder keyValueFinder)
            : this(shareConfiguration, appConfiguration, keyValueFinder, TimeSpan.FromSeconds(60))
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public ConfigurationWatcher(IEnumerable<ShareConfigurationBuilder> shareConfiguration, IEnumerable<IShareFileReference> appConfiguration, ICustomKeyValueFinder keyValueFinder, TimeSpan sleepTimeSpan) : base(null, typeof(ConfigurationWatcher).Name)
        {
            this.shareConfiguration = shareConfiguration == null ? new List<ShareConfigurationBuilder>() : shareConfiguration.ToList();
            this.appConfiguration = appConfiguration == null ? new List<IShareFileReference>() : appConfiguration.ToList();
            this.keyValueFinder = keyValueFinder;
            this.sleepTimeSpan = sleepTimeSpan;
            this.moreTimeLimit = new Dictionary<string, DateTime>();
            this.changeQueue = new System.Collections.Concurrent.ConcurrentQueue<ChangeQueue>();
            this.fileWather = new Dictionary<string, FileSystemWatcher>(StringComparer.OrdinalIgnoreCase);

            if (shareConfiguration != null)
            {
                foreach (var share in shareConfiguration)
                {
                    var watcher = new Never.IO.FileWatcher(share.File) { EnableRaisingEvents = true };
                    this.fileWather.Add(share.File.FullName, watcher);
                    this.moreTimeLimit.Add(share.File.FullName, DateTime.Now);
                    watcher.Created += ShareWatcher_Created;
                    watcher.Changed += ShareWatcher_Changed;
                    watcher.Deleted += ShareWatcher_Deleted;
                    watcher.Renamed += ShareWatcher_Renamed;
                }
            }

            if (appConfiguration != null)
            {
                foreach (var config in appConfiguration)
                {
                    var watcher = new Never.IO.FileWatcher(config.Builder.File.File) { EnableRaisingEvents = true };
                    this.fileWather.Add(config.Builder.File.File.FullName, watcher);
                    this.moreTimeLimit.Add(config.Builder.File.File.FullName, DateTime.Now);
                    watcher.Created += AppWatcher_Created;
                    watcher.Changed += AppWatcher_Changed;
                    watcher.Deleted += AppWatcher_Deleted;
                    watcher.Renamed += AppWatcher_Renamed;
                }
            }

            this.Replace(Change).Start();
        }

        #endregion field and ctor

        #region change
        private void ShareWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            this.changeQueue.Enqueue(new ChangeQueue()
            {
                PathChanged = false,
                Shared = true,
                Action = 3,
                //FullName = e.FullPath,
                OldFullName = e.FullPath,
            });
        }

        private void ShareWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            this.changeQueue.Enqueue(new ChangeQueue()
            {
                PathChanged = false,
                Shared = true,
                Action = 2,
                FullName = e.FullPath,
                OldFullName = e.OldFullPath,
            });
        }

        private void ShareWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            this.changeQueue.Enqueue(new ChangeQueue()
            {
                PathChanged = false,
                Shared = true,
                Action = 1,
                FullName = e.FullPath,
            });
        }

        private void ShareWatcher_Created(object sender, FileSystemEventArgs e)
        {
            this.changeQueue.Enqueue(new ChangeQueue()
            {
                PathChanged = false,
                Shared = true,
                Action = 0,
                FullName = e.FullPath,
            });
        }

        private void SharePathWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            this.changeQueue.Enqueue(new ChangeQueue()
            {
                PathChanged = true,
                Shared = true,
                Action = 3,
                //FullName = e.FullPath,
                OldFullName = e.FullPath,
            });
        }

        private void SharePathWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            this.changeQueue.Enqueue(new ChangeQueue()
            {
                PathChanged = true,
                Shared = true,
                Action = 2,
                FullName = e.FullPath,
                OldFullName = e.OldFullPath,
            });
        }

        private void SharePathWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            this.changeQueue.Enqueue(new ChangeQueue()
            {
                PathChanged = true,
                Shared = true,
                Action = 1,
                FullName = e.FullPath,
            });
        }

        private void SharePathWatcher_Created(object sender, FileSystemEventArgs e)
        {
            this.changeQueue.Enqueue(new ChangeQueue()
            {
                PathChanged = true,
                Shared = true,
                Action = 0,
                FullName = e.FullPath,
            });
        }

        private void AppWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            this.changeQueue.Enqueue(new ChangeQueue()
            {
                PathChanged = false,
                Shared = false,
                Action = 3,
                //FullName = e.FullPath,
                OldFullName = e.FullPath,
            });
        }


        private void AppWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            this.changeQueue.Enqueue(new ChangeQueue()
            {
                PathChanged = false,
                Shared = false,
                Action = 2,
                FullName = e.FullPath,
                OldFullName = e.OldFullPath,
            });
        }

        private void AppWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            this.changeQueue.Enqueue(new ChangeQueue()
            {
                PathChanged = false,
                Shared = false,
                Action = 1,
                FullName = e.FullPath,
            });
        }

        private void AppWatcher_Created(object sender, FileSystemEventArgs e)
        {
            this.changeQueue.Enqueue(new ChangeQueue()
            {
                PathChanged = false,
                Shared = false,
                Action = 0,
                FullName = e.FullPath,
            });
        }

        private void AppPathWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            this.changeQueue.Enqueue(new ChangeQueue()
            {
                PathChanged = true,
                Shared = false,
                Action = 3,
                //FullName = e.FullPath,
                OldFullName = e.FullPath,
            });
        }

        private void AppPathWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            this.changeQueue.Enqueue(new ChangeQueue()
            {
                PathChanged = true,
                Shared = false,
                Action = 2,
                FullName = e.FullPath,
                OldFullName = e.OldFullPath,
            });
        }

        private void AppPathWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            this.changeQueue.Enqueue(new ChangeQueue()
            {
                PathChanged = true,
                Shared = false,
                Action = 1,
                FullName = e.FullPath,
            });
        }

        private void AppPathWatcher_Created(object sender, FileSystemEventArgs e)
        {
            this.changeQueue.Enqueue(new ChangeQueue()
            {
                PathChanged = true,
                Shared = false,
                Action = 0,
                FullName = e.FullPath,
            });
        }

        /// <summary>
        /// 新加共享文件，读取文件夹下面的所有的文件（只支持.json与.config）2种，并且建立监听文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"></param>
        /// <param name="encoding"></param>
        /// <param name="onBuilding"></param>
        /// <returns></returns>
        public ConfigurationWatcher AddShareFile(string path, string searchPattern, SearchOption searchOption, Encoding encoding, EventHandler<ShareFileEventArgs> onBuilding = null)
        {
            if (this.fileWather.ContainsKey(path))
                return this;

            var watcher = new FileSystemWatcher(path, searchPattern) { EnableRaisingEvents = true };
            this.fileWather.Add(path, watcher);
            watcher.Created += SharePathWatcher_Created;
            watcher.Changed += SharePathWatcher_Changed;
            watcher.Deleted += SharePathWatcher_Deleted;
            watcher.Renamed += SharePathWatcher_Renamed;
            foreach (var file in System.IO.Directory.GetFiles(path, searchPattern, searchOption).Select(ta => new FileInfo(ta)))
            {
                this.HandleShareFile(new ChangeQueue()
                {
                    PathChanged = true,
                    Action = 0,
                    FullName = file.FullName,
                    Shared = true,
                    OldFullName = null,
                }, encoding);
            }

            return this;
        }

        /// <summary>
        /// 新加共享文件，并且建立监听文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="encoding"></param>
        /// <param name="onBuilding"></param>
        /// <returns></returns>
        public ConfigurationWatcher AddShareFile(FileInfo file, Encoding encoding, EventHandler<ShareFileEventArgs> onBuilding = null)
        {
            if (!file.Exists)
                throw new FileNotFoundException("找不到文件", file.FullName);

            if (this.fileWather.ContainsKey(file.FullName))
                return this;

            if (this.ShareFileEventHandler != null)
            {
                this.ShareFileEventHandler += onBuilding;
            }

            this.HandleShareFile(new ChangeQueue()
            {
                PathChanged = false,
                Action = 0,
                FullName = file.FullName,
                Shared = true,
                OldFullName = null,
            }, encoding);

            return this;
        }

        /// <summary>
        /// 新加应用文件，读取文件夹下面的所有的文件（只支持.json与.config）2种，并且建立监听文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public ConfigurationWatcher AddAppFile(string path, string searchPattern, SearchOption searchOption, Encoding encoding)
        {
            if (this.fileWather.ContainsKey(path))
                return this;

            var watcher = new FileSystemWatcher(path, searchPattern) { EnableRaisingEvents = true };
            this.fileWather.Add(path, watcher);
            watcher.Created += AppPathWatcher_Created;
            watcher.Changed += AppPathWatcher_Changed;
            watcher.Deleted += AppPathWatcher_Deleted;
            watcher.Renamed += AppPathWatcher_Renamed;

            foreach (var file in System.IO.Directory.GetFiles(path, searchPattern, searchOption).Select(ta => new FileInfo(ta)))
            {
                this.HandleAppFile(new ChangeQueue()
                {
                    PathChanged = true,
                    Action = 0,
                    FullName = file.FullName,
                    Shared = false,
                    OldFullName = null,
                }, encoding);
            }

            return this;
        }

        /// <summary>
        /// 新加应用文件，并且建立监听文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public ConfigurationWatcher AddAppFile(FileInfo file, Encoding encoding)
        {
            if (!file.Exists)
                throw new FileNotFoundException("找不到文件", file.FullName);

            if (this.fileWather.ContainsKey(file.FullName))
                return this;

            this.HandleAppFile(new ChangeQueue()
            {
                PathChanged = false,
                Action = 0,
                FullName = file.FullName,
                Shared = false,
                OldFullName = null,
            }, encoding);

            return this;
        }

        /// <summary>
        /// 刷新任务
        /// </summary>
        /// <returns></returns>
        public TimeSpan Change()
        {
            while (this.changeQueue.TryDequeue(out var change))
            {
                switch (change.PathChanged)
                {
                    //路径级的修改
                    case true:
                        {
                            switch (change.Shared)
                            {
                                //应用级文件
                                case false:
                                    {
                                        this.HandleAppFile(change);
                                    }
                                    break;
                                //共享及文件
                                case true:
                                    {
                                        this.HandleShareFile(change);
                                    }
                                    break;
                            }
                        }
                        break;
                    //文件的修改
                    case false:
                        {
                            switch (change.Shared)
                            {
                                //应用级文件
                                case false:
                                    {
                                        this.HandleAppFile(change);
                                    }
                                    break;
                                //共享及文件
                                case true:
                                    {
                                        this.HandleShareFile(change);
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }

            return this.sleepTimeSpan;
        }

        /// <summary>
        /// 是否可以刷新
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private bool CanRefresh(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
                return false;

            var time = fileInfo.LastWriteTime;
            if (this.moreTimeLimit.TryGetValue(fileInfo.FullName, out var lastTime))
            {
                var sp = fileInfo.LastWriteTime - lastTime;
                if (sp <= TimeSpan.FromSeconds(1))
                {
                    return false;
                }
                else
                {
                    this.moreTimeLimit[fileInfo.FullName] = fileInfo.LastWriteTime;
                    return true;
                }
            }

            this.moreTimeLimit[fileInfo.FullName] = fileInfo.LastWriteTime;
            return true;
        }

        /// <summary>
        /// 处理share级
        /// </summary>
        /// <param name="change"></param>
        /// <param name="outsideEncoding"></param>
        private void HandleShareFile(ChangeQueue change, Encoding outsideEncoding = null)
        {
            var oldFile = change.OldFullName == null ? null : new FileInfo(change.OldFullName);
            var newFile = change.FullName == null ? null : new FileInfo(change.FullName);
            if (newFile != null && !this.CanRefresh(newFile))
                return;

            switch (change.Action)
            {
                //新加
                case 0:
                    {
                        var shareBuilder = new ShareConfigurationBuilder(new ConfigFileInfo() { File = newFile, Encoding = outsideEncoding ?? Encoding.UTF8 });
                        if (this.ShareFileEventHandler != null)
                        {
                            shareBuilder.OnBuilding += this.ShareFileEventHandler;
                        }

                        shareBuilder.Build();
                        switch (shareBuilder.FileType)
                        {
                            case ConfigFileType.Json:
                                {
                                    if (this.shareConfiguration.Any(ta => ta.JsonShareFile.Name == shareBuilder.JsonShareFile.Name))
                                    {
                                    }
                                    else
                                    {
                                        this.shareConfiguration.Add(shareBuilder);
                                    }
                                }
                                break;
                            case ConfigFileType.Xml:
                                {
                                    if (this.shareConfiguration.Any(ta => ta.XmlShareFile.Name == shareBuilder.XmlShareFile.Name))
                                    {
                                    }
                                    else
                                    {
                                        this.shareConfiguration.Add(shareBuilder);
                                    }
                                }
                                break;
                        }

                        if (!change.PathChanged && !this.fileWather.ContainsKey(newFile.FullName))
                        {
                            var watcher = new Never.IO.FileWatcher(shareBuilder.File) { EnableRaisingEvents = true };
                            this.fileWather.Add(shareBuilder.File.FullName, watcher);
                            this.moreTimeLimit.Add(shareBuilder.File.FullName, DateTime.Now);
                            watcher.Created += ShareWatcher_Created;
                            watcher.Changed += ShareWatcher_Changed;
                            watcher.Deleted += ShareWatcher_Deleted;
                            watcher.Renamed += ShareWatcher_Renamed;
                        }

                        this.WriteShareToLog(change, shareBuilder, null);
                    }
                    break;
                //修改
                case 1:
                    {
                        var oldBuilder = this.shareConfiguration.FirstOrDefault(ta => ta.File.FullName == newFile.FullName).Rebuild();
                        //引起引用文件的修改，先找出相应的引用，找到后等新的builder替换成功后再更新引用文件里面的引用
                        var refences = new List<IShareFileReference>();
                        foreach (var b in new[] { oldBuilder.JsonShareFile, oldBuilder.XmlShareFile })
                        {
                            if (b == null)
                                continue;

                            foreach (var app in this.appConfiguration)
                            {
                                if (app.Reference.IsNotNullOrEmpty() && app.Reference.Any(ta => ta.Name.IsEquals(b.Name)))
                                {
                                    refences.Add(app);
                                    continue;
                                }
                            }
                        }

                        if (refences.IsNotNullOrEmpty())
                        {
                            refences.UseForEach(ta => ta.Builder.Rebuild(this.shareConfiguration));
                            this.EatException(() =>
                            {
                                this.OnShareFileChanged?.Invoke(this, new ConfigurationWatcherEventArgs() { Builders = refences.Select(ta => ta.Builder) });
                            });
                        }

                        this.WriteShareToLog(change, oldBuilder, refences);
                    }
                    break;
                //重命名
                case 2:
                    {
                        var oldBuilder = this.shareConfiguration.FirstOrDefault(ta => ta.File.FullName == oldFile.FullName);
                        var newBuilder = new ShareConfigurationBuilder(new ConfigFileInfo() { File = newFile, Encoding = oldBuilder.Encoding });

                        //引起引用文件的修改，先找出相应的引用，找到后等新的builder替换成功后再更新引用文件里面的引用
                        var refences = new List<IShareFileReference>();
                        foreach (var b in new[] { oldBuilder.JsonShareFile, oldBuilder.XmlShareFile })
                        {
                            if (b == null)
                                continue;

                            foreach (var app in this.appConfiguration)
                            {
                                if (app.Reference.IsNotNullOrEmpty() && app.Reference.Any(ta => ta.Name.IsEquals(b.Name)))
                                {
                                    refences.Add(app);
                                    continue;
                                }
                            }
                        }

                        this.shareConfiguration.Remove(oldBuilder);
                        this.shareConfiguration.Add(newBuilder.Build(oldBuilder));
                        oldBuilder = null;
                        if (!change.PathChanged)
                        {
                            if (this.fileWather.ContainsKey(oldFile.FullName))
                            {
                                var watcher = this.fileWather[oldFile.FullName];
                                watcher.Path = System.IO.Path.GetDirectoryName(newFile.FullName);
                                watcher.Filter = newFile.Name;
                            }
                        }

                        if (refences.IsNotNullOrEmpty())
                        {
                            refences.UseForEach(ta => ta.Builder.Rebuild(this.shareConfiguration));
                            this.EatException(() =>
                            {
                                this.OnShareFileRenamed?.Invoke(this, new ConfigurationWatcherEventArgs() { Builders = refences.Select(ta => ta.Builder) });
                            });
                        }

                        this.WriteShareToLog(change, newBuilder, refences);
                    }
                    break;
                //删除
                case 3:
                    {
                        var oldBuilder = this.shareConfiguration.FirstOrDefault(ta => ta.File.FullName == oldFile.FullName);
                        //引起引用文件的修改，先找出相应的引用，找到后等新的builder替换成功后再更新引用文件里面的引用
                        var refences = new List<IShareFileReference>();
                        foreach (var b in new[] { oldBuilder.JsonShareFile, oldBuilder.XmlShareFile })
                        {
                            if (b == null)
                                continue;

                            foreach (var app in this.appConfiguration)
                            {
                                if (app.Reference.IsNotNullOrEmpty() && app.Reference.Any(ta => ta.Name.IsEquals(b.Name)))
                                {
                                    refences.Add(app);
                                    continue;
                                }
                            }
                        }

                        if (!change.PathChanged)
                        {
                            if (this.fileWather.ContainsKey(oldFile.FullName))
                            {
                                this.fileWather[oldFile.FullName].Dispose();
                                this.fileWather.Remove(oldFile.FullName);
                            }
                        }


                        this.shareConfiguration.Remove(oldBuilder);
                        if (refences.IsNotNullOrEmpty())
                        {
                            refences.UseForEach(ta => ta.Builder.Rebuild(this.shareConfiguration));
                            this.EatException(() =>
                            {
                                this.OnShareFileDeleted?.Invoke(this, new ConfigurationWatcherEventArgs() { Builders = refences.Select(ta => ta.Builder) });
                            });
                        }

                        this.WriteShareToLog(change, oldBuilder, refences);
                        oldBuilder.Dispose();
                    }
                    break;
            }
        }

        /// <summary>
        /// 处理app级
        /// </summary>
        /// <param name="change"></param>
        /// <param name="outsideEncoding"></param>
        private void HandleAppFile(ChangeQueue change, Encoding outsideEncoding = null)
        {
            var oldFile = change.OldFullName == null ? null : new FileInfo(change.OldFullName);
            var newFile = change.FullName == null ? null : new FileInfo(change.FullName);
            if (newFile != null && !this.CanRefresh(newFile))
                return;

            switch (change.Action)
            {
                //新加
                case 0:
                    {
                        IShareFileReference newBuilder = null;
                        switch (newFile.Extension.ToLower())
                        {
                            case ".json":
                                {
                                    var fileInfo = new ConfigFileInfo() { File = newFile, Encoding = outsideEncoding ?? Encoding.UTF8 };
                                    var builder = new JsonConfigurationBuilder(this.shareConfiguration, fileInfo, this.keyValueFinder) { }.Build();
                                    if (this.appConfiguration.Any(ta => ta.Builder.Name == builder.Name))
                                    {

                                    }
                                    else
                                    {
                                        this.appConfiguration.Add(builder);
                                    }

                                    newBuilder = builder;
                                }
                                break;
                            case ".conf":
                                {
                                    var fileInfo = new ConfigFileInfo() { File = newFile, Encoding = outsideEncoding };
                                    var builder = new XmlConfigurationBuilder(this.shareConfiguration, fileInfo, this.keyValueFinder) { }.Build();
                                    if (this.appConfiguration.Any(ta => ta.Builder.Name == builder.Name))
                                    {

                                    }
                                    else
                                    {
                                        this.appConfiguration.Add(builder);
                                    }

                                    newBuilder = builder;
                                }
                                break;
                        }

                        if (!change.PathChanged && !this.fileWather.ContainsKey(newFile.FullName))
                        {
                            var watcher = new Never.IO.FileWatcher(newFile) { EnableRaisingEvents = true };
                            this.fileWather.Add(newFile.FullName, watcher);
                            this.moreTimeLimit.Add(newFile.FullName, DateTime.Now);
                            watcher.Created += AppWatcher_Created;
                            watcher.Changed += AppWatcher_Changed;
                            watcher.Deleted += AppWatcher_Deleted;
                            watcher.Renamed += AppWatcher_Renamed;
                        }

                        this.EatException(() =>
                        {
                            this.OnAppFileChanged?.Invoke(this, new ConfigurationWatcherEventArgs() { Builders = new[] { newBuilder.Builder } });
                        });

                        this.WriteAppToLog(change, newBuilder);
                    }
                    break;

                //修改
                case 1:
                    {
                        var oldBuilder = this.appConfiguration.FirstOrDefault(ta => ta.Builder.File.File.FullName == newFile.FullName);
                        oldBuilder.Builder.Rebuild(this.shareConfiguration);
                        this.EatException(() =>
                        {
                            this.OnAppFileChanged?.Invoke(this, new ConfigurationWatcherEventArgs() { Builders = new[] { oldBuilder.Builder } });
                        });

                        this.WriteAppToLog(change, oldBuilder);
                    }
                    break;
                //重命名
                case 2:
                    {
                        var oldBuilder = this.appConfiguration.FirstOrDefault(ta => ta.Builder.File.File.FullName == oldFile.FullName);
                        var refence = default(IConfigurationBuilder);
                        switch (oldBuilder.Builder.FileType)
                        {
                            case ConfigFileType.Json:
                                {
                                    var fileInfo = new ConfigFileInfo() { File = newFile, Encoding = oldBuilder.Builder.File.Encoding };
                                    var newBuilder = new JsonConfigurationBuilder(this.shareConfiguration, fileInfo, this.keyValueFinder) { }.Build();
                                    this.appConfiguration.Remove(oldBuilder);
                                    this.appConfiguration.Add(newBuilder);
                                    refence = newBuilder;
                                }
                                break;
                            case ConfigFileType.Xml:
                                {
                                    var fileInfo = new ConfigFileInfo() { File = newFile, Encoding = oldBuilder.Builder.File.Encoding };
                                    var newBuilder = new XmlConfigurationBuilder(this.shareConfiguration, fileInfo, this.keyValueFinder) { }.Build();
                                    this.appConfiguration.Remove(oldBuilder);
                                    this.appConfiguration.Add(newBuilder);
                                    refence = newBuilder;
                                }
                                break;
                        }


                        if (!change.PathChanged && this.fileWather.ContainsKey(oldFile.FullName))
                        {
                            var watcher = this.fileWather[oldFile.FullName];
                            watcher.Path = System.IO.Path.GetDirectoryName(newFile.FullName);
                            watcher.Filter = newFile.Name;
                        }

                        if (refence != null)
                        {
                            this.EatException(() =>
                            {
                                this.OnAppFileRenamed?.Invoke(this, new ConfigurationWatcherEventArgs() { Builders = new[] { refence } });
                            });
                        }

                        this.WriteAppToLog(change, refence as IShareFileReference);
                    }
                    break;
                //删除
                case 3:
                    {
                        var oldBuilder = this.appConfiguration.FirstOrDefault(ta => ta.Builder.File.File.FullName == oldFile.FullName);
                        this.appConfiguration.Remove(oldBuilder);

                        if (!change.PathChanged && this.fileWather.ContainsKey(oldFile.FullName))
                        {
                            this.fileWather[oldFile.FullName].Dispose();
                            this.fileWather.Remove(oldFile.FullName);
                        }

                        this.EatException(() =>
                        {
                            this.OnAppFileDeleted?.Invoke(this, new ConfigurationWatcherEventArgs() { Builders = new[] { oldBuilder.Builder } });
                        });

                        this.WriteAppToLog(change, oldBuilder);
                    }
                    break;
            }
        }

        /// <summary>
        /// 写日志,吃掉异常
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        protected override void HandleException(Exception ex, string message)
        {
            if (this.LoggerBuilder == null)
                return;

            this.EatException(() =>
            {
                this.LoggerBuilder.Invoke().Build(typeof(ConfigurationWatcher)).Error(message, ex);
            });
        }

        private void WriteShareToLog(ChangeQueue change, ShareConfigurationBuilder builder, IEnumerable<IShareFileReference> references)
        {
            if (this.LoggerBuilder == null)
                return;

            this.EatException(() =>
            {
                var sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendFormat("share {0} action : {1}, at {2};", builder.File.Name, change.ToAction(), DateTime.Now.ToString());
                if (references.IsNotNullOrEmpty())
                {
                    sb.AppendLine();
                    sb.AppendFormat("the flowing files [");
                    var sname = string.Empty;
                    foreach(var r in references)
                    {
                        if (r == null || r.Builder == null)
                            continue;

                        sb.AppendLine();
                        sb.AppendFormat("{0}", r.Builder.Name);
                    }

                    sb.AppendFormat("] are using the share {0} file;", builder.File.Name);
                }

                this.LoggerBuilder.Invoke().Build(typeof(ConfigurationWatcher)).Info(sb.ToString());
            });
        }

        private void WriteAppToLog(ChangeQueue change, IShareFileReference builder)
        {
            if (this.LoggerBuilder == null)
                return;

            this.EatException(() =>
            {
                var sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendFormat("app {0} action : {1}, at {2};", builder.Builder.Name, change.ToAction(), DateTime.Now.ToString());
                if (builder.Reference.IsNotNullOrEmpty())
                {
                    sb.AppendFormat("the share files [");
                    var sname = string.Empty;
                    foreach (var r in builder.Reference)
                    {
                        if (r == null)
                            continue;

                        sb.AppendLine();
                        sb.AppendFormat("{0}", r.Name);
                        
                    }
                    sb.AppendLine();
                    sb.AppendFormat("] are using the app {0} file;", builder.Builder.Name);
                }

                this.LoggerBuilder.Invoke().Build(typeof(ConfigurationWatcher)).Info(sb.ToString());
            });
        }

        /// <summary>
        /// 吃掉异常
        /// </summary>
        /// <param name="action"></param>
        private void EatException(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                this.HandleException(ex, string.Empty);
            }
            finally
            {

            }
        }
        #endregion

        #region index

        /// <summary>
        /// 返回内容
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IConfigurationBuilder this[string name]
        {
            get
            {
                var first = this.appConfiguration.FirstOrDefault(ta => ta.Builder.Name == name);
                return first == null || first.Builder == null ? null : first.Builder;
            }
        }

        #endregion
    }
}
