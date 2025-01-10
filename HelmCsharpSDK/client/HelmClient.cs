using System.Diagnostics;

namespace HelmClientLibrary
{
    public class HelmClient
    {
        private readonly string helmPath;

        public HelmClient(string helmPath = "helm")
        {
            this.helmPath = helmPath;
        }

        private string RunCommand(string args)
        {

            Console.WriteLine($"Running command: {args}");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = helmPath,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"Helm command failed with exit code {process.ExitCode}: {error}");
            }

            // print the output of the command
            Console.WriteLine($"Output: {output}");

            // return the error output
            Console.WriteLine($"Error: {error}");

            return output;
        }

        public void EnsureRepoAdded(string repoName, string repoUrl)
        {
            try
            {
                RunCommand($"repo add {repoName} {repoUrl}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding repo {repoName}: {ex.Message}");
                throw;
            }
        }


        public void EnsureReposUpdated()
        {
            try
            {
                RunCommand($"repo update");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating repos: {ex.Message}");
                throw;
            }
        }

        public bool ReleaseExists(string releaseName, string namespaceName = "default")
        {
            try
            {
                var output = RunCommand($"list --namespace {namespaceName} -q");
                var releases = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                return Array.Exists(releases, r => r.Trim() == releaseName);
            }
            catch
            {
                return false;
            }
        }

        public string UpgradeOrInstallRelease(string releaseName, string chart, string namespaceName = "default", string additionalArgs = "")
        {

            if (ReleaseExists(releaseName, namespaceName))
            {
                Console.WriteLine($"Release '{releaseName}' already exists in namespace '{namespaceName}'. Upgrading...");
            }
            else
            {
                Console.WriteLine($"Release '{releaseName}' does not exist in namespace '{namespaceName}'. Installing...");
            }

            return RunCommand($"upgrade --install {releaseName} {chart} --namespace {namespaceName} --create-namespace {additionalArgs}");
        }

        /// <summary>
        /// Uninstalls a Helm release from a specific namespace.
        /// </summary>
        /// <param name="releaseName">The name of the release to uninstall.</param>
        /// <param name="namespaceName">The namespace from which to uninstall the release.</param>
        /// <returns>The output of the Helm command.</returns>
        public string UninstallRelease(string releaseName, string namespaceName = "default")
        {
            if (ReleaseExists(releaseName, namespaceName))
            {
                Console.WriteLine($"Uninstalling release '{releaseName}' from namespace '{namespaceName}'...");
                return RunCommand($"uninstall {releaseName} --namespace {namespaceName}");
            }
            else
            {
                Console.WriteLine($"Release '{releaseName}' does not exist in namespace '{namespaceName}'. Nothing to uninstall.");
                return string.Empty;
            }
        }
    }
}
