local utils = require("mods").requireFromPlugin("libraries.goober.utils")

local entity = {}

entity.name = "GooberHelper/Waterfall"
entity.color = { 0.0, 0.5, 1.0, 0.5 }
entity.fieldInformation = {
    width = utils.intField(8),
    height = utils.intField(8),
    depth = utils.intField(-9999),
    speed = utils.numberField(200),
    backgroundWaterColor = utils.colorField("LightSkyBlue"), -- this is from the vanilla water entity
    backgroundWaterOpacity = utils.numberField(0.3), -- this too
    waterColor = utils.colorField("White"),
    waterTextureLayers = utils.listField(
        "objects/waterfall/GooberHelper/water",
        utils.stringField("", {
            width = 400
        })
    ),
    waterLayerDistance = utils.numberField(0),
    waterSpeed = utils.numberField(200),
    waterPadding = utils.intField(3),
    splashColor = utils.colorField("White"),
    splashTextures = utils.listField(
        "objects/waterfall/GooberHelper/splash",
        utils.stringField("", {
            width = 400
        })
    ),
    splashSpeed = utils.numberField(96),
    splashSize = utils.numberField(0.75),
    splashOpacity = utils.numberField(0.75),
    splashDensity = utils.numberField(0.1),
    splashDistance = utils.numberField(48),
    nonCollidable = utils.booleanField(false)
}

entity.placements = {
    name = "waterfall",
    data = utils.fieldInformationToPlacementData(entity.fieldInformation)
}

entity.ignoredFields = {
    "_name",
    "_id",
    "originX",
    "originY",
    "_spriteCache"
}

entity.fieldOrder = {
    "x",
    "y",
    "width",
    "height",

    "depth",

    "speed",

    "backgroundWaterColor",
    "backgroundWaterOpacity",

    "waterColor",
    "waterTextureLayers",
    "waterLayerDistance",
    "waterSpeed",
    "waterPadding",

    "splashColor",
    "splashTextures",
    "splashSpeed",
    "splashSize",
    "splashOpacity",
    "splashDensity",
    "splashDistance",

    "nonCollidable",
}

-- crashout incoming
-- i tried so hard to make this work
-- it would be so cool to be able to see what the entity would look like in game through loenn
-- and i did it
-- it was functioning fine
-- but FOR SOME REASON
-- loenn doesnt have a way to cache the sprites of an entity
-- so it would rerender when literally anything happened in/to the room
-- since computing the splash sprites and tiling the water textures for each entity takes a while, it made editing some unrelated entity lag loenn for a solid amount of time
-- i tried using a thing where it would cache the sprites and doing things like moving and resizing it would invalidate the cache or something similar, but i just ran into the issue of 
-- well
-- literally every other entity function 😭
-- there isnt a callback for resizing the entity on creation, editing the room that the entity is in, copy and pasting the entity, and probably other things that i havent found yet
-- the following code is unusable until loenn either adds support for what im trying to do or i get a christmas spirit brain blast that gives me a path to make this work
-- i hope you enjoyed reading that
-- goodbye

-- local drawableSpriteStruct = require("structs.drawable_sprite")


-- -- IM SORRY LOENN PEOPLE
-- -- I SWEAR I HAVE A REASON FOR THIS
-- -- LOOK AT THE BOTTOM OF THE CODE
-- local state = require("loaded_state")
-- local selectionUtils = require("selections")

-- -- so the keybind to add a node can be used to refresh the entity
-- entity.nodeLimits = { 0, 1 }

-- local function createWaterSprites(sprites, instance)
--     local texture = instance.waterTextureLayers and instance.waterTextureLayers:split(",")()[1] or "objects/waterfall/GooberHelper/water"
--     local sprite = nil

--     local y = 0

--     while y < instance.height do
--         local x = 0

--         while x < instance.width do
--             sprite = drawableSpriteStruct.fromTexture(texture, instance)

