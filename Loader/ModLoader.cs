using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class ModLoader
{

    static string game_name = "YandereSimulator";
    static string data_path = game_name + "_Data/";
    static string loader_path = data_path + "Yandere_Loader/";
    static string dll_name = "Assembly-CSharp.dll";
    static string dlls_path = data_path + "Managed/";
    static string dll_path = dlls_path + dll_name;
    static string temp_base = dlls_path + "Assembly-Temp-";
    static string remove_path = dlls_path + "Assembly-NoMod.dll";
    static string mods_path = "mods/";
    static string log_name = loader_path + "logs/" + DateTime.Now.Date.ToString().Substring(0, DateTime.Now.Date.ToString().Length - 9).Replace('/', '-') + "/";
    static string log_path = log_name + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "_log.txt";

    List<Mod> mods_loaded = new List<Mod>();

    static StreamWriter log;
    static LoaderForm form = new LoaderForm();

    static void Main(string[] args)
    {
        Form.CheckForIllegalCrossThreadCalls = false;
        form.Show();
        Application.Run(form);
    }

    static Thread thread;


    public void Program() {
         thread = new Thread(Thread);
        thread.Start();
    }

     static void Thread()
     {

        Console.SetOut(new ControlWriter(LoaderForm.textBox1));
        try
        {
            ModLoader.Launch();
        }
        catch (Exception e)
        {
            write_line("[CRITICAL]: Uncaught error: " + e.ToString());
        }
        Application.Exit();
    }

    public static void FastClose() {
        thread.Interrupt();
        try { File.Copy(remove_path, dll_path, true); } catch {}
        try {
            // File.Copy("./temp/YandereSimulator_original.exe", "./YandereSimulator.exe", true); 
        } catch {}
    }

    static void Launch() {

        ModLoader yanLoader = new ModLoader();
        yanLoader.createDirectory(log_name);

        write_line("[INFO]: Initialized Loader. \nInformations: \n Version: 0.8 \n More informations: remg1 mn1 b0 shp0 ");

        // yanLoader.first_run();
        try
        {
            File.Copy(dll_path, remove_path, false);
        }
        catch {
            write_line("[INFO]: Don't close the windows next time !");
        }
        try { Directory.Delete("zips", true); } catch
        {
            write_line("[INFO]: No zip mod found during last session.");
        }

        File.Delete("./YandereSimulator_Data/Yandere_Loader/mods.xml");

        try { Directory.Delete("./temp/", true); } catch {}
        yanLoader.createDirectory("./temp/");
        new DirectoryInfo("./temp/").Attributes = FileAttributes.Hidden;

        yanLoader.createDirectory(loader_path);
        yanLoader.createDirectory(mods_path);
        yanLoader.createDirectory(mods_path + "YML");
        yanLoader.createDirectory(loader_path + "textures");

        /*
        File.WriteAllBytes(mods_path + "YML/YanLoaderMod.dll", Loader.Properties.Resources.ymldll);
        File.WriteAllBytes(mods_path + "YML/modpkg.json", Loader.Properties.Resources.ymljson);
        File.WriteAllBytes(loader_path + "textures/mods_menu.png", Loader.Properties.Resources.picture1);
        File.WriteAllBytes(loader_path + "textures/mods_menu_dark.png", Loader.Properties.Resources.picture2);
        File.WriteAllBytes(loader_path + "textures/command_bar.png", Loader.Properties.Resources.picture3);
        // */

        yanLoader.init_mods();

        // File.Move("./YandereSimulator.exe", "./temp/YandereSimulator_original.exe");
        // File.WriteAllBytes("./YandereSimulator.exe", Loader.Properties.Resources.yanexe);

        Process process = new Process();
        process.StartInfo.FileName = game_name + ".exe";
        process.Start();

        log.Close();

        process.WaitForExit();

        bool flag = false;
        while (!flag)
        {
            try
            {
                // File.Copy("./temp/YandereSimulator_original.exe", "./YandereSimulator.exe", true);
                File.Copy(remove_path, dll_path, true);
                flag = true;
            }
            catch
            {

            }
        }

        System.Threading.Thread.Sleep(10000);

        Directory.Delete("./temp/", true);
        File.Delete(remove_path);
    }

    static void write_line(string text)
    {
        try { log.Close(); } catch {}
            var a = File.Create(log_path);
                        a.Close();
        
        string before_text = "[" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "] ";
        log = new StreamWriter(log_path);
         Console.Out.WriteLine(before_text + text);
         log.AutoFlush = true;
         log.WriteLine(before_text + text);
    }

    void updateProgressBar() {
        form.GetProgressBar().Suspend();
        form.GetProgressBar().PerformStep();
        form.GetProgressBar().Resume();
    }

    void init_mods()
    {
        form.GetProgressBar().Maximum = Directory.GetDirectories(mods_path).Length + Directory.GetFiles(mods_path, "*.zip").Length;
        foreach (string path in Directory.GetDirectories(mods_path))
        {
            if (is_mod(path))
            {
                Mod mod = new Mod(path);
                File.Copy(path + "/" + mod.getParameters()["id"].ToString() + ".dll", dlls_path + mod.getParameters()["id"].ToString() + ".dll", true);
                init_mod(dlls_path + mod.getParameters()["id"].ToString() + ".dll", mod);
                updateProgressBar();
            }
            else
            {
                write_line("[INFO]: " + path + " is not a valid mod, ignoring it.");
            }
        }
        foreach (string path in Directory.GetFiles(mods_path, "*.zip"))
        {
            Mod.Zip mod = new Mod.Zip(path);
            if (is_mod(mod.GetPath()))
            {
                File.Copy(mod.GetPath() + "/" + mod.getParameters()["id"].ToString() + ".dll", dlls_path + mod.getParameters()["id"].ToString() + ".dll", true);
                init_mod(dlls_path + mod.getParameters()["id"].ToString() + ".dll", mod);
                updateProgressBar();

            }
            else
            {
                write_line("[INFO]: " + path + " is not a valid mod, ignoring it.");
            }
        }
        registerMod(mods_loaded);
        patch_dll();

        foreach (string file in Directory.GetFiles("./YandereSimulator_Data/Managed/", "Assembly-Temp-" + "*.dll"))
            try { File.Delete(file); } catch {}

        form.GetProgressBar().Suspend();
        form.GetProgressBar().Maximum = 1;
        form.GetProgressBar().PerformStep();
        form.GetProgressBar().Resume();
    }

    void init_mod(string mod, Mod.Zip modclass)
    {
        init_mod(mod, new Mod(modclass.GetPath()));
    }

    void init_mod(string mod, Mod modclass)
    {
        try
        {
            if (Directory.Exists(modclass.getDllsPath()))
                foreach (string dll in Directory.GetFiles(modclass.getDllsPath(), "*.dll"))
                    File.Copy(dll, dlls_path);

            var temp_path = temp_base + mods_loaded.Count + "-" + modclass.getParameters()["id"] + ".dll";
            File.Copy(dll_path, temp_path, true);

            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(dlls_path);

            var reader = new ReaderParameters()
            {
                AssemblyResolver = resolver
            };

            var game_dll = AssemblyDefinition.ReadAssembly(temp_path, reader).MainModule;
            var welcome = game_dll.Types.Single(t => t.Name == "WelcomeScript");
            var start = welcome.Methods.Single(m => m.Name == "Start");

            var mod_dll = AssemblyDefinition.ReadAssembly(mod).MainModule;
            var mod_installer = mod_dll.Types.Single(t => t.Name == modclass.getParameters()["id"] + "_installer");
            var mod_method = mod_installer.Methods.Single(m => m.Name == "Install");

            start.Body.GetILProcessor().InsertBefore(start.Body.Instructions[0], start.Body.GetILProcessor().Create(OpCodes.Call, start.Module.ImportReference(mod_method)));

            game_dll.Write(dll_path);

            write_line("[INFO]: Mod \"" + modclass.getParameters()["name"].ToString() + "\" is loaded !");
            mods_loaded.Add(modclass);
        }
        catch (Exception e)
        {
            write_line("[ERROR]: Failed to load the mod \"" + modclass.getParameters()["name"].ToString() + "\"" + " : " + e.ToString());
        }
    }

    void first_run()
    {
        if (!File.Exists(data_path + "first_run"))
        {
            backup_dll(false);
            File.Create(data_path + "first_run");
        }
    }

    void backup_dll(bool action)
    {
        if (!action)
        {
            try
            {
                File.Copy(dll_path, remove_path, true);
            }
            catch (Exception e) {
                write_line("[CRITICAL]: Failed to backup dll: " + e.ToString());
                Application.Exit();
            }
        }
        else
        {
            try
            {
                File.Copy(remove_path, dll_path, true);
                Application.Exit();
            }
            catch (Exception e) {
                write_line("[CRITICAL]: Failed to backup dll: " + e.ToString());
            }
        }
    }

    private void registerMod(List<Mod> mods)
    {
        var xml = new StreamWriter(File.Create("./YandereSimulator_Data/Yandere_Loader/mods.xml", 255, FileOptions.None));
        xml.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\" ?>");
        xml.WriteLine("<Mods>");

        foreach (Mod mod in mods) {
            xml.WriteLine("\t<Mod>");
            xml.Write("\t\t<id>");
            xml.Write(mod.getParameters()["id"]);
            xml.Write("</id>\n");
            xml.Write("\t\t<name>");
            xml.Write(mod.getParameters()["name"]);
            xml.Write("</name>\n");
            xml.Write("\t\t<version>");
            xml.Write(mod.getParameters()["version"]);
            xml.Write("</version>\n");
            xml.WriteLine("\t</Mod>");
        }

        xml.WriteLine("</Mods>");

        xml.Close();
    }

    void patch_dll()
    {
        try
        {
            var temp_path = temp_base + "patch.dll";


            File.Copy(dll_path, temp_path, true);


            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(dlls_path);

            var reader = new ReaderParameters()
            {
                AssemblyResolver = resolver
            };

            var game_dll = AssemblyDefinition.ReadAssembly(temp_path, reader).MainModule;
            var welcome = game_dll.Types.Single(t => t.Name == "WelcomeScript");

            var labels = welcome.Resolve().Fields.Single(f => f.Name == "FlashingLabels");
            labels.IsPrivate = false;
            labels.IsPublic = true;
            labels.Attributes = Mono.Cecil.FieldAttributes.Public;

            var csharp_dll = AssemblyDefinition.ReadAssembly(dlls_path + "Assembly-CSharp-firstpass.dll", reader);
            var ui_label = csharp_dll.MainModule.Types.Single(t => t.Name == "UILabel");
            var set_text = ui_label.Methods.Single(m => m.Name == "set_text");

            var setref = game_dll.ImportReference(set_text);

            var start = welcome.Methods.Single(m => m.Name == "Start");
            var il2 = start.Body.GetILProcessor();


            il2.InsertBefore(start.Body.Instructions[0], il2.Create(OpCodes.Callvirt, setref));
            il2.InsertBefore(start.Body.Instructions[0], il2.Create(OpCodes.Ldstr, "Warning ! Do not submit bug reports, Yandere Loader is loaded !"));
            il2.InsertBefore(start.Body.Instructions[0], il2.Create(OpCodes.Ldelem_Ref));
            il2.InsertBefore(start.Body.Instructions[0], il2.Create(OpCodes.Ldc_I4_2));
            il2.InsertBefore(start.Body.Instructions[0], il2.Create(OpCodes.Ldfld, labels));
            il2.InsertBefore(start.Body.Instructions[0], il2.Create(OpCodes.Ldarg_0));

            game_dll.Write(dll_path);
        }
        catch (Exception e)
        {
            write_line("[CRITICAL]: Failed to patch dll: " + e.ToString());
            Application.Exit();
        }
    }

    bool is_mod(string path)
    {
        try
        {
            string str = new Mod(path).getParameters()["name"].ToString();
            return true;
        }
        catch
        {
            return false;
        }
    }

    bool createDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            return false;
        }
        return true;
    }

}
