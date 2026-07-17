
namespace Blaze.Runtime.Math
{
    public struct Randomizer
	{
		public ulong seed;

		public Randomizer(int _seed)
		{
			this.seed = (ulong)(6364136223846793005L * (long)_seed + 1442695040888963407L);
		}

		public Randomizer(uint _seed)
		{
			this.seed = 6364136223846793005UL * (ulong)_seed + 1442695040888963407UL;
		}

		public Randomizer(long _seed)
		{
			this.seed = (ulong)_seed;
		}

		public Randomizer(ulong _seed)
		{
			this.seed = _seed;
		}

		public int Bits32(int num)
		{
			int num2 = (int)(this.seed >> 64 - num);
			this.seed = 6364136223846793005UL * this.seed + 1442695040888963407UL;
			return num2;
		}

		public int Int32(uint range)
		{
			int num = (int)((this.seed >> 32) * (ulong)range >> 32);
			this.seed = 6364136223846793005UL * this.seed + 1442695040888963407UL;
			return num;
		}

		public int Int32(int min, int max)
		{
			int num = min + (int)((this.seed >> 32) * (ulong)(max - min + 1) >> 32);
			this.seed = 6364136223846793005UL * this.seed + 1442695040888963407UL;
			return num;
		}

		public uint UInt32(uint range)
		{
			uint num = (uint)((this.seed >> 32) * (ulong)range >> 32);
			this.seed = 6364136223846793005UL * this.seed + 1442695040888963407UL;
			return num;
		}

		public uint UInt32(uint min, uint max)
		{
			uint num = min + (uint)((this.seed >> 32) * (ulong)(max - min + 1U) >> 32);
			this.seed = 6364136223846793005UL * this.seed + 1442695040888963407UL;
			return num;
		}

		public ulong ULong64()
		{
			ulong num = this.seed;
			this.seed = 6364136223846793005UL * this.seed + 1442695040888963407UL;
			return num;
		}
	}
}