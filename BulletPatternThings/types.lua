-- the code to generate this can be found in Helpers/LuaHelper.cs

---@type Bullet
Bullet = require("#Celeste.Mod.GooberHelper.Entities.Bullet")

---@class Bullet
---@field level Level
---@field Parent BulletActivator
---@field Velocity Vector2
---@field Acceleration Vector2
---@field Color Color
---@field Texture string
---@field Scale number
---@field Effect string
---@field Additive boolean
---@field LowResolution boolean
---@field Rotation number
---@field RotationMode BulletRotationMode
---@field CullDistance number
---@field RotationCenter Vector2
---@field PositionRotationSpeed number
---@field VelocityRotationSpeed number
---@field Friction number
---@field PlayerCollider PlayerCollider
---@field Position Vector2
---@field ColliderRadius number
---@field ActualPosition Vector2
---@field Depth number
---@overload fun(parent: BulletActivator, template: BulletTemplate, position: Vector2?, velocity: Vector2?, acceleration: Vector2?, color: Color?, texture: string, scale: table, effect: string, additive: table, lowResolution: table, rotation: table, colliderRadius: table, cullDistance: table, rotationCenter: Vector2?, positionRotationSpeed: table, velocityRotationSpeed: table, friction: table): Bullet
local Bullet_ = {}

---@return nil
---@param key string
---@param to table
---@param time number
---@param easer Easer
function Bullet_:InterpolateValue(key, to, time, easer) return {} end

---@return nil
function Bullet_:RemoveSelf() return {} end


---@type BulletActivator
BulletActivator = require("#Celeste.Mod.GooberHelper.Entities.BulletActivator")

---@class BulletActivator
---@field BulletFieldCenter Vector2
---@field Activated boolean
---@field ShaderPath string
---@field Depth number
---@overload fun(data: EntityData, offset: Vector2): BulletActivator
local BulletActivator_ = {}

---@return BetterCoroutine
---@param coroutine LuaCoroutine
function BulletActivator_:AddLuaCoroutine(coroutine) return {} end

---@return boolean
function BulletActivator_:CheckFlags() return false end

---@return nil
function BulletActivator_:Activate() return {} end

---@return nil
function BulletActivator_.CmdReloadLua() return {} end

---@return nil
function BulletActivator_:RemoveSelf() return {} end


---@type BulletTemplate
BulletTemplate = require("#Celeste.Mod.GooberHelper.BulletTemplate")

---@class BulletTemplate
---@field Velocity Vector2?
---@field Acceleration Vector2?
---@field Color Color?
---@field Texture string
---@field Scale number?
---@field Effect string
---@field Additive boolean?
---@field LowResolution boolean?
---@field Rotation number?
---@field ColliderRadius number?
---@field CullDistance number?
---@field RotationCenter Vector2?
---@field PositionRotationSpeed number?
---@field VelocityRotationSpeed number?
---@field Friction number?
---@overload fun(velocity: Vector2?, acceleration: Vector2?, color: Color?, texture: string, scale: table, effect: string, additive: table, lowResolution: table, rotation: table, colliderRadius: table, cullDistance: table, rotationCenter: Vector2?, positionRotationSpeed: table, velocityRotationSpeed: table, friction: table): BulletTemplate
---@operator add(BulletTemplate): BulletTemplate
local BulletTemplate_ = {}

---@return nil
---@param bullet Bullet
function BulletTemplate_:ApplyToBullet(bullet) return {} end

---@return nil
---@param template BulletTemplate
function BulletTemplate_:ApplyToBulletTemplate(template) return {} end


---@type Vector2
Vector2 = require("#Microsoft.Xna.Framework.Vector2")

---@class Vector2
---@field X number
---@field Y number
---@field Zero Vector2
---@field One Vector2
---@field UnitX Vector2
---@field UnitY Vector2
---@overload fun(x: number, y: number): Vector2
---@overload fun(value: number): Vector2
---@operator unm(): Vector2
---@operator add(Vector2): Vector2
---@operator sub(Vector2): Vector2
---@operator mul(Vector2): Vector2
---@operator mul(number): Vector2
---@operator mul(Vector2): Vector2
---@operator div(Vector2): Vector2
---@operator div(number): Vector2
local Vector2_ = {}

---@return boolean
---@param other Vector2
function Vector2_:Equals(other) return false end

---@return number
function Vector2_:Length() return 0 end

---@return number
function Vector2_:LengthSquared() return 0 end

---@return nil
function Vector2_:Normalize() return {} end

---@return Vector2
---@param value1 Vector2
---@param value2 Vector2
---@overload fun(value1: Vector2, value2: Vector2, result: Vector2): nil
function Vector2_.Add(value1, value2) return {} end

---@return Vector2
---@param value1 Vector2
---@param value2 Vector2
---@param value3 Vector2
---@param amount1 number
---@param amount2 number
---@overload fun(value1: Vector2, value2: Vector2, value3: Vector2, amount1: number, amount2: number, result: Vector2): nil
function Vector2_.Barycentric(value1, value2, value3, amount1, amount2) return {} end

---@return Vector2
---@param value1 Vector2
---@param value2 Vector2
---@param value3 Vector2
---@param value4 Vector2
---@param amount number
---@overload fun(value1: Vector2, value2: Vector2, value3: Vector2, value4: Vector2, amount: number, result: Vector2): nil
function Vector2_.CatmullRom(value1, value2, value3, value4, amount) return {} end

---@return Vector2
---@param value1 Vector2
---@param min Vector2
---@param max Vector2
---@overload fun(value1: Vector2, min: Vector2, max: Vector2, result: Vector2): nil
function Vector2_.Clamp(value1, min, max) return {} end

---@return number
---@param value1 Vector2
---@param value2 Vector2
---@overload fun(value1: Vector2, value2: Vector2, result: number): nil
function Vector2_.Distance(value1, value2) return 0 end

---@return number
---@param value1 Vector2
---@param value2 Vector2
---@overload fun(value1: Vector2, value2: Vector2, result: number): nil
function Vector2_.DistanceSquared(value1, value2) return 0 end

---@return Vector2
---@param value1 Vector2
---@param value2 Vector2
---@overload fun(value1: Vector2, value2: Vector2, result: Vector2): nil
---@overload fun(value1: Vector2, divider: number): Vector2
---@overload fun(value1: Vector2, divider: number, result: Vector2): nil
function Vector2_.Divide(value1, value2) return {} end

---@return number
---@param value1 Vector2
---@param value2 Vector2
---@overload fun(value1: Vector2, value2: Vector2, result: number): nil
function Vector2_.Dot(value1, value2) return 0 end

---@return Vector2
---@param value1 Vector2
---@param tangent1 Vector2
---@param value2 Vector2
---@param tangent2 Vector2
---@param amount number
---@overload fun(value1: Vector2, tangent1: Vector2, value2: Vector2, tangent2: Vector2, amount: number, result: Vector2): nil
function Vector2_.Hermite(value1, tangent1, value2, tangent2, amount) return {} end

---@return Vector2
---@param value1 Vector2
---@param value2 Vector2
---@param amount number
---@overload fun(value1: Vector2, value2: Vector2, amount: number, result: Vector2): nil
function Vector2_.Lerp(value1, value2, amount) return {} end

---@return Vector2
---@param value1 Vector2
---@param value2 Vector2
---@overload fun(value1: Vector2, value2: Vector2, result: Vector2): nil
function Vector2_.Max(value1, value2) return {} end

---@return Vector2
---@param value1 Vector2
---@param value2 Vector2
---@overload fun(value1: Vector2, value2: Vector2, result: Vector2): nil
function Vector2_.Min(value1, value2) return {} end

---@return Vector2
---@param value1 Vector2
---@param value2 Vector2
---@overload fun(value1: Vector2, scaleFactor: number): Vector2
---@overload fun(value1: Vector2, scaleFactor: number, result: Vector2): nil
---@overload fun(value1: Vector2, value2: Vector2, result: Vector2): nil
function Vector2_.Multiply(value1, value2) return {} end

---@return Vector2
---@param value Vector2
---@overload fun(value: Vector2, result: Vector2): nil
function Vector2_.Negate(value) return {} end

---@return Vector2
---@param value Vector2
---@overload fun(value: Vector2, result: Vector2): nil
function Vector2_.Normalize(value) return {} end

---@return Vector2
---@param vector Vector2
---@param normal Vector2
---@overload fun(vector: Vector2, normal: Vector2, result: Vector2): nil
function Vector2_.Reflect(vector, normal) return {} end

---@return Vector2
---@param value1 Vector2
---@param value2 Vector2
---@param amount number
---@overload fun(value1: Vector2, value2: Vector2, amount: number, result: Vector2): nil
function Vector2_.SmoothStep(value1, value2, amount) return {} end

