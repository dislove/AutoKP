using System;
namespace ZYC
{
	public enum MouseEventFlag : uint
	{
		Move = 1u,
		LeftDown,
		LeftUp = 4u,
		RightDown = 8u,
		RightUp = 16u,
		MiddleDown = 32u,
		MiddleUp = 64u,
		XDown = 128u,
		XUp = 256u,
		Wheel = 2048u,
		VirtualDesk = 16384u,
		Absolute = 32768u
	}
}
