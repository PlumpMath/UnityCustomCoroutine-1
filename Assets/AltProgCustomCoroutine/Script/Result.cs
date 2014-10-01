// Modification of https://github.com/muscly/unity3d-extensions/blob/master/Threading/Dispatcher.cs
using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using Object = System.Object;

namespace AltProg
{

public class IResult
{
}

public class Result<T> : IResult
{
	T r;

	public Result( T r )
	{
		this.r = r;
	}

	internal T GetResult()
	{
		return r;
	}

}

public static class Result
{
	public static Result<T> New<T>( T r )
	{
		return new Result<T>(r);
	}
}

}
