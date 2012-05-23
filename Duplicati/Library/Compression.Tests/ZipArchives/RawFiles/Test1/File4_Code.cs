using Microsoft.Xna.Framework;

namespace DefeatInDetail.Library.Camera
{
    public class Camera : ICamera
    {
        #region Properties
        public Vector2 Position { get; set; }
        public float Zoom { get; set; }

        private float _rotation;
        private Vector2 _origin;
        private bool _updateMatrix;
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private float _maxZoom { get; set; }
        private float _minZoom { get; set; }
        private Rectangle _viewport { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        private Matrix _transform = Matrix.Identity;
        private float _maxWorldWidth;
        private float _maxWorldHeight;
        #endregion

        public void Initialize(float worldWidth, float worldHeight)
        {
            _rotation = 0.0f;
            Position = new Vector2(0, 0);
            Zoom = 1f;

            // ReSharper disable PossibleLossOfFraction
            _origin = new Vector2(_viewport.Width / 2, _viewport.Height / 2);
            // ReSharper restore PossibleLossOfFraction

            _updateMatrix = true;

            _maxWorldWidth = worldWidth;
            _maxWorldHeight = worldHeight;
        }

        public void UpdateDelta(Vector2 position)
        {
            var newPosition = new Vector2
                                  {
                                      X = MathHelper.Clamp(Position.X - position.X, 0, _maxWorldWidth),
                                      Y = MathHelper.Clamp(Position.Y - position.Y, 0, _maxWorldHeight)
                                  };
            Position = newPosition;

            _updateMatrix = true;
        }

        public void UpdatePosition(Vector2 position)
        {
            var newPosition = new Vector2
                                  {
                                      X = MathHelper.Clamp(position.X, 0, _maxWorldWidth),
                                      Y = MathHelper.Clamp(position.Y, 0, _maxWorldHeight)
                                  };
            Position = newPosition;

            _updateMatrix = true;
        }

        public void ZoomBy(float amount)
        {
            Zoom += amount;
            Zoom = MathHelper.Clamp(Zoom, _maxZoom, _minZoom);
            _updateMatrix = true;
        }

        public Matrix TransformMatrix()
        {
            if (_updateMatrix)
            {
                _transform = Matrix.CreateTranslation(new Vector3(-Position, 0)) *
                            Matrix.CreateRotationZ(_rotation) *
                            Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                            Matrix.CreateTranslation(new Vector3(_origin, 0));

                _updateMatrix = false;
            }

            return _transform;
        }
    }
}
