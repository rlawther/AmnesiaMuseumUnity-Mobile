
using System.Threading;
namespace Toolbelt {
/*** Generic threading encapsulated in a class
 * Simply override ThreadFunction with your own functionality
 **/
abstract public class ThreadedClass {

	private Thread thread = null;
	volatile protected bool stopRequested = false;
	bool stopped = true;
	
	~ThreadedClass() {
		this.StopThread();
	}
	
	public void StartThread() 
	{
		this.thread = new Thread( new ThreadStart(this.ThreadFunction) );
		this.stopped = false;
		this.thread.Start();
	}
	
	public void StopThread() 
	{
		this.stopRequested = true;
		if (this.thread != null) this.thread.Join();
		this.stopped = true;
		this.thread = null;
	}
	
	public bool Stopped() { return this.stopped; }
	
	/// The thread will be running this function.
	/// Do your own check for this.stopRequested if using an indefinite loop.
	protected abstract void ThreadFunction();
}
}