---@return Vector2
---@param value1 Vector2
---@param value2 Vector2
---@overload fun(value1: Vector2, value2: Vector2, result: Vector2): nil
function Vector2_.Subtract(value1, value2) return {} end

---@return Vector2
---@param position Vector2
---@param matrix Matrix
---@overload fun(position: Vector2, matrix: Matrix, result: Vector2): nil
---@overload fun(value: Vector2, rotation: Quaternion): Vector2
---@overload fun(value: Vector2, rotation: Quaternion, result: Vector2): nil
---@overload fun(sourceArray: Vector2[], matrix: Matrix, destinationArray: Vector2[]): nil
---@overload fun(sourceArray: Vector2[], sourceIndex: number, matrix: Matrix, destinationArray: Vector2[], destinationIndex: number, length: number): nil
---@overload fun(sourceArray: Vector2[], rotation: Quaternion, destinationArray: Vector2[]): nil
---@overload fun(sourceArray: Vector2[], sourceIndex: number, rotation: Quaternion, destinationArray: Vector2[], destinationIndex: number, length: number): nil
function Vector2_.Transform(position, matrix) return {} end

---@return Vector2
---@param normal Vector2
---@param matrix Matrix
---@overload fun(normal: Vector2, matrix: Matrix, result: Vector2): nil
---@overload fun(sourceArray: Vector2[], matrix: Matrix, destinationArray: Vector2[]): nil
---@overload fun(sourceArray: Vector2[], sourceIndex: number, matrix: Matrix, destinationArray: Vector2[], destinationIndex: number, length: number): nil
function Vector2_.TransformNormal(normal, matrix) return {} end

---@overload fun(value: Vector2, scaleFactor: number): Vector2
---@overload fun(scaleFactor: number, value: Vector2): Vector2
---@overload fun(value1: Vector2, divider: number): Vector2

---@type Vector3
Vector3 = require("#Microsoft.Xna.Framework.Vector3")

---@class Vector3
---@field X number
---@field Y number
---@field Z number
---@field Zero Vector3
---@field One Vector3
---@field UnitX Vector3
---@field UnitY Vector3
---@field UnitZ Vector3
---@field Up Vector3
---@field Down Vector3
---@field Right Vector3
---@field Left Vector3
---@field Forward Vector3
---@field Backward Vector3
---@overload fun(x: number, y: number, z: number): Vector3
---@overload fun(value: number): Vector3
---@overload fun(value: Vector2, z: number): Vector3
---@operator add(Vector3): Vector3
---@operator unm(): Vector3
---@operator sub(Vector3): Vector3
---@operator mul(Vector3): Vector3
---@operator mul(number): Vector3
---@operator mul(Vector3): Vector3
---@operator div(Vector3): Vector3
---@operator div(number): Vector3
local Vector3_ = {}

---@return boolean
---@param other Vector3
function Vector3_:Equals(other) return false end

---@return number
function Vector3_:Length() return 0 end

---@return number
function Vector3_:LengthSquared() return 0 end

---@return nil
function Vector3_:Normalize() return {} end

---@return Vector3
---@param value1 Vector3
---@param value2 Vector3
---@overload fun(value1: Vector3, value2: Vector3, result: Vector3): nil
function Vector3_.Add(value1, value2) return {} end

---@return Vector3
---@param value1 Vector3
---@param value2 Vector3
---@param value3 Vector3
---@param amount1 number
---@param amount2 number
---@overload fun(value1: Vector3, value2: Vector3, value3: Vector3, amount1: number, amount2: number, result: Vector3): nil
function Vector3_.Barycentric(value1, value2, value3, amount1, amount2) return {} end

---@return Vector3
---@param value1 Vector3
---@param value2 Vector3
---@param value3 Vector3
---@param value4 Vector3
---@param amount number
---@overload fun(value1: Vector3, value2: Vector3, value3: Vector3, value4: Vector3, amount: number, result: Vector3): nil
function Vector3_.CatmullRom(value1, value2, value3, value4, amount) return {} end

---@return Vector3
---@param value1 Vector3
---@param min Vector3
---@param max Vector3
---@overload fun(value1: Vector3, min: Vector3, max: Vector3, result: Vector3): nil
function Vector3_.Clamp(value1, min, max) return {} end

---@return Vector3
---@param vector1 Vector3
---@param vector2 Vector3
---@overload fun(vector1: Vector3, vector2: Vector3, result: Vector3): nil
function Vector3_.Cross(vector1, vector2) return {} end

---@return number
---@param vector1 Vector3
---@param vector2 Vector3
---@overload fun(value1: Vector3, value2: Vector3, result: number): nil
function Vector3_.Distance(vector1, vector2) return 0 end

---@return number
---@param value1 Vector3
---@param value2 Vector3
---@overload fun(value1: Vector3, value2: Vector3, result: number): nil
function Vector3_.DistanceSquared(value1, value2) return 0 end

---@return Vector3
---@param value1 Vector3
---@param value2 Vector3
---@overload fun(value1: Vector3, value2: Vector3, result: Vector3): nil
---@overload fun(value1: Vector3, value2: number): Vector3
---@overload fun(value1: Vector3, value2: number, result: Vector3): nil
function Vector3_.Divide(value1, value2) return {} end

---@return number
---@param vector1 Vector3
---@param vector2 Vector3
---@overload fun(vector1: Vector3, vector2: Vector3, result: number): nil
function Vector3_.Dot(vector1, vector2) return 0 end

---@return Vector3
---@param value1 Vector3
---@param tangent1 Vector3
---@param value2 Vector3
---@param tangent2 Vector3
---@param amount number
---@overload fun(value1: Vector3, tangent1: Vector3, value2: Vector3, tangent2: Vector3, amount: number, result: Vector3): nil
function Vector3_.Hermite(value1, tangent1, value2, tangent2, amount) return {} end

---@return Vector3
---@param value1 Vector3
---@param value2 Vector3
---@param amount number
---@overload fun(value1: Vector3, value2: Vector3, amount: number, result: Vector3): nil
function Vector3_.Lerp(value1, value2, amount) return {} end

---@return Vector3
---@param value1 Vector3
---@param value2 Vector3
---@overload fun(value1: Vector3, value2: Vector3, result: Vector3): nil
function Vector3_.Max(value1, value2) return {} end

---@return Vector3
---@param value1 Vector3
---@param value2 Vector3
---@overload fun(value1: Vector3, value2: Vector3, result: Vector3): nil
function Vector3_.Min(value1, value2) return {} end

---@return Vector3
---@param value1 Vector3
---@param value2 Vector3
---@overload fun(value1: Vector3, scaleFactor: number): Vector3
---@overload fun(value1: Vector3, scaleFactor: number, result: Vector3): nil
---@overload fun(value1: Vector3, value2: Vector3, result: Vector3): nil
function Vector3_.Multiply(value1, value2) return {} end

---@return Vector3
---@param value Vector3
---@overload fun(value: Vector3, result: Vector3): nil
function Vector3_.Negate(value) return {} end

---@return Vector3
---@param value Vector3
---@overload fun(value: Vector3, result: Vector3): nil
function Vector3_.Normalize(value) return {} end

---@return Vector3
---@param vector Vector3
---@param normal Vector3
---@overload fun(vector: Vector3, normal: Vector3, result: Vector3): nil
function Vector3_.Reflect(vector, normal) return {} end

---@return Vector3
---@param value1 Vector3
---@param value2 Vector3
---@param amount number
---@overload fun(value1: Vector3, value2: Vector3, amount: number, result: Vector3): nil
function Vector3_.SmoothStep(value1, value2, amount) return {} end

---@return Vector3
---@param value1 Vector3
---@param value2 Vector3
---@overload fun(value1: Vector3, value2: Vector3, result: Vector3): nil
function Vector3_.Subtract(value1, value2) return {} end

---@return Vector3
---@param position Vector3
---@param matrix Matrix
---@overload fun(position: Vector3, matrix: Matrix, result: Vector3): nil
---@overload fun(sourceArray: Vector3[], matrix: Matrix, destinationArray: Vector3[]): nil
---@overload fun(sourceArray: Vector3[], sourceIndex: number, matrix: Matrix, destinationArray: Vector3[], destinationIndex: number, length: number): nil
---@overload fun(value: Vector3, rotation: Quaternion): Vector3
---@overload fun(value: Vector3, rotation: Quaternion, result: Vector3): nil
---@overload fun(sourceArray: Vector3[], rotation: Quaternion, destinationArray: Vector3[]): nil
---@overload fun(sourceArray: Vector3[], sourceIndex: number, rotation: Quaternion, destinationArray: Vector3[], destinationIndex: number, length: number): nil
function Vector3_.Transform(position, matrix) return {} end

