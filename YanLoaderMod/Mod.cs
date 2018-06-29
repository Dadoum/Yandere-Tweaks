using UnityEngine;
using System.Xml;

public class YanLoaderMod_installer : MonoBehaviour
{
    private string[] ConsoleList = new string[255];

    GUIStyle styled = new GUIStyle();

    GameObject mod;
    bool modactive;
    AnimationClip modenter;
    AnimationClip modexit;

    AssetBundle asset;

    private static string commandinput = "";

    bool isActive = false;
    static bool isHooked = false;
    string str = "";
    static System.String[] log = new System.String[13];

    public Vector2 scrollPosition = Vector2.zero;

    public static void Install()
    {
        if (!isHooked)
        {
            new GameObject("Loader Mod").AddComponent<YanLoaderMod_installer>();
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Cursor.visible = true;

        asset = new WWW("file:///" + Application.dataPath + "/Yandere_Loader/YML.unity3d").assetBundle;
        mod = (GameObject) asset.LoadAssetAsync("ModMenuContainer", typeof(GameObject)).asset;
        modactive = false;

        DontDestroyOnLoad(mod);

        mod.SetActive(true);

        using (XmlReader reader = XmlReader.Create(Application.dataPath + "/Yandere_Loader/mods.xml"))
        {
            while (reader.Read())
            {
                // Only detect start elements.
                if (reader.IsStartElement())
                {
                    // Get element name and switch on it.
                    switch (reader.Name)
                    {
                        case "Mods":
                            break;

                        case "Mod":
                            break;

                        case "name":
                            if (reader.Read())
                            {
                                str += reader.Value.Trim() + "\n";
                            }
                            break;

                        case "version":
                            if (reader.Read())
                            {
                                str += "Version: " + reader.Value.Trim() + "\n\n";
                            }
                            break;
                    }
                }
            }
        }

        log[1] = log[2] = log[3] = log[4] = log[5] = "";
    }

    void OnGUI()
    {
        var text = "Yandere Loader ʀᴇʟᴇᴀsᴇ 1.0.0";
        GUI.enabled = true;
        GUI.color = Color.black;
        var position1 = new Rect(0, 0, 1000, 20);
        position1.x--;
        GUI.Label(position1, text);
        position1.x += 2;
        GUI.Label(position1, text);
        position1.x--;
        position1.y--;
        GUI.Label(position1, text);
        position1.y += 2;
        GUI.Label(position1, text);
        position1.y--;
        GUI.color = Color.white;
        GUI.Label(position1, text);

        if (FindObjectOfType<TitleMenuScript>().GetComponent<TitleMenuScript>().Selected == 7)
        {
            mod.transform.position = GameObject.Find("8 Extras").transform.position;
            mod.transform.rotation = GameObject.Find("8 Extras").transform.rotation;

            FindObjectOfType<TitleMenuScript>().GetComponent<TitleMenuScript>().Timer = 0f;

            if (!modactive)
            {
                mod.GetComponent<Animation>().clip = (AnimationClip) asset.LoadAssetAsync("appear", typeof(AnimationClip)).asset;
                mod.GetComponent<Animation>().Play(((AnimationClip)asset.LoadAssetAsync("appear", typeof(AnimationClip)).asset).name);
            }
            modactive = true;
        }

        if (modactive && !(FindObjectOfType<TitleMenuScript>().GetComponent<TitleMenuScript>().Selected == 7)) {
            mod.GetComponent<Animation>().clip = (AnimationClip) asset.LoadAssetAsync("hide", typeof(AnimationClip)).asset;
            mod.GetComponent<Animation>().Play(((AnimationClip)asset.LoadAssetAsync("hide", typeof(AnimationClip)).asset).name);
        }

        if (!isActive && !(FindObjectOfType<TitleMenuScript>().GetComponent<TitleMenuScript>().Selected == 7))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (isActive) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            commandinput = GUI.TextField(new Rect(0, Screen.height - 20, Screen.width, 20), commandinput);


            GUI.skin.box.alignment = TextAnchor.UpperLeft;

            GUI.Box(new Rect(0, Screen.height - (11 * 20), Screen.width, Screen.height - 200), log[12] + "\n" + log[11] + "\n" + log[10] + "\n" + log[9] + "\n" + log[8] + "\n" + log[7] + "\n" + log[6] + "\n" + log[5] + "\n" + log[4] + "\n" + log[3] + "\n" + log[2] + "\n" + log[1] + "\n" + log[0]);
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Insert))
        {
            isActive = !isActive;
        }



        GameObject.Find("8 Extras").GetComponent<UILabel>().text = "Mods";
        GameObject.Find("8 Extras").GetComponent<UILabel>().color = Color.white;

        if (isActive)
        {
            if (Event.current.keyCode == KeyCode.Return)
            {
                execute(commandinput);
                commandinput = "";
            }
        }

    }

    void execute(string command) {

        string[] args = command.ToLower().Split(' ');

        switch (args[0]) {
            case "":
                break;
            case "help":
                logText("help: Show list of commands.");
                logText("say: Write text in chat.");
                break;
            case "say":
                logText(command.Substring(args[0].Length + 1));
                break;
            default:
                logText("Unknown command.");
                break;
        }
    }

    void logText(System.String text)
    {
        log[12] = log[11];
        log[11] = log[10];
        log[10] = log[9];
        log[9] = log[8];
        log[8] = log[7];
        log[7] = log[6];
        log[6] = log[5];
        log[5] = log[4];
        log[4] = log[3];
        log[3] = log[2];
        log[2] = log[1];
        log[1] = log[0];
        log[0] = text;
    }
}
