local utils = {}

--#region general

function utils.transferFields(target, source)
    if source == nil then
        return target
    end

    for key, value in pairs(source) do
        target[key] = value
    end

    return target
end

--#endregion general

--#region field types

local function genericField(type, value, data)
    return utils.transferFields(
        {
            fieldType = type,
            default = value
        },
        data
    )
end

---@param value boolean
function utils.booleanField(value, data)
    return genericField("boolean", value, data)
end

---@param value number
function utils.numberField(value, data)
    return genericField("number", value, data)
end

---@param value number
function utils.intField(value, data)
    return genericField("integer", value, data)
end

---@param value string
function utils.stringField(value, data)
    return genericField("string", value, data)
end

---@param value string
function utils.colorField(value, data)
    if data == nil then
        data = {}
    end

    data.allowXNAColors = true

    return genericField("color", value, data)
end

---@param value string
function utils.listField(value, elementOptions, data)
    if data == nil then
        data = {}
    end

    data.elementOptions = elementOptions

    return genericField("list", value, data)
end

--#endregion field types

--#region placement generation

function utils.fieldInformationToPlacementData(fieldInfo)
    local data = {}

    for key, value in pairs(fieldInfo) do
        data[key] = value.default
    end

    return data
end

--#endregion placement generation

return utils