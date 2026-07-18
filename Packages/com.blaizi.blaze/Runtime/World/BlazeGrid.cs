using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

namespace Blaze.Runtime.World
{
    public class BlazeGrid : ManagedBehaviour
    {
        [SerializeField]
        protected Vector2Int m_Size;
        [SerializeField]
        protected BlazeTile m_TilePfb;

        [SerializeField]
        protected UnityEvent m_OnResize = new();

        protected List<BlazeTile> m_Tiles = new();

        public virtual Vector2Int Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return m_Size;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                m_Size = value;
            }
        }
        public virtual BlazeTile TilePfb
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return m_TilePfb;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                m_TilePfb = value;
            }
        }
        public virtual UnityEvent OnResize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return m_OnResize;
            }
        }
        public virtual IReadOnlyList<BlazeTile> Tiles 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return m_Tiles.AsReadOnly();
            }
        }

        public virtual Vector2 CenterPosition
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Vector2 pos = (Vector2)transform.position + new Vector2(Size.x / 2, Size.y / 2);
                if (Size.x % 2 == 0)
                {
                    pos.x -= 0.5f;
                }
                if (Size.y % 2 == 0)
                {
                    pos.y -= 0.5f;
                }
                return pos;
            }
        }

        public virtual void Init()
        {
            m_Tiles = new(new BlazeTile[Size.x * Size.y]);

            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    CreateTile(new Vector2Int(x, y));
                }
            }
        }

        public virtual void CreateTile(Vector2Int position)
        {
            var tile = Instantiate(GetTilePfbForPosition(position), GridToWorldPosition(position), Quaternion.identity, transform);
            m_Tiles[position.x + position.y * Size.x] = tile;

            tile.grid = this;
            tile.gridPosition = position;

            QCoroutineRunner.Instance.StartCoroutine(tile.Init());
        }

        public virtual void DestroyTile(Vector2Int pos)
        {
            int i = GridPosToI(pos);
            var tile = GetTileAt(i);
            if (!tile)
            {
                return;
            }
            tile._OnDestroy();
            SetTileAt(i, null);
            Destroy(tile.gameObject);
        }

        public void SetTileAt(Vector2Int pos, BlazeTile tile)
        {
            SetTileAt(GridPosToI(pos), tile);
        }

        public void SetTileAt(int i, BlazeTile tile)
        {
            if (i < 0 || i >= m_Tiles.Count)
            {
                return;
            }
            m_Tiles[i] = tile;
        }

        public virtual void Resize(Vector2Int newSize)
        {
            Vector2Int oldSize = Size;

            Vector2Int minSize = new Vector2Int(Mathf.Min(oldSize.x, newSize.x), Mathf.Min(oldSize.y, newSize.y));
            Vector2Int maxSize = new Vector2Int(Mathf.Max(oldSize.x, newSize.x), Mathf.Max(oldSize.y, newSize.y));

            for (int x = 0; x < maxSize.x; x++)
            {
                for (int y = 0; y < maxSize.y; y++)
                {
                    if ((x < oldSize.x && y < oldSize.y) && (x >= newSize.x || y >= newSize.y))
                    {
                        DestroyTile(new Vector2Int(x, y));
                    }
                }
            }
            
            List<BlazeTile> newTiles = new List<BlazeTile>(new BlazeTile[newSize.x * newSize.y]);
            for (int x = 0; x < minSize.x; x++)
            {
                for (int y = 0; y < minSize.y; y++)
                {
                    BlazeTile tile = m_Tiles[x + y * oldSize.x];
                    if (tile != null)
                    {
                        newTiles[x + y * newSize.x] = tile;
                    }
                }
            }

            Size = newSize;
            m_Tiles = newTiles;

            // Create new tiles.
            for (int x = 0; x < newSize.x; x++)
            {
                for (int y = 0; y < newSize.y; y++)
                {
                    if (x >= oldSize.x || y >= oldSize.y)
                    {
                        CreateTile(new Vector2Int(x, y));
                    }
                }
            }

            m_OnResize.Invoke();
        }

        public virtual BlazeTile GetTileAt(int i)
        {
            return i < 0 || i >= m_Tiles.Count ? null : m_Tiles[i];
        }
        public virtual BlazeTile GetTileAt(Vector2Int pos)
        {
            if (pos.x < 0 || pos.y < 0 || pos.x >= Size.x || pos.y >= Size.y)
            {
                return null;
            }
            return GetTileAt(GridPosToI(pos));
        }

        public virtual Vector2 GridToWorldPosition(Vector2Int gPos)
        {
            return (Vector2)gPos + (Vector2)transform.position;
        }

        public virtual Vector2Int WorldToGridPosition(Vector2 pos)
        {
            Vector2 tmp = pos - (Vector2)transform.position;
            if (tmp.x > 0.0f)
            {
                tmp.x += 0.5f;
            }
            if (tmp.y > 0.0f)
            {
                tmp.y += 0.5f;
            }
            if (tmp.x < 0.0f)
            {
                tmp.x -= 0.5f;                
            }
            if (tmp.y < 0.0f)
            {
                tmp.y -= 0.5f;
            }
            return new Vector2Int((int)tmp.x, (int)tmp.y);
        }

        public virtual Vector2 GetRectPosition(Vector2Int pos, int size)
        {
            int hs = size / 2;
            return GridToWorldPosition(pos + new Vector2Int(hs, hs)) - (size % 2 == 0 ? Vector2.one / 2 : Vector2.zero);
        }

        public virtual List<BlazeTile> GetTilesInRect(RectInt rect)
        {
            List<BlazeTile> list = new();
            for (int x = 0; x < rect.width; x++)
            {
                for (int y = 0; y < rect.height; y++)
                {
                    int cx = x + rect.x;
                    int cy = y + rect.y;
                    if (IsPositionInBounds(new Vector2Int(cx, cy)))
                    {
                        list.Add(m_Tiles[GridPosToI(new Vector2Int(cx, cy))]);
                    }
                }
            }
            return list;
        }

        public virtual bool IsPositionInBounds(Vector2Int pos)
        {
            if (pos.x < 0 || pos.y < 0 ||
                pos.x >= this.Size.x || pos.y >= this.Size.y)
            {
                return false;
            }
            return true;
        }


        public virtual bool IsRectInBounds(RectInt rect)
        {
            return rect.xMin >= 0 && rect.yMin >= 0 && rect.xMax < Size.x && rect.yMax < Size.y;
        }

        public virtual int GridPosToI(Vector2Int gPos)
        {
            return gPos.x + gPos.y * Size.x;
        }

        public virtual Vector2 GetPosition()
        {
            return CenterPosition;
        }

        protected virtual BlazeTile GetTilePfbForPosition(Vector2Int position)
        {
            return TilePfb;
        }
    }
}
