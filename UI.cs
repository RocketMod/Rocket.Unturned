using SDG.Unturned;
using System;
using System.IO;
using UnityEngine;

namespace Rocket.Unturned
{
    internal class UI : MonoBehaviour
    {
        internal static UI Instance;
        internal static void Instantiate()
        {
            Instance = new GameObject().AddComponent<UI>();
        }

        Transform billboard = null;

        private void Awake()
        {
            DontDestroyOnLoad(base.gameObject);
            Camera.main.transform.position = new Vector3(0, 1, 1);
            Camera.main.transform.rotation = new Quaternion(0, 5, 0, 0);

            ObjectAsset objectAsset = (ObjectAsset)Assets.find(EAssetType.OBJECT, 402);
            billboard = ((GameObject)UnityEngine.Object.Instantiate(objectAsset.Model)).transform;
            billboard.transform.rotation = new Quaternion(-19.73f, 0, 0, 20);
            billboard.transform.position = new Vector3(0.54f, -6.81f, -3);
        }


        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0, Screen.height-250, Screen.width, 250));
            GUILayout.BeginVertical();
            if (GUILayout.Button("Note: You can't play Unturned with Rocket installed. Rocket is a mod for Unturned 3 servers only. Please read the wiki in order to find out how to host Rocket servers.", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                System.Diagnostics.Process.Start("https://github.com/RocketFoundation/Rocket/wiki/Installing-Rocket");
                dumpImages();
            }
            if (GUILayout.Button("Exit", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                Application.Quit();
            }
            //if (cam != null)
            //{
            //    Vector3 _cam = cam.transform.position;
            //    GUILayout.Label("Position: " + (int)_cam.x + ", " + (int)_cam.y + ", " + (int)_cam.z);
            //}
            //else cam = Camera.main;

            //checkbox = GUILayout.Toggle(checkbox, "Checkbox 1");
            //checkbox1 = GUILayout.Toggle(checkbox1, "Checkbox 2");

            //GUILayout.Label("Scrollbar: " + (int)scrollbar);
            //scrollbar = GUILayout.HorizontalSlider(scrollbar, 20, 2000);

           // cam.transform.rotation = new Quaternion(0,5,0,0);


            //if (GUILayout.Button("Button"))
            //{
            //    dumpImages();
            //}

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void dumpImages()
        {
            Directory.CreateDirectory("Images");
            Asset[] assets = Assets.find(EAssetType.ITEM);
            foreach (ItemAsset asset in assets)
            {
                try
                {
                    ushort id = ((Asset)asset).Id;
                    ItemTool.getIcon(id, 100, new byte[0], (Texture2D t) =>
                    {
                        byte[] bytes = t.EncodeToPNG();
                        string filename = "Images/" + id + ".png";
                        System.IO.File.WriteAllBytes(filename, bytes);
                    });
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.ToString());
                }
            }
        }

    }
}
