// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System;
using System.IO;
using System.Net.Sockets;

namespace DiabloSpeech.Core.Twitch
{
    public class NetworkStreamTcpAdapter : INetworkStream
    {
        TcpClient client;

        public NetworkStreamTcpAdapter(string hostname, int port)
        {
            client = new TcpClient(hostname, port);
        }

        public Stream BaseStream
        {
            get { return client.GetStream(); }
        }

        public void Close()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                client.Close();
                client = null;
            }
        }
    }
}
