using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System;
using _ = ru.appforge.utils.U;

public class AndroidLocalizator : EditorWindow {

	static string path = "/Plugins/Android/res/values-{0}/strings.xml";
	static string dirPath = "/Plugins/Android/res/";
	static Dictionary<string, string> locals;
	static Dictionary<string, List<XmlDocument>> docs = new Dictionary<string, List<XmlDocument>>();
	static Dictionary<XmlDocument, string> docsDirs = new Dictionary<XmlDocument, string>();
	static Dictionary<string, string> names = new Dictionary<string, string>();
	static Dictionary<string, string[]> langs = new Dictionary<string, string[]>();
	static string[] tdirs;
	static bool isInited = false;

	Vector2 scrollPosition = Vector2.zero;
	float currentScrollViewHeight;
	bool resize = false;
	Rect cursorChangeRect;
	
	[MenuItem ("Android Localizator/Localizator")]
	public static void  ShowWindow () {
		EditorWindow.GetWindow(typeof(AndroidLocalizator));

		if (!isInited) {
			InitLocals();

			try {
				foreach (var kvp in locals) {
					var xds = new List<XmlDocument>();
//					Debug.LogError(_.ToString(Directory.GetDirectories(Application.dataPath + dirPath, "*-"+kvp.Value+"*")));
					tdirs = Directory.GetDirectories(Application.dataPath + dirPath, "*-"+kvp.Value+"*");

					langs.AddOrReplace(kvp.Value, tdirs);

					foreach (var lang in tdirs) {
						var xd = new XmlDocument();
						var path = lang + "/strings.xml";
						if (File.Exists(path)) {
							xd.Load(path);
							xds.Add(xd);
							docsDirs.Add(xd, path);
						} else {

						}
					}

					docs.AddOrReplace(kvp.Value, xds);

					var xl = xds[0].SelectNodes("/resources/string[@name='app_name']");
					if (xl.Count > 0) {
						names.AddOrReplace(kvp.Value, xl[0].InnerText);
					} else {
						names.AddOrReplace(kvp.Value, string.Empty);
					}
				}
				isInited = true;
			} catch (Exception ex) {
				Debug.LogException(ex);
			}
		}
	}

	void OnEnable() {
		this.position = new Rect(200,200,400,300);
		currentScrollViewHeight = this.position.height;
		//cursorChangeRect = new Rect(0,currentScrollViewHeight,this.position.width,5f);
	}
	
	void OnGUI () {
		if (!isInited) return;
		// The actual window code goes here
		EditorGUILayout.BeginVertical();
		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(currentScrollViewHeight));
		foreach (var kvp in locals) {
			names[kvp.Value] = EditorGUILayout.TextField (kvp.Key, names[kvp.Value]);
		}

		if(GUILayout.Button("Localize!"))
		{
			Localize();
		}
		EditorGUILayout.EndScrollView();

		ResizeScrollView();
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndVertical();
		Repaint();
	}

	void Localize ()
	{
		foreach (var kvp in docs) {
			foreach (var doc in kvp.Value) {
//				Debug.LogError(kvp.Key +"   "+doc);

//				if (names[kvp.Key].Length > 0) {
					XmlNodeList nms = doc.SelectNodes("/resources/string[@name='app_name']");
					if (nms.Count > 0) {
						nms[0].InnerText = names[kvp.Key];
					} else if (nms.Count == 0) {
						XmlElement newe = doc.CreateElement("string");
						newe.SetAttribute("name","app_name");
						newe.InnerText = names[names[kvp.Key].Length > 0 ? kvp.Key : "en"];
						doc.DocumentElement.AppendChild(newe);
					}
//				} else {
//					XmlNodeList toRem = doc.SelectNodes("/resources/string[@name='app_name']");
//					if (toRem.Count > 0) {
//						doc.DocumentElement.RemoveChild(toRem[0]);
//					}
//				}
				doc.Save(docsDirs[doc]);
			}
		}

		foreach (var lang in tdirs) {
			var path = lang + "/strings.xml";
			if (!File.Exists(path)) {
				try {
					var xd = new XmlDocument();
					var xnr = xd.CreateXmlDeclaration("1.0", "UTF-8", "yes");
					xd.AppendChild(xnr);

					var xns = xd.CreateElement("resources");
					xns.SetAttribute("xmlns:android", "http://schemas.android.com/apk/res/android");
					xd.AppendChild(xns);

					var xnl = xd.CreateElement("string");
					xnl.SetAttribute("name","app_name");
					xnl.InnerText = names["en"];
					xd.DocumentElement.AppendChild(xnl);
					xd.Save(path);
					Debug.LogFormat("Created new strings.xml at: {0}", path);
				} catch (Exception e) {
					Debug.LogException(e);
				}
			}
		}
	}

	private void ResizeScrollView(){
		GUI.DrawTexture(cursorChangeRect,EditorGUIUtility.whiteTexture);
		EditorGUIUtility.AddCursorRect(cursorChangeRect,MouseCursor.ResizeVertical);
		
		if( Event.current.type == EventType.mouseDown && cursorChangeRect.Contains(Event.current.mousePosition)){
			resize = true;
		}
		if(resize){
			currentScrollViewHeight = Event.current.mousePosition.y;
			cursorChangeRect.Set(cursorChangeRect.x,currentScrollViewHeight,cursorChangeRect.width,cursorChangeRect.height);
		}
		if(Event.current.type == EventType.MouseUp)
			resize = false;        
	}
	
	static void InitLocals ()
	{
		locals = new Dictionary<string, string>() {
			{"English", "en"},
			{"Russian", "ru"},
			{"Arabic", "ar"},
			{"Azerbaijani", "az"},
			{"Bulgarian", "bg"},
			{"Bengali", "bn"},
			{"Catalan", "ca"},
			{"Czech", "cs"},
			{"Danish", "da"},
			{"German", "de"},
			{"Greek", "el"},
			{"Spanish", "es"},
			{"Estonian", "et"},
			{"Basque", "eu"},
			{"Persian(Farsi)", "fa"},
			{"Finnish", "fi"},
			{"French", "fr"},
			{"Irish", "ga"},
			{"Galician", "gl"},
			{"Gujarati", "gu"},
			{"Hausa", "ha"},
			{"Hebrew", "he"},
			{"Hindi", "hi"},
			{"Craotian", "hr"},
			{"Hungarian", "hu"},
			{"Armenian", "hy"},
			{"Indonesian", "id"},
			{"Igbo", "ig"},
			{"Icelandic", "is"},
			{"Italian", "it"},
			{"Japanese", "ja"},
			{"Georgian", "ka"},
			{"Kazakh", "kk"},
			{"Kannada", "kn"},
			{"Korean", "ko"},
			{"Lithuanian", "lt"},
			{"Latvian", "lv"},
			{"Macedonian", "mk"},
			{"Malayalam", "ml"},
			{"Marathi", "mr"},
			{"Malay", "ms"},
			{"Norwegian Bokmal", "nb"},
			{"Dutch", "nl"},
			{"Panjabi", "pa"},
			{"Polish", "pl"},
			{"Portuguese", "pt"},
			{"Romanian", "ro"},
			{"Saraiki", "sk"},
			{"Slovene", "sl"},
			{"Serbian", "sr"},
			{"Swedish", "sv"},
			{"Tamil", "ta"},
			{"Telugu", "te"},
			{"Thai", "th"},
			{"Turkish", "tr"},
			{"Ukrainian", "uk"},
			{"Vietnamese", "vi"},
			{"Yoruba", "yo"},
			{"Chinese", "zh"}
		};
	}
}
