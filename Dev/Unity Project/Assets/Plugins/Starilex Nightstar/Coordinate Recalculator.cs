using UnityEngine;
using UnityEditor;
using KSPPartTools;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.ComponentModel;
using System.Reflection;

public class CoordinateRecalculator : EditorWindow {

    public struct PropStruct {
        public string Name;
        public float[] Position;
        public float[] Rotation;
        public float[] Scale;
    }

    public string GameDataPath = null;
    public string[] Data = null;
    public List <string> RawPropsData = new List <string>();
    public List<PropStruct> PropsData = new List <PropStruct>();

    [MenuItem("Starilex/Coordinate Recalculator")]
    public static void ShowWindow() {
        EditorWindow.GetWindow<CoordinateRecalculator>("Coordinate Recalculator");
    }

    public void OnSelectionChange(){
        Repaint();
    }
    void OnGUI() {
        this.autoRepaintOnSceneChange = true;
        GUI.contentColor = Color.cyan;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal("Box");
            
                GUILayout.BeginVertical("Box",  GUILayout.Width(Screen.width/2 - 5), GUILayout.MinWidth(350));
                    GUILayout.BeginHorizontal("Box");
                        GUILayout.FlexibleSpace();
                        GUI.skin.label.fontSize = 16;
                        GUILayout.Label("Coordinate Recalculator");
                        GUI.skin.label.fontSize = 12;
                        GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal("Box");
                        GUILayout.Space(30);
                        GUILayout.Label("Selected GameData    :", GUILayout.ExpandWidth(false));
                        GUILayout.Space(10);
                        if(File.Exists(Application.dataPath.Substring(0,Application.dataPath.Length-6)+"PartTools.cfg")){
                            GameDataPath = File.ReadAllLines(Application.dataPath.Substring(0,Application.dataPath.Length-6)+"PartTools.cfg")[0].Substring(11);
                            GUILayout.Label(GameDataPath);
                        }
                        else {
                            GUILayout.Label("Select the GameData folder using KSPartTools", GUILayout.MinWidth(140));
                        }
                        GUILayout.Space(10);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal("Box");
                        GUILayout.Space(30);
                        GUILayout.Label("Selected Root Object   :", GUILayout.ExpandWidth(false));
                        GUILayout.Space(10);
                        if (Selection.activeGameObject != null){
                            GUILayout.Label(Selection.activeGameObject.name);  }
                        else{ GUILayout.Label("None",GUILayout.MinWidth(140)); }
                        GUILayout.Space(10);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal("Box");
                        GUILayout.Space(30);
                        GUILayout.Label("IVA Textures                   :", GUILayout.ExpandWidth(false));
                        GUILayout.Space(10);
                        if(GUILayout.Button("Calculate")){
                            if(GameDataPath == null){
                                    Debug.LogError("The GameData folder needs to be set to calculate the textures");
                                }
                            else {
                                    foreach (string DDSFile in Directory.GetFiles(GameDataPath+ '/' + Selection.activeGameObject.GetComponent<InternalSpace>().spaceDirectory + '/', "*.dds"))
                                    {
                                    byte[] RawBytes = System.IO.File.ReadAllBytes(DDSFile);

                                     if(RawBytes[84] == 68 && RawBytes[85] == 88 && RawBytes[86] == 84 && RawBytes[87] == 53){
                                        if (RawBytes[4] != 124)
                                            Debug.LogError("Invalid DDS DXTn texture. Unable to read");  //this header byte should be 124 for DDS image files
                                    
                                        int height = RawBytes[13] * 256 + RawBytes[12];
                                        int width = RawBytes[17] * 256 + RawBytes[16];
                                         
                                        int DDS_HEADER_SIZE = 128;
                                        byte[] dxtBytes = new byte[RawBytes.Length - DDS_HEADER_SIZE];
                                        Buffer.BlockCopy(RawBytes, DDS_HEADER_SIZE, dxtBytes, 0, RawBytes.Length - DDS_HEADER_SIZE);
                                    
                                        Texture2D texture = new Texture2D(width, height,TextureFormat.DXT5,false);
                                        texture.LoadRawTextureData(dxtBytes);
                                        texture.Apply();
                                        texture.Compress(false);
                                        
                                        RenderTexture renderTex = RenderTexture.GetTemporary(
                                                    texture.width,
                                                    texture.height,
                                                    0,
                                                    RenderTextureFormat.Default,
                                                    RenderTextureReadWrite.Linear);

                                        Graphics.Blit(texture, renderTex);
                                        RenderTexture previous = RenderTexture.active;
                                        RenderTexture.active = renderTex;
                                        Texture2D readableText = new Texture2D(texture.width, texture.height);
                                        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
                                        readableText.Apply();
                                        RenderTexture.active = previous;
                                        RenderTexture.ReleaseTemporary(renderTex);

                                        File.WriteAllBytes(DDSFile.Substring(0,DDSFile.Length-3) + "png", readableText.EncodeToPNG());

                                        }
                                    }
                                }
                            }
                        GUILayout.Space(10);
                    GUILayout.EndHorizontal();
                ///*
                GUILayout.Space(5);
                    GUILayout.BeginHorizontal("Box");
                        GUILayout.FlexibleSpace();
                        if(GUILayout.Button("Calculate Coordinates",GUILayout.Width(210))){
                            if(Selection.activeGameObject != null){
                                if(Selection.activeGameObject.GetComponent("Prop") != null){
                                    Debug.LogError("The selected object is not the root of any IVA, select the capsule / internal space and not a prop of it.");
                                }
                                else{

                                    Data =  System.IO.File.ReadAllLines(GameDataPath + '/' + Selection.activeGameObject.GetComponent<InternalSpace>().spaceDirectory + '/' + Selection.activeGameObject.GetComponent<InternalSpace>().spaceConfig + ".cfg");
                                    
                                    // Separate the props
                                    bool InProp = false;
                                    for (int i = 0; i < Data.Length; i++) {
                                        if(Data[i].Trim() == "PROP"){
                                            InProp = true;
                                            RawPropsData.Add(Data[i].Trim().Replace(" ",""));
                                        }
                                        else if(Data[i].Trim() == "}" && InProp == true){
                                            RawPropsData.Add(Data[i].Trim().Replace(" ",""));
                                            InProp = false;
                                        }
                                        else if(InProp == true){
                                            RawPropsData.Add(Data[i].Trim().Replace(" ",""));
                                        }
                                    }
                                    
                                    // Formatting raw data into structs
                                    PropStruct Buffer = new PropStruct();
                                    for (int i = 0; i < RawPropsData.Count; i++) {

                                        if      (RawPropsData[i] == "{"){ InProp = true  ;}
                                        else if (RawPropsData[i] == "}"){ InProp = false ;}

                                        else if (InProp == true){

                                            if(RawPropsData[i].StartsWith("name")){
                                                Buffer.Name = RawPropsData[i].Substring(5);
                                            }
                                            else if(RawPropsData[i].StartsWith("position")){
                                                string[] pos = RawPropsData[i].Substring(9).Split(',');
                                                float[] float_pos = {
                                                                float.Parse(pos[0], CultureInfo.InvariantCulture),
                                                                float.Parse(pos[1], CultureInfo.InvariantCulture),
                                                                float.Parse(pos[2], CultureInfo.InvariantCulture)
                                                            };
                                                Buffer.Position = float_pos;
                                            }
                                            else if(RawPropsData[i].StartsWith("rotation")){
                                                string[] rot = RawPropsData[i].Substring(9).Split(',');
                                                float[] float_rot = {
                                                                float.Parse(rot[0], CultureInfo.InvariantCulture),
                                                                float.Parse(rot[1], CultureInfo.InvariantCulture),
                                                                float.Parse(rot[2], CultureInfo.InvariantCulture),
                                                                float.Parse(rot[3], CultureInfo.InvariantCulture)
                                                            };
                                                Buffer.Rotation = float_rot;
                                            }
                                            else if(RawPropsData[i].StartsWith("scale")){
                                                string[] scale = RawPropsData[i].Substring(6).Split(',');
                                                float[] float_rot = {
                                                                float.Parse(scale[0], CultureInfo.InvariantCulture),
                                                                float.Parse(scale[1], CultureInfo.InvariantCulture),
                                                                float.Parse(scale[2], CultureInfo.InvariantCulture),
                                                            };
                                                Buffer.Scale = float_rot;
                                                PropsData.Add(Buffer);
                                            }
                                        }
                                    }
                                    GameObject RootOjbect = Selection.activeGameObject;

                                    for(int i = 1; i < RootOjbect.transform.childCount; i++){
                                        for(int j = 0; j < PropsData.Count; j++){
                                            if(RootOjbect.transform.GetChild(i).GetComponent<Prop>().propName == PropsData[j].Name){
                                                RootOjbect.transform.GetChild(i).transform.position = new Vector3( PropsData[j].Position[0], PropsData[j].Position[1],PropsData[j].Position[2]);
                                                RootOjbect.transform.GetChild(i).transform.rotation = new Quaternion( PropsData[j].Rotation[0], PropsData[j].Rotation[1], PropsData[j].Rotation[2], PropsData[j].Rotation[3]);
                                                RootOjbect.transform.GetChild(i).transform.localScale = new Vector3( PropsData[j].Scale[0], PropsData[j].Scale[1], PropsData[j].Scale[2]);
                                                PropsData.RemoveAt(j);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    //*/
                GUILayout.Space(5);
                GUILayout.EndVertical();


            GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        
        // Clearing zone
        Data = null;
        RawPropsData.Clear();
        PropsData.Clear();
    }
}