---@return Vector3
---@param normal Vector3
---@param matrix Matrix
---@overload fun(normal: Vector3, matrix: Matrix, result: Vector3): nil
---@overload fun(sourceArray: Vector3[], matrix: Matrix, destinationArray: Vector3[]): nil
---@overload fun(sourceArray: Vector3[], sourceIndex: number, matrix: Matrix, destinationArray: Vector3[], destinationIndex: number, length: number): nil
function Vector3_.TransformNormal(normal, matrix) return {} end

---@overload fun(value: Vector3, scaleFactor: number): Vector3
---@overload fun(scaleFactor: number, value: Vector3): Vector3
---@overload fun(value: Vector3, divider: number): Vector3

---@type Vector4
Vector4 = require("#Microsoft.Xna.Framework.Vector4")

---@class Vector4
---@field X number
---@field Y number
---@field Z number
---@field W number
---@field Zero Vector4
---@field One Vector4
---@field UnitX Vector4
---@field UnitY Vector4
---@field UnitZ Vector4
---@field UnitW Vector4
---@overload fun(x: number, y: number, z: number, w: number): Vector4
---@overload fun(value: Vector2, z: number, w: number): Vector4
---@overload fun(value: Vector3, w: number): Vector4
---@overload fun(value: number): Vector4
---@operator unm(): Vector4
---@operator add(Vector4): Vector4
---@operator sub(Vector4): Vector4
---@operator mul(Vector4): Vector4
---@operator mul(number): Vector4
---@operator mul(Vector4): Vector4
---@operator div(Vector4): Vector4
---@operator div(number): Vector4
local Vector4_ = {}

---@return boolean
---@param other Vector4
function Vector4_:Equals(other) return false end

---@return number
function Vector4_:Length() return 0 end

---@return number
function Vector4_:LengthSquared() return 0 end

---@return nil
function Vector4_:Normalize() return {} end

---@return Vector4
---@param value1 Vector4
---@param value2 Vector4
---@overload fun(value1: Vector4, value2: Vector4, result: Vector4): nil
function Vector4_.Add(value1, value2) return {} end

---@return Vector4
---@param value1 Vector4
---@param value2 Vector4
---@param value3 Vector4
---@param amount1 number
---@param amount2 number
---@overload fun(value1: Vector4, value2: Vector4, value3: Vector4, amount1: number, amount2: number, result: Vector4): nil
function Vector4_.Barycentric(value1, value2, value3, amount1, amount2) return {} end

---@return Vector4
---@param value1 Vector4
---@param value2 Vector4
---@param value3 Vector4
---@param value4 Vector4
---@param amount number
---@overload fun(value1: Vector4, value2: Vector4, value3: Vector4, value4: Vector4, amount: number, result: Vector4): nil
function Vector4_.CatmullRom(value1, value2, value3, value4, amount) return {} end

---@return Vector4
---@param value1 Vector4
---@param min Vector4
---@param max Vector4
---@overload fun(value1: Vector4, min: Vector4, max: Vector4, result: Vector4): nil
function Vector4_.Clamp(value1, min, max) return {} end

---@return number
---@param value1 Vector4
---@param value2 Vector4
---@overload fun(value1: Vector4, value2: Vector4, result: number): nil
function Vector4_.Distance(value1, value2) return 0 end

---@return number
---@param value1 Vector4
---@param value2 Vector4
---@overload fun(value1: Vector4, value2: Vector4, result: number): nil
function Vector4_.DistanceSquared(value1, value2) return 0 end

---@return Vector4
---@param value1 Vector4
---@param value2 Vector4
---@overload fun(value1: Vector4, divider: number): Vector4
---@overload fun(value1: Vector4, divider: number, result: Vector4): nil
---@overload fun(value1: Vector4, value2: Vector4, result: Vector4): nil
function Vector4_.Divide(value1, value2) return {} end

---@return number
---@param vector1 Vector4
---@param vector2 Vector4
---@overload fun(vector1: Vector4, vector2: Vector4, result: number): nil
function Vector4_.Dot(vector1, vector2) return 0 end

---@return Vector4
---@param value1 Vector4
---@param tangent1 Vector4
---@param value2 Vector4
---@param tangent2 Vector4
---@param amount number
---@overload fun(value1: Vector4, tangent1: Vector4, value2: Vector4, tangent2: Vector4, amount: number, result: Vector4): nil
function Vector4_.Hermite(value1, tangent1, value2, tangent2, amount) return {} end

---@return Vector4
---@param value1 Vector4
---@param value2 Vector4
---@param amount number
---@overload fun(value1: Vector4, value2: Vector4, amount: number, result: Vector4): nil
function Vector4_.Lerp(value1, value2, amount) return {} end

---@return Vector4
---@param value1 Vector4
---@param value2 Vector4
---@overload fun(value1: Vector4, value2: Vector4, result: Vector4): nil
function Vector4_.Max(value1, value2) return {} end

---@return Vector4
---@param value1 Vector4
---@param value2 Vector4
---@overload fun(value1: Vector4, value2: Vector4, result: Vector4): nil
function Vector4_.Min(value1, value2) return {} end

---@return Vector4
---@param value1 Vector4
---@param value2 Vector4
---@overload fun(value1: Vector4, scaleFactor: number): Vector4
---@overload fun(value1: Vector4, scaleFactor: number, result: Vector4): nil
---@overload fun(value1: Vector4, value2: Vector4, result: Vector4): nil
function Vector4_.Multiply(value1, value2) return {} end

---@return Vector4
---@param value Vector4
---@overload fun(value: Vector4, result: Vector4): nil
function Vector4_.Negate(value) return {} end

---@return Vector4
---@param vector Vector4
---@overload fun(vector: Vector4, result: Vector4): nil
function Vector4_.Normalize(vector) return {} end

---@return Vector4
---@param value1 Vector4
---@param value2 Vector4
---@param amount number
---@overload fun(value1: Vector4, value2: Vector4, amount: number, result: Vector4): nil
function Vector4_.SmoothStep(value1, value2, amount) return {} end

---@return Vector4
---@param value1 Vector4
---@param value2 Vector4
---@overload fun(value1: Vector4, value2: Vector4, result: Vector4): nil
function Vector4_.Subtract(value1, value2) return {} end

---@return Vector4
---@param position Vector2
---@param matrix Matrix
---@overload fun(position: Vector3, matrix: Matrix): Vector4
---@overload fun(vector: Vector4, matrix: Matrix): Vector4
---@overload fun(position: Vector2, matrix: Matrix, result: Vector4): nil
---@overload fun(position: Vector3, matrix: Matrix, result: Vector4): nil
---@overload fun(vector: Vector4, matrix: Matrix, result: Vector4): nil
---@overload fun(sourceArray: Vector4[], matrix: Matrix, destinationArray: Vector4[]): nil
---@overload fun(sourceArray: Vector4[], sourceIndex: number, matrix: Matrix, destinationArray: Vector4[], destinationIndex: number, length: number): nil
---@overload fun(value: Vector2, rotation: Quaternion): Vector4
---@overload fun(value: Vector3, rotation: Quaternion): Vector4
---@overload fun(value: Vector4, rotation: Quaternion): Vector4
---@overload fun(value: Vector2, rotation: Quaternion, result: Vector4): nil
---@overload fun(value: Vector3, rotation: Quaternion, result: Vector4): nil
---@overload fun(value: Vector4, rotation: Quaternion, result: Vector4): nil
---@overload fun(sourceArray: Vector4[], rotation: Quaternion, destinationArray: Vector4[]): nil
---@overload fun(sourceArray: Vector4[], sourceIndex: number, rotation: Quaternion, destinationArray: Vector4[], destinationIndex: number, length: number): nil
function Vector4_.Transform(position, matrix) return {} end

---@overload fun(value1: Vector4, scaleFactor: number): Vector4
---@overload fun(scaleFactor: number, value1: Vector4): Vector4
---@overload fun(value1: Vector4, divider: number): Vector4

---@type Color
Color = require("#Microsoft.Xna.Framework.Color")

