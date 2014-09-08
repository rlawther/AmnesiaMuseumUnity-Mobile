using UnityEngine;
using System.Collections;
using System;

namespace Toolbelt {
public class DebugPanel : MonoBehaviour {
	
	private ItemMenu itemMenu; //1
	private DebugGlobalsMenu globalsViewer; //2
	//private DebugTabMenu configViewer; 3
	private SaveMenu saveMenu; //4
		
	private Transform SceneRoot;
	public GUISkin dropSkin;
	public bool guiEnabled = false;
		
	private GetDebugCamera getDebugCamera;
	private GameObject optionalGlobalsGUI; //select a gameobject with things to display in Globals GUI tab.
	
	
	private int width = 0;
	private int height = 0;
	
	private const int TAB_HEIGHT = 30;
	private int TOP_OFFSET = 0;
	private int LEFT_OFFSET = 0;
	
	public enum SelectedTab
	{
		Items,
		Globals,
		Config,
		Save
	};

	private readonly string[] TAB_NAMES = Enum.GetNames(typeof(SelectedTab));
	private DebugTabMenu[] debugTabs = new DebugTabMenu[Enum.GetNames(typeof(SelectedTab)).Length];
	public SelectedTab selectedTab = SelectedTab.Items;

	// Use this for initialization
	void Start () {
		this.getDebugCamera = ToolbeltManager.FirstInstance.sceneCamera.GetComponent<GetDebugCamera>();
		this.optionalGlobalsGUI = ToolbeltManager.FirstInstance.sceneCamera.gameObject;
		this.SceneRoot = ToolbeltManager.FirstInstance.transform;
		
		
		this.itemMenu = new ItemMenu(new Rect(LEFT_OFFSET, TAB_HEIGHT+TOP_OFFSET, this.width,this.height - TAB_HEIGHT), this.SceneRoot, this.dropSkin);
		//this.dropDownList = new DebugDropDownList(new Rect (0, TAB_HEIGHT,this.width/2,this.height-TAB_HEIGHT), this.SceneRoot);
		//this.dropDownList.dropSkin = this.dropSkin;
		//this.itemViewer = new DebugItemViewer(new Rect(this.width/2, TAB_HEIGHT,this.width/2,this.height-TAB_HEIGHT));
		
		this.globalsViewer = new DebugGlobalsMenu(new Rect(LEFT_OFFSET, TAB_HEIGHT+TOP_OFFSET,this.width/2,this.height-TAB_HEIGHT), this.getDebugCamera, this.optionalGlobalsGUI);
		this.saveMenu = new SaveMenu(new Rect(LEFT_OFFSET,TAB_HEIGHT+TOP_OFFSET,this.width/2,this.height-TAB_HEIGHT),this.SceneRoot.gameObject,this.RefreshRootObject);
		
		this.debugTabs[0] = this.itemMenu;
		this.debugTabs[1] = this.globalsViewer;
		this.debugTabs[2] = new DebugTabMenu(); // STUB
		this.debugTabs[3] = this.saveMenu;
		
		
		
	}
	
	//pass this callback onto SaveMenu...
	private void RefreshRootObject(GameObject newRoot) {		
		this.SceneRoot = newRoot.transform;
		this.itemMenu.setRoot(newRoot.transform);
		this.saveMenu.toSave = newRoot;
	}
	
	void Awake() {
		if (Application.isEditor) {
			this.width = Screen.width;
			this.height = Screen.height;
			this.TOP_OFFSET = 0;
			this.LEFT_OFFSET = 0;
		} else {
			this.TOP_OFFSET = 30;
			this.LEFT_OFFSET = 30;
			this.width = 1400 - LEFT_OFFSET; //avie single projector width
			this.height = 1050 - TOP_OFFSET; // avie single projector height
			
		}
	}
	
	// Update is called once per frame
	void Update () {
		this.checkHotkey();	
	}

	void DrawTabs() {
		GUILayout.BeginArea(new Rect(LEFT_OFFSET,TOP_OFFSET, this.width, TAB_HEIGHT), "",GUI.skin.box);
		GUILayout.BeginHorizontal(GUILayout.Height(20));
		SelectedTab oldSelectedTab = this.selectedTab;
		this.selectedTab = (SelectedTab) GUILayout.Toolbar((int)this.selectedTab,TAB_NAMES );
		
		if (oldSelectedTab != this.selectedTab)
			this.OnChangeTab(oldSelectedTab, this.selectedTab);
		
		
		GUILayout.EndHorizontal();
		GUILayout.EndArea ();
	}

	void OnChangeTab(SelectedTab old, SelectedTab current) {
		this.debugTabs[(int)old].OnUnselect();
		this.debugTabs[(int)current].OnSelect();
	}
	
	private void checkHotkey() {
		if ((Input.GetKey(KeyCode.LeftControl) &&Input.GetKeyDown(KeyCode.D))) {
			this.guiEnabled = !this.guiEnabled;
		}
	}


	void OnGUI() {

		if (this.guiEnabled) {
			GUILayout.BeginArea(new Rect(0f,0f,this.width, this.height),"","");
				
			this.DrawTabs();
			DebugTabMenu currentTab = this.debugTabs[(int)this.selectedTab];
			if (currentTab != null ) {			
				currentTab.DrawGUI();
			}
			/*
			// Items tab
			if (this.dropDownList != null && this.selectedTab == SelectedTab.Items) {
				this.dropDownList.DrawGUI();
				if (this.itemViewer != null) {
					if (this.dropDownList.selectedTransform != this.itemViewer.getSelectedTransform()) {
						this.itemViewer.setToView(this.dropDownList.selectedTransform);
					}
					this.itemViewer.DrawGUI();
				}
			}

			if (this.globalsViewer != null && this.selectedTab == SelectedTab.Globals) {
				this.globalsViewer.DrawGUI();
			}
			if (this.saveMenu != null && this.selectedTab == SelectedTab.Save) {
				this.saveMenu.DrawGUI();
			}
			*/

			GUILayout.EndArea();

		}
	}
}
}