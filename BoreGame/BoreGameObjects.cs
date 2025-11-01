using System.Collections.Generic;

namespace BoreGame;

public class BoreGameObjects
{
    public enum EBoreGameType
    {
        BoreClassic,
        BoreDigital
    }
    
    public class ProfileObject
    {
        public ProfileObject(string server, string port, EBoreGameType boreGameType, string backgroundImageBase64)
        {
            Server = server;
            Port = port;
            BoreGameType = boreGameType;
            BackgroundImageBase64 = backgroundImageBase64;
        }

        public string Server { set; get; }
        
        public string Port { set; get; }

        public EBoreGameType BoreGameType { set; get; }

        public string BackgroundImageBase64 { set; get; }

    }
}