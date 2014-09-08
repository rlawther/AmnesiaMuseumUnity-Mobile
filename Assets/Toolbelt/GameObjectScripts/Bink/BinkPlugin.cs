//BinkPlugin.cs - author: Aaron Pfeiffer - copyright 2014 RAD Game Tools

//Whether to manually tick bink. 
#define BINK_QUEUE_TICK_MANUALLY

//Unity defines are not reliable, and don't work properly in the editor (IE you can't tell whether you're in the editor on windows vs. mac), so we're gonna
//make our own defines
//#define UNITY_MAC
#define UNITY_WIN

//C# Unity script to allow Bink rendering
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using BinkInterface;
using System.Diagnostics;
using System.Threading;
using Toolbelt;

[DontStore]
public class BinkPlugin : MonoBehaviour
{
	private enum BinkPluginType
	{
		//Overlay draws after all other Unity rendering. It's best for FMV video, though you could do FMV video using a Texture too
		Overlay,
	
		//Puts bink movie on this object's texture. YUV->RGB conversion done on the CPU. Slower than GPUTexture, but works on all hardware. 
		CPUTexture,
	
		//Puts bink movie on this object's texture. YUV->RGB conversion done on the GPU. Faster than CPUTexture. Currently only supported on OpenGL. 
		GPUTexture,
	};

	private enum BinkOverlayType
	{
		//Plays at upper-left corner of screen
		UpperLeft,
		
		//Plays at original movie resolution, centered on the screen.
		Centered,
	
		//Fits to the screen. If movie aspect doesn't match screen aspect, movie will overlap outside the screen and movie edges will be clipped off.
		FitToScreen,
	
		//Fits to the screen. If movie aspect doesn't match screen aspect, movie will play inside the screen. 
		FitToScreenBounds,
	};
	
	private BinkPluginType	Type = BinkPluginType.GPUTexture;
	private BinkOverlayType	OverlayType = BinkOverlayType.Centered;
	private bool Alpha = false;

	//True if we want alpha (above) and the movie actually has an alpha plane (determined at load time)
	private bool UseAlpha = false;
	public bool isGameSpeedAgnostic = false;
#if USE_BINK2_x64
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK2
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK1_x64
    [DllImport("bink_unity_plugin")]
#else //USE_BINK1
	[DllImport("bink_unity_plugin")]
	#endif
	//extern "C" EXPORT_API bool BinkVersionsMatch(char * binkVersion, char * minimumBinkVersion, int stringBufferSize);
  	private static extern bool BinkVersionsMatch (StringBuilder binkVersion, StringBuilder minimumBinkVersion, int stringBufferSize);

#if USE_BINK2_x64
	[DllImport("bink2_unity_plugin", CharSet = CharSet.Ansi)]
#elif USE_BINK2
	[DllImport("bink2_unity_plugin", CharSet = CharSet.Ansi)]
#elif USE_BINK1_x64
	[DllImport("bink_unity_plugin", CharSet = CharSet.Ansi)]
#else //USE_BINK1
	[DllImport("bink_unity_plugin", CharSet = CharSet.Ansi)]
	#endif
	//extern "C" PLUGIN_EXPORT_API void uTmEnter(char * str)
	private static extern void uTmEnter ([MarshalAs(UnmanagedType.LPStr)]string str);
	
#if USE_BINK2_x64
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK2
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK1_x64
    [DllImport("bink_unity_plugin")]
#else //USE_BINK1
	[DllImport("bink_unity_plugin")]
	#endif
	//extern "C" PLUGIN_EXPORT_API void uTmLeave()
	private static extern void uTmLeave ();

#if USE_BINK2_x64
	[DllImport("bink2_unity_plugin")]
#elif USE_BINK2
	[DllImport("bink2_unity_plugin")]
#elif USE_BINK1_x64
	[DllImport("bink_unity_plugin")]
#else //USE_BINK1
	[DllImport("bink_unity_plugin")]
	#endif
	//extern "C" PLUGIN_EXPORT_API bool BinkRenderIsTextureSetReady(void * _texture_set)
	private static extern bool BinkRenderIsTextureSetReady (IntPtr texture_set);
	
#if USE_BINK2_x64
	[DllImport("bink2_unity_plugin")]
#elif USE_BINK2
	[DllImport("bink2_unity_plugin")]
#elif USE_BINK1_x64
	[DllImport("bink_unity_plugin")]
#else //USE_BINK1
	[DllImport("bink_unity_plugin")]
	#endif
	//extern "C" PLUGIN_EXPORT_API bool BinkRenderOkToRender()
	private static extern bool BinkRenderOkToRender ();

	//Call once before rendering each bink movie
#if USE_BINK2_x64
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK2
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK1_x64
    [DllImport("bink_unity_plugin")]
#else //USE_BINK1
	[DllImport("bink_unity_plugin")]
	#endif
	//extern "C" void PLUGIN_EXPORT_API BinkRenderRespondToReset(HBINK bink, void * YTexturePtr, void * cBTexturePtr, void * cRTexturePtr, void * ATexturePtr)
	public static extern void BinkRenderRespondToReset (IntPtr bink, IntPtr texture_set, IntPtr YTexturePtr, IntPtr cBTexturePtr, IntPtr cRTexturePtr, IntPtr ATexturePtr);

