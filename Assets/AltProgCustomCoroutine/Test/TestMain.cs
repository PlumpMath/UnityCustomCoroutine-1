using UnityEngine;
using System;
using System.Collections;
using AltProg;
using Object = System.Object;

namespace AltProg.Test
{

public class TestMain : MonoBehaviour 
{
	int updateCount;

	void Start () 
	{
		Coroutines.Start( TestYieldNull() );

		Coroutines.Start( TestAsyncApi(0) );

		Coroutines.Start( TestAsyncApi(0.1f) );

		Coroutines.Start( TestTooFastCallback() );

		Coroutines.Start( TestMultiLevel() );

		Coroutines.Start( TestException() );

		// Unity Coroutine
		StartCoroutine( TestException() );
	}

	void Update()
	{
		++ updateCount;
	}

	IEnumerator TestYieldNull()
	{
		TestUtil.AssertEq( 0, updateCount );

		yield return null;

		TestUtil.AssertEq( 1, updateCount );

		yield return null;

		TestUtil.AssertEq( 2, updateCount );
	}

	IEnumerator TestAsyncApi( float delaySeconds )
	{
		IAsyncResult ar = TestApi.Begin_AddAsync( Coroutines.AsyncCallback, 3, 5, delaySeconds );
		yield return ar;

		int res = TestApi.End_AddAsync( ar );

		TestUtil.AssertEq( 8, res);
	}

	IEnumerator TestTooFastCallback()
	{
		IAsyncResult ar = TestApi.Begin_TooFastCallback( Coroutines.AsyncCallback, 10 );
		yield return ar;

		int res = TestApi.End_TooFastCallback( ar );

		TestUtil.AssertEq( 10, res);
	}

	IEnumerator TestMultiLevel()
	{
		TestUtil.AssertEq( 0, updateCount );

		yield return Coroutines.Start( Count2() );

		TestUtil.AssertEq( 3, updateCount );
	}

	IEnumerator Count2()
	{
		TestUtil.AssertEq( 0, updateCount );

		yield return null;

		TestUtil.AssertEq( 1, updateCount );

		yield return null;

		TestUtil.AssertEq( 2, updateCount );
	}

	IEnumerator TestException()
	{
		throw new Exception("Test");

		yield return null;

		TestUtil.Fail( "After Exception" );
	}
}

}
