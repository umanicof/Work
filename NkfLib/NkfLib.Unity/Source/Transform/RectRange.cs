using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    public class RectRange : MonoBehaviour
    {
        public Rect Rect = default;
        enum Axiz { x, y, z }
        [SerializeField] Axiz _zeroSizeAxiz = Axiz.y;

        public float xMax { get { return Rect.xMax; } }
        public float xMin { get { return Rect.xMin; } }
        public float yMax { get { return Rect.yMax; } }
        public float yMin { get { return Rect.xMin; } }
        public int col { get { return Mathf.RoundToInt(xMax - xMin); } }
        public int row { get { return Mathf.RoundToInt(yMax - yMin); } }
        public int size { get { return Mathf.RoundToInt(col * row); } }

        void OnDrawGizmos()
        {
            if (Rect == Rect.zero)
                return;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

            Vector3 position = Vector3.zero;
            Vector3 size = Vector3.zero;
            switch (_zeroSizeAxiz)
            {
                case Axiz.x:
                    position = new Vector3(0.0f, Rect.x, Rect.y);
                    size =  new Vector3(0.1f, Rect.width, Rect.height);
                    break;
                case Axiz.y:
                    position = new Vector3(Rect.x, 0.0f, Rect.y);
                    size =  new Vector3(Rect.width, 0.1f, Rect.height);
                    break;
                case Axiz.z:
                    position = new Vector3(Rect.x, Rect.y, 0.0f);
                    size =  new Vector3(Rect.width, Rect.height, 0.1f);
                    break;
            }

            Gizmos.DrawWireCube(position, size);
        }
    }
}