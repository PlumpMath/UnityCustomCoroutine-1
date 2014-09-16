using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Object = System.Object;

namespace AltProg
{

public class Coroutines : Singleton<Coroutines> 
{
	// Cr = Coroutine
	// Mt = Main Thread
	LinkedList<Coroutine> activeCrMt = new LinkedList<Coroutine>();
	LinkedList<Coroutine> activeCrMtNew = new LinkedList<Coroutine>();
	LinkedList<Coroutine> activeCr = new LinkedList<Coroutine>();
	Dictionary<IAsyncResult, Coroutine> inactiveCr = new Dictionary<IAsyncResult, Coroutine>();

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

		LinkedListNode<Coroutine> node = activeCrMt.First;
		while ( node != null )
		{
			// in case of remove the node inside MoveNext()
			var nextNode = node.Next;

			MoveNext( node );

			node = nextNode;
		}
	}

	void MoveNext( LinkedListNode<Coroutine> node )
	{
		Coroutine co = node.Value;

		// Continue Coroutine
		bool hasNext = co.Enumerator.MoveNext();

		// end of coroutine
		if ( false == hasNext )
		{
			// (node.List will be activeCoMt.)
			node.List.Remove( node );
			TryAsyncCallback( co, false );
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
		else if ( o is IAsyncResult )
		{
			var ar = (IAsyncResult)o;

			// Check if it is already completed
			if ( ar.IsCompleted )
			{
				// (node is in activeCoMt or activeCoMtNew )
				// Do nothing.
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
		else
		{
			Debug.LogError( string.Format( "Unknown Enumerator Item. '{0}'", o ) );
			return;
		}
	}

	public static ICoroutine Start( IEnumerator en )
	{
		return Instance.StartImpl( en );
	}

	ICoroutine StartImpl( IEnumerator en )
	{
		var co = new Coroutine(en);

		var node = new LinkedListNode<Coroutine>( co );
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
		Coroutine co;

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
