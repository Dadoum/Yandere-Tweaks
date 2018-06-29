using Newtonsoft.Json.Linq;
using System.IO;
using System.IO.Compression;

public class Mod
{

    JObject data;
    string persistpath;

    public Mod(string path)
    {
        data = JObject.Parse(File.ReadAllText(path + "/modpkg.json"));
        persistpath = path;
    }

    public class Zip
    {

        JObject data;
        string persistpath;

        public Zip(string path)
        {
            ZipArchive zip = new ZipArchive(new StreamReader(path).BaseStream, ZipArchiveMode.Read);
            zip.ExtractToDirectory("zips/" + path.Substring(0, path.Length - 4));
            File.SetAttributes("zips/" + path.Substring(0, path.Length - 4), FileAttributes.Hidden);
            File.SetAttributes("zips/", FileAttributes.Hidden);
            data = JObject.Parse(File.ReadAllText("zips/" + path.Substring(0, path.Length - 4) + "/modpkg.json"));
            persistpath = "zips/" + path.Substring(0, path.Length - 4);

        }

        public string GetPath()
        {
            return persistpath;
        }

        public JObject getParameters()
        {
            return data;
        }

        public string getDllPath()
        {
            return persistpath + "/" + data["id"] + ".dll";
        }

        public string getDllsPath()
        {
            return persistpath + "/" + data["id"] + "_Dlls";
        }

    }

    public JObject getParameters()
    {
        return data;
    }

    public string getDllPath()
    {
        return persistpath + "/" + data["id"] + ".dll";
    }

    public string getDllsPath()
    {
        return persistpath + "/" + data["id"] + "_Dlls";
    }

 
}
