using System.Text.RegularExpressions;
using Reck.Exceptions;

namespace Reck.Utils;

//  I really think we don't need this
internal static class UrlFormatter
{
    private static Regex rgx = new Regex("[^A-Za-z0-9]");

    internal static string FormatBaseUrl(string url)
    {
        //  Separate the base url from the parameters
        string[] url_splitted = url.Split('?');

        //  Base url
        string base_url = url_splitted[0];
        //  Directories path of the base url
        string[] dirs = base_url.Split('/');

        //  Our urls will always use slash, not back-slash
        base_url.Replace('\\', '/');

        //  A directory must start with a letter, can only contain alpha and numerical chars, and must not be empty
        foreach (string dir in dirs){
            if (dir.Length == 0){
                throw new BadApiUrlException(url,
                    $"Directory name must be valid. Got an empty string ( Maybe did you double '/' ? Are there some '/' at the trailing end ? )");
            }

            if (rgx.IsMatch(dir)){
                throw new BadApiUrlException(url, $"Directory name must not contain special letter ( {dir} )");
            }

            if (!Char.IsLetter(dir[0])){
                throw new BadApiUrlException(url, $"Directory name must start with a letter ( {dir} )");
            }
        }

        //  If our url is empty or doesn't start with '/', add one
        if (base_url.Length == 0 || base_url[0] != '/'){
            base_url = $"/{base_url}";
        }

        return $"{base_url}";
    }

    internal static string FormatDirectoryName(string dir)
    {
        //  Removes any '/' or '\'
        dir = dir.Replace('\\', '/').Replace("/", "");

        //  Check for emptiness
        if (dir.Length == 0){
            throw new BadApiUrlException(dir, $"Directory name must not be empty.");
        }

        //  Check for any special character in the directory's name
        if (rgx.IsMatch(dir)){
            throw new BadApiUrlException(dir, $"Directory name must not contain any special character ( {dir} )");
        }

        //  Check whether or not the directory's name start with a letter
        if (!Char.IsLetter(dir[0])){
            throw new BadApiUrlException(dir, $"Directory name must start with a letter ( {dir} )");
        }

        return dir;
    }
}
