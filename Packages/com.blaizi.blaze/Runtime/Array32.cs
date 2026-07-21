using System;
using Blaze.Runtime.Math;

namespace Blaze.Runtime
{
    public class Array32<T>
    {
    	public uint size;
    	public T[] buffer;
    	private uint m_UnusedCount;
    	private uint[] m_UnusedItems;

    	public Array32(uint arraySize)
    	{
    		this.size = arraySize;
    		this.buffer = new T[arraySize];
    		this.m_UnusedCount = arraySize - 1U;
    		this.m_UnusedItems = new uint[arraySize];
    		for (uint num = 1U; num < arraySize; num += 1U)
    		{
    			this.m_UnusedItems[(int)((UIntPtr)(num - 1U))] = num;
    		}
    		this.m_UnusedItems[(int)((UIntPtr)(this.m_UnusedCount++))] = 0U;
    	}

    	public bool CreateItem(out uint item)
    	{
    		if (this.m_UnusedCount == 0U)
    		{
    			item = 0U;
    			return false;
    		}
    		item = this.m_UnusedItems[(int)((UIntPtr)(this.m_UnusedCount -= 1U))];
    		return true;
    	}

    	public bool CreateItem(out uint item, ref Randomizer r)
    	{
    		if (this.m_UnusedCount == 0U)
    		{
    			item = 0U;
    			return false;
    		}
    		int num = r.Int32(this.m_UnusedCount);
    		item = this.m_UnusedItems[num];
    		this.m_UnusedItems[num] = this.m_UnusedItems[(int)((UIntPtr)(this.m_UnusedCount -= 1U))];
    		return true;
    	}

    	public uint NextFreeItem()
    	{
    		if (this.m_UnusedCount == 0U)
    		{
    			return 0U;
    		}
    		return this.m_UnusedItems[(int)((UIntPtr)(this.m_UnusedCount - 1U))];
    	}

    	public uint NextFreeItem(ref Randomizer r)
    	{
    		if (this.m_UnusedCount == 0U)
    		{
    			return 0U;
    		}
    		int num = r.Int32(this.m_UnusedCount);
    		return this.m_UnusedItems[num];
    	}

    	public void ReleaseItem(uint item)
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