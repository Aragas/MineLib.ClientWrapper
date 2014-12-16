using System;
using MineLib.Network;
using MineLib.Network.Data.Structs;

namespace MineLib.ClientWrapper
{
    public partial class Minecraft
    {
        public IAsyncResult BeginConnectToServer(AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.DoAsyncSending(typeof(BeginConnectToServer), new BeginConnectToServerParameters(asyncCallback, state));
        }

        public IAsyncResult BeginKeepAlive(int value, AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.DoAsyncSending(typeof(BeginKeepAlive), new BeginKeepAliveParameters(value, asyncCallback, state));
        }

        public IAsyncResult BeginSendClientInfo(AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.DoAsyncSending(typeof(BeginSendClientInfo), new BeginSendClientInfoParameters(asyncCallback, state));
        }

        public IAsyncResult BeginRespawn(AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.DoAsyncSending(typeof(BeginRespawn), new BeginRespawnParameters(asyncCallback, state));
        }

        public IAsyncResult BeginPlayerMoved(IPlaverMovedData data, AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.DoAsyncSending(typeof(BeginPlayerMoved), new BeginPlayerMovedParameters(data, asyncCallback, state));
        }

        public IAsyncResult BeginPlayerMoved(PlaverMovedMode mode, IPlaverMovedData data, AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.DoAsyncSending(typeof(BeginPlayerMoved), new BeginPlayerMovedParameters(mode, data, asyncCallback, state));
        }

        public IAsyncResult BeginPlayerSetRemoveBlock(PlayerSetRemoveBlockMode mode, IPlayerSetRemoveBlockData data, AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.DoAsyncSending(typeof(BeginPlayerSetRemoveBlock), new BeginPlayerSetRemoveBlockParameters(mode, data, asyncCallback, state));
        }

        public IAsyncResult BeginPlayerSetRemoveBlock(IPlayerSetRemoveBlockData data, AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.DoAsyncSending(typeof(BeginPlayerSetRemoveBlock), new BeginPlayerSetRemoveBlockParameters(data, asyncCallback, state));
        }

        public IAsyncResult BeginSendMessage(string message, AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.DoAsyncSending(typeof(BeginSendMessage), new BeginSendMessageParameters(message, asyncCallback, state));
        }

        public IAsyncResult BeginPlayerHeldItem(short slot, AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.DoAsyncSending(typeof(BeginPlayerHeldItem), new BeginPlayerHeldItemParameters(slot, asyncCallback, state));
        }
    }
}
