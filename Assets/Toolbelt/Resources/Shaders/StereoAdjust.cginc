
inline float2 stereoAdjust(float2 textureCoord, int stereoType, bool leftEye) 
{
    if (stereoType == 0) {
        // do nothing. No stereo mode.
    } else if (stereoType == 1){
        // Left Eye Left half , Right Eye Right half 
        if(leftEye){
            textureCoord.x *= 0.5; 
        } else {			
            textureCoord.x = textureCoord.x * 0.5 + 0.5; 
        }
    } else if(stereoType == 2  ){
        // Left Eye Top half , Right Eye Bottom half
        if(leftEye){
            textureCoord.y *= 0.5; 	
        } else {
            textureCoord.y = textureCoord.y * 0.5 + 0.5;
        }
    } else if(stereoType == 3 ){
        // Left Eye Right half , Right Eye Left half */
        if(leftEye){
            textureCoord.x = textureCoord.x * 0.5 + 0.5; 
        } else {			
            textureCoord.x *= 0.5; 
        }
    }
    else if (stereoType == 4)	// Left Eye Bottom, Right Eye Top
    {
        if(leftEye){
            textureCoord.y = textureCoord.y * 0.5 + 0.5;
        } else {
            textureCoord.y *= 0.5; 	
        }
    } 
    
    return textureCoord;
} 