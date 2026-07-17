using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Blaze.Runtime.World
{
    public class BlazeGrid : MonoBehaviour
    {
        public Vector2Int size;
        public BlazeTile tilePfb;

        public UnityEvent onResize = new();

        protected List<BlazeTile> m_Tiles = new();

        public virtual IEnumerator Init()
        {
            m_Tiles = new(new BlazeTile[size.x * size.y]);

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    CreateTile(new Vector2Int(x, y));
                }
            }

            yield break;
        }

        public virtual void CreateTile(Vector2Int pos)
        {
            var tile = Instantiate(tilePfb, GridToWorldPosition(pos), Quaternion.identity, transform);
            m_Tiles[pos.x + pos.y * size.x] = tile;

            tile.grid = this;
            tile.gridPosition = pos;

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
            Vector2Int oldSize = size;

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

            size = newSize;
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

            onResize.Invoke();
        }

        public virtual BlazeTile GetTileAt(int i)
        {
            return i < 0 || i >= m_Tiles.Count ? null : m_Tiles[i];
        }
        public virtual BlazeTile GetTileAt(Vector2Int pos)
        {
            if (pos.x < 0 || pos.y < 0 || pos.x >= size.x || pos.y >= size.y)
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
                pos.x >= this.size.x || pos.y >= this.size.y)
            {
                return false;
            }
            return true;
        }

        public virtual int GridPosToI(Vector2Int gPos)
        {
            return gPos.x + gPos.y * size.x;
        }

        public virtual Vector2 GetPosition()
        {
            return CenterPosition;
        }

        public virtual Vector2 CenterPosition
        {
            get
            {
                Vector2 pos = (Vector2)transform.position + new Vector2(size.x / 2, size.y / 2);
                if (size.x % 2 == 0)
                {
                    pos.x -= 0.5f;
                }
                if (size.y % 2 == 0)
                {
                    pos.y -= 0.5f;
                }
                return pos;
            }
        }
    }
}
