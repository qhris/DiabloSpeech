// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System;
using System.IO;

namespace DiabloSpeech.Business.Twitch
{
    public interface INetworkStream : IDisposable
    {
        Stream BaseStream { get; }
        void Close();
    }
}
