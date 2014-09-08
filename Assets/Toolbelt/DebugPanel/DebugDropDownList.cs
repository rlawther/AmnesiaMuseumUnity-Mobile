/* Usage:
 * Add script to an object in the game.
 * root = the transform of 
 * dropSkin = the GUISkin used to set the look of your list.
 *    The GUISkin must contain 3 Custom Styles for each level of the Hierarchy
 *      Element 0 = the settings for element with children
 *      Element 1 = the settings for element with children that has been expanded
 *      Element 2 = the settings for element with no children
*//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using icExtensionMethods;

namespace Toolbelt {
public class DebugDropDownList : DebugTabMenu
{
    private Rect DropDownRect;          // Size and Location for drop down
    private Rect canvas;
    private Transform currentRoot;      // selected object transform
    private Vector2 ListScrollPos;      // scroll list position
    private string selectedItemCaption;  // name of selected item
    private string lastCaption;         // last selected item
    private int guiWidth;               // width of drop list
    private int guiHeight;               // height of drop list
    private bool textChanged;           // if text in text box has changed look for item
    private bool clearDropList;         // clear text box
    private bool DropdownVisible = true;        // show drop down list    
 
    public Transform root;              // top of the Hierarchy
    public GUISkin dropSkin;            // GUISkin for drop down list    
	public Transform selectedTransform; // when they click on the text of the item that they select.
    private bool targetChange;           // text in text box was changed, update list
 	
	public bool visible; //show/hide this part of the gui.
	
	private const int redText = 3;
    private class GuiListItem        //The class that contains our list items 
    {
        public string Name;         // name of the item
        
        public int GuiStyle {
        	get { return this.showChildren ? OpenedStyle : UnOpenedStyle; }
        }        // current style to use
        
        public int UnOpenedStyle; // unselected GUI style
        public int OpenedStyle;   // selected GUI style		
        public int Depth;           // depth in the Hierarchy
		public Transform transform; // the actual transform
        public bool Selected;       // if the item is selected		
        public bool showChildren; // show child objects in list
 		
	
        public GuiListItem(bool mSelected, string mName,int depth, Transform t)
        {
            this.Selected = mSelected;
            this.Name = mName;            
            this.showChildren = true;
            this.Depth = depth;
			this.transform = t;
			this.Selected = false;
            this.UnOpenedStyle = 0;
            this.OpenedStyle = 0;
        }
		        
 
        // Accessors
        public void enable()// don't show in list
        {
            Selected = true;
        }
        public void disable()// show in list
        {
            Selected = false;
        }
        
    }
 
    //Declare our list of stuff 
    private List<GuiListItem> MyListOfStuff; 
 
    // Initialization
    public DebugDropDownList(Rect canvas, Transform root)
    {
		this.canvas = canvas;
        // Manually position our list, because the dropdown will appear over other controls 
		
        DropDownRect = new Rect(canvas.xMin, canvas.yMin, canvas.width, 30);                
        lastCaption = selectedItemCaption = "Select a Part...";
		this.root = root;
 		
		this.refreshDropList(); 
		this.visible = true;
    }

