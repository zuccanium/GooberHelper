namespace Celeste.Mod.GooberHelper.DataStructures {
    public class DummyCircle {
        public float Radius { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        
        public DummyCircle(float radius, float x, float y) {
            Radius = radius;
            X = x;
            Y = y;
        }

        public DummyCircle(Circle circle) {
            Radius = circle.Radius;
            X = circle.Position.X;
            Y = circle.Position.Y;
        }

        public DummyCircle() {}

        public Circle ToCircle()
            => new(Radius, X, Y);
    }
}