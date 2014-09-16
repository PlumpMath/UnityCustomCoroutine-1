using System;
using System.Threading;
using System.Collections.Generic;

namespace AltProg
{

public static class TestApi 
{
	static Dictionary<IAsyncResult, int> results = new Dictionary<IAsyncResult, int>();

	class TestAsyncResult : IAsyncResult
	{
		public bool IsCompleted;

		Object IAsyncResult.AsyncState { get { return null; } }
		WaitHandle IAsyncResult.AsyncWaitHandle { get { return null; } }
		bool IAsyncResult.CompletedSynchronously { get { return false; } }
		bool IAsyncResult.IsCompleted { get { return IsCompleted;} }
	}

	public static IAsyncResult Begin_AddAsync( AsyncCallback cb, int a, int b, float delaySeconds )
	{
		var ar = new TestAsyncResult();

		var t = new Thread( () => 
		{
			Thread.Sleep( (int)(delaySeconds * 1000) );

			lock (results)
			{
				results.Add( ar, a + b );
			}

			ar.IsCompleted = true;
			cb( ar );
		});

		t.Start();

		return ar;
	}

	public static int End_AddAsync( IAsyncResult ar )
	{
		lock (results)
		{
			return results[ar];
		}
	}

	public static IAsyncResult Begin_TooFastCallback( AsyncCallback cb, int arg )
	{
		var ar = new TestAsyncResult();

		lock (results)
		{
			results.Add( ar, arg );
		}

		ar.IsCompleted = true;
		cb( ar );

		return ar;
	}

	public static int End_TooFastCallback( IAsyncResult ar )
	{
		lock (results)
		{
			return results[ar];
		}
	}
}

}
