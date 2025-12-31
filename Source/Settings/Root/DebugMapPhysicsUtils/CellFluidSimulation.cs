using System;

namespace Celeste.Mod.GooberHelper.Settings.Root.DebugMapPhysicsUtils {
    //REFERENCED CODE FROM https://github.com/jongallant/LiquidSimulator/blob/master/Assets/Scripts/LiquidSimulator.cs A LOT
    //THANK YOU
    public class CellFluidSimulation {
        public static readonly float MinValue = 0f;
        public static readonly float MaxValue = 1f;

        public static readonly float MaxCompression = 0f;

        public static readonly float MinFlow = 0f;
        public static readonly float MaxFlow = 4f;

        public static readonly float FlowSpeed = 1.5f;


        public VirtualMap<float> Cells;
        public VirtualMap<float> Diffs;
        public VirtualMap<bool> Tiles;
        public bool PlacedAnything = false;

        public CellFluidSimulation(VirtualMap<bool> tiles)
            => Tiles = tiles;

        public void AddFluid(Vector2 position, float amount, int radius = 2) {
            if(!PlacedAnything) {
                Cells = new VirtualMap<float>(Tiles.Columns, Tiles.Rows, 0);
                Diffs = new VirtualMap<float>(Tiles.Columns, Tiles.Rows, 0);

                PlacedAnything = true;
            }

            for(var x = -radius; x <= radius; x += 1) {
                for(var y = -radius; y <= radius; y += 1) {
                    var nx = (int)position.X + x;
                    var ny = (int)position.Y + y;

                    if(new Vector2(x, y).Length() > radius || Tiles[nx, ny])
                        continue;

                    Cells[nx, ny] = Math.Min(Cells[nx, ny] + amount * MathF.Exp(-MathF.Pow(new Vector2(x, y).Length() / radius * 3f, 2f)), MaxValue);
                }
            }
        }

        public static float CalculateVerticalFlow(float source, float destination) {
            var sum = source + destination;

            return sum <= MaxValue
                ? MaxValue

            : sum < 2f * MaxValue + MaxCompression
                ? (MaxValue * MaxValue + sum * MaxCompression) / (MaxValue + MaxCompression)

            : (sum + MaxCompression) / 2f;
        }

        public void Update() {
            if(!PlacedAnything)
                return;

            for(var x = 0; x < Cells.Columns; x++)
                for(var y = 0; y < Cells.Rows; y++)
                    Diffs[x, y] = 0;

            for(var x = 0; x < Cells.Columns; x++) {
                for(var y = 0; y < Cells.Rows; y++) {
                    var startValue = Cells[x, y];

                    if(startValue == 0)
                        continue;

                    if(startValue < MinValue)
                        startValue = 0;

                    var remainingValue = Cells[x, y];

                    void updateStuff(float flow, int xOffset, int yOffset) {
                        if(flow > MinFlow) flow *= FlowSpeed;
                        flow = Math.Clamp(flow, 0f, MathF.Min(MaxFlow, remainingValue));

                        remainingValue -= flow;
                        Diffs[x, y] -= flow;
                        Diffs[x + xOffset, y + yOffset] += flow;
                    }

                    bool noMoreFluid() {
                        if(remainingValue < MinValue) {
                            Diffs[x, y] -= remainingValue;

                            return true;
                        }
                        
                        return false;
                    }

                    //flowing down
                    if(!Tiles[x, y + 1] && y + 1 < Cells.Rows) {
                        var cellUnder = Cells[x, y + 1];
                        var flow = 1f * (CalculateVerticalFlow(remainingValue, cellUnder) - cellUnder);

                        updateStuff(flow, 0, 1);

                        if(noMoreFluid())
                            continue;
                    }


                    //flowing to the side
                    for(var dir = -1; dir <= 1; dir += 2) {
                        if(!Tiles[x + dir, y] && x + dir < Cells.Columns && x + dir >= 0) {
                            var cellSide = Cells[x + dir, y];
                            var flow = (remainingValue - cellSide) / 3;

                            updateStuff(flow, dir, 0);

                            if(noMoreFluid())
                                break;
                        }
                    }

                    if(noMoreFluid())
                        continue;

                    // flowing up
                    if(!Tiles[x, y - 1]) {
                        var cellAbove = Cells[x, y - 1];
                        var flow = 1f * (remainingValue - CalculateVerticalFlow(remainingValue, cellAbove));

                        updateStuff(flow, 0, -1);
                    }
                }
            }

            for(var x = 0; x < Cells.Columns; x++)
                for(var y = 0; y < Cells.Rows; y++)
                    Cells[x, y] = Math.Clamp(Cells[x, y] + Diffs[x, y], MinValue, MaxValue);
        }

        public void Render(Vector2 offset, Color color1, Color color2) {
            if(!PlacedAnything)
                return;

            for(var x = 0; x < Cells.Columns; x++) {
                for(var y = 0; y < Cells.Rows; y++) {
                    if(Tiles[x, y] == true)
                        continue;

                    var blendedColor = Color.Lerp(color1, color2, Math.Abs(Diffs[x, y] + Diffs[x, y - 1]) * 2f) * (1 - MathF.Pow(1 - (Cells[x, y] + Cells[x, y - 1]) / 2, 5)) * MaxValue;

                    Draw.Rect(offset.X + x, offset.Y + y, 1, 1, blendedColor);
                }
            }
        }
    }
}