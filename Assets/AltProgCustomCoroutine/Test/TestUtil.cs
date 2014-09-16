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
