using RMC.Core.Exceptions;
using UnityEngine;

namespace RMC.Backend.Baas
{
	//  Class Attributes ----------------------------------
	
	/// <summary>
	/// The configuration for the backend system.
	/// </summary>
	public abstract class Configuration : ScriptableObject
	{
		//  Properties ------------------------------------

		//  Fields ----------------------------------------

		//  Methods ---------------------------------------
		public virtual bool IsValid()
		{
			throw new MustOverrideMethodException(this);
		}
	}
}
