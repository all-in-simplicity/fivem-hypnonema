namespace Hypnonema.Server.Utils
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Threading.Tasks;

    using CitizenFX.Core;

    using Octokit;

    public class UpdateChecker
    {
        private const string RepositoryName = "fivem-hypnonema";

        private const string RepositoryOwner = "thiago-dev";

        private static Version LocalVersion => new Version(GetAssemblyFileVersion());

        public static async Task CheckForNewerVersion()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            var client = new GitHubClient(new ProductHeaderValue("hypnonema"));

            try
            {
                var releases = await client.Repository.Release.GetAll(RepositoryOwner, RepositoryName);
                if (releases == null) return;

                var latestVersion = new Version(releases[0].TagName);

                var versionComparison = LocalVersion.CompareTo(latestVersion);
                if (versionComparison < 0)

                    // Newer Version found.
                    Debug.WriteLine(
                        $"^5There is a newer version \"{latestVersion}\" of Hypnonema available for download!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static string GetAssemblyFileVersion()
        {
            var attribute = (AssemblyFileVersionAttribute) Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).Single();
            return attribute.Version;
        }
    }
}