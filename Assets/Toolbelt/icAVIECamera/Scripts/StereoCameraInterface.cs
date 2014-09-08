using UnityEngine;
using System.Collections;

namespace Toolbelt {

public interface StereoCameraInterface 
{
	StereoMode GetCurrentEye();
	void SetCurrentEye(StereoMode eye);
}

}
