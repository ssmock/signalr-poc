using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using Microsoft.AspNet.SignalR.Hubs;

namespace signalr_poc.File
{
    [HubName("FileViewer")]
    public class FileViewerHub : Hub
    {
        readonly FileChecker _checker;

        public FileViewerHub() : this(FileChecker.Instance) { /* Nothing */ }

        public FileViewerHub(FileChecker checker)
        {
            _checker = checker;
        }

        public string GetFileContents()
        {
            return _checker.GetFileContents();
        }
    }
}