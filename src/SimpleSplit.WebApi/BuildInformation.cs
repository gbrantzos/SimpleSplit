using System.Globalization;

namespace SimpleSplit.WebApi
{
    public class BuildInformation
    {
        public string Version { get; set; } = "0.0.1";
        public DateTime BuildAt { get; private init; }
        public string Branch { get; private init; }
        public string Commit { get; private init; }
        public bool IsDebug { get; private init; } = true;
        public string Mode => IsDebug ? "DEBUG" : "RELEASE";


        public static BuildInformation Instance { get; }

        static BuildInformation()
        {
            var type = typeof(BuildInformation);
            using var buildData = type
                .Assembly
                .GetManifestResourceStream($"{type.FullName}.txt");
            if (buildData == null)
                throw new ArgumentException("Build information stream is null!");

            using var reader = new StreamReader(buildData);
            var text = reader.ReadToEnd().Trim();
            var infoParts = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (infoParts!.Length < 3)
                throw new ArgumentException("Invalid build information.");

            Instance = new BuildInformation
            {
                BuildAt = DateTime.ParseExact(infoParts[0], "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture),
                Branch = infoParts[1]
                    .Substring(infoParts[1].IndexOf(":", StringComparison.CurrentCultureIgnoreCase) + 1).Trim(),
                Commit = infoParts[2]
                    .Substring(infoParts[2].IndexOf(":", StringComparison.CurrentCultureIgnoreCase) + 1).Trim(),
                #if DEBUG
                IsDebug = true
                #else
                IsDebug = false;
                #endif
            };
        }
    }
}