local options = require("mods").requireFromPlugin("libraries.goober.options")

local trigger = {}

trigger.name = "GooberHelper/GooberHelperOptions"
trigger.placements = {
    name = "gooberHelperOptions",
    data = {
        enable = "",
        disable = "",
        flag = "",
        notFlag = "",
        revertOnLeave = false,
        revertOnDeath = false,
        resetAll = false,
    }
}

local disableOptions = {}
local isOption = {}
local isDisableOption = {}

local specialInputFields = {}

for _, value in ipairs(options) do
    local disableOptionName = value

    if value:sub(-1, -1) == "]" then
        local field = {
            options = {},
            canBeNumber = false
        }

        local splitter = value:find(":")
        local optionKey = value:sub(1, splitter - 1)
        local optionEnumContent = value:sub(splitter + 3, -2)

        for str in string.gmatch(optionEnumContent, "([^|]+)") do
            local id = str:lower()
            
            if id == "number" then
                field.canBeNumber = true
            else
                field.options[id] = true
            end
        end

        specialInputFields[optionKey] = field

        disableOptionName = optionKey
    else
        if value:sub(1, 1) ~= "=" then
            isOption[value] = true
        end
    end

    table.insert(disableOptions, disableOptionName)
    isDisableOption[disableOptionName] = true
end

local function createOptionsField(fieldOptions, validator)
    return {
        fieldType = "list",
        elementDefault = "",
        elementSeparator = ",",
        elementOptions = {
            width = 500,
            minWidth = 500,
            fieldType = "string",
            options = fieldOptions,
            validator = validator,
            searchable = true
        }
    }
end

trigger.fieldInformation = {
    enable = createOptionsField(
        options,
        function(input)
            if #input == 0 then return true end

            local splitter = input:find(":") or 1
            local valueData = specialInputFields[input:sub(1, splitter - 1)]

            if valueData == nil then
                return isOption[input] == true
            end

            if input:sub(splitter + 1, splitter + 1) == " " then
                splitter = splitter + 1
            end

            if valueData.canBeNumber and tonumber(input:sub(splitter + 1, -1)) ~= nil then
                return true
            end
            
            return valueData.options[input:sub(splitter + 1, -1):lower()] ~= nil
        end
    ),
    disable = createOptionsField(
        disableOptions,
        function(input)
            if #input == 0 then return true end

            return isDisableOption[input] == true
        end
    ),
    flag = {
        fieldType = "string"
    },
    notFlag = {
        fieldType = "string"
    },
    revertOnLeave = {
        fieldType = "boolean"
    },
    revertOnDeath = {
        fieldType = "boolean"
    },
    resetAll = {
        fieldType = "boolean"
    },
}

trigger.fieldOrder = {
    "x",
    "y",
    "width",
    "height",
    "enable",
    "disable",
    "flag",
    "notFlag",
    "revertOnLeave",
    "revertOnDeath",
    "resetAll",
}

return trigger