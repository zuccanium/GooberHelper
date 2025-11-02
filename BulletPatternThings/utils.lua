_G.Celeste = require("#Celeste")
_G.Engine = require("#Monocle.Engine")
_G.Calc = require("#Monocle.Calc")
_G.Random = require("#System.Random")
_G.LuaHelper = require("#Celeste.Mod.GooberHelper.LuaHelper")
_G.SyncedMusicHelper = require("#Celeste.Mod.GooberHelper.SyncedMusicHelper")

---the time interval (in seconds) between each beat of the current song (assigned in the PlaySyncedMusic function)
_G.spb = 0

---@class BulletData
    ---@field velocity Vector2?
    ---@field acceleration Vector2?
    ---@field color Color? 
    ---@field texture string? 
    ---@field scale number?
    ---@field effect string? the effect (aka shader) used to render the bullet (defaults to "coloredBullet")
    ---@field additive boolean? whether the rendering uses additive blending or not
    ---@field lowResolution boolean?
    ---@field rotation number?
    ---@field colliderRadius number?
    ---@field rotationMode BulletRotationMode?
    ---@field cullDistance number? the distance away from the edge of the screen for the bullet to be removed
    ---@field rotationCenter Vector2? the point around which the bullet should rotate with a speed of PositionRotationSpeed
    ---@field positionRotationSpeed number? the speed at which the bullet position should rotate around RotationCenter
    ---@field velocityRotationSpeed number? the speed at which the bullet velocity should rotate. this can usually just be equal to PositionRotationSpeed
    ---@field friction number? exponential decay in bullet speed

---@class BulletCreationData : BulletData
    ---@field position Vector2?
    ---@field template BulletTemplate? a set of bullet properties to inherit from.\ you can combine them with addition (e.g. scaledBulletTemplate + coolEffectBulletTemplate)

---returns a coroutine that
---1. plays a song
---2. stalls while that song hasnt started
---3. assigns the global spb variable
---@return table
---@param path string
---@param bpm number bpm of the song (used to calculate spb) (can be ignored if you want)
_G.PlaySyncedMusic = function(path, bpm)
    if bpm then
        _G.spb = 60/bpm
    end

    return SyncedMusicHelper.PlaySyncedMusic(path)
end

---@return function
---@param method table method to make generic
---@param ... Type type arguments
_G.Generic = function(method, ...)
    LuaHelper.MakeGeneric(method, ...)

    --i swear it exists
    return _G["generic_method"]
end

---@return Type
---@param typeName string name of the type to get (**include namespaces and capitalize it correctly!!!**)
_G.GetType = function(typeName)
    return LuaHelper.GetCSharpType(typeName)
end

---substitute for a ternary operator in lua
---@generic T
---@generic U
---@param condition boolean
---@param value1 `T`
---@param value2 `U`
---@return `T` | `U`
_G.Ternary = function(condition, value1, value2)
    if condition then
        return value1
    else
        return value2
    end
end

---puts the player in a state designed to mimmick touhou gameplay
_G.EnterTouhouState = function()
    Player.StateMachine.State = Celeste.Mod.GooberHelper.States.TouhouState.TouhouStateId
end

---returns a unit vector with a random direction
---@return Vector2
_G.RandomDirection = function()
    return Angle(Calc.NextFloat(Random.Shared) * 360)
end

---@return number
---@param a number lower bound
---@param b number higher bound
_G.RandomRange = function(a, b)
    return Calc.NextFloat(Random.Shared) * (b - a) + a
end

---creates a bullet template that bullets can inherit properties from\
---you can also add them together to combine them (right overrides left) (e.g. scaledBulletTemplate + coolEffectBulletTemplate)
---@param props BulletData
---@return BulletTemplate
_G.CreateBulletTemplate = function(props)
    return Celeste.Mod.GooberHelper.BulletTemplate(
        props.velocity,
        props.acceleration,
        props.color,
        props.texture,
        props.scale,
        props.effect,
        props.additive,
        props.lowResolution,
        props.rotation,
        props.colliderRadius,
        props.cullDistance,
        props.rotationCenter,
        props.positionRotationSpeed,
        props.velocityRotationSpeed,
        props.friction
    )
end

---@return Bullet
---@param props BulletCreationData
_G.Shoot = function(props)
    return Celeste.Mod.GooberHelper.Entities.Bullet(
        Parent,
        props.template,
        props.position,
        props.velocity,
        props.acceleration,
        props.color,
        props.texture,
        props.scale,
        props.effect,
        props.additive,
        props.lowResolution,
        props.rotation,
        props.colliderRadius,
        props.cullDistance,
        props.rotationCenter,
        props.positionRotationSpeed,
        props.velocityRotationSpeed,
        props.friction
    )
end

---returns a vector with a specified angle (degrees)
---@return Vector2
---@param angle number
_G.Angle = function(angle)
    return Calc.AngleToVector(angle / 180 * math.pi, 1)
end

---returns a color with specified hue, saturation, and value
---@return Color
---@param h number hue
---@param s number saturation
---@param v number value
_G.Hsv = function(h, s, v)
    return Calc.HsvToColor((((h / 360) % 1) + 1) % 1, s, v)
end

---returns the player position in the local coordinate space
---@return Vector2
_G.GetPlayerPosition = function()
    return Player.Position - Parent.BulletFieldCenter
end

---returns a vector that points from an input position to the player
---@return Vector2
---@param position Vector2 the position that the output vector should point from
_G.TowardsPlayer = function(position)
    return Calc.SafeNormalize(GetPlayerPosition() - position)
end

---adds a coroutine to the scene that runs independently of the caller coroutine
---@param co function
_G.AddCoroutine = function(co)
    return Parent:AddLuaCoroutine(Celeste.Mod.LuaCoroutine({value = coroutine.create(co), resume = ThreadProxyResume}))
end