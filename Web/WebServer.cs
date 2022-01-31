using System.Net;

namespace Web
{
    public class WebServer
    {
        public event EventHandler<RequestReciverEventArgs>? RequestRecived;

        //private TcpListener _tcpListner = new TcpListener(new IPEndPoint(IPAddress.Any, 8080));
        private HttpListener? _HTTPListener;
        private readonly int _port;
        private bool _enabled;
        private readonly object _SyncRoot = new object();

        public int Port => _port;
        public bool Enabled { get => _enabled; set { if (value) Start(); else Stop(); } }

        public WebServer(int port) => _port = port;

        public void Start() 
        {
            if (_enabled) return;
            lock(_SyncRoot)
            {
                if (_enabled) return;

                // before as first start the server you need add next couple lines(that guarantee rights to use next prefixes):
                // netsh http add urlacl url=http://*:8080/ user=user_name
                // netsh http add urlacl url=http://+:8080/ user=user_name 
                _HTTPListener = new HttpListener();
                _HTTPListener.Prefixes.Add($"http://*:{_port}/"); 
                _HTTPListener.Prefixes.Add($"http://+:{_port}/"); 
                _enabled = true;
                ListnAsync();
            }
        }

        public void Stop() 
        { 
            if(!_enabled) return;
            lock (_SyncRoot)
            {
                if (!_enabled) return;

                _HTTPListener = null;
                _enabled = false;
            }
        }

        private async void ListnAsync()
        {
            var listner = _HTTPListener;

            listner.Start();

            HttpListenerContext context = null;

            while (_enabled)
            {
                var get_context_task = listner.GetContextAsync();
                if (context != null)
                    ProcessRequest(context);
                context = await get_context_task.ConfigureAwait(false);
            }

            listner.Stop();
        }

        private async void ProcessRequest(HttpListenerContext context)
        {
            await Task.Run(() => RequestRecived?.Invoke(this, new RequestReciverEventArgs(context)));
        } 
    }

    public class RequestReciverEventArgs : EventArgs 
    {
        public HttpListenerContext Context { get; }

        public RequestReciverEventArgs(HttpListenerContext context) { Context = context; }
    }
}