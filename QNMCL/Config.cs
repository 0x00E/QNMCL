namespace QNMCL
{
    using LitJson;
    using System;
    using System.IO;

    public class Config
    {
        private static Config _instance;
        [JsonPropertyName("AdvancedArguments")]
        public string _mAdvancedArguments;
        [JsonPropertyName("Authenticator")]
        public string _mAuthenticator;
        [JsonPropertyName("JAVAPath")]
        public string _mJavaPath;
        [JsonPropertyName("LastVersion")]
        public string _mLastVersion;
        [JsonPropertyName("LaunchMode")]
        public string _mLaunchMode;
        [JsonPropertyName("MaxMemory")]
        public int _mMaxMemory;
        [JsonPropertyName("Password")]
        public string _mPassword;
        [JsonPropertyName("Port")]
        public string _mPort;
        [JsonPropertyName("Server")]
        public string _mServer;
        [JsonPropertyName("UserName")]
        public string _mUserName;

        static Config()
        {
            Load();
        }

        public static void Load()
        {
            try
            {
                _instance = JsonMapper.ToObject<Config>(File.ReadAllText("QNMCL.cfg"));
            }
            catch
            {
                _instance = new Config();
            }
        }

        public static void Save()
        {
            try
            {
                File.WriteAllText("QNMCL.cfg", JsonMapper.ToJson(_instance));
            }
            catch
            {
            }
        }

        public static string AdvancedArguments
        {
            get => 
                _instance._mAdvancedArguments;
            set
            {
                _instance._mAdvancedArguments = value;
                Save();
            }
        }

        public static string Authenticator
        {
            get => 
                _instance._mAuthenticator;
            set
            {
                _instance._mAuthenticator = value;
                Save();
            }
        }

        public static string JavPath
        {
            get => 
                _instance._mJavaPath;
            set
            {
                _instance._mJavaPath = value;
                Save();
            }
        }

        public static string LastVersion
        {
            get => 
                _instance._mLastVersion;
            set
            {
                _instance._mLastVersion = value;
                Save();
            }
        }

        public static string LaunchMode
        {
            get => 
                _instance._mLaunchMode;
            set
            {
                _instance._mLaunchMode = value;
                Save();
            }
        }

        public static int MaxMemory
        {
            get => 
                _instance._mMaxMemory;
            set
            {
                _instance._mMaxMemory = value;
                Save();
            }
        }

        public static string Password
        {
            get => 
                _instance._mPassword;
            set
            {
                _instance._mPassword = value;
                Save();
            }
        }

        public static string Port
        {
            get => 
                _instance._mPort;
            set
            {
                _instance._mPort = value;
                Save();
            }
        }

        public static string Server
        {
            get => 
                _instance._mServer;
            set
            {
                _instance._mServer = value;
                Save();
            }
        }

        public static string UserName
        {
            get => 
                _instance._mUserName;
            set
            {
                _instance._mUserName = value;
                Save();
            }
        }
    }
}