	//Call once before rendering each bink movie
#if USE_BINK2_x64
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK2
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK1_x64
    [DllImport("bink_unity_plugin")]
#else //USE_BINK1
	[DllImport("bink_unity_plugin")]
	#endif
	//extern "C" void PLUGIN_EXPORT_API BinkRenderMarkReset(void * texture_set, bool reset)
	public static extern void BinkRenderMarkReset (IntPtr texture_set, bool reset);

	//maxSimultaneousMovies is the maximum number of movies (including movies on a texture) you want to handle playing *simultaneously*. If you only every play one movie at a time, this can be set to 1. 
#if USE_BINK2_x64
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK2
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK1_x64
    [DllImport("bink_unity_plugin")]
#else //USE_BINK1
	[DllImport("bink_unity_plugin")]
	#endif
    //extern "C" EXPORT_API bool BinkRenderQueueInitalize(int maxSimultaneousMovies, int numthreads)
	public static extern bool BinkRenderQueueInitialize (int maxSimultaneousMovies, int numthreads);

	//Create the movie-independent rendering shaders bink uses. 
#if USE_BINK2_x64
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK2
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK1_x64
    [DllImport("bink_unity_plugin")]
#else //USE_BINK1
	[DllImport("bink_unity_plugin")]
	#endif
    //extern "C" EXPORT_API bool BinkRenderQueueCreateShaders()
  	public static extern bool BinkRenderQueueCreateShaders ();

	//Destroy the movie-independent rendering shaders bink uses. 
#if USE_BINK2_x64
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK2
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK1_x64
    [DllImport("bink_unity_plugin")]
#else //USE_BINK1
	[DllImport("bink_unity_plugin")]
	#endif
    //extern "C" PLUGIN_EXPORT_API void BinkRenderQueueDestroyShaders()
  	public static extern void BinkRenderQueueDestroyShaders ();

	//Bink may pad textures internally for performance. Use this to figure out those sizes. 
#if USE_BINK2_x64
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK2
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK1_x64
    [DllImport("bink_unity_plugin")]
#else //USE_BINK1
	[DllImport("bink_unity_plugin")]
	#endif
	//extern "C" PLUGIN_EXPORT_API void BinkGetFrameBufferDimensions(HBINK Bink, unsigned int * ya_texture_width, unsigned int * ya_texture_height, unsigned int * crcb_texture_width, unsigned int * crcb_texture_height )
	public static extern void BinkGetFrameBufferDimensions (IntPtr Bink, ref uint ya_texture_width, ref uint ya_texture_height, ref uint crcb_texture_width, ref uint crcb_texture_height);

	//Checks to see if the texture set got invalidated and needs to be reset
#if USE_BINK2_x64
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK2
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK1_x64
    [DllImport("bink_unity_plugin")]
#else //USE_BINK1
	[DllImport("bink_unity_plugin")]
	#endif
    //extern "C" PLUGIN_EXPORT_API bool BinkRenderMovieHasAlpha(HBINK Bink)
	public static extern bool BinkRenderMovieHasAlpha (IntPtr Bink);
	
	//Creates a texture_set for the bink movie. 
	//Important: is *up to you* to destroy your texture set after you're done playing your movie, with BinkRenderQueueDestroyTextureSet( ). Failure to do so will leak memory. 
#if USE_BINK2_x64
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK2
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK1_x64
    [DllImport("bink_unity_plugin")]
#else //USE_BINK1
	[DllImport("bink_unity_plugin")]
	#endif
    //extern "C" PLUGIN_EXPORT_API void* BinkRenderQueueCreateTextureSet(HBINK Bink, void * YTexturePtr, void * cBTexturePtr, void * cRTexturePtr, void * ATexturePtr)
	public static extern IntPtr BinkRenderQueueCreateTextureSet (IntPtr Bink, IntPtr YTexturePtr, IntPtr cBTexturePtr, IntPtr cRTexturePtr, IntPtr ATexturePtr);
	
	//Destroys a texture_set for the bink movie. 
#if USE_BINK2_x64
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK2
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK1_x64
    [DllImport("bink_unity_plugin")]
#else //USE_BINK1
	[DllImport("bink_unity_plugin")]
	#endif
    //extern "C" PLUGIN_EXPORT_API void BinkRenderQueueDestroyTextureSet(void * texture_set, bool external_textures)
	public static extern void BinkRenderQueueDestroyTextureSet (IntPtr texture_set, bool external_textures);
		
	//Call once at the start of every frame to reset counters
#if USE_BINK2_x64
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK2
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK1_x64
    [DllImport("bink_unity_plugin")]
#else //USE_BINK1
	[DllImport("bink_unity_plugin")]
	#endif
    //extern "C" void PLUGIN_EXPORT_API BinkRenderFrameInit()
	public static extern void BinkRenderFrameInit ();