---@class Color
---@field B number
---@field G number
---@field R number
---@field A number
---@field PackedValue number
---@field Transparent Color
---@field AliceBlue Color
---@field AntiqueWhite Color
---@field Aqua Color
---@field Aquamarine Color
---@field Azure Color
---@field Beige Color
---@field Bisque Color
---@field Black Color
---@field BlanchedAlmond Color
---@field Blue Color
---@field BlueViolet Color
---@field Brown Color
---@field BurlyWood Color
---@field CadetBlue Color
---@field Chartreuse Color
---@field Chocolate Color
---@field Coral Color
---@field CornflowerBlue Color
---@field Cornsilk Color
---@field Crimson Color
---@field Cyan Color
---@field DarkBlue Color
---@field DarkCyan Color
---@field DarkGoldenrod Color
---@field DarkGray Color
---@field DarkGreen Color
---@field DarkKhaki Color
---@field DarkMagenta Color
---@field DarkOliveGreen Color
---@field DarkOrange Color
---@field DarkOrchid Color
---@field DarkRed Color
---@field DarkSalmon Color
---@field DarkSeaGreen Color
---@field DarkSlateBlue Color
---@field DarkSlateGray Color
---@field DarkTurquoise Color
---@field DarkViolet Color
---@field DeepPink Color
---@field DeepSkyBlue Color
---@field DimGray Color
---@field DodgerBlue Color
---@field Firebrick Color
---@field FloralWhite Color
---@field ForestGreen Color
---@field Fuchsia Color
---@field Gainsboro Color
---@field GhostWhite Color
---@field Gold Color
---@field Goldenrod Color
---@field Gray Color
---@field Green Color
---@field GreenYellow Color
---@field Honeydew Color
---@field HotPink Color
---@field IndianRed Color
---@field Indigo Color
---@field Ivory Color
---@field Khaki Color
---@field Lavender Color
---@field LavenderBlush Color
---@field LawnGreen Color
---@field LemonChiffon Color
---@field LightBlue Color
---@field LightCoral Color
---@field LightCyan Color
---@field LightGoldenrodYellow Color
---@field LightGray Color
---@field LightGreen Color
---@field LightPink Color
---@field LightSalmon Color
---@field LightSeaGreen Color
---@field LightSkyBlue Color
---@field LightSlateGray Color
---@field LightSteelBlue Color
---@field LightYellow Color
---@field Lime Color
---@field LimeGreen Color
---@field Linen Color
---@field Magenta Color
---@field Maroon Color
---@field MediumAquamarine Color
---@field MediumBlue Color
---@field MediumOrchid Color
---@field MediumPurple Color
---@field MediumSeaGreen Color
---@field MediumSlateBlue Color
---@field MediumSpringGreen Color
---@field MediumTurquoise Color
---@field MediumVioletRed Color
---@field MidnightBlue Color
---@field MintCream Color
---@field MistyRose Color
---@field Moccasin Color
---@field NavajoWhite Color
---@field Navy Color
---@field OldLace Color
---@field Olive Color
---@field OliveDrab Color
---@field Orange Color
---@field OrangeRed Color
---@field Orchid Color
---@field PaleGoldenrod Color
---@field PaleGreen Color
---@field PaleTurquoise Color
---@field PaleVioletRed Color
---@field PapayaWhip Color
---@field PeachPuff Color
---@field Peru Color
---@field Pink Color
---@field Plum Color
---@field PowderBlue Color
---@field Purple Color
---@field Red Color
---@field RosyBrown Color
---@field RoyalBlue Color
---@field SaddleBrown Color
---@field Salmon Color
---@field SandyBrown Color
---@field SeaGreen Color
---@field SeaShell Color
---@field Sienna Color
---@field Silver Color
---@field SkyBlue Color
---@field SlateBlue Color
---@field SlateGray Color
---@field Snow Color
---@field SpringGreen Color
---@field SteelBlue Color
---@field Tan Color
---@field Teal Color
---@field Thistle Color
---@field Tomato Color
---@field Turquoise Color
---@field Violet Color
---@field Wheat Color
---@field White Color
---@field WhiteSmoke Color
---@field Yellow Color
---@field YellowGreen Color
---@overload fun(color: Vector4): Color
---@overload fun(color: Vector3): Color
---@overload fun(r: number, g: number, b: number): Color
---@overload fun(r: number, g: number, b: number): Color
---@overload fun(r: number, g: number, b: number, alpha: number): Color
---@overload fun(r: number, g: number, b: number, alpha: number): Color
---@overload fun(color: Color, alpha: number): Color
---@overload fun(color: Color, alpha: number): Color
---@operator mul(number): Color
local Color_ = {}

---@return boolean
---@param other Color
function Color_:Equals(other) return false end

---@return Vector3
function Color_:ToVector3() return {} end

---@return Vector4
function Color_:ToVector4() return {} end

---@return Color
---@param value1 Color
---@param value2 Color
---@param amount number
function Color_.Lerp(value1, value2, amount) return {} end

---@return Color
---@param vector Vector4
---@overload fun(r: number, g: number, b: number, a: number): Color
function Color_.FromNonPremultiplied(vector) return {} end

---@return Color
---@param value Color
---@param scale number
function Color_.Multiply(value, scale) return {} end


---@enum BulletRotationMode
_G.BulletRotationMode = {
	None = 0,
	Velocity = 1,
	PositionChange = 2,
}


---@type Ease
Ease = require("#Monocle.Ease")

---@class Ease
---@field Linear Easer
---@field SineIn Easer
---@field SineOut Easer
---@field SineInOut Easer
---@field QuadIn Easer
---@field QuadOut Easer
---@field QuadInOut Easer
---@field CubeIn Easer
---@field CubeOut Easer
---@field CubeInOut Easer
---@field QuintIn Easer
---@field QuintOut Easer
---@field QuintInOut Easer
---@field ExpoIn Easer
---@field ExpoOut Easer
---@field ExpoInOut Easer
---@field BackIn Easer
---@field BackOut Easer
---@field BackInOut Easer
---@field BigBackIn Easer
---@field BigBackOut Easer
---@field BigBackInOut Easer
---@field ElasticIn Easer
---@field ElasticOut Easer
---@field ElasticInOut Easer
---@field BounceIn Easer
---@field BounceOut Easer
---@field BounceInOut Easer
local Ease_ = {}

---@return Easer
---@param easer Easer
function Ease_.Invert(easer) return {} end

---@return Easer
---@param first Easer
---@param second Easer
function Ease_.Follow(first, second) return {} end

---@return number
---@param eased number
function Ease_.UpDown(eased) return 0 end


---@type Calc
Calc = require("#Monocle.Calc")

---@class Calc
---@field Random Random
---@field Right number
---@field Up number
---@field Left number
---@field Down number
---@field UpRight number
---@field UpLeft number
---@field DownRight number
---@field DownLeft number
---@field DegToRad number
---@field RadToDeg number
---@field DtR number
---@field RtD number
---@field Circle number
---@field HalfCircle number
---@field QuarterCircle number
---@field EighthCircle number
local Calc_ = {}

---@return number
---@param e Type
function Calc_.EnumLength(e) return 0 end

---@return T
---@param str string
function Calc_.StringToEnum(str) return {} end

---@return T[]
---@param strs string[]
function Calc_.StringsToEnums(strs) return {} end

---@return boolean
---@param str string
function Calc_.EnumHasString(str) return false end

---@return boolean
---@param str string
---@param match string
function Calc_.StartsWith(str, match) return false end

---@return boolean
---@param str string
---@param match string
function Calc_.EndsWith(str, match) return false end

---@return boolean
---@param str string
---@param matches string[]
function Calc_.IsIgnoreCase(str, matches) return false end

---@return string
---@param num number
---@param minDigits number
function Calc_.ToString(num, minDigits) return "" end

---@return string[]
---@param text string
---@param font SpriteFont
---@param maxLineWidth number
---@param newLine Char
function Calc_.SplitLines(text, font, maxLineWidth, newLine) return {} end

---@return number
---@param target T
---@param a T
---@param b T
---@overload fun(target: T, a: T, b: T, c: T): number
---@overload fun(target: T, a: T, b: T, c: T, d: T): number
---@overload fun(target: T, a: T, b: T, c: T, d: T, e: T): number
---@overload fun(target: T, a: T, b: T, c: T, d: T, e: T, f: T): number
function Calc_.Count(target, a, b) return 0 end

---@return T
---@param index number
---@param a T
---@param b T
---@overload fun(index: number, a: T, b: T, c: T): T
---@overload fun(index: number, a: T, b: T, c: T, d: T): T
---@overload fun(index: number, a: T, b: T, c: T, d: T, e: T): T
---@overload fun(index: number, a: T, b: T, c: T, d: T, e: T, f: T): T
function Calc_.GiveMe(index, a, b) return {} end

---@return nil
---@param newSeed number
---@overload fun(random: Random): nil
---@overload fun(): nil
function Calc_.PushRandom(newSeed) return {} end

---@return nil
function Calc_.PopRandom() return {} end

