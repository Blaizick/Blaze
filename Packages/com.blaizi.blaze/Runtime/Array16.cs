using System;
using Blaze.Runtime.Math;

namespace Blaze.Runtime
{
    public class Array16<T>
    {
    	public uint size;
    	public T[] buffer;

    	private uint m_UnusedCount;
    	private ushort[] m_UnusedItems;

    	public Array16(uint arraySize)
    	{
    		this.size = arraySize;
    		this.buffer = new T[arraySize];
    		this.m_UnusedCount = arraySize - 1U;
    		this.m_UnusedItems = new ushort[arraySize];
    		for (uint num = 1U; num < arraySize; num += 1U)
    		{
    			this.m_UnusedItems[(int)((UIntPtr)(num - 1U))] = (ushort)num;
    		}
    		this.m_UnusedItems[(int)((UIntPtr)(this.m_UnusedCount++))] = 0;
    	}

    	public bool CreateItem(out ushort item)
    	{
    		if (this.m_UnusedCount == 0U)
    		{
    			item = 0;
    			return false;
    		}
    		item = this.m_UnusedItems[(int)((UIntPtr)(this.m_UnusedCount -= 1U))];
    		return true;
    	}

    	public bool CreateItem(out ushort item, ref Randomizer r)
    	{
    		if (this.m_UnusedCount == 0U)
    		{
    			item = 0;
    			return false;
    		}
    		int num = r.Int32(this.m_UnusedCount);
    		item = this.m_UnusedItems[num];
    		this.m_UnusedItems[num] = this.m_UnusedItems[(int)((UIntPtr)(this.m_UnusedCount -= 1U))];
    		return true;
    	}

    	public ushort NextFreeItem()
    	{
    		if (this.m_UnusedCount == 0U)
    		{
    			return 0;
    		}
    		return this.m_UnusedItems[(int)((UIntPtr)(this.m_UnusedCount - 1U))];
    	}

    	public ushort NextFreeItem(ref Randomizer r)
    	{
    		if (this.m_UnusedCount == 0U)
    		{
    			return 0;
    		}
    		int num = r.Int32(this.m_UnusedCount);
    		return this.m_UnusedItems[num];
    	}

    	public void ReleaseItem(ushort item)
    	{
    		this.m_UnusedItems[(int)((UIntPtr)(this.m_UnusedCount++))] = item;
    	}

    	public void ClearUnused()
    	{
    		this.m_UnusedCount = 0U;
    	}

    	public uint ItemCount()
    	{
    		return this.size - this.m_UnusedCount;
    	}
    }
}