	//Call once before rendering each bink movie
#if USE_BINK2_x64
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK2
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK1_x64
    [DllImport("bink_unity_plugin")]
#else //USE_BINK1
	[DllImport("bink_unity_plugin")]
	#endif
    //extern "C" void PLUGIN_EXPORT_API BinkRenderPreUpdate(HBINK bink, void * YTexturePtr, void * cBTexturePtr, void * cRTexturePtr, void * ATexturePtr)
	public static extern void BinkRenderPreUpdate (IntPtr bink, IntPtr texture_set, IntPtr YTexturePtr, IntPtr cBTexturePtr, IntPtr cRTexturePtr, IntPtr ATexturePtr);

	//Call once after rendering each bink movie
#if USE_BINK2_x64
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK2
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK1_x64
    [DllImport("bink_unity_plugin")]
#else //USE_BINK1
	[DllImport("bink_unity_plugin")]
	#endif
    //extern "C" void PLUGIN_EXPORT_API BinkRenderPostUpdate(HBINK bink)
  	public static extern void BinkRenderPostUpdate (IntPtr bink);

	//Each frame, add the movie you want to be rendered
	//If you set auto_tick_movie, the plugin will tick the bink movie for you. It's a much simpler call, but less flexible than doing it yourself. 
	//On some platforms, P/Invoke is expensive (iOS), so it can be worth it to set auto_tick_movie so you can make just one P/Invoke call per frame instead of many.  
#if USE_BINK2_x64
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK2
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK1_x64
    [DllImport("bink_unity_plugin")]
#else //USE_BINK1
	[DllImport("bink_unity_plugin")]
	#endif
	//extern "C" EXPORT_API bool BinkRenderQueueAdd(HBINK Bink, void * texture_set, bool auto_tick_movie, int draw_width, int draw_height, int screen_width, int screen_height, float draw_x_offset, float draw_y_offset, float draw_x_scale, float draw_y_scale, float draw_alpha_level, int draw_is_premultiplied_alpha, void * YTexturePtr, void * cBTexturePtr, void * cRTexturePtr, void * ATexturePtr)
	public static extern bool BinkRenderQueueAdd (IntPtr Bink, IntPtr texture_set, bool auto_tick_movie, int draw_width, int draw_height, int screen_width, int screen_height, float draw_x_offset, float draw_y_offset, float draw_x_scale, float draw_y_scale, float draw_alpha_level, int draw_is_premultiplied_alpha, IntPtr YTexturePtr, IntPtr cBTexturePtr, IntPtr cRTexturePtr, IntPtr ATexturePtr);

	//Each frame, add the movie you want to be rendered
	//If you set auto_tick_movie, the plugin will tick the bink movie for you. It's a much simpler call, but less flexible than doing it yourself. 
	//On some platforms, P/Invoke is expensive (iOS), so it can be worth it to set auto_tick_movie so you can make just one P/Invoke call per frame instead of many.  
#if USE_BINK2_x64
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK2
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK1_x64
    [DllImport("bink_unity_plugin")]
#else //USE_BINK1
	[DllImport("bink_unity_plugin")]
	#endif
	//extern "C" EXPORT_API bool BinkRenderQueueRemove(HBINK Bink)
	public static extern bool BinkRenderQueueRemove (IntPtr Bink);

#if USE_BINK2_x64
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK2
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK1_x64
    [DllImport("bink_unity_plugin")]
#else //USE_BINK1
	[DllImport("bink_unity_plugin")]
	#endif
    //extern "C" PLUGIN_EXPORT_API void BindTextureToBinkFrame(HBINK bink, int textureIDy, int textureIDCb, int textureIDCr, int textureIDa, unsigned int binkFormat, int width, int height)
  	public static extern void BindTextureToBinkFrame (IntPtr Bink, int textureIDy, int textureIDCb, int textureIDCr, int textureIDa, uint binkFormat, int width, int height);

#if USE_BINK2_x64
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK2
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK1_x64
    [DllImport("bink_unity_plugin")]
#else //USE_BINK1
	[DllImport("bink_unity_plugin")]
	#endif
    //extern "C" PLUGIN_EXPORT_API void BinkGetTexturePtrs(void * texture_set, void** texturePtry, void ** texturePtrCb, void ** texturePtrCr, void ** texturePtra)
	public static extern void BinkGetTexturePtrs (IntPtr texture_set, ref IntPtr texturePtry, ref IntPtr texturePtrCb, ref IntPtr texturePtrCr, ref IntPtr texturePtra);

	//Shut down the render queue at application shutdown
#if USE_BINK2_x64
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK2
      [DllImport("bink2_unity_plugin")]
#elif USE_BINK1_x64
    [DllImport("bink_unity_plugin")]
#else //USE_BINK1
	[DllImport("bink_unity_plugin")]
	#endif
    //extern "C" EXPORT_API void BinkRenderQueueShutdown()
  	public static extern void BinkRenderQueueShutdown ();

	//Information about a loaded bink movie
	public class MovieData
	{
		public IntPtr Bink = IntPtr.Zero;
		public IntPtr TextureSet = IntPtr.Zero;
		public int BinkWidth = 0;
		public int BinkHeight = 0;
		public int currentFrame = 0;
		public int lastRenderedFrame = -1;
		public bool needsBlit = false;
		public bool visible = true;
		public int numVisible = 0;
		public float lastRealTimeUpdateSeconds;
	}

