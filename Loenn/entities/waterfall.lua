local entity = {}

entity.name = "GooberHelper/Waterfall"
entity.depth = 9000
entity.color = {0.0, 0.5, 1.0, 0.5}
entity.placements = {
    name = "Waterfall",
    data = {
        width = 8,
        height = 8,

        depth = -9999,

        speed = 200,

        waterColor = "ffffff",
        waterTextureLayers = "objects/waterfall/GooberHelper/water",
        waterLayerDistance = 0,
        waterSpeed = 200,
        waterPadding = 3,

        splashColor = "ffffff",
        splashTextures = "objects/waterfall/GooberHelper/splash",
        splashSpeed = 96,
        splashSize = 0.75,
        splashOpacity = 0.75,
        splashDensity = 0.5,
        splashDistance = 48,

        nonCollidable = false,
    }
}

entity.fieldInformation = {
    depth = {
        fieldType = "integer"
    },
    waterColor = {
        fieldType = "color"
    },
    waterTextureLayers = {
        fieldType = "list",
        elementOptions = {
            width = 400,
            fieldType = "string"
        }
    },
    waterPadding = {
        fieldType = "integer"
    },
    splashColor = {
        fieldType = "color"
    },
    splashTextures = {
        fieldType = "list",
        elementOptions = {
            width = 400,
            fieldType = "string"
        }
    },
}

entity.fieldOrder = {
    "x",
    "y",
    "width",
    "height",

    "depth",

    "speed",

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

return entity