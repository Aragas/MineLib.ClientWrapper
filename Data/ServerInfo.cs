using System.Drawing;

namespace MineLib.ClientWrapper.Data
{
    public struct ServerVersion
    {
        public string Name;
        public int Protocol;
    }

    public struct Players
    {
        public int Max;
        public int Online;
    }

    public struct Sample
    {   
    }

    public struct ServerInfo
    {
        public ServerVersion Version;
        public Players Players;
        public Sample Sample;
        public string Description;

        public Image Favicon;
    }

}