	public MovieData Movie = new MovieData ();	

	
	public float MovieTime {
		get { return this.getBinkPluginOptions ().movieTime; }
		set { this.getBinkPluginOptions ().movieTime = value; }
	}
	
	public float MovieSpeed {
		get { return this.getBinkPluginOptions ().movieSpeed; }
	}
	
	public bool SmartRender {
		get { return this.getBinkPluginOptions ().smartRender; }
	}
	
	public bool CanCatchup {
		get { return this.getBinkPluginOptions ().canCatchup; }
	}
	
	private BinkPlayOptions getBinkPluginOptions ()
	{
		return this.gameObject.GetComponent<BinkPlayOptions> ();
	}

	private bool LoopMovie {
		get { return this.getBinkPluginOptions ().loopMovie; }
	}

	private bool movieLoaded = false;

	public bool MovieLoaded {
		get { return this.movieLoaded;}
	}
	
	public void SetAlpha (bool b)
	{
		this.Alpha = b;
	}
	
	public float getAspectRatio ()
	{
		return (float)this.Movie.BinkWidth / this.Movie.BinkHeight;
  	}
  	
	public Texture GetFinalTexture ()
	{
		if (this.Type == BinkPluginType.GPUTexture && !usingOpenGL()) {
			return this.rt;
		} else {
			return this.m_Texture;
		}
	}
	
	
	//Load a movie and add it to the playing movie list
	public bool _LoadMovie (String name)
	{		
		
		long flags = Bink.BINKALPHA;
		switch (Type) {
		case BinkPluginType.Overlay:
		#if UNITY_WIN 
			flags = Bink.BINKALPHA | Bink.BINKIOPROCESSOR;
			if (!usingOpenGL()) {
				flags |= Bink.BINKNOFRAMEBUFFERS;
			}
		#endif
			break;
		case BinkPluginType.CPUTexture:
		case BinkPluginType.GPUTexture:
		#if UNITY_WIN 
			flags = Bink.BINKALPHA | Bink.BINKIOPROCESSOR;
		#endif
			break;
		default:
			break;
		}
	
		Movie.Bink = Bink.BinkOpen (name, flags);
	
		if (Movie.Bink != IntPtr.Zero) {
			Bink.BINKSUMMARY summary = new Bink.BINKSUMMARY ();
			Bink.BinkGetSummary (Movie.Bink, ref summary);
			Movie.BinkWidth = (int)summary.Width;
			Movie.BinkHeight = (int)summary.Height;

			UseAlpha = Alpha && BinkRenderMovieHasAlpha (Movie.Bink);

			if (!usingOpenGL()) {
				CreateRenderTextures ();
			}

			CreateTextureSet ();

			if (!usingOpenGL() && Movie.TextureSet == IntPtr.Zero) {
				UnityEngine.Debug.LogError (String.Format ("Failed to create Bink texture set for {0}", name));
			}
		} else {
			UnityEngine.Debug.LogError (String.Format ("BinkOpen failed to open {0}", name));
			return false;
		}
		
		this.movieLoaded = true;
		return true;
	}
  