    public override void DrawGUI()
    {
		this.UpdateTextBox();
		if (!this.visible) {
			return;
		}
		
		
		//***********************************************************************
		// Dropdown box, with searcher button, clear button and refresh button  *
		//***********************************************************************
		GUILayout.BeginArea(DropDownRect, "", "box");
		GUILayout.BeginHorizontal();
		string ButtonText = (DropdownVisible) ? "<<" : ">>";
		DropdownVisible = GUILayout.Toggle(DropdownVisible, ButtonText, "button", GUILayout.Width(32), GUILayout.Height(20));
		GUI.SetNextControlName("PartSelect");
		selectedItemCaption = GUILayout.TextField(selectedItemCaption);
		clearDropList = GUILayout.Toggle(clearDropList, "Clear", "button", GUILayout.Width(40), GUILayout.Height(20));
		bool shouldRefresh = GUILayout.Button("Refresh", "button", GUILayout.Width(40), GUILayout.Height(20));
		
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		
		//***********************************************************************
		// Tree list with all items                                             *
		//***********************************************************************
        //Show the dropdown list if required (make sure any controls that should appear behind the list are before this block) 
        if (DropdownVisible)
        {
            GUI.SetNextControlName("ScrollView");
            GUILayout.BeginArea(new Rect(this.canvas.xMin, 
			                             this.canvas.yMin + DropDownRect.height,
			                             this.canvas.width,
			                             this.canvas.height - (DropDownRect.height)),
			                    GUI.skin.box);
            ListScrollPos = GUILayout.BeginScrollView(ListScrollPos, dropSkin.scrollView);
            GUILayout.BeginVertical();
 
            for (int i = 0; i < MyListOfStuff.Count; i++)
            {
				GuiListItem gli = MyListOfStuff[i];
				
				
				if (gli.Selected) {
					
					GUILayout.BeginHorizontal(GUILayout.Height(15));
					GUILayout.Space (15*gli.Depth);
	                if (GUILayout.Button(" ", dropSkin.customStyles[gli.GuiStyle])) //if they press
	                {
	                    HandleSelectedButton(i);
	                }
					
					if (gli.transform == this.selectedTransform) { //whether to make it red.
						GUILayout.Button(gli.Name, dropSkin.customStyles[DebugDropDownList.redText]); //button that does nothing.
					} else {
						if (GUILayout.Button(gli.Name, dropSkin.button)) //if they press the name
						{
							HandleSelectedText(i);
						}
					}
					
					
					GUILayout.EndHorizontal();
				}
				
				
				
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
        
        
		
		if (shouldRefresh) {
			this.refreshDropList ();
		}
		
    }
	
	void refreshDropList() {
		MyListOfStuff = new List<GuiListItem>(); //Initialize our list of stuff 
        // fill the list
        BuildList(root, 0);
        // set GUI for each item in list
        SetupGUISetting();
        // fill the list
        FillListRoot(root);
        FillList(root);
	}
 
    void UpdateTextBox()
    {
		
        //check if text box info changed
        if (selectedItemCaption != lastCaption)
        {
            textChanged = true;
        }
 
        // if text box info changed look for part matching text
        if (textChanged)
        {
			if (selectedItemCaption == "") //just became empty string. display all the things.
			{
			}
			else
			{
	            lastCaption = selectedItemCaption;
	            textChanged = false;
	            // go though list to find item
				foreach(GuiListItem gli in MyListOfStuff)             
	            {				
	                if (gli.Name.StartsWith(selectedItemCaption, System.StringComparison.CurrentCultureIgnoreCase))
	                {	 
	                    gli.enable();
	                    gli.showChildren = true;	                    
	                }
	                else
	                {
	                    gli.disable();
	                    gli.showChildren = false;	                    
	                }
	            }
			}
        }
        // reset message if list closed and text box is empty
        if (selectedItemCaption == "" && !DropdownVisible)
        {
            lastCaption = selectedItemCaption = "Select a Part...";
            ClearList(root);
            FillList(root);
        }
 
        // if Clear button pushed
        if (clearDropList)
        {
            clearDropList = false;
            selectedItemCaption = "";
			this.selectedTransform = null;
        }
    }
 
 	public void HandleSelectedText(int selection) {
		this.selectedTransform = MyListOfStuff[selection].transform;
	}
	
    public void HandleSelectedButton(int selection)
    {		
        // do the stuff, camera etc        
        GuiListItem selected = MyListOfStuff[selection];
		selectedItemCaption = selected.Name;
		lastCaption = selectedItemCaption; //to prevent from refreshing stuff..
        
		currentRoot = selected.transform;
 		
		
        // toggle item show child
        selected.showChildren = !selected.showChildren;
         
        // fill my drop down list with the children of the current selected object
        if (selected.showChildren) {
			FillList (currentRoot);
        } else {
            ClearList(currentRoot);
        }
        
    }
 	
 	public void FillListRoot(Transform root) {
		GuiListItem glia = this.getGuiListItem(root);
		if (glia != null) {
			glia.enable ();
			glia.showChildren = true;		
		}
 	}
 	
    // show only items that are the root and its children
    public void FillList(Transform root)
    {

        foreach (Transform child in root)
        {
			GuiListItem gli = this.getGuiListItem (child);
			if (gli != null) {
	            gli.enable();
	            gli.showChildren = false;	            
            }
        }
    }
	
	// turn off children objects
    public void ClearList(Transform root)
    {
        Transform[] childs = root.GetComponentsInChildren<Transform>();
        foreach (Transform child in childs)
        {
			if (child == root) continue; //don't run clearlist on the root
			GuiListItem gli = this.getGuiListItem(child);
			
			gli.disable();
			gli.showChildren = false;			
        }
    }
	
	private GuiListItem getGuiListItem(Transform t) 
	{
		foreach (GuiListItem gli in this.MyListOfStuff)
		{
			if (gli.transform == t) {
				return gli;
			}
		}		
		return null;
	}
		

 
    // recursively build the list so the hierarchy is intact
	// depth is the depth of root
    void BuildList(Transform root, int depth)
    {
		MyListOfStuff.Add(new GuiListItem(false,root.name,depth,root));
        // for add all the childrenz
        foreach (Transform child in root)
        {
            BuildList(child,depth+1);           
        }
    }
 
    public void ResetDropDownList()
    {
        selectedItemCaption = "";
        ClearList(root);
        FillList(root);
    }

    // sets the drop list elements to use the correct GUI skin custom style
    private void SetupGUISetting()
    {        
        // check all the parts for hierarchy depth
        foreach (GuiListItem gli in MyListOfStuff)
        {
            Transform currentTransform = gli.transform; 
 
            if (currentTransform.childCount > 0)
            {                
                gli.UnOpenedStyle = 0; //Has children but is closed.
                gli.OpenedStyle = 1; //Has children and is open.				
            }
            else
            {             
                gli.UnOpenedStyle = 2; //Will look the same regardless of whether it is selected or not.
                gli.OpenedStyle = 2;				
            }
 
        }
    }
	
	
}
}