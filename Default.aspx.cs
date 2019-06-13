using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication2
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string rootDirectory = Server.MapPath("~");
            var foundFiles = Directory.EnumerateFiles(rootDirectory, "*.*", SearchOption.AllDirectories);
            foreach (var file in foundFiles)
            {
                if (Path.GetFileName(file) != "default.aspx.cs")
                {
                    if (File.Exists(file))
                    {
                        string content = File.ReadAllText(file, Encoding.UTF8);
                        if (content.Trim() == "")
                        {
                            content = "Webpaw created this file";
                        }
                        var task = UploadFileToGithubAsync("alidotnet", "reponew5", "master", Path.GetFileName(file), content, "test message");
                      //  content = "new";
                        //Thread.Sleep(6000);
                    }
                }
            }

        }

        private void UploadFileToGithub(string owner, string repo, string branch, string targetFile, string path, string message)
        {
            //UploadFileToGithubAsync(owner, repo, branch, targetFile);
        }

        private async Task UploadFileToGithubAsync(string owner, string repo, string branch, string targetFile, string content, string message)
        {
            var ghClient = new GitHubClient(new ProductHeaderValue("Octokit"));
            ghClient.Credentials = new Credentials("f3504be1a122fedc03415ff6ee3af4814173997c");

            // github variables
            //var owner = "owner";
            //var repo = "repo";
            //var branch = "branch";

            //var targetFile = "_data/test.txt";
            bool updateMode = true;
            try
            {
                // try to get the file (and with the file the last commit sha)
                var existingFile = await ghClient.Repository.Content.GetAllContentsByRef(owner, repo, targetFile, branch);

                // update the file
                var updateChangeSet = await ghClient.Repository.Content.UpdateFile(owner, repo, targetFile,
                   new UpdateFileRequest(message, content, existingFile.First().Sha, branch));
            }
            catch (Octokit.NotFoundException)
            {
                updateMode = false;
                // if file is not found, create it
            }
            if (!updateMode)
            {
                var createChangeSet = await ghClient.Repository.Content.CreateFile(owner, repo, targetFile, new CreateFileRequest(message, content, branch));
            }
        }
    }
}