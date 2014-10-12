using MineLib.Network;

namespace MineLib.ClientWrapper
{
    public partial class Minecraft
    {
        /// <summary>
        ///     Login to Minecraft.net and store credentials
        /// </summary>
        private void MainLogin()
        {
            var result = Yggdrasil.Login(ClientLogin, ClientPassword);

            switch (result.Status)
            {
                case YggdrasilStatus.Success:
                    AccessToken = result.Response.AccessToken;
                    ClientToken = result.Response.ClientToken;
                    _clientUsername = result.Response.Profile.Name;
                    SelectedProfile = result.Response.Profile.ID;
                    break;

                default:
                    VerifyNames = false; // -- Fall back to no auth.
                    break;
            }
        }

        /// <summary>
        ///     Uses a client's stored credentials to verify with Minecraft.net
        /// </summary>
        public bool MainRefreshSession()
        {
            if (!VerifyNames)
                return false;

            var result = Yggdrasil.RefreshSession(AccessToken, ClientToken);

            switch (result.Status)
            {
                case YggdrasilStatus.Success:
                    AccessToken = result.Response.AccessToken;
                    ClientToken = result.Response.ClientToken;
                    return true;

                default:
                    return false;
            }
        }

        public bool MainVerifySession()
        {
            if (!VerifyNames)
                return false;

            return Yggdrasil.VerifySession(AccessToken);
        }

        public bool MainInvalidate()
        {
            if (!VerifyNames)
                return false;

            return Yggdrasil.Invalidate(AccessToken, ClientToken);
        }

        public bool MainLogout()
        {
            if (!VerifyNames)
                return false;

            return Yggdrasil.Logout(ClientLogin, ClientPassword);
        }
    }
}
