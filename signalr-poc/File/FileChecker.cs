using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Security.Cryptography;
using IO = System.IO;
using System.Reflection;

namespace signalr_poc.File
{
    public class FileChecker
    {
        private const string FILE_NAME = "test.txt";

        readonly Lazy<string> _filePath = new Lazy<string>(() => GetFilePath());
        
        readonly Timer _checkTimer;

        readonly IHubConnectionContext<dynamic> _clients;

        static readonly Lazy<FileChecker> _instance = 
            new Lazy<FileChecker>(() =>
                new FileChecker(
                    GlobalHost.ConnectionManager.GetHubContext<FileViewerHub>().Clients));
        
        string _currentFileHash;
        
        public static FileChecker Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private FileChecker(IHubConnectionContext<dynamic> hubConnectionContext)
        {
            _clients = hubConnectionContext;

            _checkTimer = new Timer(CheckFile, null, 1000, 1000);

            _currentFileHash = string.Empty;
        }

        void CheckFile(object state)
        {
            if(FileHasChanged())
            {
                var fileContents = GetFileContents();

                _clients.All.UpdateText(fileContents);
            }
        }

        public string GetFileContents()
        {
            var contents = IO.File.ReadAllText(_filePath.Value);

            return contents;
        }

        bool FileHasChanged()
        {
            var newHash = GetFileHash();

            if (newHash != _currentFileHash)
            {
                _currentFileHash = newHash;

                return true;
            }
            else return false;
        }

        static string GetFilePath()
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().CodeBase;
            var assemblyUri = new UriBuilder(assemblyLocation);
            var assemblyPath = Uri.UnescapeDataString(assemblyUri.Path);
            var directoryName = IO.Path.GetDirectoryName(assemblyPath);

            return IO.Path.Combine(directoryName, FILE_NAME);
        }

        string GetFileHash()
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = IO.File.OpenRead(_filePath.Value))
                {
                    return Convert.ToBase64String(md5.ComputeHash(stream));
                }
            }
        }
    }
}