using UnityEngine;
using System.Collections;

namespace NkfLib.Unity
{
	/*
     * Texture2Dへの描画関数群.
     * できる限り、2の累乗のサイズのほうが速度が速い。
     */
	public class DrawTexture2D
	{
		private Texture2D m_tex;
		private int m_width, m_height;
		private Color[] m_linesColor = null;

		public void Begin(Texture2D tex)
		{
			m_tex    = tex;
			m_width  = m_tex.width;
			m_height = m_tex.height;

			if (m_linesColor == null || m_linesColor.Length != m_width)
			{
				m_linesColor = null;
				m_linesColor = new Color[m_width];
			}
		}

		public void End()
		{
			m_tex.Apply();
		}

		// 全面を指定色でクリア.
		public void Clear(Color col)
		{
			for (int i = 0; i < m_width; i++) m_linesColor[i] = col;
			for (int y = 0; y < m_height; y++) m_tex.SetPixels(0, y, m_width, 1, m_linesColor);
		}

		// 指定の位置にピクセル描画.
		public void SetPixel(int x, int y, Color col)
		{
			if (x < 0 || x >= m_width || y < 0 || y >= m_height) return;
			m_tex.SetPixel(x, y, col);
		}

		// 水平ラインの描画.
		private void DrawLineH(int x1, int y1, int x2, Color col, int dottedPixel = 0)
		{
			if (y1 < 0 || y1 >= m_height) return;
			if (x1 > x2)
			{
				int tmp = x1;
				x1 = x2;
				x2 = tmp;
			}
			if (x2 <= 0 || x1 >= m_width) return;
			if (x1 < 0) x1 = 0;
			if (x2 >= m_width) x2 = m_width - 1;
			for (int x = 0; x <= x2 - x1; x++) m_linesColor[x] = col;
#if false // 点線の追加
		m_tex.SetPixels(x1, y1, x2 - x1 + 1, 1, m_linesColor);
#else
			if (dottedPixel == 0)
			{
				for (int x = x1; x <= x2; x++)
				{
					m_tex.SetPixel(x, y1, col);
				}
			}
			else
			{
				// 点線の描画
				bool draw = false;
				for (int x = x1; x <= x2; x++)
				{
					if (x % dottedPixel == 0)
					{
						draw = !draw;
					}
					if (!draw) continue;
					m_tex.SetPixel(x, y1, col);
				}
			}
#endif
		}

		// 垂直ラインの描画.
		private void DrawLineV(int x1, int y1, int y2, Color col, int dottedPixel = 0)
		{
			if (x1 < 0 || x1 >= m_width) return;
			if (y1 > y2)
			{
				int tmp = y1;
				y1 = y2;
				y2 = tmp;
			}
			if (y2 <= 0 || y1 >= m_height) return;
			if (y1 < 0) y1 = 0;
			if (y2 >= m_height) y2 = m_height - 1;

#if false // 点線の追加
		for (int y = y1; y <= y2; y++) {
			m_tex.SetPixel(x1, y, col);
		}
#else
			if (dottedPixel == 0)
			{
				for (int y = y1; y <= y2; y++)
				{
					m_tex.SetPixel(x1, y, col);
				}
			}
			else
			{
				// 点線の描画
				bool draw = false;
				for (int y = y1; y <= y2; y++)
				{
					if (y % dottedPixel == 0)
					{
						draw = !draw;
					}
					if (!draw) continue;
					m_tex.SetPixel(x1, y, col);
				}
			}
#endif
		}