--             if sprite == nil then
--                 return
--             end

--             sprite:setJustification(0, 0)
--             sprite:addPosition(x, y)
--             sprite:useRelativeQuad(0, 0, math.min(instance.width - x, sprite.meta.width), math.min(instance.height - y, sprite.meta.height))
--             sprite:setColor(instance.waterColor)

--             table.insert(sprites, sprite)

--             x = x + sprite.meta.width
--         end

--         if sprite == nil then
--             break
--         end

--         y = y + sprite.meta.height
--     end
-- end

-- local function createSplashSprites(sprites, instance)
--     if instance.splashTextures == "" then
--         return
--     end

--     local splashTextures = instance.splashTextures ~= nil
--         and instance.splashTextures:split(",")()
--         or { "objects/waterfall/GooberHelper/splash" }

--     local x = 0

--     while x < instance.width do
--         local sprite = drawableSpriteStruct.fromTexture(splashTextures[math.random(1, #splashTextures)], instance)
--         local angle = math.random() * math.pi * 2
--         local progress = math.random()

--         if sprite then
--             local offset = {
--                 math.cos(angle) * progress * (instance.splashDistance or 48),
--                 math.sin(angle) * progress * (instance.splashDistance or 48)
--             }

--             sprite:addPosition(table.unpack(offset))
--             sprite:addPosition(x, instance.height)
--             sprite:setColor(instance.splashColor or "ffffff")
--             sprite:setAlpha((instance.splashOpacity or 0.75) * (1 - progress))
--             sprite:setScale(instance.splashSize or 0.75)

--             table.insert(sprites, sprite)
--         end

--         x = x + 1 / (instance.splashDensity or 0.5)
--     end
-- end

-- function entity.sprite(room, instance, viewport)
--     if instance._spriteCache ~= nil then
--         return instance._spriteCache
--     end

--     local sprites = {}

--     createWaterSprites(sprites, instance)
--     createSplashSprites(sprites, instance)

--     instance._spriteCache = sprites

--     return sprites
-- end

-- function entity.depth(room, instance, viewport)
--     return instance.depth
-- end

-- -- sprite cache invalidators
-- -- faster to just move everything instead of invalidating the cache
-- function entity.onMove(room, instance, nodeIndex, offsetX, offsetY)
--     if instance._spriteCache == nil then
--         return
--     end

--     for _, value in ipairs(instance._spriteCache) do
--         value.x = value.x + offsetX
--         value.y = value.y + offsetY
--     end
-- end

-- function entity.onResize(room, instance, offsetX, offsetY, directionX, directionY)
--     instance._spriteCache = nil
-- end

-- -- CURSED DUMB SOLUTION WHILE LOENN DOESNT SUPPORT A BETTER ONE
-- -- AAAAAAAAAAAAAAA AERJTGFSZEJDRFLIOUHSZALEIDRFHGIUOJSHZRDETFGUHJSZRUDEUFTJHGISLZRDHFTGILSZHEDIRF5GTHUJNIASZEUDRF5HJGNLSAZHJELliuhjliHILHAelirGHLIERHFiueh RIGHIULERDFghliehrgflherigLerfHUGLEHRNFgluEhRGLERHg
-- -- UIEHJR4TGIUAEHRITFAEUDLRFGHILAESDHTFRGhhiuhFTGLIUiedarfGHLAIUERHGLIhliERH GIe 3yh4rtg 8l79u  43EY5H LTGEUI45hy
-- -- goodvibes
-- --
-- -- i have been on this problem for a combined like 5 hours
-- -- this will have to work
-- function entity.nodeAdded(room, instance, nodeIndex)
--     instance._spriteCache = nil

--     -- SKETCHY SKETCHY AAAAAAAAAAAAAAAAAAAAA
--     -- THIS SUCKS
--     selectionUtils.redrawTargetLayers(room, state.selectionToolTargets)
-- end

return entity