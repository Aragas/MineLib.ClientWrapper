using MineLib.Network;

namespace MineLib.ClientWrapper
{
    public partial class Minecraft
    {
        /// <summary>
        ///     Login to Minecraft.net and store credentials
        /// </summary>
        private void Login()
        {
            if (VerifyNames)
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
            else
            {
                AccessToken = "None";
                SelectedProfile = "None";
            }
        }

        /// <summary>
        ///     Uses a client's stored credentials to verify with Minecraft.net
        /// </summary>
        public bool RefreshSession()
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

        public bool VerifySession()
        {
            if (!VerifyNames)
                return false;

            return Yggdrasil.VerifySession(AccessToken);
        }

        public bool Invalidate()
        {
            if (!VerifyNames)
                return false;

            return Yggdrasil.Invalidate(AccessToken, ClientToken);
        }

        public bool Logout()
        {
            if (!VerifyNames)
                return false;

            return Yggdrasil.Logout(ClientLogin, ClientPassword);
        }

    }
}