		// ラインの描画.
		// ・点線は水平、垂直ラインのみ有効
		public void DrawLine(int x1, int y1, int x2, int y2, Color col, int dottedPixel = 0)
		{
			if (x1 == x2)
			{
				DrawLineV(x1, y1, y2, col, dottedPixel);
				return;
			}
			if (y1 == y2)
			{
				DrawLineH(x1, y1, x2, col, dottedPixel);
				return;
			}

			// クリッピング.
			if (x1 < 0 && x2 < 0) return;
			if (x1 >= m_width && x2 >= m_width) return;
			if (y1 < 0 && y2 < 0) return;
			if (y1 >= m_height && y2 >= m_height) return;

			// スクリーン左とのクリッピング.
			if (x1 < 0 && x2 >= 0)
			{
				y1 = (y2 - y1) * (0 - x1) / (x2 - x1) + y1;
				x1 = 0;
			}
			else if (x2 < 0 && x1 >= 0)
			{
				y2 = (y2 - y1) * (0 - x1) / (x2 - x1) + y1;
				x2 = 0;
			}

			// スクリーン右とのクリッピング.
			if (x1 >= m_width && x2 < m_width)
			{
				y1 = (y2 - y1) * (m_width - x1) / (x2 - x1) + y1;
				x1 = m_width - 1;
			}
			else if (x2 >= m_width && x1 < m_width)
			{
				y2 = (y2 - y1) * (m_width - x1) / (x2 - x1) + y1;
				x2 = m_width - 1;
			}

			// スクリーン上とのクリッピング.
			if (y1 < 0 && y2 >= 0)
			{
				x1 = (x2 - x1) * (0 - y1) / (y2 - y1) + x1;
				y1 = 0;
			}
			else if (y2 < 0 && y1 >= 0)
			{
				x2 = (x2 - x1) * (0 - y1) / (y2 - y1) + x1;
				y2 = 0;
			}

			// スクリーン下とのクリッピング.
			if (y1 >= m_height && y2 < m_height)
			{
				x1 = (x2 - x1) * (m_height - y1) / (y2 - y1) + x1;
				y1 = m_height - 1;
			}
			else if (y2 >= m_height && y1 < m_height)
			{
				x2 = (x2 - x1) * (m_height - y1) / (y2 - y1) + x1;
				y2 = m_height - 1;
			}

			if (x1 < 0) x1 = 0;
			if (x2 < 0) x2 = 0;
			if (x1 >= m_width) x1 = m_width - 1;
			if (x2 >= m_width) x2 = m_width - 1;
			if (y1 < 0) y1 = 0;
			if (y2 < 0) y2 = 0;
			if (y1 >= m_height) y1 = m_height - 1;
			if (y2 >= m_height) y2 = m_height - 1;

			int a = Mathf.Abs(x2 - x1);
			int b = Mathf.Abs(y2 - y1);
			int dx = 1;
			int dy = 1;
			if (x1 > x2) dx = -1;
			if (y1 > y2) dy = -1;

			int e = 0;
			if (a > b)
			{
				int y = y1;
				for (int x = x1; x != x2; x += dx)
				{
					e += b;
					if (e > a)
					{
						e -= a;
						y += dy;
					}
					m_tex.SetPixel(x, y, col);
				}
			}
			else
			{
				int x = x1;
				for (int y = y1; y != y2; y += dy)
				{
					e += a;
					if (e > b)
					{
						e -= b;
						x += dx;
					}
					m_tex.SetPixel(x, y, col);
				}
			}
			m_tex.SetPixel(x2, y2, col);
		}

		// 矩形枠の描画.
		public void DrawRectangle(int x1, int y1, int x2, int y2, Color col)
		{
			DrawLineH(x1, y1, x2, col);
			DrawLineH(x1, y2, x2, col);
			DrawLineV(x1, y1, y2, col);
			DrawLineV(x2, y1, y2, col);
		}

		// 矩形の塗りつぶし描画.
		public void DrawRectangleFill(int x1, int y1, int x2, int y2, Color col)
		{
			if (x1 > x2)
			{
				int tmp = x1;
				x1 = x2;
				x2 = tmp;
			}
			if (y1 > y2)
			{
				int tmp = y1;
				y1 = y2;
				y2 = tmp;
			}
			if (x1 < 0 && x2 < 0) return;
			if (x1 >= m_width && x2 >= m_width) return;
			if (y1 < 0 && y2 < 0) return;
			if (y1 >= m_height && y2 >= m_height) return;
			if (x1 < 0) x1 = 0;
			if (x2 >= m_width) x2 = m_width - 1;
			if (y1 < 0) y1 = 0;
			if (y2 >= m_height - 1) y2 = m_height - 1;

			int scanLineSize = x2 - x1 + 1;
			for (int x = 0; x < scanLineSize; x++) m_linesColor[x] = col;
			for (int y = y1; y <= y2; y++) m_tex.SetPixels(x1, y, scanLineSize, 1, m_linesColor);
		}
	}
}