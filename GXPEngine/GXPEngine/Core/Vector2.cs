using System;

namespace GXPEngine.Core
{
	public struct Vector2
	{
		public float x;
		public float y;
		
		public Vector2 (float x, float y)
		{
			this.x = x;
			this.y = y;
		}
		
		override public string ToString() {
			return "[Vector2 " + x + ", " + y + "]";
		}

		public float GetLength() {
			return (float)Math.Sqrt(x * x + y * y);
        }

		public void Normalize() //normalize vector
        {
			float distance = GetLength();
			this = new Vector2(x / distance, y / distance);
        }

		public void SetLength(float length) //set length of vector
        {
			Normalize();
			this = new Vector2(x * length, y * length);
        }

		public float GetLookAtAngle(Vector2 target) //get angle from 1 vector to another
        {
			return ((float)Math.Atan2(target.y - y, target.x - x) * (180 / (float)Math.PI)) + 90;
        }

		public float GetDistance(Vector2 target) //get distance between 2 vectors
        {
			return (Mathf.Sqrt(Mathf.Pow(x - target.x, 2) + Mathf.Pow(y - target.y, 2)));
        }
	}
}

