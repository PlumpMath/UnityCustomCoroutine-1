// Modification of https://github.com/muscly/unity3d-extensions/blob/master/Threading/Dispatcher.cs
using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using Object = System.Object;

namespace AltProg
{

public interface ICoroutine : IAsyncResult
{
}

class Coroutine : ICoroutine
{
	IEnumerator en;
	bool isCompleted;

	public IEnumerator Enumerator { get { return en; } }

	Object IAsyncResult.AsyncState { get { return null; } }
	WaitHandle IAsyncResult.AsyncWaitHandle { get { return null; } }
	bool IAsyncResult.CompletedSynchronously { get { return false; } }
	bool IAsyncResult.IsCompleted { get { return isCompleted;} }

	internal Coroutine( IEnumerator en )
	{
		this.en = en;
	}

	internal void SetCompleted()
	{
		isCompleted = true;
	}
}

}