	private void CreateTextureSet ()
	{
		if (Type == BinkPluginType.GPUTexture && !usingOpenGL()) {
			Movie.TextureSet = BinkRenderQueueCreateTextureSet (Movie.Bink, m_yRenderTexture.GetNativeTexturePtr (), m_cBRenderTexture.GetNativeTexturePtr (), m_cRRenderTexture.GetNativeTexturePtr (), UseAlpha ? m_ARenderTexture.GetNativeTexturePtr () : IntPtr.Zero);
		} else {
			Movie.TextureSet = BinkRenderQueueCreateTextureSet (Movie.Bink, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
		}
	}
	
	private static bool BinkInitialized = false;
  
	public static Vector2 GetMainGameViewSize ()
	{
		//Weird junk to try to get the Unity window at the right size even in the editor. 
		System.Type T = System.Type.GetType ("UnityEditor.GameView,UnityEditor");
		System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod ("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
		System.Object Res = GetSizeOfMainGameView.Invoke (null, null);
		return (Vector2)Res;
	}
 
	private int ScreenWidth = 0;
	private int ScreenHeight = 0;
	private float XOffset = 0;
	private float YOffset = 0;
	private float xScale = 1;
	private float yScale = 1;
  
	float GetExtraEditorYOffset ()
	{
		if (Application.isEditor) {
#if UNITY_WIN      
			return 36.0f; //Horrible hack to make things line up when previewing in the editor. The value 36 was found by experimentation. 
#elif UNITY_MAC
    return 0.0f;
#endif
		} else {
			return 0.0f;
		}
	} 
  
	//An example. There are lots of ways to do this. 
	//Note this doesn't work perfectly in Unity preview window because of oddness with the way that window
	//works, but it will work in a standalone unity app. 
	void FitMovieToScreen (int MovieWidth, int MovieHeight)
	{
		XOffset = 0.0f;
		YOffset = 0.0f;
    
		ScreenWidth = Screen.width;
		ScreenHeight = Screen.height;
    
		if (Application.isEditor) {
			Vector2 dims = GetMainGameViewSize ();
			ScreenWidth = (int)dims [0];
			ScreenHeight = (int)dims [1];
		}
    
		float screenAspect = ((float)ScreenWidth) / (float)ScreenHeight;
		float movieAspect = ((float)MovieWidth) / (float)MovieHeight;

		bool fitWidth;
		if (OverlayType == BinkOverlayType.FitToScreenBounds) {
			fitWidth = screenAspect > movieAspect;
		} else {
			fitWidth = screenAspect < movieAspect;
		}
		
		if (fitWidth) {
			//Fit width
			float scale = ((float)ScreenWidth) / (float)MovieWidth;
			MovieWidth = Screen.width;
			MovieHeight = (int)(scale * (float)MovieHeight);
			YOffset = 0.5f * (float)(((float)ScreenHeight) - (float)MovieHeight);
			xScale = scale;
			yScale = scale;
		} else {
			//Fit height
			float scale = ((float)ScreenHeight) / (float)MovieHeight;
			MovieHeight = (int)ScreenHeight;
			MovieWidth = (int)(scale * (float)MovieWidth);
			XOffset = 0.5f * (((float)ScreenWidth) - (float)MovieWidth);
			xScale = scale;
			yScale = scale;
		}
  
		YOffset += GetExtraEditorYOffset ();
	}
  
	void CenterMovieOnScreen (int MovieWidth, int MovieHeight)
	{ 
		ScreenWidth = Screen.width;
		ScreenHeight = Screen.height;
    
		XOffset = 0.5f * (((float)ScreenWidth) - (float)MovieWidth);
		YOffset = 0.5f * (((float)ScreenHeight) - (float)MovieHeight) + GetExtraEditorYOffset ();
		xScale = 1.0f;
		yScale = 1.0f;
	}
  
	void UpdateBinkRenderParameters (int MovieWidth, int MovieHeight)
	{
		switch (OverlayType) {
		case BinkOverlayType.UpperLeft:
			XOffset = 0.0f;
			YOffset = GetExtraEditorYOffset ();
			ScreenWidth = Screen.width;
			ScreenHeight = Screen.height;
			break;
		case BinkOverlayType.FitToScreen:
			FitMovieToScreen (MovieWidth, MovieHeight);
			break;
		case BinkOverlayType.FitToScreenBounds:
			FitMovieToScreen (MovieWidth, MovieHeight);
			break;
		case BinkOverlayType.Centered:
			CenterMovieOnScreen (MovieWidth, MovieHeight);
			break;
		}
	}

	//Used with CPU Texturing
	private Texture2D m_Texture;
	private Color32[] m_Pixels;
	private GCHandle m_PixelsHandle;

	//Used with OpenGL GPU Texturing
	private Texture2D m_yTexture;
	private Texture2D m_cBTexture;
	private Texture2D m_cRTexture;
	private Texture2D m_aTexture;

	//Used with D3D GPU Texturing
	public RenderTexture m_yRenderTexture;
	private RenderTexture m_cBRenderTexture;
	private RenderTexture m_cRRenderTexture;
	private RenderTexture m_ARenderTexture;
	private RenderTexture rt;
	private Material BinkMaterial;
	
	//Cached Native pointer to the render textures
	private IntPtr NativeYTexture = IntPtr.Zero;
	private IntPtr NativecBTexture = IntPtr.Zero;
	private IntPtr NativecRTexture = IntPtr.Zero;
	private IntPtr NativeATexture = IntPtr.Zero;

	private void CacheNativeRenderPointer ()
	{
		//Calls to GetNativeTexturePtr() force the main thread and the rendering thread to sync, so only do this when absolutely necessary
		if (Type == BinkPluginType.GPUTexture && !usingOpenGL()) {
			NativeYTexture = m_yRenderTexture.GetNativeTexturePtr ();
			NativecBTexture = m_cBRenderTexture.GetNativeTexturePtr ();
			NativecRTexture = m_cRRenderTexture.GetNativeTexturePtr ();
			if (UseAlpha) {
				NativeATexture = m_ARenderTexture.GetNativeTexturePtr ();
			}
		}
	}

	void CreateRenderTextures ()
	{
		uint ya_texture_width = 0;
		uint ya_texture_height = 0;
		uint cbcr_texture_width = 0;
		uint cbcr_texture_height = 0;
		BinkGetFrameBufferDimensions (Movie.Bink, ref ya_texture_width, ref ya_texture_height, ref cbcr_texture_width, ref cbcr_texture_height);

		m_yRenderTexture = new RenderTexture ((int)ya_texture_width, (int)ya_texture_height, 0, RenderTextureFormat.R8);
		m_yRenderTexture.isPowerOfTwo = false;
		m_yRenderTexture.Create ();
		m_cBRenderTexture = new RenderTexture ((int)cbcr_texture_width, (int)cbcr_texture_height, 0, RenderTextureFormat.R8);
		m_cBRenderTexture.isPowerOfTwo = false;
		m_cBRenderTexture.Create ();
		m_cRRenderTexture = new RenderTexture ((int)cbcr_texture_width, (int)cbcr_texture_height, 0, RenderTextureFormat.R8);
		m_cRRenderTexture.isPowerOfTwo = false;
		m_cRRenderTexture.Create ();
		if (UseAlpha) {
			m_ARenderTexture = new RenderTexture ((int)ya_texture_width, (int)ya_texture_height, 0, RenderTextureFormat.R8);
			m_ARenderTexture.isPowerOfTwo = false;
			m_ARenderTexture.Create ();
		}
		CacheNativeRenderPointer ();
	}

	void BinkSetup ()
	{

		if (!BinkInitialized) {	
			#if UNITY_WIN
			Bink.BinkSoundUseDirectSound (0);
			#endif
	
			BinkRenderQueueInitialize (1024, 4);
					
			BinkInitialized = true;  
		}
	}
	
	public bool LoadMovie (string moviePath)
	{
		this.BinkSetup ();
		
		if (moviePath.Length == 0) {
			return false;
		}
	
		if (BinkRenderQueueCreateShaders ()) {
			_LoadMovie (moviePath);
			
			if (Movie.Bink == IntPtr.Zero) {
				return false;
			}
			if (Movie.Bink != IntPtr.Zero) {
				
				if (Type == BinkPluginType.CPUTexture) {
					// Create texture that will be updated in the plugin
					m_Texture = new Texture2D (Movie.BinkWidth, Movie.BinkHeight, TextureFormat.ARGB32, false);
					// Create the pixel array for the plugin to write into at startup    
					m_Pixels = m_Texture.GetPixels32 (0);
					// "pin" the array in memory, so we can pass direct pointer to it's data to the plugin,
					// without costly marshaling of array of structures.
					m_PixelsHandle = GCHandle.Alloc (m_Pixels, GCHandleType.Pinned);
	
					//Assign texture to ourselves.
					if (renderer) {
						renderer.material.mainTexture = m_Texture;
					} else {
						UnityEngine.Debug.Log ("Game object has no renderer to assign the texture to!");
					}
				}


				if (Type == BinkPluginType.GPUTexture) {						
					if (usingOpenGL()) {
						// Create texture that will be updated in the plugin. Note, the textureformat doesn't really matter here, 
						// as we'll be managing the texels ourselves in the fragment shader. 
						m_yTexture = new Texture2D (Movie.BinkWidth, Movie.BinkHeight, TextureFormat.ARGB32, false);
						m_cBTexture = new Texture2D (Movie.BinkWidth / 2, Movie.BinkHeight / 2, TextureFormat.RGBA32, false);
						m_cRTexture = new Texture2D (Movie.BinkWidth / 2, Movie.BinkHeight / 2, TextureFormat.RGBA32, false);

						if (UseAlpha) {
							//You only really want to do this if your movie have alpha and you want to use it. 
							m_aTexture = new Texture2D (Movie.BinkWidth, Movie.BinkHeight, TextureFormat.RGBA32, false);
						}
					}


					if (usingOpenGL() && renderer) {
						renderer.material.SetTexture ("yTexture", m_yTexture);
						renderer.material.SetTexture ("cBTexture", m_cBTexture);
						renderer.material.SetTexture ("cRTexture", m_cRTexture);
						
						if (UseAlpha) {
							renderer.material.SetTexture ("aTexture", m_aTexture);				
						}
					} else {
						//Hook the material up to our textures
						Material m = new Material (Shader.Find ("RAD/BinkShader"));
						m.SetTexture ("yTexture", m_yRenderTexture);
						m.SetTexture ("cBTexture", m_cBRenderTexture);
						m.SetTexture ("cRTexture", m_cRRenderTexture);
						if (UseAlpha) {
							m.SetTexture ("aTexture", m_ARenderTexture);				
						}

						//Disable rendering GPUTextured objects until the plugin, running on the 
						//render thread, has a chance to fill out the texture
						if (renderer) {
							renderer.enabled = false;
						}

						//Adjust UV's since D3D gpu texturing oversizes the texture's vertical component so that it's a multiple of 8 on bink1
						//Note we also use texScale.x = -1 and texOffset.x = 1 to flip the image vertically since Unity's texture up is the opposite of binks
						Vector2 texScale = new Vector2 (1.0f, -((float)Movie.BinkHeight) / (float)m_yRenderTexture.height);
						Vector2 texOffset = new Vector2 (0.0f, 1.0f);
						m.SetTextureScale ("yTexture", texScale);
						m.SetTextureOffset ("yTexture", texOffset);

						m.SetTextureScale ("cBTexture", texScale);
						m.SetTextureOffset ("cBTexture", texOffset);

						m.SetTextureScale ("cRTexture", texScale);
						m.SetTextureOffset ("cRTexture", texOffset);

						if (UseAlpha) {
							m.SetTextureScale ("aTexture", texScale);
							m.SetTextureOffset ("aTexture", texOffset);
						}
						this.rt = new RenderTexture (Movie.BinkWidth, Movie.BinkHeight, 0, RenderTextureFormat.Default);
						this.rt.wrapMode = TextureWrapMode.Repeat;
						this.BinkMaterial = m;
					}

				}
			}
		}
		return true;
	}

	IEnumerator Start ()
	{
		this.Movie.lastRealTimeUpdateSeconds = Time.realtimeSinceStartup;
		yield return StartCoroutine ("CallPluginAtEndOfFrames");
	}

	private void CallPluginRenderLoop ()
	{
		// Rendering of scene objects can happen here
		// Add the movie to the render plugin queue so it gets rendered
		if (Movie.Bink != IntPtr.Zero) {
			if (Type == BinkPluginType.GPUTexture && !usingOpenGL()) {
				if (renderer) {
					renderer.enabled = BinkRenderIsTextureSetReady (Movie.TextureSet) && m_yRenderTexture.IsCreated ();
				}	
			}

			//TextureSet was invalidated by window resize, re-create it. 
			if (!usingOpenGL() && Movie.TextureSet == IntPtr.Zero) {
				CreateTextureSet ();

				//Do not attempt to render if we cannot create the texture set
				if (Movie.TextureSet == IntPtr.Zero) {
					return;
				}
			}

			xScale = 1.0f;
			yScale = 1.0f;
			
			//For multiple-movie testing, scale and offset the second one so you can see it. 
			UpdateBinkRenderParameters (Movie.BinkWidth, Movie.BinkHeight);

			if (Type == BinkPluginType.GPUTexture && !usingOpenGL()) {
				if (!m_yRenderTexture.IsCreated ()) {
					m_yRenderTexture.Create ();
					m_cBRenderTexture.Create ();
					m_cRRenderTexture.Create ();
					
					if (UseAlpha) {
						m_ARenderTexture.Create ();
					}
					CreateTextureSet ();
					CacheNativeRenderPointer ();
					BinkRenderMarkReset (Movie.TextureSet, true);
				}
			}
			
			bool tickAutomatically = true;
#if BINK_QUEUE_TICK_MANUALLY
			tickAutomatically = false;
			//
			Bink.BINKSUMMARY summary = new Bink.BINKSUMMARY ();
			Bink.BinkGetSummary (Movie.Bink, ref summary);
			uint targetFrame = this.getTargetFrame (summary);
			bool skip = false;
			
			if (targetFrame == Movie.lastRenderedFrame) {
				skip = true;				
			}
			
			if (!BinkRenderOkToRender ()) {
				skip = true;
			}
			if (!this.Movie.visible && this.SmartRender && Movie.lastRenderedFrame >= 0) {
				//only render if visible, or if first frame of video.	
				skip = true;				
			}
			
			this.Movie.visible = false; //will be set to true if any camera can see it.
      
			if (!skip) {				
				BinkRenderRespondToReset (Movie.Bink, Movie.TextureSet, NativeYTexture, NativecBTexture, NativecRTexture, NativeATexture);

				BinkRenderPreUpdate (Movie.Bink, Movie.TextureSet, NativeYTexture, NativecBTexture, NativecRTexture, NativeATexture);
				// Notify plugin we're about to update Bink
				//
				int frameDiff = ((int)targetFrame - Movie.currentFrame);
				if (frameDiff < -1 || // We have already gone past the target frame. We must rewind 
					frameDiff > this.getFrameRate (summary) * 0.5f) { // The target frame is after us, but by a significant amount so we should jump  								
					
					Bink.BinkGoto (Movie.Bink, targetFrame, 0); 
					Movie.currentFrame = (int)targetFrame;
				}
				
					
				//
				// Decompress a frame
				//
				Bink.BinkDoFrame (Movie.Bink);
				Movie.lastRenderedFrame = Movie.currentFrame;
				Movie.currentFrame++;
				Movie.needsBlit = true;
//				if (CanCatchup && frameDiff == 1) {
//					Bink.BinkNextFrame(Movie.Bink);
//					Bink.BinkDoFrame(Movie.Bink);
//					Movie.currentFrame++;
//				}

				//
				// Keep playing the Movie.
				//
				Bink.BinkNextFrame (Movie.Bink);

				//
				// Notify plugin we're done updating
				//
				BinkRenderPostUpdate (Movie.Bink);
			}
			
			
#endif //BINK_QUEUE_TICK_MANUALLY

			BinkRenderQueueAdd (Movie.Bink, Movie.TextureSet, tickAutomatically, Movie.BinkWidth, Movie.BinkHeight, (int)(Screen.width * xScale), (int)(Screen.height * yScale), XOffset, YOffset, xScale, yScale, 1.0f, 0, NativeYTexture, NativecBTexture, NativecRTexture, NativeATexture);
		}

	}
	
	private float getFrameRate (Bink.BINKSUMMARY summary)
	{
		return (summary.FrameRate / (float)summary.FrameRateDiv);		
	}
	
	private uint getTargetFrame (Bink.BINKSUMMARY summary)
	{
		
		uint result = (uint)(this.MovieTime * this.getFrameRate (summary));
		if (this.LoopMovie) {			
			result %= summary.TotalFrames;
		} else if (result > summary.TotalFrames) {
			result = summary.TotalFrames;
		}
		return result;
		
	}

	private void BinkConvertToTexture (MovieData movie)
	{
		switch (Type) {	
		case BinkPluginType.CPUTexture:
			Bink.BinkCopyToBuffer (Movie.Bink, m_PixelsHandle.AddrOfPinnedObject (), m_Texture.width * 4, (uint)m_Texture.height, 0, 0, Bink.BINKSURFACE32RA | Bink.BINKNOSKIP);
			m_Texture.SetPixels32 (m_Pixels, 0);
			m_Texture.Apply ();
			break;
		case BinkPluginType.GPUTexture:

			if (usingOpenGL()) {
				BindTextureToBinkFrame (Movie.Bink, m_yTexture.GetNativeTextureID (), m_cBTexture.GetNativeTextureID (), m_cRTexture.GetNativeTextureID (), UseAlpha ? m_aTexture.GetNativeTextureID () : -1, Bink.BINKSURFACE32RA, m_yTexture.width, m_yTexture.height);
			}

			break;
		default:
			UnityEngine.Debug.LogError (String.Format ("BinkConvertToTexture called with unsupported Type {0}", Type.ToString ()));
			break;
		}
	}

	public void Update ()
	{
		
		if (this.Movie.needsBlit && this.BinkMaterial != null && Movie.TextureSet != IntPtr.Zero) {
			Graphics.Blit (null, this.rt, this.BinkMaterial);
			this.Movie.needsBlit = false;
		
		} 		
		
	}
	private float getDeltaTime() {
		float result = Time.deltaTime;
		float t = Time.realtimeSinceStartup;
		if (this.isGameSpeedAgnostic) {
			result = t - this.Movie.lastRealTimeUpdateSeconds;
		}
		
		this.Movie.lastRealTimeUpdateSeconds = t;
		
		return result;
	}
	private IEnumerator CallPluginAtEndOfFrames ()
	{
		while (true) {
			
			// Wait until all frame rendering is done
			yield return new WaitForEndOfFrame ();
			if (!this.movieLoaded)
				continue;
			BinkPlayOptions bpo = this.getBinkPluginOptions ();
			
			if (bpo == null)
				continue;
				
			if (bpo.playAfterSeconds > 0f && bpo.movieSpeed > 0f) {
				// Simple delay before movie starts playing
				bpo.playAfterSeconds -= this.getDeltaTime();
			} else {
			
				bpo.movieTime += bpo.movieSpeed * this.getDeltaTime();
	      
				if (Type == BinkPluginType.Overlay ||
					(Type == BinkPluginType.GPUTexture && !usingOpenGL())) {
					CallPluginRenderLoop ();
	
					// Issue a plugin event with arbitrary integer identifier.
					// The plugin can distinguish between different
					// things it needs to do based on this ID.
					// For our simple plugin, it does not matter which ID we pass here.
					GL.IssuePluginEvent (1);
				} else {
					if (Movie.Bink != IntPtr.Zero) {
						//
						// Notify plugin we're about to update Bink
						//
						Bink.BINKSUMMARY summary = new Bink.BINKSUMMARY ();
						Bink.BinkGetSummary (Movie.Bink, ref summary);
						if (Bink.BinkWait (Movie.Bink) == 0) {
							BinkRenderPreUpdate (Movie.Bink, Movie.TextureSet, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
	
							//
							// Decompress a frame
							//
							Bink.BinkDoFrame (Movie.Bink);
							
							//
							// if we are falling behind, decompress an extra frame to catch up
							//          
							while (Bink.BinkShouldSkip(Movie.Bink) != 0) {
								Bink.BinkNextFrame (Movie.Bink);
								Bink.BinkDoFrame (Movie.Bink);
							}
							
							BinkConvertToTexture (Movie);
							
							//
							// Keep playing the Movie.
							//
							Bink.BinkNextFrame (Movie.Bink);
							BinkRenderPostUpdate (Movie.Bink);
						}
					}
				}
			}
		}
	}

	private void OnApplicationQuit ()
	{
		if (BinkInitialized) {
			//Clear out any remaining render queue entries and shut down the render queue
			BinkRenderQueueShutdown ();
			
			BinkRenderQueueDestroyShaders ();
			
			BinkInitialized = false;
		}
	}

	public void Cleanup ()
	{
		
		if (Movie.Bink != IntPtr.Zero) {
			if (Movie.TextureSet != IntPtr.Zero) {
				BinkRenderQueueDestroyTextureSet (Movie.TextureSet, Type == BinkPluginType.GPUTexture);
			}
			Bink.BinkClose (Movie.Bink);
			Movie.Bink = IntPtr.Zero;
		}
		
		if (Type == BinkPluginType.CPUTexture) {
			// Free the pinned array handle.
			m_PixelsHandle.Free ();
		}
	}

	private void OnDisable ()
	{
		this.Cleanup ();

	}
	void OnEnable() {
		
		this.Movie.visible = true;
	}
	void OnWillRenderObject() {		
		this.Movie.visible = true;
	}	

	
	private bool usingOpenGL() {
		return SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL");
	}  
}
