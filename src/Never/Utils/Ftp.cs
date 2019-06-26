using System;
using System.IO;
using System.Net;

namespace Never.Utils
{
    /// <summary>
    /// FTP上传文件
    /// </summary>
    public sealed class Ftp
    {
        #region dict

        private static Caching.CounterDictCache<string, bool> createFptDict = new Caching.CounterDictCache<string, bool>(200);

        private static Threading.IRigidLocker locker = new Threading.MonitorLocker();

        #endregion dict

        #region 创建路径

        /// <summary>
        /// 检查ftp路径
        /// </summary>
        /// <param name="ftpPath">ftp路径</param>
        /// <param name="userId">用户Id</param>
        /// <param name="password">用户密码</param>
        public static void CheckFtpDir(string ftpPath, string userId, string password)
        {
            if (string.IsNullOrEmpty(ftpPath))
                throw new ArgumentNullException("要检查的路径不可为空");

            createFptDict.GetValue(ftpPath, () =>
            {
                var request = (FtpWebRequest)WebRequest.Create(ftpPath);
                request.Credentials = new NetworkCredential(userId, password);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                try
                {
                    using (var response = request.GetResponse())
                    {
                        return true;
                    }
                }
                catch
                {
                }

                return false;
            });
        }

        /// <summary>
        /// 创建ftp路径
        /// </summary>
        /// <param name="ftpPath">ftp路径</param>
        /// <param name="userId">用户Id</param>
        /// <param name="password">用户密码</param>
        public static void CreateFtpDir(string ftpPath, string userId, string password)
        {
            if (string.IsNullOrEmpty(ftpPath))
                throw new ArgumentNullException("要创建的路径不可为空");

            if (createFptDict.GetValue(ftpPath))
                return;

            locker.EnterLock(() =>
            {
                /*没有目录，则听一下*/
                CheckFtpDir(ftpPath, userId, password);

                if (createFptDict.GetValue(ftpPath))
                    return;

                var request = (FtpWebRequest)WebRequest.Create(ftpPath);
                request.Credentials = new NetworkCredential(userId, password);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                using (var response = request.GetResponse())
                {
                    createFptDict.SetValue(ftpPath, true);
                }
            });
        }

        /// <summary>
        /// 创建ftp路径
        /// </summary>
        /// <param name="ftpServer">ftp服务器</param>
        /// <param name="userId">用户Id</param>
        /// <param name="password">用户密码</param>
        /// <param name="uploadPath">ftp路径</param>
        public static void CreateFtpDir(string ftpServer, string userId, string password, string uploadPath)
        {
            if (string.IsNullOrEmpty(ftpServer))
                throw new ArgumentNullException("ftpServer的路径不可为空");

            var temp = string.Concat(ftpServer.TrimEnd('/'), '/', (uploadPath ?? string.Empty).Trim('/'));
            CreateFtpDir(temp, userId, password);
        }

        #endregion 创建路径

        #region 上传文件

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="ftpServer">ftp服务器</param>
        /// <param name="userId">用户Id</param>
        /// <param name="password">用户密码</param>
        /// <param name="uploadPath">ftp路径</param>
        /// <param name="localFile">本地文件</param>
        /// <returns></returns>
        public static bool Upload(string ftpServer, string userId, string password, string uploadPath, FileInfo localFile)
        {
            if (string.IsNullOrEmpty(ftpServer))
                throw new ArgumentNullException("Ftp上传路径不可为空，请检查");

            if (localFile == null)
                throw new ArgumentNullException("要上传的文件不可为空");

            if (!localFile.Exists)
                throw new FileNotFoundException("要上传的文件不存在");

            CreateFtpDir(ftpServer, userId, password, uploadPath);

            try
            {
                var request = WebRequest.Create(string.Concat(ftpServer.TrimEnd('/'), "/", (uploadPath ?? string.Empty).Trim('/'), "/", localFile.Name)) as FtpWebRequest;
                request.Credentials = new NetworkCredential(userId, password);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.UseBinary = true;
                request.ContentLength = localFile.Length;

                byte[] buffer = new byte[2028];
                using (FileStream fs = localFile.OpenRead())
                using (Stream st = request.GetRequestStream())
                {
                    int content = fs.Read(buffer, 0, buffer.Length);
                    while (true)
                    {
                        if (content == 0)
                            break;
                        st.Write(buffer, 0, content);
                        content = fs.Read(buffer, 0, buffer.Length);
                    }
                }
            }
            catch (UnauthorizedAccessException uex)
            {
                throw new Exception(string.Format("无法读取{0}文件，权限是否出现了问题?", localFile), uex);
            }
            catch (ProtocolViolationException pex)
            {
                throw new Exception("上传过程中网络出现了问题", pex);
            }
            catch (InvalidOperationException inex)
            {
                throw new Exception("当前Ftp限制了上传或该用户不可上传文件", inex);
            }

            return true;
        }

        #endregion 上传文件

        #region 下载文件

        /// <summary>
        /// 下载图片，以【流】显示内容
        /// </summary>
        /// <param name="ftpServer">The FTP server.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="password">The password.</param>
        /// <param name="fullName">The full name.</param>
        /// <returns></returns>
        public static MemoryStream Download(string ftpServer, string userId, string password, string fullName)
        {
            try
            {
                var request = WebRequest.Create(string.Concat(ftpServer.TrimEnd('/'), "/", fullName.Trim('/').Replace(@"\", "/"))) as FtpWebRequest;
                request.Credentials = new NetworkCredential(userId, password);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.UseBinary = true;

                byte[] buffer = new byte[2028];
                using (var resp = request.GetResponse())
                {
                    var st = resp.GetResponseStream();
                    var memory = new MemoryStream();
                    int content = st.Read(buffer, 0, buffer.Length);
                    memory.Write(buffer, 0, buffer.Length);

                    while (true)
                    {
                        if (content == 0)
                            break;

                        content = st.Read(buffer, 0, buffer.Length);
                        memory.Capacity += content;
                        memory.Write(buffer, 0, content);
                    }

                    memory.Position = 0;
                    return memory;
                }
            }
            catch (ProtocolViolationException pex)
            {
                throw new Exception("下载过程中网络出现了问题", pex);
            }
            catch (InvalidOperationException inex)
            {
                throw new Exception("未找到文件，或者用户无法访问文件", inex);
            }
        }

        #endregion 下载文件
    }
}