using UnityEngine;
using System.Collections;

namespace Toolbelt {
public class ItemMenu : DebugTabMenu {

	private DebugDropDownList ddl;
	private DebugItemViewer iv;
	
	public ItemMenu(Rect canvas, Transform root, GUISkin skin) {
		this.ddl = new DebugDropDownList(new Rect(canvas.xMin,canvas.yMin,canvas.width/2,canvas.height),root);
		this.ddl.dropSkin = skin;
		this.iv = new DebugItemViewer(new Rect(canvas.xMin+(canvas.width/2),canvas.yMin,canvas.width/2,canvas.height));
		
	}
	
	public void setRoot(Transform root) {
		this.ddl.root = root;
		this.iv.setToView(null);
	}
	public override void DrawGUI ()
	{
		this.ddl.DrawGUI();
		if (ddl.selectedTransform != this.iv.getSelectedTransform()) {
			this.iv.setToView(ddl.selectedTransform);
		}
		this.iv.DrawGUI();
	}
}

}