---@return T
---@param random Random
---@param a T
---@param b T
---@overload fun(random: Random, a: T, b: T, c: T): T
---@overload fun(random: Random, a: T, b: T, c: T, d: T): T
---@overload fun(random: Random, a: T, b: T, c: T, d: T, e: T): T
---@overload fun(random: Random, a: T, b: T, c: T, d: T, e: T, f: T): T
---@overload fun(random: Random, choices: T[]): T
---@overload fun(random: Random, choices: List<System.Linq.Enumerable+SelectArrayIterator`2[System.Type,System.String]>): T
function Calc_.Choose(random, a, b) return {} end

---@return number
---@param random Random
---@param min number
---@param max number
---@overload fun(random: Random, min: number, max: number): number
---@overload fun(random: Random, min: Vector2, max: Vector2): Vector2
function Calc_.Range(random, min, max) return 0 end

---@return number
---@param random Random
function Calc_.Facing(random) return 0 end

---@return boolean
---@param random Random
---@param chance number
function Calc_.Chance(random, chance) return false end

---@return number
---@param random Random
---@overload fun(random: Random, max: number): number
function Calc_.NextFloat(random) return 0 end

---@return number
---@param random Random
function Calc_.NextAngle(random) return 0 end

---@return Vector2
---@param random Random
function Calc_.ShakeVector(random) return {} end

---@return Vector2
---@param list List<System.Linq.Enumerable+SelectArrayIterator`2[System.Type,System.String]>
---@param to Vector2
---@overload fun(list: Vector2[], to: Vector2): Vector2
---@overload fun(list: Vector2[], to: Vector2, index: number): Vector2
function Calc_.ClosestTo(list, to) return {} end

---@return nil
---@param list List<System.Linq.Enumerable+SelectArrayIterator`2[System.Type,System.String]>
---@param random Random
---@overload fun(list: List<System.Linq.Enumerable+SelectArrayIterator`2[System.Type,System.String]>): nil
function Calc_.Shuffle(list, random) return {} end

---@return nil
---@param list List<System.Linq.Enumerable+SelectArrayIterator`2[System.Type,System.String]>
---@param random Random
---@param first T
---@overload fun(list: List<System.Linq.Enumerable+SelectArrayIterator`2[System.Type,System.String]>, first: T): nil
function Calc_.ShuffleSetFirst(list, random, first) return {} end

---@return nil
---@param list List<System.Linq.Enumerable+SelectArrayIterator`2[System.Type,System.String]>
---@param random Random
---@param notFirst T
---@overload fun(list: List<System.Linq.Enumerable+SelectArrayIterator`2[System.Type,System.String]>, notFirst: T): nil
function Calc_.ShuffleNotFirst(list, random, notFirst) return {} end

---@return Color
---@param color Color
function Calc_.Invert(color) return {} end

---@return Color
---@param hex string
---@overload fun(hex: number): Color
function Calc_.HexToColor(hex) return {} end

---@return Color
---@param hue number
---@param s number
---@param v number
function Calc_.HsvToColor(hue, s, v) return {} end

---@return string
---@param time TimeSpan
function Calc_.ShortGameplayFormat(time) return "" end

---@return string
---@param time TimeSpan
function Calc_.LongGameplayFormat(time) return "" end

---@return number
---@param num number
function Calc_.Digits(num) return 0 end

---@return number
---@param c Char
function Calc_.HexToByte(c) return 0 end

---@return number
---@param num number
---@param zeroAt number
---@param oneAt number
function Calc_.Percent(num, zeroAt, oneAt) return 0 end

---@return number
---@param value number
---@param threshold number
function Calc_.SignThreshold(value, threshold) return 0 end

---@return number
---@param values number[]
function Calc_.Min(values) return 0 end

---@return number
---@param values number[]
---@overload fun(a: number, b: number, c: number, d: number): number
function Calc_.Max(values) return 0 end

---@return number
---@param f number
function Calc_.ToRad(f) return 0 end

---@return number
---@param f number
function Calc_.ToDeg(f) return 0 end

---@return number
---@param negative boolean
---@param positive boolean
---@param both number
function Calc_.Axis(negative, positive, both) return 0 end

---@return number
---@param value number
---@param min number
---@param max number
---@overload fun(value: number, min: number, max: number): number
function Calc_.Clamp(value, min, max) return 0 end

---@return number
---@param value number
function Calc_.YoYo(value) return 0 end

---@return number
---@param val number
---@param min number
---@param max number
---@param newMin number
---@param newMax number
function Calc_.Map(val, min, max, newMin, newMax) return 0 end

---@return number
---@param counter number
---@param newMin number
---@param newMax number
function Calc_.SineMap(counter, newMin, newMax) return 0 end

---@return number
---@param val number
---@param min number
---@param max number
---@param newMin number
---@param newMax number
function Calc_.ClampedMap(val, min, max, newMin, newMax) return 0 end

---@return number
---@param value1 number
---@param value2 number
---@param amount number
---@param snapThreshold number
function Calc_.LerpSnap(value1, value2, amount, snapThreshold) return 0 end

---@return number
---@param value1 number
---@param value2 number
---@param lerp number
function Calc_.LerpClamp(value1, value2, lerp) return 0 end

---@return Vector2
---@param value1 Vector2
---@param value2 Vector2
---@param amount number
---@param snapThresholdSq number
function Calc_.LerpSnap(value1, value2, amount, snapThresholdSq) return {} end

---@return Vector2
---@param vec Vector2
function Calc_.Sign(vec) return {} end

---@return Vector2
---@param vec Vector2
---@overload fun(vec: Vector2, length: number): Vector2
---@overload fun(vec: Vector2, ifZero: Vector2): Vector2
---@overload fun(vec: Vector2, ifZero: Vector2, length: number): Vector2
function Calc_.SafeNormalize(vec) return {} end

---@return Vector2
---@param vec Vector2
function Calc_.TurnRight(vec) return {} end

---@return number
---@param angle number
---@param axis number
---@overload fun(angleRadians: number, axis: Vector2): number
function Calc_.ReflectAngle(angle, axis) return 0 end

---@return Vector2
---@param lineA Vector2
---@param lineB Vector2
---@param closestTo Vector2
function Calc_.ClosestPointOnLine(lineA, lineB, closestTo) return {} end

---@return Vector2
---@param vec Vector2
function Calc_.Round(vec) return {} end

---@return number
---@param value number
---@param increment number
---@overload fun(value: number, increment: number, offset: number): number
function Calc_.Snap(value, increment) return 0 end

---@return number
---@param angleDegrees number
function Calc_.WrapAngleDeg(angleDegrees) return 0 end

---@return number
---@param angleRadians number
function Calc_.WrapAngle(angleRadians) return 0 end

---@return Vector2
---@param angleRadians number
---@param length number
function Calc_.AngleToVector(angleRadians, length) return {} end

---@return number
---@param val number
---@param target number
---@param maxMove number
function Calc_.AngleApproach(val, target, maxMove) return 0 end

---@return number
---@param startAngle number
---@param endAngle number
---@param percent number
function Calc_.AngleLerp(startAngle, endAngle, percent) return 0 end

---@return number
---@param val number
---@param target number
---@param maxMove number
function Calc_.Approach(val, target, maxMove) return 0 end

---@return number
---@param radiansA number
---@param radiansB number
function Calc_.AngleDiff(radiansA, radiansB) return 0 end

---@return number
---@param radiansA number
---@param radiansB number
function Calc_.AbsAngleDiff(radiansA, radiansB) return 0 end

---@return number
---@param radiansA number
---@param radiansB number
function Calc_.SignAngleDiff(radiansA, radiansB) return 0 end

---@return number
---@param from Vector2
---@param to Vector2
function Calc_.Angle(from, to) return 0 end

---@return Color
---@param current Color
---@param a Color
---@param b Color
function Calc_.ToggleColors(current, a, b) return {} end

---@return number
---@param currentAngle number
---@param angleA number
---@param angleB number
---@overload fun(currentAngle: number, angleA: number, angleB: number, angleC: number): number
function Calc_.ShorterAngleDifference(currentAngle, angleA, angleB) return 0 end

