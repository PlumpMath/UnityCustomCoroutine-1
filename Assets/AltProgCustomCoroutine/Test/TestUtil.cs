using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AltProg.Test
{

public static class TestUtil 
{
	public static void AssertEq<T>( T expected, T actual )
	{
		bool equal = EqualityComparer<T>.Default.Equals( expected, actual );

		if ( equal )
		{
			Debug.Log( string.Format("[OK] {0} == {1}", expected, actual ) );
		}
		else
		{
			Debug.LogError( string.Format("[Error] {0} == {1}", expected, actual ) );
		}
	}

	public static void AssertThrow<T>( System.Action codeblock ) where T : System.Exception
	{
		try
		{
			codeblock();
		}
		catch( T e )
		{
			Debug.Log( string.Format("[OK] Throw Exception. ( {0} : \"{1}\" )", typeof(T), e.Message ) );
			return;
		}

		Debug.Log( string.Format("[Error] Expected Exception. But No Exception. ( {0} )", typeof(T)) );
	}

	public static void Fail( string format, params Object[] args )
	{
		Debug.LogError( "[Fail] " + string.Format( format, args ) );
	}

	public static void Assert( bool expr, string format, params Object[] args )
	{
		if ( expr )
		{
			Debug.Log( "[OK] " + string.Format( format, args ) );
		}
		else
		{
			Debug.LogError( "[Error] " + string.Format( format, args ) );
		}
	}
}

}
