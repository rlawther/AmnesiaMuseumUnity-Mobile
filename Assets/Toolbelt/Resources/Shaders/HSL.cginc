inline float3 RGB_to_HSL (float3 colour) 
{ 
    float  r, g, b, delta; 
    float  colorMax, colorMin; 
    float  h=0, s=0, l=0; 
    float3 hsl; 
     
    r = colour[0]; 
    g = colour[1]; 
    b = colour[2]; 
 
    colorMax = max (r,g); 
    colorMax = max (colorMax,b); 
   
    colorMin = min (r,g); 
    colorMin = min (colorMin,b);
    
    float C = colorMax - colorMin;
 
    l = 0.5 * (colorMax + colorMin);	// lightness
 
    if (colorMax != 0) {
        s = C / (1.0 - abs(2.0*l - 1.0));
    }
 
    if (C != 0) // if not achromatic 
    { 
        if (r == colorMax) 
            h = (g-b) / C;
        else if (g == colorMax) 
            h = 2.0 + (b-r) / C;
        else // b is max 
            h = 4.0 + (r-g) / C;
 
        h *= 60; 
 
        if(h < 0) 
            h += 360; 
     
        hsl[0] = h / 360.0;    // moving h to be between 0 and 1.
        hsl[1] = s; 
        hsl[2] = l; 
    }
    else  // achromatic (no colour or hue. grey)
    {
    	hsl[0] = 0;
        hsl[1] = 0; 
        hsl[2] = r; // r = g = b , so just use r as the value
    }
  
    return hsl; 
}

inline float3 HSL_to_RGB (float3 hsl) 
{
    // Using formala from http://en.wikipedia.org/wiki/HSL_and_HSV
    float3 color; 
    float  r = 0, g = 0, b = 0; 
 
    if (hsl[1] == 0) 
    { 
        if (hsl[2] != 0) // achromatic (no colour or hue. grey)
            color = float3(hsl[2], hsl[2], hsl[2]);
    } 
    else 
    { 
    	float C = (1.0 - abs(2.0 * hsl[2] - 1.0)) * hsl[1]; //hsv[2] * hsv[1];
    	float H = hsl[0] * 360.0/60.0;
    	float X = C * (1.0 - abs( fmod( H, 2.0) - 1 ) );
    	
    	int floorH = int(floor(H));
    	
        if (floorH == 0) 
        { 
            r = C; 
            g = X; 
            b = 0; 
        } 
        else if (floorH == 1) 
        { 
            r = X;  
            g = C;  
            b = 0; 
        } 
        else if (floorH == 2) 
        { 
            r = 0; 
            g = C; 
            b = X; 
        } 
        else if (floorH == 3) 
        { 
            r = 0; 
            g = X; 
            b = C; 
        } 
        else if (floorH == 4) 
        { 
            r = X; 
            g = 0; 
            b = C; 
        } 
        else if (floorH == 5) 
        { 
            r = C; 
            g = 0; 
            b = X; 
        } 
         
        float m = hsl[2] - 0.5 * C;
        color = float3(r + m, g + m, b + m);
    } 
    return color; 
}

inline float3 doHSLCorrection(float3 colour, float3 adjustment) 
{
	float3 hsl = RGB_to_HSL(colour);
	hsl.r = fmod(hsl.r + adjustment.r, 1.0);// Hue adjustment is additive (i.e rotating around the hue circle in range [0,1])
	hsl.gb *= adjustment.gb;// Saturation, Value are multiplicative
	hsl = HSL_to_RGB(hsl);
	return hsl;
} 
