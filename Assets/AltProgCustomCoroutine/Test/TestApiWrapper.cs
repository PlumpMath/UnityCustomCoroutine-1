using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace AltProg.Test
{

public static class TestApiWrapper 
{
	public static IEnumerator AddAsync( int a, int b, float delaySeconds )
	{
		IAsyncResult ar = TestApi.Begin_AddAsync( Coroutines.AsyncCallback, a, b, delaySeconds );
		yield return ar;

		yield return Result.New( TestApi.End_AddAsync( ar ) );
	}
}

}
