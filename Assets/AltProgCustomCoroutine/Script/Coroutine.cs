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
	void Get();
}

public interface ICoroutine<T> : ICoroutine
{
	new T Get();
}


interface ICoroutineImpl
{
	IEnumerator Enumerator { get; } 
	IResult Result { set; }
	Exception Exception { set; }
}

class Coroutine<T> : ICoroutine<T>, ICoroutine,  ICoroutineImpl
{
	IEnumerator en;
	bool isCompleted;
	IResult result;
	Exception e;

	// ICoroutineImpl
	IEnumerator ICoroutineImpl.Enumerator { get { return en; } }
	IResult ICoroutineImpl.Result { set { result = value; } }
	Exception ICoroutineImpl.Exception { set { e = value; } }

	// ICoroutine<T>
	T ICoroutine<T>.Get() 
	{ 
		if ( e != null )
			throw e;

		if ( ! ( result is Result<T> ) )
			throw new Exception( string.Format( "Invalid Result Type. Expected:{0}, Actual:{1}", typeof(T), result.GetType().GetGenericArguments()[0] ) );

		var genericResult = (Result<T>)result;
		return genericResult.GetResult();
	}
		
	// ICoroutine
	void ICoroutine.Get() 
	{ 
		if ( e != null )
			throw e;
	}

	// IAsyncResult
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