---@return boolean
---@param array T[]
---@param index number
---@overload fun(list: List<System.Linq.Enumerable+SelectArrayIterator`2[System.Type,System.String]>, index: number): boolean
function Calc_.IsInRange(array, index) return false end

---@return T[]
---@param items T[]
function Calc_.Array(items) return {} end

---@return T[]
---@param array T[]
---@param length number
---@overload fun(array: T[], length0: number, length1: number): T[]
function Calc_.VerifyLength(array, length) return {} end

---@return boolean
---@param val number
---@param interval number
function Calc_.BetweenInterval(val, interval) return false end

---@return boolean
---@param val number
---@param prevVal number
---@param interval number
function Calc_.OnInterval(val, prevVal, interval) return false end

---@return Vector2
---@param from Vector2
---@param to Vector2
---@param length number
---@overload fun(from: Entity, to: Entity, length: number): Vector2
function Calc_.Toward(from, to, length) return {} end

---@return Vector2
---@param vector Vector2
function Calc_.Perpendicular(vector) return {} end

---@return number
---@param vector Vector2
function Calc_.Angle(vector) return 0 end

---@return Vector2
---@param val Vector2
---@param minX number
---@param minY number
---@param maxX number
---@param maxY number
function Calc_.Clamp(val, minX, minY, maxX, maxY) return {} end

---@return Vector2
---@param val Vector2
function Calc_.Floor(val) return {} end

---@return Vector2
---@param val Vector2
function Calc_.Ceiling(val) return {} end

---@return Vector2
---@param val Vector2
function Calc_.Abs(val) return {} end

---@return Vector2
---@param val Vector2
---@param target Vector2
---@param maxMove number
function Calc_.Approach(val, target, maxMove) return {} end

---@return Vector2
---@param vec Vector2
function Calc_.FourWayNormal(vec) return {} end

---@return Vector2
---@param vec Vector2
function Calc_.EightWayNormal(vec) return {} end

---@return Vector2
---@param vec Vector2
---@param slices number
function Calc_.SnappedNormal(vec, slices) return {} end

---@return Vector2
---@param vec Vector2
---@param slices number
function Calc_.Snapped(vec, slices) return {} end

---@return Vector2
---@param vec Vector2
function Calc_.XComp(vec) return {} end

---@return Vector2
---@param vec Vector2
function Calc_.YComp(vec) return {} end

---@return Vector2[]
---@param list string
---@param seperator Char
function Calc_.ParseVector2List(list, seperator) return {} end

---@return Vector2
---@param vec Vector2
---@param angleRadians number
function Calc_.Rotate(vec, angleRadians) return {} end

---@return Vector2
---@param vec Vector2
---@param targetAngleRadians number
---@param maxMoveRadians number
---@overload fun(from: Vector3, target: Vector3, maxRotationRadians: number): Vector3
function Calc_.RotateTowards(vec, targetAngleRadians, maxMoveRadians) return {} end

---@return Vector2
---@param vector Vector3
function Calc_.XZ(vector) return {} end

---@return Vector3
---@param v Vector3
---@param target Vector3
---@param amount number
function Calc_.Approach(v, target, amount) return {} end

---@return Vector3
---@param v Vector3
function Calc_.SafeNormalize(v) return {} end

---@return number[][]
---@param csv string
---@param width number
---@param height number
function Calc_.ReadCSVIntGrid(csv, width, height) return {} end

---@return number[]
---@param csv string
function Calc_.ReadCSVInt(csv) return {} end

---@return number[]
---@param csv string
function Calc_.ReadCSVIntWithTricks(csv) return {} end

---@return string[]
---@param csv string
function Calc_.ReadCSV(csv) return {} end

---@return string
---@param data number[][]
function Calc_.IntGridToCSV(data) return "" end

---@return boolean[][]
---@param data string
---@param rowSep Char
function Calc_.GetBitData(data, rowSep) return {} end

---@return nil
---@param combineInto boolean[][]
---@param data string
---@param rowSep Char
---@overload fun(combineInto: boolean[][], data: boolean[][]): nil
function Calc_.CombineBitData(combineInto, data, rowSep) return {} end

---@return number[]
---@param strings string[]
function Calc_.ConvertStringArrayToIntArray(strings) return {} end

---@return number[]
---@param strings string[]
function Calc_.ConvertStringArrayToFloatArray(strings) return {} end

---@return boolean
---@param filename string
function Calc_.FileExists(filename) return false end

---@return boolean
---@param obj T
---@param filename string
function Calc_.SaveFile(obj, filename) return false end

---@return boolean
---@param filename string
---@param data T
function Calc_.LoadFile(filename, data) return false end

---@return XmlDocument
---@param filename string
function Calc_.LoadContentXML(filename) return {} end

---@return XmlDocument
---@param filename string
function Calc_.LoadXML(filename) return {} end

---@return boolean
---@param filename string
function Calc_.ContentXMLExists(filename) return false end

---@return boolean
---@param filename string
function Calc_.XMLExists(filename) return false end

---@return boolean
---@param xml XmlElement
---@param attributeName string
function Calc_.HasAttr(xml, attributeName) return false end

---@return string
---@param xml XmlElement
---@param attributeName string
---@overload fun(xml: XmlElement, attributeName: string, defaultValue: string): string
function Calc_.Attr(xml, attributeName) return "" end

---@return number
---@param xml XmlElement
---@param attributeName string
---@overload fun(xml: XmlElement, attributeName: string, defaultValue: number): number
function Calc_.AttrInt(xml, attributeName) return 0 end

---@return number
---@param xml XmlElement
---@param attributeName string
---@overload fun(xml: XmlElement, attributeName: string, defaultValue: number): number
function Calc_.AttrFloat(xml, attributeName) return 0 end

---@return Vector3
---@param xml XmlElement
---@param attributeName string
function Calc_.AttrVector3(xml, attributeName) return {} end

---@return Vector2
---@param xml XmlElement
---@param xAttributeName string
---@param yAttributeName string
---@overload fun(xml: XmlElement, xAttributeName: string, yAttributeName: string, defaultValue: Vector2): Vector2
function Calc_.AttrVector2(xml, xAttributeName, yAttributeName) return {} end

---@return boolean
---@param xml XmlElement
---@param attributeName string
---@overload fun(xml: XmlElement, attributeName: string, defaultValue: boolean): boolean
function Calc_.AttrBool(xml, attributeName) return false end

---@return Char
---@param xml XmlElement
---@param attributeName string
---@overload fun(xml: XmlElement, attributeName: string, defaultValue: Char): Char
function Calc_.AttrChar(xml, attributeName) return {} end

---@return T
---@param xml XmlElement
---@param attributeName string
---@overload fun(xml: XmlElement, attributeName: string, defaultValue: T): T
function Calc_.AttrEnum(xml, attributeName) return {} end

---@return Color
---@param xml XmlElement
---@param attributeName string
---@overload fun(xml: XmlElement, attributeName: string, defaultValue: Color): Color
---@overload fun(xml: XmlElement, attributeName: string, defaultValue: string): Color
function Calc_.AttrHexColor(xml, attributeName) return {} end

---@return Vector2
---@param xml XmlElement
---@overload fun(xml: XmlElement, defaultPosition: Vector2): Vector2
function Calc_.Position(xml) return {} end

---@return number
---@param xml XmlElement
---@overload fun(xml: XmlElement, defaultX: number): number
function Calc_.X(xml) return 0 end

---@return number
---@param xml XmlElement
---@overload fun(xml: XmlElement, defaultY: number): number
function Calc_.Y(xml) return 0 end

---@return number
---@param xml XmlElement
---@overload fun(xml: XmlElement, defaultWidth: number): number
function Calc_.Width(xml) return 0 end

---@return number
---@param xml XmlElement
---@overload fun(xml: XmlElement, defaultHeight: number): number
function Calc_.Height(xml) return 0 end

---@return Rectangle
---@param xml XmlElement
function Calc_.Rect(xml) return {} end

---@return number
---@param xml XmlElement
function Calc_.ID(xml) return 0 end

---@return number
---@param xml XmlElement
function Calc_.InnerInt(xml) return 0 end

---@return number
---@param xml XmlElement
function Calc_.InnerFloat(xml) return 0 end

---@return boolean
---@param xml XmlElement
function Calc_.InnerBool(xml) return false end

---@return T
---@param xml XmlElement
function Calc_.InnerEnum(xml) return {} end

---@return Color
---@param xml XmlElement
function Calc_.InnerHexColor(xml) return {} end

---@return boolean
---@param xml XmlElement
---@param childName string
function Calc_.HasChild(xml, childName) return false end

---@return string
---@param xml XmlElement
---@param childName string
---@overload fun(xml: XmlElement, childName: string, defaultValue: string): string
function Calc_.ChildText(xml, childName) return "" end

---@return number
---@param xml XmlElement
---@param childName string
---@overload fun(xml: XmlElement, childName: string, defaultValue: number): number
function Calc_.ChildInt(xml, childName) return 0 end

---@return number
---@param xml XmlElement
---@param childName string
---@overload fun(xml: XmlElement, childName: string, defaultValue: number): number
function Calc_.ChildFloat(xml, childName) return 0 end

---@return boolean
---@param xml XmlElement
---@param childName string
---@overload fun(xml: XmlElement, childName: string, defaultValue: boolean): boolean
function Calc_.ChildBool(xml, childName) return false end

---@return T
---@param xml XmlElement
---@param childName string
---@overload fun(xml: XmlElement, childName: string, defaultValue: T): T
function Calc_.ChildEnum(xml, childName) return {} end

---@return Color
---@param xml XmlElement
---@param childName string
---@overload fun(xml: XmlElement, childName: string, defaultValue: Color): Color
---@overload fun(xml: XmlElement, childName: string, defaultValue: string): Color
function Calc_.ChildHexColor(xml, childName) return {} end

---@return Vector2
---@param xml XmlElement
---@param childName string
---@overload fun(xml: XmlElement, childName: string, defaultValue: Vector2): Vector2
function Calc_.ChildPosition(xml, childName) return {} end

---@return Vector2
---@param xml XmlElement
function Calc_.FirstNode(xml) return {} end

---@return Vector2?
---@param xml XmlElement
---@overload fun(xml: XmlElement, offset: Vector2): Vector2?
function Calc_.FirstNodeNullable(xml) return {} end

---@return Vector2[]
---@param xml XmlElement
---@param includePosition boolean
---@overload fun(xml: XmlElement, offset: Vector2, includePosition: boolean): Vector2[]
function Calc_.Nodes(xml, includePosition) return {} end

---@return Vector2
---@param xml XmlElement
---@param nodeNum number
function Calc_.GetNode(xml, nodeNum) return {} end

---@return Vector2?
---@param xml XmlElement
---@param nodeNum number
function Calc_.GetNodeNullable(xml, nodeNum) return {} end

---@return nil
---@param xml XmlElement
---@param attributeName string
---@param setTo table
function Calc_.SetAttr(xml, attributeName, setTo) return {} end

---@return nil
---@param xml XmlElement
---@param childName string
---@param setTo table
function Calc_.SetChild(xml, childName, setTo) return {} end

---@return XmlElement
---@param doc XmlDocument
---@param childName string
---@overload fun(xml: XmlElement, childName: string): XmlElement
function Calc_.CreateChild(doc, childName) return {} end

---@return number
---@param a Entity
---@param b Entity
function Calc_.SortLeftToRight(a, b) return 0 end

---@return number
---@param a Entity
---@param b Entity
function Calc_.SortRightToLeft(a, b) return 0 end

---@return number
---@param a Entity
---@param b Entity
function Calc_.SortTopToBottom(a, b) return 0 end

---@return number
---@param a Entity
---@param b Entity
function Calc_.SortBottomToTop(a, b) return 0 end

---@return number
---@param a Entity
---@param b Entity
function Calc_.SortByDepth(a, b) return 0 end

---@return number
---@param a Entity
---@param b Entity
function Calc_.SortByDepthReversed(a, b) return 0 end

---@return nil
function Calc_.Log() return {} end

---@return nil
function Calc_.TimeLog() return {} end

---@return nil
---@param obj table[]
function Calc_.Log(obj) return {} end

---@return nil
---@param obj table
function Calc_.TimeLog(obj) return {} end

---@return nil
---@param collection IEnumerable<System.Linq.Enumerable+SelectArrayIterator`2[System.Type,System.String]>
function Calc_.LogEach(collection) return {} end

---@return nil
---@param obj table
function Calc_.Dissect(obj) return {} end

---@return nil
function Calc_.StartTimer() return {} end

---@return nil
function Calc_.EndTimer() return {} end

---@return Delegate
---@param obj table
---@param method string
function Calc_.GetMethod(obj, method) return {} end

---@return T
---@param arr T[][]
---@param at Pnt
function Calc_.At(arr, at) return {} end

---@return string
---@param path string
function Calc_.ConvertPath(path) return "" end

---@return string
---@param stream BinaryReader
function Calc_.ReadNullTerminatedString(stream) return "" end

---@return IEnumerator
---@param numerators IEnumerator[]
function Calc_.Do(numerators) return {} end

---@return Rectangle
---@param rect Rectangle
---@param clamp Rectangle
function Calc_.ClampTo(rect, clamp) return {} end

---@return XmlDocument
---@param filename string
function Calc_.orig_LoadContentXML(filename) return {} end

---@return XmlDocument
---@param filename string
function Calc_.orig_LoadXML(filename) return {} end

---@return boolean
---@param filename string
function Calc_.orig_ContentXMLExists(filename) return false end

---@return boolean
---@param filename string
function Calc_.orig_XMLExists(filename) return false end

---@return Color
---@param hex string
function Calc_.HexToColorWithAlpha(hex) return {} end


---@type Type
Type = require("#System.Type")

---@class Type
---@field Delimiter Char
---@field EmptyTypes Type[]
---@field Missing table
---@field FilterAttribute MemberFilter
---@field FilterName MemberFilter
---@field FilterNameIgnoreCase MemberFilter
---@field IsInterface boolean
---@field MemberType MemberTypes
---@field Namespace string
---@field AssemblyQualifiedName string
---@field FullName string
---@field Assembly Assembly
---@field Module Module
---@field IsNested boolean
---@field DeclaringType Type
---@field DeclaringMethod MethodBase
---@field ReflectedType Type
---@field UnderlyingSystemType Type
---@field IsTypeDefinition boolean
---@field IsArray boolean
---@field IsByRef boolean
---@field IsPointer boolean
---@field IsConstructedGenericType boolean
---@field IsGenericParameter boolean
---@field IsGenericTypeParameter boolean
---@field IsGenericMethodParameter boolean
---@field IsGenericType boolean
---@field IsGenericTypeDefinition boolean
---@field IsSZArray boolean
---@field IsVariableBoundArray boolean
---@field IsByRefLike boolean
---@field IsFunctionPointer boolean
---@field IsUnmanagedFunctionPointer boolean
---@field HasElementType boolean
---@field GenericTypeArguments Type[]
---@field GenericParameterPosition number
---@field GenericParameterAttributes GenericParameterAttributes
---@field Attributes TypeAttributes
---@field IsAbstract boolean
---@field IsImport boolean
---@field IsSealed boolean
---@field IsSpecialName boolean
---@field IsClass boolean
---@field IsNestedAssembly boolean
---@field IsNestedFamANDAssem boolean
---@field IsNestedFamily boolean
---@field IsNestedFamORAssem boolean
---@field IsNestedPrivate boolean
---@field IsNestedPublic boolean
---@field IsNotPublic boolean
---@field IsPublic boolean
---@field IsAutoLayout boolean
---@field IsExplicitLayout boolean
---@field IsLayoutSequential boolean
---@field IsAnsiClass boolean
---@field IsAutoClass boolean
---@field IsUnicodeClass boolean
---@field IsCOMObject boolean
---@field IsContextful boolean
---@field IsEnum boolean
---@field IsMarshalByRef boolean
---@field IsPrimitive boolean
---@field IsValueType boolean
---@field IsSignatureType boolean
---@field IsSecurityCritical boolean
---@field IsSecuritySafeCritical boolean
---@field IsSecurityTransparent boolean
---@field StructLayoutAttribute StructLayoutAttribute
---@field TypeInitializer ConstructorInfo
---@field TypeHandle RuntimeTypeHandle
---@field GUID Guid
---@field BaseType Type
---@field DefaultBinder Binder
---@field IsSerializable boolean
---@field ContainsGenericParameters boolean
---@field IsVisible boolean
local Type_ = {}

---@return Type
---@param typeName string
---@param throwOnError boolean
---@param ignoreCase boolean
---@overload fun(typeName: string, throwOnError: boolean): Type
---@overload fun(typeName: string): Type
---@overload fun(typeName: string, assemblyResolver: Func<System.Linq.Enumerable+SelectArrayIterator`2[System.Type,System.String]>, typeResolver: Func<System.Linq.Enumerable+SelectArrayIterator`2[System.Type,System.String]>): Type
---@overload fun(typeName: string, assemblyResolver: Func<System.Linq.Enumerable+SelectArrayIterator`2[System.Type,System.String]>, typeResolver: Func<System.Linq.Enumerable+SelectArrayIterator`2[System.Type,System.String]>, throwOnError: boolean): Type
---@overload fun(typeName: string, assemblyResolver: Func<System.Linq.Enumerable+SelectArrayIterator`2[System.Type,System.String]>, typeResolver: Func<System.Linq.Enumerable+SelectArrayIterator`2[System.Type,System.String]>, throwOnError: boolean, ignoreCase: boolean): Type
function Type_.GetType(typeName, throwOnError, ignoreCase) return {} end

---@return Type
---@param handle RuntimeTypeHandle
function Type_.GetTypeFromHandle(handle) return {} end

---@return Type
function Type_:GetType() return {} end

---@return Type
function Type_:GetElementType() return {} end

---@return number
function Type_:GetArrayRank() return 0 end

---@return Type
function Type_:GetGenericTypeDefinition() return {} end

---@return Type[]
function Type_:GetGenericArguments() return {} end

---@return Type[]
function Type_:GetOptionalCustomModifiers() return {} end

---@return Type[]
function Type_:GetRequiredCustomModifiers() return {} end

---@return Type[]
function Type_:GetGenericParameterConstraints() return {} end

---@return boolean
---@param targetType Type
function Type_:IsAssignableTo(targetType) return false end

---@return ConstructorInfo
---@param types Type[]
---@overload fun(bindingAttr: BindingFlags, types: Type[]): ConstructorInfo
---@overload fun(bindingAttr: BindingFlags, binder: Binder, types: Type[], modifiers: ParameterModifier[]): ConstructorInfo
---@overload fun(bindingAttr: BindingFlags, binder: Binder, callConvention: CallingConventions, types: Type[], modifiers: ParameterModifier[]): ConstructorInfo
function Type_:GetConstructor(types) return {} end

---@return ConstructorInfo[]
---@overload fun(bindingAttr: BindingFlags): ConstructorInfo[]
function Type_:GetConstructors() return {} end

---@return EventInfo
---@param name string
---@overload fun(name: string, bindingAttr: BindingFlags): EventInfo
function Type_:GetEvent(name) return {} end

---@return EventInfo[]
---@overload fun(bindingAttr: BindingFlags): EventInfo[]
function Type_:GetEvents() return {} end

---@return FieldInfo
---@param name string
---@overload fun(name: string, bindingAttr: BindingFlags): FieldInfo
function Type_:GetField(name) return {} end

---@return FieldInfo[]
---@overload fun(bindingAttr: BindingFlags): FieldInfo[]
function Type_:GetFields() return {} end

---@return Type[]
function Type_:GetFunctionPointerCallingConventions() return {} end

---@return Type
function Type_:GetFunctionPointerReturnType() return {} end

---@return Type[]
function Type_:GetFunctionPointerParameterTypes() return {} end

---@return MemberInfo[]
---@param name string
---@overload fun(name: string, bindingAttr: BindingFlags): MemberInfo[]
---@overload fun(name: string, type: MemberTypes, bindingAttr: BindingFlags): MemberInfo[]
function Type_:GetMember(name) return {} end

---@return MemberInfo[]
function Type_:GetMembers() return {} end

---@return MemberInfo
---@param member MemberInfo
function Type_:GetMemberWithSameMetadataDefinitionAs(member) return {} end

---@return MemberInfo[]
---@param bindingAttr BindingFlags
function Type_:GetMembers(bindingAttr) return {} end

---@return MethodInfo
---@param name string
---@overload fun(name: string, bindingAttr: BindingFlags): MethodInfo
---@overload fun(name: string, bindingAttr: BindingFlags, types: Type[]): MethodInfo
---@overload fun(name: string, types: Type[]): MethodInfo
---@overload fun(name: string, types: Type[], modifiers: ParameterModifier[]): MethodInfo
---@overload fun(name: string, bindingAttr: BindingFlags, binder: Binder, types: Type[], modifiers: ParameterModifier[]): MethodInfo
---@overload fun(name: string, bindingAttr: BindingFlags, binder: Binder, callConvention: CallingConventions, types: Type[], modifiers: ParameterModifier[]): MethodInfo
---@overload fun(name: string, genericParameterCount: number, types: Type[]): MethodInfo
---@overload fun(name: string, genericParameterCount: number, types: Type[], modifiers: ParameterModifier[]): MethodInfo
---@overload fun(name: string, genericParameterCount: number, bindingAttr: BindingFlags, binder: Binder, types: Type[], modifiers: ParameterModifier[]): MethodInfo
---@overload fun(name: string, genericParameterCount: number, bindingAttr: BindingFlags, binder: Binder, callConvention: CallingConventions, types: Type[], modifiers: ParameterModifier[]): MethodInfo
function Type_:GetMethod(name) return {} end

---@return MethodInfo[]
---@overload fun(bindingAttr: BindingFlags): MethodInfo[]
function Type_:GetMethods() return {} end

---@return Type
---@param name string
---@overload fun(name: string, bindingAttr: BindingFlags): Type
function Type_:GetNestedType(name) return {} end

---@return Type[]
---@overload fun(bindingAttr: BindingFlags): Type[]
function Type_:GetNestedTypes() return {} end

---@return PropertyInfo
---@param name string
---@overload fun(name: string, bindingAttr: BindingFlags): PropertyInfo
---@overload fun(name: string, returnType: Type): PropertyInfo
---@overload fun(name: string, types: Type[]): PropertyInfo
---@overload fun(name: string, returnType: Type, types: Type[]): PropertyInfo
---@overload fun(name: string, returnType: Type, types: Type[], modifiers: ParameterModifier[]): PropertyInfo
---@overload fun(name: string, bindingAttr: BindingFlags, binder: Binder, returnType: Type, types: Type[], modifiers: ParameterModifier[]): PropertyInfo
function Type_:GetProperty(name) return {} end

---@return PropertyInfo[]
---@overload fun(bindingAttr: BindingFlags): PropertyInfo[]
function Type_:GetProperties() return {} end

---@return MemberInfo[]
function Type_:GetDefaultMembers() return {} end

---@return RuntimeTypeHandle
---@param o table
function Type_.GetTypeHandle(o) return {} end

---@return Type[]
---@param args table[]
function Type_.GetTypeArray(args) return {} end

---@return TypeCode
---@param type Type
function Type_.GetTypeCode(type) return {} end

---@return Type
---@param clsid Guid
---@overload fun(clsid: Guid, throwOnError: boolean): Type
---@overload fun(clsid: Guid, server: string): Type
---@overload fun(clsid: Guid, server: string, throwOnError: boolean): Type
function Type_.GetTypeFromCLSID(clsid) return {} end

---@return Type
---@param progID string
---@overload fun(progID: string, throwOnError: boolean): Type
---@overload fun(progID: string, server: string): Type
---@overload fun(progID: string, server: string, throwOnError: boolean): Type
function Type_.GetTypeFromProgID(progID) return {} end

---@return table
---@param name string
---@param invokeAttr BindingFlags
---@param binder Binder
---@param target table
---@param args table[]
---@overload fun(name: string, invokeAttr: BindingFlags, binder: Binder, target: table, args: table[], culture: CultureInfo): table
---@overload fun(name: string, invokeAttr: BindingFlags, binder: Binder, target: table, args: table[], modifiers: ParameterModifier[], culture: CultureInfo, namedParameters: string[]): table
function Type_:InvokeMember(name, invokeAttr, binder, target, args) return {} end

---@return Type
---@param name string
---@overload fun(name: string, ignoreCase: boolean): Type
function Type_:GetInterface(name) return {} end

---@return Type[]
function Type_:GetInterfaces() return {} end

---@return InterfaceMapping
---@param interfaceType Type
function Type_:GetInterfaceMap(interfaceType) return {} end

---@return boolean
---@param o table
function Type_:IsInstanceOfType(o) return false end

---@return boolean
---@param other Type
function Type_:IsEquivalentTo(other) return false end

---@return Type
function Type_:GetEnumUnderlyingType() return {} end

---@return Array
function Type_:GetEnumValues() return {} end

---@return Array
function Type_:GetEnumValuesAsUnderlyingType() return {} end

---@return Type
---@overload fun(rank: number): Type
function Type_:MakeArrayType() return {} end

---@return Type
function Type_:MakeByRefType() return {} end

---@return Type
---@param typeArguments Type[]
function Type_:MakeGenericType(typeArguments) return {} end

---@return Type
function Type_:MakePointerType() return {} end

---@return Type
---@param genericTypeDefinition Type
---@param typeArguments Type[]
function Type_.MakeGenericSignatureType(genericTypeDefinition, typeArguments) return {} end

---@return Type
---@param position number
function Type_.MakeGenericMethodParameter(position) return {} end

---@return boolean
---@param o Type
function Type_:Equals(o) return false end

---@return Type
---@param typeName string
---@param throwIfNotFound boolean
---@param ignoreCase boolean
function Type_.ReflectionOnlyGetType(typeName, throwIfNotFound, ignoreCase) return {} end

---@return boolean
---@param value table
function Type_:IsEnumDefined(value) return false end

---@return string
---@param value table
function Type_:GetEnumName(value) return "" end

---@return string[]
function Type_:GetEnumNames() return {} end

---@return Type[]
---@param filter TypeFilter
---@param filterCriteria table
function Type_:FindInterfaces(filter, filterCriteria) return {} end

---@return MemberInfo[]
---@param memberType MemberTypes
---@param bindingAttr BindingFlags
---@param filter MemberFilter
---@param filterCriteria table
function Type_:FindMembers(memberType, bindingAttr, filter, filterCriteria) return {} end

---@return boolean
---@param c Type
function Type_:IsSubclassOf(c) return false end

---@return boolean
---@param c Type
function Type_:IsAssignableFrom(c) return false end


