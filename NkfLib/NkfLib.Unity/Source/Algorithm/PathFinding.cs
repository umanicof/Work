using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NkfLib.Unity
{
	// 経路探索クラス
	public class PathFinding : MonoBehaviour
	{
		public static PathFinding Current;
		void Awake() { Current = this; }

		public class Node
		{
			// ノードが属する経路探索クラス
			PathFinding _owner;

			/// ステータス
			enum Status
			{
				Init,
				Opened,
				Closed,
			}
			Status _status = Status.Init;
			public bool IsInit { get { return _status == Status.Init; } }
			public bool IsOpened { get { return _status == Status.Opened; } }
			public bool IsClosed { get { return _status == Status.Closed; } }

			public bool Impassable { get { return PassedCost == Mathf.Infinity; } }

			// スコア
			public float Score { get { return Cost + _heuristic; } }

			// コスト
			public float PassedCost { get; set; } // 自身の通過コスト（Infinityは通過不可）
			float _parentCost;
			public float Cost { get { return _parentCost + PassedCost; } }

			// ヒューリスティック・コスト
			float _heuristic;

			// 親ノード
			public Node Parent { get; private set; }

			// 位置
			public Vector2Int Point { get; private set; }

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="point"></param>
			public Node(PathFinding owner, Vector2Int point)
			{
				_owner = owner;
				Point = point;
			}

			/// <summary>
			/// リセット
			/// ・計算結果を破棄して再計算可能な状態に戻す
			/// </summary>
			public void Reset()
			{
				Parent = null;
				_heuristic = 0;
				_parentCost = 0;
				_status = Status.Init;
			}

			/// <summary>
			/// ヒューリスティック・コストの計算.
			/// </summary>
			/// <param name="allowdiag"></param>
			/// <param name="xgoal"></param>
			/// <param name="ygoal"></param>
			void CalcHeuristic()
			{
				if (!_owner.ExecAstar) return;
				if (_owner.AllowedDiagonal)
				{
					// 斜め移動あり
					var dx = Mathf.Abs(_owner.Goal.x - Point.x);
					var dy = Mathf.Abs(_owner.Goal.y - Point.y);
					// 大きい方をコストにする
					_heuristic =  dx > dy ? dx : dy;
				}
				else
				{
					// 縦横移動のみ
					var dx = Mathf.Abs(_owner.Goal.x - Point.x);
					var dy = Mathf.Abs(_owner.Goal.y - Point.y);
					_heuristic = dx + dy;
				}
				//Dump();
			}

			/// <summary>
			/// オープン
			/// </summary>
			/// <param name="parent"></param>
			/// <param name="cost"></param>
			public bool Open(Node parent)
			{
				//Debug.Log (string.Format("Open: ({0},{1})", Position.x, Position.y));

				// 初期状態でない
				if (!IsInit) return false;

				// オープン
				Parent = parent;
				_status = Status.Opened;
				_parentCost = parent == null ? 0 : parent.Cost;
				CalcHeuristic();    // ヒューリスティック・コストを計算

				_owner._openNodes.Add(this);

				return true;
			}

			/// <summary>
			/// クローズ
			/// </summary>
			public bool Close()
			{
				//Debug.Log (string.Format ("Closed: ({0},{1})", Position.x, Position.y));

				// オープンでない
				if (!IsOpened) return false;

				_status = Status.Closed;
				_owner._openNodes.Remove(this);

				return true;
			}

			/// <summary>
			/// 経路（ルートの親までの位置のリスト）を取得
			/// ・リストは必ず１以上、終端ノードが含まれる
			/// </summary>
			/// <param name="list"></param>
			public List<Vector2> GetPath()
			{
				var path = new List<Vector2>();
#if true
				// 自身を含む
				Node node = this;
				do
				{
					path.Add(node.Point);
					node = node.Parent;
				} while (node != null);
#elif false
			// 自身を含まない
			Node node = Parent;
			while (node != null) {
				path.Add(node.Point);
				node = node.Parent;
			};
#else
			// 始端ノードは含まれない
			path.Add(Point);
			Node node = Parent;
			while (node != null) {
				path.Add(node.Point);
				node = node.Parent;
			};
#endif
				return path;
			}

			/// <summary>
			/// ダンプ出力
			/// </summary>
			public void Dump()
			{
				Debug.Log(string.Format("({0},{1})[{2}] cost={3} heuris={4} score={5}", Point.x, Point.y, _status, Cost, _heuristic, Score));
			}
			public void DumpRecursive()
			{
				Dump();
				if (Parent != null)
				{
					// 再帰的にダンプする.
					Parent.DumpRecursive();
				}
			}
		}

		// 斜め移動を許可するかどうか.
		public bool AllowedDiagonal { get; set; } = false;

		/// <summary>
		/// 実行種別
		/// ・ダイクストラ法を使用して、全経路を計算する
		/// </summary>
		enum ExecType
		{
			None,       // 実行なし
			Astar,      // ダイクストラ法を使用して、全経路を計算
			Dijkstra,   // A-starを使用して、最短経路を計算
		}
		ExecType _execType;
		bool ExecNone { get { return _execType == ExecType.None; } }
		bool ExecDijkstra { get { return _execType == ExecType.Dijkstra; } }
		bool ExecAstar { get { return _execType == ExecType.Astar; } }

		// ゴール位置
		Vector2Int Goal { get; set; }

		// オープンノードリスト
		List<Node> _openNodes = new List<Node>();

		// ノードリスト
		Dictionary<Vector2Int, Node> _nodes = new Dictionary<Vector2Int, Node>();

		/// <summary>
		/// リセット
		/// ・計算結果を破棄して再計算可能な状態に戻す
		/// ・ゴール位置を変えた場合はリセットが必要
		/// </summary>
		/// <returns></returns>
		public void Reset()
		{
			_execType = ExecType.None;
			_openNodes.Clear();
			foreach (var node in _nodes)
			{
				node.Value.Reset();
			}
		}

		/// <summary>
		/// ノード取得
		/// </summary>
		/// <returns></returns>
		public Node GetNode(Vector2Int point)
		{
			_nodes.TryGetValue(point, out Node node);
			return node;
		}

		/// <summary>
		/// 指定の位置に隣接している通過可能なノード取得
		/// ・始点・終点が通行不可
		/// </summary>
		/// <returns></returns>
		Node GetAdjoinPassableNode(Vector2 position)
		{
			// 自身の点を確認
			var point = position.RoundToInt();
			var node = GetNode(point);
			if (node != null && !node.Impassable) return node;

			var shiftX = position.x - point.x;
			var shiftY = position.y - point.y;

			int offsetX = (int)Mathf.Sign(shiftX);
			int offsetY = (int)Mathf.Sign(shiftY);
			if (Mathf.Abs(shiftX) >= Mathf.Abs(shiftY))
			{
				node = GetNode(new Vector2Int(point.x + offsetX, point.y));
				if (node != null && !node.Impassable) return node;
				node = GetNode(new Vector2Int(point.x, point.y + offsetY));
				if (node != null && !node.Impassable) return node;
			}
			else
			{
				node = GetNode(new Vector2Int(point.x, point.y + offsetY));
				if (node != null && !node.Impassable) return node;
				node = GetNode(new Vector2Int(point.x + offsetX, point.y));
				if (node != null && !node.Impassable) return node;
			}
			node = GetNode(new Vector2Int(point.x + offsetX, point.y + offsetY));
			if (node != null && !node.Impassable) return node;

			return null;
		}

		/// <summary>
		/// ノードの登録
		/// ・passedCostの最小は1。通りにくさに応じて増やすこと。
		///   通行不可のノードであれば Mathf.Infinity
		/// ・同じ位置にノードが登録されたら、passedCostは大きい方で上書きする
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public Node AddNode(Vector2Int point, float passedCost = 1)
		{
			var node = GetNode(point);
			if (node == null)
			{
				node = new Node(this, point);
			}

			// 通過コストの登録
			node.PassedCost = Mathf.Max(node.PassedCost, passedCost);
			_nodes[node.Point] = node;
			return node;
		}

		/// <summary>
		/// ノードの削除
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public Node RemoveNode(Vector2Int point)
		{
			var node = GetNode(point);
			if (node != null)
			{
				_nodes.Remove(node.Point);
			}
			return node;
		}

		public void RemoveNodeAll()
		{
			_nodes.Clear();
		}

		/// <summary>
		/// 周りのノードをオープン
		/// </summary>
		/// <param name="parent"></param>
		void OpenAround(Node parent)
		{
			// 通行不可
			// ・通行不可ノードは、始点としては機能するが、他のノードにつながらない
			if (parent.Impassable) return;

			var xbase = parent.Point.x; // 基準座標(X).
			var ybase = parent.Point.y; // 基準座標(Y).
			if (AllowedDiagonal)
			{
				// 8方向を開く
				for (int j = 0; j < 3; j++)
				{
					for (int i = 0; i < 3; i++)
					{
						var x = xbase + i - 1; // -1～1
						var y = ybase + j - 1; // -1～1
						GetNode(new Vector2Int(x, y))?.Open(parent);
					}
				}
			}
			else
			{
				// 4方向を開く
				var x = xbase;
				var y = ybase;
				GetNode(new Vector2Int(x-1, y))?.Open(parent); // 右
				GetNode(new Vector2Int(x+1, y))?.Open(parent); // 左
				GetNode(new Vector2Int(x, y-1))?.Open(parent); // 上
				GetNode(new Vector2Int(x, y+1))?.Open(parent); // 下
			}
		}

		/// <summary>
		/// オープンリストから最小スコアのノードを取得
		/// </summary>
		/// <returns></returns>
		Node SearchMinScoreOpenedNode()
		{
			// 最小スコア
			float min = Mathf.Infinity;
			// 最小実コスト
			float minCost = Mathf.Infinity;
			Node minNode = null;

			foreach (Node node in _openNodes)
			{
				var score = node.Score;
				if (score > min) continue; // スコアが大きい
				if (score == min)
				{ // スコアが同じ場合はランダムで取捨選択
					if (Random.value < 0.5f) continue;
				}

				// 最小値更新.
				min = score;
				minCost = node.Cost;
				minNode = node;
			}
			return minNode;
		}

		/// <summary>
		/// A-starによる経路探索
		/// ・始点に障害物がある場合は経路に含める
		///   終点に障害物がある場合は経路に含めない
		/// </summary>
		/// <returns></returns>
		public List<Vector2> FindingPathByAstar(Vector2 start, Vector2 goal)
		{
			var startPoint = start.RoundToInt();
			var nextNode = GetAdjoinPassableNode(start);
			var goalNode = GetAdjoinPassableNode(goal);

			var path = new List<Vector2>();
			if (nextNode == null || goalNode == null) return path;

			// リセット
			if (!ExecNone)
			{
				Reset();
			}

			_execType = ExecType.Astar;
			Goal = goalNode.Point;

			var node = goalNode;
			node.Open(null);
			while (true)
			{
				node.Close();
				OpenAround(node);
				// 最小スコアのオープンノードを探す
				node = SearchMinScoreOpenedNode();
				if (node == null) return path; // 袋小路
				if (node == nextNode)
				{
					// パスを取得
					//node.DumpRecursive();
					path = node.GetPath();
					if (startPoint != nextNode.Point)
					{
						path.Insert(0, startPoint);
					}
					return path;
				}
			}
		}

		/// <summary>
		/// ダイクストラ法による経路探索
		/// ・始点に障害物がある場合は経路に含める
		///   終点に障害物がある場合は経路に含めない
		/// </summary>
		/// <returns></returns>
		public List<Vector2> FindingPathByDijkstra(Vector2 start, Vector2 goal)
		{
			var startPoint = start.RoundToInt();
			var nextNode = GetAdjoinPassableNode(start);
			var goalNode = GetAdjoinPassableNode(goal);

			var path = new List<Vector2>();
			if (nextNode == null || goalNode == null) return path;

			// ダイクストラ法で実行済み ＆ ゴール位置に変化なし
			if (ExecDijkstra && goalNode.Point == Goal)
			{
				if (!nextNode.IsClosed) return path; // ゴールに繋がっていない
													 // パスを取得
				path = nextNode.GetPath();
				if (startPoint != nextNode.Point)
				{
					path.Insert(0, startPoint);
				}
				return path;
			}

			// リセット
			if (!ExecNone)
			{
				Reset();
			}

			_execType = ExecType.Dijkstra;
			Goal = goalNode.Point;

			var node = goalNode;
			node.Open(null);
			while (true)
			{
				node.Close();
				OpenAround(node);
				// 最小スコアのオープンノードを探す
				node = SearchMinScoreOpenedNode();
				if (node == null) break; // 袋小路
			}

			if (!nextNode.IsClosed) return path; // ゴールに繋がっていない

			// パスを取得
			//startNode.DumpRecursive();
			path = nextNode.GetPath();
			if (startPoint != nextNode.Point)
			{
				path.Insert(0, startPoint);
			}
			return path;
		}
	}
}