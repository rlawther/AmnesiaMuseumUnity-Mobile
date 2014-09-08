using UnityEngine;
using System.Collections;

public class ClusterManager : Singleton<ClusterManager>
{
    protected ClusterManager() { } // guarantee this will be always a singleton only - can't use the constructor!    

    // Backing Stores 
    private bool mIsMaster = false;
    private int mNumSlaves = -1;
    private int mClientNumber = -1; //0 based. IG1 = 0, IG2 = 1...
    private bool mUseStereo = true;
    private bool mClusterDebug = false;
    private bool mDisplayAll = false; 
	private int mNumSlavesToShow = 1;
	private bool mHadArgs = false;
	
    // Accessor methods
    public bool IsMaster { get { return mIsMaster; } }
    public int NumSlaves { get { return mNumSlaves; } }
    public int ClientID { get { return mClientNumber; } }
    public bool UseStereo { get { return mUseStereo; } }
    public bool Debug { get { return mClusterDebug; } }
    public bool DisplayAll { get { return mDisplayAll; } }
    public int NumSlavesToShow { get { return mNumSlavesToShow; } }
	public bool hadArgs { get { return mHadArgs; } }
    
    public bool _editorForceMaster = false;
    public int _editorForceClientID = 1;
    public bool _editorForceDisplayAll = false; 

    void Awake()
    {
        // In editor mode ignore the command line argumetns 
        // and allow the developer to set master/clientID 
        // from the Editor Inspector
        if (!Application.isEditor)
        {
            // Setup readonly status from command line
            string[] args = System.Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-server")
                {
                	mHadArgs = true;
                    mIsMaster = true;
                    mNumSlaves = System.Convert.ToInt32(args[i + 1]);
                }
                if (args[i] == "-client")
                {
					mHadArgs = true;
                	mNumSlavesToShow = 1;
                    mIsMaster = false;
                    mClientNumber = System.Convert.ToInt32(args[i + 1]);
                }
                if (args[i] == "-doubleclient")
                {
					mHadArgs = true;
                	mIsMaster = false;
					mNumSlavesToShow = 2;
                	mClientNumber = System.Convert.ToInt32(args[i+1]);
                }
                if (args[i] == "-tripleclient") 
                {
					mHadArgs = true;
					mIsMaster = false;
					mNumSlavesToShow = 3;
					mClientNumber = System.Convert.ToInt32(args[i+1]);
                }
                if (args[i] == "-stereo")
                {
					mHadArgs = true;
                    mUseStereo = true;
                }
                if (args[i] == "-displayAll")
                {
					mHadArgs = true;
                    this.mDisplayAll = true;
                }
                if (args[i] == "-fakeclient")
                {
					mHadArgs = true;
                    mIsMaster = false;
                    mClientNumber = System.Convert.ToInt32(args[i + 1]);
                }
            }
        }
        else
        {
            mIsMaster = _editorForceMaster;            
            if (!mIsMaster)
            {
                mClientNumber = _editorForceClientID;
            }
            this.mDisplayAll = _editorForceDisplayAll;
        }
    }

    void Update()
    {
        // This command is picked up and run on all slave nodes 
        //if (Input.GetKeyDown(KeyCode.F1))
        //{
        //    mClusterDebug = !mClusterDebug; 
        //}

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

    }

}