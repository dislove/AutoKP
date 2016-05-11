using System;
namespace ZYC
{
	public struct MemoryInformation
	{
		public double AvailablePageFile;
		public double AvailablePhysicalMemory;
		public double AvailableVirtualMemory;
		public uint SizeofStructure;
		public double MemoryInUse;
		public double TotalPageSize;
		public double TotalPhysicalMemory;
		public double TotalVirtualMemory;
	}
}
