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

		//Coroutines.Start( TestException() );

		// Unity Coroutine
		//StartCoroutine( TestException() );
	
		Coroutines.Start( TestGetResult() );

		Coroutines.Start( TestGetResult_InvalidType() );

		Coroutines.Start( TestGetResult_Exception() );
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

	/*
	IEnumerator TestException()
	{
		throw new Exception("Test");

		yield return null;

		TestUtil.Fail( "After Exception" );
	}
	*/

	IEnumerator TestGetResultApi()
	{
		yield return null;
		yield return Result.New(33);
		throw new Exception("Do Not Reach");
	}

	IEnumerator TestGetResult()
	{
		var co = Coroutines.Start<int>( TestGetResultApi() );
		yield return co;

		TestUtil.AssertEq( 33, co.Get() );
	}

	IEnumerator TestGetResult_InvalidType()
	{
		var co = Coroutines.Start<string>( TestGetResultApi() );
		yield return co;

		TestUtil.AssertThrow<Exception>( () => co.Get() );
	}

	IEnumerator TestGetResultApi_Exception()
	{
		yield return null;
		throw new Exception("Exception in Coroutine");
	}

	IEnumerator TestGetResult_Exception()
	{
		var co = Coroutines.Start<int>( TestGetResultApi_Exception() );
		yield return co;

		TestUtil.AssertThrow<Exception>( () => co.Get() );
	}
}

}
