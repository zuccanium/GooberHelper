local headers = {}

function headers.groupHeader(name)
    return "---- " .. name .. " ----"
end

function headers.categoryHeader(name)
    return "====== " .. name:upper() .. " ======"
end

return headers