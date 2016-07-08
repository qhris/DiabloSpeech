// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Business.Twitch;
using System;
using System.IO;

namespace Tests.Twitch.Mocks
{
    public class NetworkStreamMock : INetworkStream
    {
        public Stream BaseStream { get; private set; }

        public NetworkStreamMock(Stream stream)
        {
            BaseStream = stream;
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
                if (BaseStream != null)
                {
                    BaseStream.Close();
                    BaseStream = null;
                }
            }
        }
    }
}
