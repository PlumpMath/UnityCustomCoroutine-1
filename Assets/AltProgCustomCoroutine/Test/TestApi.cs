using System;
using System.Threading;
using System.Collections.Generic;

namespace AltProg.Test
{

public static class TestApi 
{
	class TestAsyncResult : IAsyncResult
	{
		public bool IsCompleted;
		public Exception e;
		public int v;

		Object IAsyncResult.AsyncState { get { return null; } }
		WaitHandle IAsyncResult.AsyncWaitHandle { get { return null; } }
		bool IAsyncResult.CompletedSynchronously { get { return false; } }
		bool IAsyncResult.IsCompleted { get { return IsCompleted;} }
	}

	// if a is 0, Begin_AddAsync will throw an exception.
	// if b is 0, End_AddAsync will throw an exception.
	public static IAsyncResult Begin_AddAsync( AsyncCallback cb, int a, int b, float delaySeconds )
	{
		if ( a == 0 ) throw new Exception( "a is 0" );

		var ar = new TestAsyncResult();

		var t = new Thread( () => 
		{
			Thread.Sleep( (int)(delaySeconds * 1000) );

			ar.IsCompleted = true;
			if ( b == 0 ) 
				ar.e = new Exception( "b is 0" );
			else 
				ar.v = a + b;
			
			cb( ar );
		});

		t.Start();

		return ar;
	}

	public static int End_AddAsync( IAsyncResult ar )
	{
		var testAr = (TestAsyncResult)ar;

		if ( testAr.e != null ) throw testAr.e;
		return testAr.v;
	}

	public static IAsyncResult Begin_TooFastCallback( AsyncCallback cb, int arg )
	{
		var ar = new TestAsyncResult();

		ar.v = arg;
		ar.IsCompleted = true;

		cb( ar );

		return ar;
	}

	public static int End_TooFastCallback( IAsyncResult ar )
	{
		var testAr = (TestAsyncResult)ar;
		return testAr.v;
	}
}

}
