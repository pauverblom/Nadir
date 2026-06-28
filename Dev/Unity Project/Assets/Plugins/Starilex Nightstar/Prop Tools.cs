using UnityEngine;
using UnityEditor;
using KSPPartTools;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

public class PropTools : EditorWindow {

    Vector3 Position = new Vector3();
    Quaternion Rotation = new Quaternion();
    Vector3 Scale    = new Vector3();
    string GameDataPath = null;
    string PluginPath = null;

    [MenuItem("Starilex/Prop Tools")]
    public static void ShowWindow() {
        EditorWindow.GetWindow<PropTools>("Prop Tools");
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
                        GUILayout.Label("Prop Tools");
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
                        GUILayout.Label("Selected Prop               : ", GUILayout.ExpandWidth(false));
                        GUILayout.Space(10);
                        if (Selection.activeGameObject != null){
                            GUILayout.Label(Selection.activeGameObject.name);  }
                        else{ GUILayout.Label("None",GUILayout.MinWidth(140)); }
                        GUILayout.Space(10);
                        if (GUILayout.Button("Open Config", GUILayout.ExpandWidth(false))){
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(GameDataPath == null) {Debug.LogError("The GameData folder must be selected to get the config path");}
                            else if(Selection.activeGameObject.GetComponent("Prop") == null){Debug.LogError("The selected Object is not a KSP Prop");}
                            else{
                                
                                PluginPath = Selection.activeGameObject.GetComponent<Prop>().propUrl;
                                
                                bool Found = false;
                                for (int i = PluginPath.Length-1; i>0; i--){
                                    if(PluginPath[i] == '/'){
                                        if(Found == true){ PluginPath = PluginPath.Substring(0,i); break;}
                                        else {Found = true;}
                                    }
                                }
                                
                                PluginPath = GameDataPath+"\\"+PluginPath;
                                PluginPath = PluginPath.Replace(@"/", @"\");
                                System.Diagnostics.Process.Start("explorer.exe", PluginPath);
                            }
                        }
                        GUILayout.Space(10);
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);

                    GUILayout.BeginHorizontal("Box");
                        GUILayout.Space(30);
                        GUILayout.Label("Position : ", GUILayout.ExpandWidth(false));
                        GUILayout.Space(10);
                        GUI.backgroundColor = Color.white;
                        GUI.contentColor = Color.cyan;
                        if(GUILayout.Button("Copy Position",GUILayout.MinWidth(100))){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Position == null) { Debug.LogError("A position must be copied before applying it");}
                            else{
                            Position = Selection.activeGameObject.transform.position;
                            } }
                        GUI.backgroundColor = Color.red;
                        GUI.contentColor = Color.white;
                        if(GUILayout.Button("Paste X")){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Position == null) { Debug.LogError("A position must be copied before applying it");}
                            else{
                            Selection.activeGameObject.transform.position = new Vector3(Position[0],Selection.activeGameObject.transform.position[1],Selection.activeGameObject.transform.position[2]);
                            } }
                        GUI.backgroundColor = Color.green;
                        if(GUILayout.Button("Paste Y")){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Position == null) { Debug.LogError("A scale must be copied before applying it");}
                            else{
                            Selection.activeGameObject.transform.position = new Vector3(Selection.activeGameObject.transform.position[0],Position[1],Selection.activeGameObject.transform.position[2]);
                            } }
                        GUI.backgroundColor = new Color(0.118f, 0.35f, 1f,1f);
                        if(GUILayout.Button("Paste Z")){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Position == null) { Debug.LogError("A position must be copied before applying it");}
                            else{
                            Selection.activeGameObject.transform.position = new Vector3(Selection.activeGameObject.transform.position[0],Selection.activeGameObject.transform.position[1],Position[2]);
                            } }
                        GUI.backgroundColor = Color.white;
                        GUI.contentColor = Color.cyan;
                        if(GUILayout.Button("Paste Position",GUILayout.MinWidth(100))){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Position == null) { Debug.LogError("A position must be copied before applying it");}
                            else{
                                Selection.activeGameObject.transform.position = Position;
                            } }
                        GUILayout.Space(10);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal("Box");
                        GUILayout.Space(30);
                        GUILayout.Label("Rotation : ", GUILayout.ExpandWidth(false));
                        GUILayout.Space(10);
                        if(GUILayout.Button("Copy Rotation",GUILayout.MinWidth(100))){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Rotation == null) { Debug.LogError("A rotation must be copied before applying it");}
                            else{
                            Rotation = Selection.activeGameObject.transform.rotation;
                            } }
                        GUI.backgroundColor = Color.red;
                        GUI.contentColor = Color.white;
                        if(GUILayout.Button("Paste X")){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Rotation == null) { Debug.LogError("A rotation must be copied before applying it");}
                            else{
                            Selection.activeGameObject.transform.rotation = new Quaternion(Rotation[0],Selection.activeGameObject.transform.rotation[1],Selection.activeGameObject.transform.rotation[2],Selection.activeGameObject.transform.rotation[3]);
                            } }
                        GUI.backgroundColor = Color.green;
                        if(GUILayout.Button("Paste Y")){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Rotation == null) { Debug.LogError("A rotation must be copied before applying it");}
                            else{
                            Selection.activeGameObject.transform.rotation = new Quaternion(Selection.activeGameObject.transform.rotation[0],Rotation[1],Selection.activeGameObject.transform.rotation[2],Selection.activeGameObject.transform.rotation[3]);
                            } }
                        GUI.backgroundColor = new Color(0.118f, 0.35f, 1f,1f);
                        if(GUILayout.Button("Paste Z")){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Scale == null) { Debug.LogError("A rotation must be copied before applying it");}
                            else{
                            Selection.activeGameObject.transform.rotation = new Quaternion(Selection.activeGameObject.transform.rotation[0],Selection.activeGameObject.transform.rotation[1],Rotation[2],Selection.activeGameObject.transform.rotation[3]);
                            } }
                        GUI.backgroundColor = Color.white;
                        GUI.contentColor = Color.cyan;
                        if(GUILayout.Button("Paste Rotation",GUILayout.MinWidth(100))){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Scale == null) { Debug.LogError("A rotation must be copied before applying it");}
                            else{
                                Selection.activeGameObject.transform.rotation = Rotation;
                            } }
                        GUILayout.Space(10);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal("Box");
                        GUILayout.Space(30);
                        GUILayout.Label("Scale     : ", GUILayout.ExpandWidth(false));
                        GUILayout.Space(10);
                        if(GUILayout.Button("Copy Scale",GUILayout.MinWidth(100))){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Scale == null) { Debug.LogError("A scale must be copied before applying it");}
                            else{
                            Scale = Selection.activeGameObject.transform.localScale;
                            } }
                        GUI.backgroundColor = Color.red;
                        GUI.contentColor = Color.white;
                        if(GUILayout.Button("Paste X")){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Scale == null) { Debug.LogError("A scale must be copied before applying it");}
                            else{
                            Selection.activeGameObject.transform.localScale = new Vector3(Scale[0],Selection.activeGameObject.transform.localScale[1],Selection.activeGameObject.transform.localScale[2]);
                            } }
                        GUI.backgroundColor = Color.green;
                        if(GUILayout.Button("Paste Y")){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Scale == null) { Debug.LogError("A scale must be copied before applying it");}
                            else{
                            Selection.activeGameObject.transform.localScale = new Vector3(Selection.activeGameObject.transform.localScale[0],Scale[1],Selection.activeGameObject.transform.localScale[2]);
                            } }
                        GUI.backgroundColor = new Color(0.118f, 0.35f, 1f,1f);
                        if(GUILayout.Button("Paste Z")){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Scale == null) { Debug.LogError("A scale must be copied before applying it");}
                            else{
                            Selection.activeGameObject.transform.localScale = new Vector3(Selection.activeGameObject.transform.localScale[0],Selection.activeGameObject.transform.localScale[1],Scale[2]);
                            } }
                        GUI.backgroundColor = Color.white;
                        GUI.contentColor = Color.cyan;
                        if(GUILayout.Button("Paste Scale",GUILayout.MinWidth(100))){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Scale == null) { Debug.LogError("A scale must be copied before applying it");}
                            else{
                            Selection.activeGameObject.transform.localScale = Scale;
                            } }
                        GUILayout.Space(10);
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);

                    GUILayout.BeginHorizontal("Box");
                        GUILayout.Space(40);
                        if(GUILayout.Button("Copy All")){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else{
                            Scale = Selection.activeGameObject.transform.localScale;
                            Rotation = Selection.activeGameObject.transform.rotation;
                            Position = Selection.activeGameObject.transform.position;
                        } }
                        GUILayout.Space(10);
                        if(GUILayout.Button("Paste All")){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Scale == null) { Debug.LogError("A scale must be copied before applying it");}
                            else if(Rotation == null) { Debug.LogError("A rotation must be copied before applying it");}
                            else if(Position == null) { Debug.LogError("A position must be copied before applying it");}
                            else{
                                Selection.activeGameObject.transform.localScale = Scale;
                                Selection.activeGameObject.transform.rotation = Rotation;
                                Selection.activeGameObject.transform.position = Position;
                        } }
                        GUILayout.Space(30);
                    GUILayout.EndHorizontal();
                    
                    GUILayout.Space(4);

                    GUILayout.BeginHorizontal("Box");
                        GUILayout.FlexibleSpace();
                        GUI.skin.label.fontSize = 14;
                        GUILayout.Label("Inversion");
                        GUI.skin.label.fontSize = 12;
                        GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.Space(4);

                    GUILayout.BeginHorizontal("Box");
                        GUILayout.Space(30);
                        GUILayout.Label("Position : ", GUILayout.ExpandWidth(false));
                        GUILayout.Space(10);
                        GUI.backgroundColor = Color.white;
                        GUI.contentColor = Color.cyan;
                        if(GUILayout.Button("Invert Position",GUILayout.MinWidth(100))){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Position == null) { Debug.LogError("A position must be copied before applying it");}
                            else{
                            Selection.activeGameObject.transform.position = new Vector3(-1*Selection.activeGameObject.transform.position[0],-1*Selection.activeGameObject.transform.position[1],-1*Selection.activeGameObject.transform.position[2]);
                            } }
                        GUI.backgroundColor = Color.red;
                        GUI.contentColor = Color.white;
                        if(GUILayout.Button("Invert X")){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Position == null) { Debug.LogError("A position must be copied before applying it");}
                            else{
                            Selection.activeGameObject.transform.position = new Vector3(-1*Selection.activeGameObject.transform.position[0],Selection.activeGameObject.transform.position[1],Selection.activeGameObject.transform.position[2]);
                            } }
                        GUI.backgroundColor = Color.green;
                        if(GUILayout.Button("Invert Y")){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Position == null) { Debug.LogError("A scale must be copied before applying it");}
                            else{
                            Selection.activeGameObject.transform.position = new Vector3(Selection.activeGameObject.transform.position[0],-1*Selection.activeGameObject.transform.position[1],Selection.activeGameObject.transform.position[2]);
                            } }
                        GUI.backgroundColor = new Color(0.118f, 0.35f, 1f,1f);
                        if(GUILayout.Button("Invert Z")){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Position == null) { Debug.LogError("A position must be copied before applying it");}
                            else{
                            Selection.activeGameObject.transform.position = new Vector3(Selection.activeGameObject.transform.position[0],Selection.activeGameObject.transform.position[1],-1*Selection.activeGameObject.transform.position[2]);
                            } }
                        GUI.backgroundColor = Color.white;
                        GUI.contentColor = Color.cyan;
                        GUILayout.Space(10);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal("Box");
                        GUILayout.Space(30);
                        GUILayout.Label("Rotation : ", GUILayout.ExpandWidth(false));
                        GUILayout.Space(10);
                        if(GUILayout.Button("Invert Rotation",GUILayout.MinWidth(100))){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Rotation == null) { Debug.LogError("A rotation must be copied before applying it");}
                            else{
                            Selection.activeGameObject.transform.rotation = new Quaternion(-1*Selection.activeGameObject.transform.rotation[0],-1*Selection.activeGameObject.transform.rotation[1],-1*Selection.activeGameObject.transform.rotation[2],Selection.activeGameObject.transform.rotation[3]);
                            } }
                        GUI.backgroundColor = Color.red;
                        GUI.contentColor = Color.white;
                        if(GUILayout.Button("Invert X")){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Rotation == null) { Debug.LogError("A rotation must be copied before applying it");}
                            else{
                            Selection.activeGameObject.transform.rotation = new Quaternion(-1*Selection.activeGameObject.transform.rotation[0],Selection.activeGameObject.transform.rotation[1],Selection.activeGameObject.transform.rotation[2],Selection.activeGameObject.transform.rotation[3]);
                            } }
                        GUI.backgroundColor = Color.green;
                        if(GUILayout.Button("Invert Y")){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Rotation == null) { Debug.LogError("A rotation must be copied before applying it");}
                            else{
                            Selection.activeGameObject.transform.rotation = new Quaternion(Selection.activeGameObject.transform.rotation[0],-1*Selection.activeGameObject.transform.rotation[1],Selection.activeGameObject.transform.rotation[2],Selection.activeGameObject.transform.rotation[3]);
                            } }
                        GUI.backgroundColor = new Color(0.118f, 0.35f, 1f,1f);
                        if(GUILayout.Button("Invert Z")){ 
                            if(Selection.activeGameObject == null) {Debug.LogError("A prop must be selected to make this action");}
                            else if(Scale == null) { Debug.LogError("A rotation must be copied before applying it");}
                            else{
                            Selection.activeGameObject.transform.rotation = new Quaternion(Selection.activeGameObject.transform.rotation[0],Selection.activeGameObject.transform.rotation[1],-1*Selection.activeGameObject.transform.rotation[2],Selection.activeGameObject.transform.rotation[3]);
                            } }
                        GUI.backgroundColor = Color.white;
                        GUI.contentColor = Color.cyan;
                        GUILayout.Space(10);
                    GUILayout.EndHorizontal();

                    GUILayout.Space(15);

                GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        
    }
}
