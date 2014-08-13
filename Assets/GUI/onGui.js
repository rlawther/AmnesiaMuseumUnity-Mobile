#pragma strict

 var buttonStyleA : GUIStyle;
 var buttonStyleB : GUIStyle;
 
function OnGUI()
{

     var w = Screen.width;
    var h = Screen.height;
    
    // Draw a button with text and an image in the center of the screen.
    //if ( GUI.Button( Rect( 20,20, 100,100 ), "Sequence A", buttonStyle) )
   /* if ( GUILayout.Button(  "A",buttonStyleA) )
	    {
	        // Print some text to the debug console
	        Debug.Log("Thanks!");
    }
    
    if ( GUILayout.Button( Rect( "B",buttonStyleB) )
    {
        // Print some text to the debug console
        Debug.Log("Thanks!");
    }
    */
    
    GUI.Button (Rect (10,10,56,56), "A", buttonStyleA);
    
    GUI.Button (Rect (w-10-56,10,56,56), "B", buttonStyleB);
}
