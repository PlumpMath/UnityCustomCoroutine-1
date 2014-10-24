using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Object = System.Object;

namespace AltProg
{

public class Coroutines : Util.Singleton<Coroutines> 
{
	// Cr = Coroutine
	// Mt = Main Thread
	LinkedList<ICoroutineImpl> activeCrMt = new LinkedList<ICoroutineImpl>();
	LinkedList<ICoroutineImpl> activeCrMtNew = new LinkedList<ICoroutineImpl>();
	LinkedList<ICoroutineImpl> activeCr = new LinkedList<ICoroutineImpl>();
	Dictionary<IAsyncResult, ICoroutineImpl> inactiveCr = new Dictionary<IAsyncResult, ICoroutineImpl>();

	void Update()
	{
		// Move activeCr to activeCrMt
		if ( activeCr.Count > 0 )
		{
			lock( activeCr )
			{
				while ( activeCr.Count > 0 )
				{
					var co = activeCr.First;
					activeCr.RemoveFirst();
					activeCrMt.AddLast( co );
				}
			}
		}

		// Move activeCrMtNew to activeCrMt
		if ( activeCrMtNew.Count > 0 )
		{
			while ( activeCrMtNew.Count > 0 )
			{
				var co = activeCrMtNew.First ;
				activeCrMtNew.RemoveFirst();
				activeCrMt.AddFirst( co );
			}
		}

		LinkedListNode<ICoroutineImpl> node = activeCrMt.First;
		while ( node != null )
		{
			// in case of remove the node inside MoveNext()
			var nextNode = node.Next;

			MoveNext( node );

			node = nextNode;
		}
	}

	void MoveNext( LinkedListNode<ICoroutineImpl> node )
	{
		ICoroutineImpl co = node.Value;

		bool hasNext;

		try
		{

			// Continue Coroutine
			hasNext = co.Enumerator.MoveNext();
		}
		catch( Exception e)
		{
            Debug.LogWarning ("An exception has occurred while the coroutine is running. \n" + e.ToString () );

			co.Exception = e;
			
			// Finish the coroutine
			// (node.List will be activeCoMt.)
			node.List.Remove( node );
			TryAsyncCallback( (IAsyncResult)co, false );
			return;
		}


		// end of coroutine
		if ( false == hasNext )
		{
			// (node.List will be activeCoMt.)
			node.List.Remove( node );
			TryAsyncCallback( (IAsyncResult)co, false );
			return;
		}

		Object o = co.Enumerator.Current;

		// yield return null
		if ( o == null )
		{
			// (node is in activeCoMt or activeCoMtNew )
			// Do nothing
			return;
		}
		// Coroutine or other async operations
		else if ( o is IAsyncResult )
		{
			var ar = (IAsyncResult)o;

			// Check if it is already completed
			if ( ar.IsCompleted )
			{
				// (node is in activeCoMt or activeCoMtNew )
				// Do nothing.( = Don't remove from the active list )
			}
			else
			{
				// Move to inactive list
				
				// (node.List may be activeCoMt or activeCoMtNew )
				node.List.Remove( node );
				
				lock ( inactiveCr )
				{
					inactiveCr.Add( (IAsyncResult)o, co );
				}
				return;
			}
		}
		// Final return value of coroutine
		else if ( o is IResult )
		{
			// Set the result
			co.Result = (IResult)o;

			// Finish the current coroutine
			// (node.List will be activeCoMt.)
			node.List.Remove( node );
			TryAsyncCallback( (IAsyncResult)co, false );
			return;
		}
		else
		{
			Debug.LogError( string.Format( "Unknown Enumerator Item. '{0}'", o ) );
			return;
		}
	}

	public static ICoroutine Start( IEnumerator en )
	{
		return Instance.StartImpl<object>( en );
	}

	public static ICoroutine<T> Start<T>( IEnumerator en )
	{
		return Instance.StartImpl<T>( en );
	}

	ICoroutine<T> StartImpl<T>( IEnumerator en )
	{
		var co = new Coroutine<T>(en);

		var node = new LinkedListNode<ICoroutineImpl>( co );
		activeCrMtNew.AddLast( node );

		// Execute the first part of the coroutine
		MoveNext( node );

		return co;
	}

	public static void AsyncCallback( IAsyncResult ar )
	{
		Instance.TryAsyncCallback( ar, true );
	}

	void TryAsyncCallback( IAsyncResult ar, bool logError )
	{
		ICoroutineImpl co;

		lock( inactiveCr )
		{
			if ( ! inactiveCr.TryGetValue( ar, out co ) )
			{
				// The callback was called before a user 'yield return' IAsyncResult or ICoroutine.
				// We can ignore this. Because MoveNext() will check if an IAsyncResult(or ICoroutine) is already completed.

				return;
			}

			inactiveCr.Remove( ar );
		}

		lock( activeCr )
		{
			activeCr.AddLast( co );
		}
	}
}

}
